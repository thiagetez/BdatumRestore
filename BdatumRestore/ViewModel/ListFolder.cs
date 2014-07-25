using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration;
using Newtonsoft.Json;

namespace BdatumRestore.ViewModel
{

    using BDatum.SDK;
    using BDatum.SDK.REST;
    using System.IO;
    using System.Windows.Input;
    using BdatumRestore.Model;
    using BdatumRestore.View;
    using System.Windows.Controls;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Threading;
    using System.Windows.Threading;

    //Classe que lista as pastas

    public class ListFolder:INotifyPropertyChanged
    {

        public delegate void updateBar(int value);
        public updateBar delegateUpdateBar;

        private static IConfiguration _configuration { get; set; }

        private EnumDirectories _EnumDir { get; set; }

        private int _FoldersCount { get; set; }

        private IFolder _SelectedFolder { get; set; }

        private int _ItensCount { get; set; }

        public static string SelectedItemPath { get; set; }
        //public static string currentPath = String.Format("{0}{1}", Path.GetTempPath(), "UploadFileTest");
        private static MainWindow _MainWindow { get; set; }
        
        public ListFolder(MainWindow MainWindow)
        {
            _MainWindow = MainWindow;
            DownloadCommand = new DownloadCommandModel(this,SelectedItemPath);
            UpdateCommand = new UpdateTreeViewCommandModel(this);
            _EnumDir=new EnumDirectories();
            m_folders = new ObservableCollection<IFolder>();
            m_files = new ObservableCollection<IFolder>();
            //TODO: listar pastas e arquivos em abas separadas
            AuthenticationMethod();
            InitTreeView();
        }


        private ObservableCollection<IFolder> m_folders;

        public ObservableCollection<IFolder> Folders
        {
            get { return m_folders; }
            set
            {
                m_folders = value;
                NotifiyPropertyChanged("Folders");
            }
        }
        private ObservableCollection<IFolder> m_files;
        public ObservableCollection<IFolder> Files
        {
            get { return m_files; }
            set
            {
                m_files = value;
                NotifiyPropertyChanged("Files");
            }
        }
        public static void AuthenticationMethod()
        {
            try
            {
                IConnectionConfiguration _connection = new ConnectionConfiguration();
                _connection.Proxy = new System.Net.WebProxy("127.0.0.1", 8888);

                string AuthPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum";
                FileStream fs=new FileStream(AuthPath+@"\settings.json",FileMode.Open,FileAccess.Read);
                var reader=new StreamReader(fs);
                string json=reader.ReadToEnd();
                AuthenticationProperties auth = JsonConvert.DeserializeObject<AuthenticationProperties>(json);

                //////TEST ONLY
                //IAuthentication _authentication = new Authentication("01234567890123456789", "01234567890123456789");

                IAuthentication _authentication = new Authentication(auth.NodeKey, auth.PartnerKey);

                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["bdatumURI"]))
                    _connection.ServiceURL = ConfigurationManager.AppSettings["bdatumURI"];

