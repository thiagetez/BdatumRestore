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
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> FileList;

        private ErrorLogs _ErrorLogs = new ErrorLogs();

        public bool isBusy { get; set; }

        private string _FolderSelected { get; set; }

        public delegate void updateBar(int value);

        public updateBar delegateUpdateBar;

        private static IConfiguration _configuration { get; set; }

        private EnumDirectories _EnumDir { get; set; }

        private int _FoldersCount { get; set; }

        private int _ErrorCount { get; set; }

        private IFolder _SelectedFolder { get; set; }

        private int _ItensCount { get; set; }

        private bool _Pause { get; set; }
        private bool _ErrorOcurred { get; set; }

        public static string SelectedItemPath { get; set; }
        //public static string currentPath = String.Format("{0}{1}", Path.GetTempPath(), "UploadFileTest");
        private static MainWindow _MainWindow { get; set; }

        /// <summary>
        /// Comando de Update
        /// </summary>
        public ICommand UpdateCommand { get; set; }

        /// <summary>
        /// Comando de Pause
        /// </summary>
        public ICommand PauseCommand { get; set; }

        /// <summary>
        /// Comando de Download
        /// </summary>
        public ICommand DownloadCommand { get; set; }
        
        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="MainWindow">Instancia do Form</param>
        public ListFolder(MainWindow MainWindow)
        {
            _MainWindow = MainWindow;
            DownloadCommand = new DownloadCommandModel(this);
            UpdateCommand = new UpdateTreeViewCommandModel(this);
            PauseCommand = new PauseCommandModel(this);
            _EnumDir=new EnumDirectories();
            m_folders = new ObservableCollection<IFolder>();
            m_files = new ObservableCollection<IFolder>();
            //TODO: listar pastas e arquivos em abas separadas
            AuthenticationMethod();
            CheckPausedRestore();
            InitTreeView();
        }

        private void CheckPausedRestore()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            if (File.Exists(path))
            {
                
                MessageBoxResult msg= MessageBox.Show("Você tem um restore pendente. Deseja retoma-lo?", "Restore pendente encontrado", MessageBoxButton.YesNo);

                if (msg == MessageBoxResult.Yes)
                {
                    PausedRestore pause = new PausedRestore();

                    List<string> list = pause.RestoreFiles();
                    _MainWindow.ShowBrowseDialog();

                    Thread thread = new Thread(new ThreadStart(() => this.DownloadCore(list)));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();

                }
                else
                {
                    string restorepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
                    File.Delete(restorepath);
                }
                
            }
        }

        /// <summary>
        ///Lista de diretórios
        /// </summary>
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


        public string FolderSelected
        {
            get
            {
                return _FolderSelected;
            }
            set
            {

                _FolderSelected = "Pasta selecionada: "+value;
                NotifiyPropertyChanged("FolderSelected");
            }
        }

        public IFolder SelectedFolder
        {
            get
            {
                return _SelectedFolder;
            }
            set
            {

                _SelectedFolder = value;
                FolderSelected = _SelectedFolder.FolderName;
            }
        }
        /// <summary>
        /// Lista de Arquivos
        /// </summary>
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
        /// <summary>
        /// Método de Autenticação
        /// </summary>
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
                IAuthentication _authentication = new Authentication("NEKSP7b6hoktg4nIg5u4", "XGzzNadCLjXFpQjTuJYF");

                //IAuthentication _authentication = new Authentication(auth.NodeKey, auth.PartnerKey);

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

        /// <summary>
        /// Enumera os arquivos para serem baixados
        /// </summary>
        [STAThread]
        internal void Download()
        {
            try
            {
                this.isBusy = true;
                Application.Current.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                         (Action)(() =>
                            {
                              _MainWindow.RestoreButton.IsEnabled = false;
                              _MainWindow.ProgressLabel.Content = "Preparando Download";
                              _MainWindow.progressBar1.Value =0;
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
                    List<string> filelist = new List<string>();

                        Application.Current.Dispatcher.Invoke(
                         DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             foreach (IFolder item in _MainWindow.FileList.SelectedItems)
                             {
                                 filelist.Add(item.FullPath);
                             }

                         }));



                        DownloadCore(filelist);
                }
                else if (_SelectedFolder != null && _MainWindow.browseDialog.SelectedPath != "")
                {
                    List<string> filelist = _EnumDir.EnumFolderAndSubFolders(_SelectedFolder.FullPath, _configuration);
                    
                    DownloadCore(filelist);
                }
                else
                {
                    _MainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => MessageBox.Show(_MainWindow, "Você precisa selecionar uma pasta ou arquivos para fazer restore ou não selecionou uma pasta para gravar os arquivos", "Erro no download")));
                    _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Erro no download"));
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                }
          }catch (Exception e)
             {
                 _ErrorLogs.CreateLogFile(e);
                 _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Erro no download"));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
             }
        }

        /// <summary>
        /// Faz Download dos arquivos
        /// </summary>
        /// <param name="Files"></param>
        private void DownloadCore(List<string> fileList )
        {

            FileList = new List<string>(fileList);
            Application.Current.Dispatcher.Invoke(
             DispatcherPriority.Background,
             (Action)(() =>
             {
                 _MainWindow.progressBar1.Maximum = fileList.Count;
                 _MainWindow.PauseButton.IsEnabled = true;
                 _MainWindow.RestoreButton.IsEnabled = false;
             }));

            int filescount = 0;

            
            foreach (string path in fileList)
            {
                string downloadDir = path.Replace("/", @"\");
                Storage storage = new Storage(_configuration);
                if (_Pause == false)
                {

                    try { storage.Download(path, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(downloadDir)); }
                    catch (Exception e) { _ErrorLogs.AddError(e, path); _ErrorOcurred = true; _ErrorCount++; }
                    finally
                    {

                        FileList.Remove(path);
                        _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = String.Format("Arquivo:{0} {1}/{2}", Path.GetFileName(path), filescount + 1, fileList.Count)));
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                               (Action)(() =>
                               {
                                   _MainWindow.progressBar1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.progressBar1.Value++));
                               }));
                        filescount++;
                    }
                }
                else
                {
                    _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download Pausado"));
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.PauseButton.IsEnabled = false));
                    return;
                }

            }

            if (_ErrorOcurred == false)
            {
                _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download completo"));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
            }
            else
            {
                _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download incompleto, verifique o log em Documents/bdatum/Logs ou contacte o suporte."));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                _ErrorLogs.CreateLogFile(filescount, _ErrorCount);

            }

            string pausedpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            if (File.Exists(pausedpath))
                File.Delete(pausedpath);
        
        }

        internal void PauseDownload()
        {
            MessageBoxResult result = MessageBox.Show("Você deseja interromper o restore?\nVocê pode retoma-lo na proxima vez que executar o programa.", "Interromper restore?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _Pause = true;
                PausedRestore paused = new PausedRestore();
                paused.CreateRestoreFiles(FileList);
            }
        }

        /// <summary>
        /// Constroi a TreeView com as pastas
        /// </summary>
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
      

        /// <summary>
        /// Atualiza a TreeView com as pastas e/ou arquivos
        /// </summary>
        /// <param name="fileItem"></param>
        internal void UpdateTreeView(object fileItem)
        {
                bool isListed = false;
                IFolder item = fileItem as IFolder;

                SelectedFolder = item;
                if (item.Folders.Count != 0)
                {
                    isListed = true;
                }
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
                            if (isListed == false)
                            {
                                string[] dirname = file.FullName.Split('/');
                                //Folders[index].Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2]+@"\", FullPath = file.FullName, isFolder = file.IsDirectory,isChildren=true});
                                item.Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2] + @"\", FullPath = file.FullName, isFolder = file.IsDirectory, isChildren = true });
                            }
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

        /// <summary>
        /// Limpa a lista de arquivos
        /// </summary>
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
