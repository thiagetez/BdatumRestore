using System;
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

    //Classe que lista as pastas
    public class ListFolder:INotifyPropertyChanged
    {
        private static IConfiguration _configuration { get; set; }

        private EnumDirectories _EnumDir { get; set; }

        private int _FoldersCount { get; set; }

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

        internal void Download()
        {
            if (_MainWindow.FileList.SelectedItems != null && _MainWindow.FileList.SelectedItems.Count != 0)
            {
                // List<IFolder> item = _MainWindow.FileList.SelectedItems as List<IFolder>;
                foreach (IFolder item in _MainWindow.FileList.SelectedItems)
                {
                    Storage storage = new Storage(_configuration);
                    string path = item.FullPath;
                    storage.Download(path, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\DownloadDirTest");
                }
            }
            else if (_MainWindow.TreeView1.SelectedItem != null)
            {
                IFolder selecteditem = (IFolder)_MainWindow.TreeView1.SelectedItem;
                List<IFolder> Files = _EnumDir.EnumFolderAndSubFolders(selecteditem.FullPath, _configuration);
                foreach (IFolder item in Files)
                {
                    Storage storage = new Storage(_configuration);
                    string path = item.FullPath;
                    storage.Download(path, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\DownloadDirTest");
                }
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
                List<BDatumFiles> FileList = new List<BDatumFiles>();
                Storage storage = new Storage(_configuration);
                FileList = storage.EnumerateFilesAndFolders(item.FullPath);
                 _FoldersCount = Folders.Count();
                int index = Folders.IndexOf(item);
                //m_folders.Clear();



                if (_ItensCount != 0)
                {
                    int itensToRemove = _FoldersCount - _ItensCount;

                    for (int i = _FoldersCount - _ItensCount; i < _FoldersCount; i++)
                    {
                        Folders.RemoveAt(itensToRemove);
              
                    }
                    _ItensCount = 0;
                }
                    

                
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