                _configuration = new BDatum.SDK.REST.Configuration(_authentication, _connection);
            }
            catch (Exception e)
            {
                throw new BDatumException(e.ToString());
            }
        }

        /// <summary>
        /// Notifica uma mudança na propriedade para o evento
        /// </summary>
        /// <param name="property"></param>
        void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand UpdateCommand { get; set; }
        public ICommand DownloadCommand { get; set; }


        
        [STAThread]
        internal void Download()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                         (Action)(() =>
                            {
                              _MainWindow.RestoreButton.IsEnabled = false;
                              _MainWindow.label3.Content = "Fazendo Download";
                             }));
                bool arg1=false;
                bool arg2=false;
                Application.Current.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                      (Action)(() =>
                             {
                                  if (_MainWindow.FileList != null)
                                      arg1 = true;
                                  else
                                      arg1 = false;

                                  if (_MainWindow.FileList.SelectedItems.Count != 0)
                                      arg2 = true;
                                  else
                                      arg2 = false;

                              }));

                _MainWindow.ShowBrowseDialog();

                if (arg1 == true && arg2 == true && _MainWindow.browseDialog.SelectedPath != "")                  
                {

                    if (_MainWindow.browseDialog.SelectedPath != "" )
                    {
                         List<string> Files =new List<string>();

                        Application.Current.Dispatcher.Invoke(
                         DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             foreach (IFolder item in _MainWindow.FileList.SelectedItems)
                             {
                                 Files.Add(item.FullPath);
                             }

                         }));

                        DownloadCore(Files);
                    }
                    else
                    {
                        _MainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => MessageBox.Show(_MainWindow, "Você precisa selecionar uma pasta para gravar os arquivos.", "Nem uma pasta foi selecionada")));
                        _MainWindow.label3.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.label3.Content = "Erro no download"));
                        _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                    }
                }
                else if (_SelectedFolder != null)
                {
                    List<string> Files = _EnumDir.EnumFolderAndSubFolders(_SelectedFolder.FullPath, _configuration);
                    DownloadCore(Files);
                }
                else
                {
                    _MainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => MessageBox.Show(_MainWindow, "Você precisa selecionar uma pasta ou arquivos para fazer restore ou não selecionou uma pasta para gravar os arquivos", "Erro no download")));
                    _MainWindow.label3.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.label3.Content = "Erro no download"));
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                }
          }catch (Exception e)
             {
              //Gravar o exception para futuro uso, pois pode acontecer exception estranhos.
              //Gravar em um LOG?
              //Json?
                _MainWindow.label3.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.label3.Content = "Erro no download"));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
             }
        }

        internal void InitTreeView()
        {
            try
            {
                IFolder item = _MainWindow.TreeView1.SelectedItem as IFolder;
                List<BDatumFiles> FileList = new List<BDatumFiles>();
                Storage storage = new Storage(_configuration);
                FileList = storage.EnumerateFilesAndFolders();
                m_folders.Clear();
                foreach (BDatumFiles file in FileList)
                {
                    if (file.IsDirectory)
                    {
                        string[] dirname = file.FullName.Split('/');
                        Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2] + @"\", FullPath = file.FullName, isFolder = file.IsDirectory });
                    }
                    else
                    {
                        string[] dirname = file.FullName.Split('/');
                        Files.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory });
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ocorreu um erro de autenticação, verifique suas credenciais e sua conexão","Fatal Error");
                _MainWindow.Close();
            }
            
        }
        private void DownloadCore(List<string> Files)
        {
            Application.Current.Dispatcher.Invoke(
             DispatcherPriority.Background,
             (Action)(() =>
             {
                 _MainWindow.progressBar1.Maximum = Files.Count;
             }));
            int filescount = 0;
            foreach (string path in Files)
            {
                string downloadDir = path.Replace("/", @"\");
                Storage storage = new Storage(_configuration);
                storage.Download(path, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(downloadDir));
                _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = String.Format("Arquivo:{0} {1}/{2}", Path.GetFileName(path), filescount+1, Files.Count)));
                Application.Current.Dispatcher.Invoke(
            DispatcherPriority.Background,
            (Action)(() =>
            {
                _MainWindow.progressBar1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.progressBar1.Value++));
            }));
                filescount++;
            }
            _MainWindow.label3.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.label3.Content = "Download completo"));
            _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
        }

        internal void UpdateTreeView(object fileItem)
        {
                IFolder item = fileItem as IFolder;

                _SelectedFolder = item;
                if (item.Folders.Count == 0)
                {
                    List<BDatumFiles> FileList = new List<BDatumFiles>();
                    Storage storage = new Storage(_configuration);
                    FileList = storage.EnumerateFilesAndFolders(item.FullPath);
                    _FoldersCount = Folders.Count();
                    int index = Folders.IndexOf(item);
                    //m_folders.Clear();

                    #region remove os arquivos da lista

                    FileListClear();

                    #endregion

                    foreach (BDatumFiles file in FileList)
                    {
                        if (file.IsDirectory)
                        {

                            string[] dirname = file.FullName.Split('/');
                            //Folders[index].Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2]+@"\", FullPath = file.FullName, isFolder = file.IsDirectory,isChildren=true});
                            item.Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2] + @"\", FullPath = file.FullName, isFolder = file.IsDirectory, isChildren = true });
                        }
                        else
                        {
                            _ItensCount++;
                            string[] dirname = file.FullName.Split('/');
                            //Folders[index].Folders.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory,isChildren=true });
                            Files.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory, isChildren = true });
                        }

                    }
                    if (_ItensCount == 0)
                    {
                        _MainWindow.FilesExistLabel.Content = "Nenhum arquivo encontrado";
                    }
                    else
                        _MainWindow.FilesExistLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    FileListClear();
                }
                
            
         }

        private void FileListClear()
        {
            if (_ItensCount != 0)
            {
                Files.Clear();
                _ItensCount = 0;
                _MainWindow.FilesExistLabel.Content = "Nenhum arquivo encontrado";
                _MainWindow.FilesExistLabel.Visibility = Visibility.Visible;
            }
        }

    }
}
