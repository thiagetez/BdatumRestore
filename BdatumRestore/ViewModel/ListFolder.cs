using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration;

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
        public static void AuthenticationMethod()
        {
            try
            {
                IConnectionConfiguration _connection = new ConnectionConfiguration();
                _connection.Proxy = new System.Net.WebProxy("127.0.0.1", 8888);

                IAuthentication _authentication = new Authentication(
                    ConfigurationManager.AppSettings["bdatumNodeKey"],
                    ConfigurationManager.AppSettings["bdatumPartnerKey"]
                );

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

                if ( arg1==true && arg2==true)                  
                {
                    // List<IFolder> item = _MainWindow.FileList.SelectedItems as List<IFolder>;
                    foreach (IFolder item in _MainWindow.FileList.SelectedItems)
                    {
                        Storage storage = new Storage(_configuration);
                        string path = item.FullPath;
                        storage.Download(path, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\DownloadDirTest");
                    }
                }
                else if (_SelectedFolder != null)
                {
                    _MainWindow.ShowBrowseDialog();

                    if (_MainWindow.browseDialog.SelectedPath != "")
                    {

                        Application.Current.Dispatcher.Invoke(
                         DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             IFolder selecteditem = (IFolder)_MainWindow.TreeView1.SelectedItem;
                         }));

                        List<string> Files = _EnumDir.EnumFolderAndSubFolders(_SelectedFolder.FullPath, _configuration);


                        Application.Current.Dispatcher.Invoke(
                         DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             _MainWindow.progressBar1.Maximum = Files.Count;
                         }));

                        foreach (string path in Files)
                        {
                            string downloadDir = path.Replace("/", @"\");
                            Storage storage = new Storage(_configuration);
                            storage.Download(path, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(downloadDir));
                            Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        (Action)(() =>
                        {
                            _MainWindow.progressBar1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.progressBar1.Value++));
                        }));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Você precisa selecionar uma pasta para gravar os arquivos.", "Nem uma pasta foi selecionada");
                        _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                    }
                }
                else
                {
                    MessageBox.Show("Você precisa selecionar uma pasta ou arquivos para fazer restore.", "Nem um item selecionado");
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                }
            }
            catch (Exception e)
            {
                _MainWindow.label3.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.label3.Content = "Erro no download"));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
            }
        }

        internal void InitTreeView()
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
                        Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2] + @"\", FullPath = file.FullName, isFolder = file.IsDirectory});
                    }
                    else
                    {
                        string[] dirname = file.FullName.Split('/');
                        Folders.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory});
                    }
                    
                }
            
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
                    if (_ItensCount != 0)
                    {
                        int itensToRemove = _FoldersCount - _ItensCount;

                        for (int i = _FoldersCount - _ItensCount; i < _FoldersCount; i++)
                        {
                            Folders.RemoveAt(itensToRemove);

                        }
                        _ItensCount = 0;
                    }

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
                            Folders.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory, isChildren = true });
                        }
                    }
                }
                
            
         }


    }
}
