using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BDatum.SDK;
using System.Configuration;

namespace BdatumRestore.ViewModel
{
    
    using BDatum.SDK.REST;
    using System.IO;
    using System.Windows.Input;
    using BdatumRestore.Model;
using BdatumRestore.View;
    using System.Windows.Controls;

    //Classe que lista as pastas
    public class ListFolder:INotifyPropertyChanged
    {
        private static IConfiguration _configuration { get; set; }

        public static string SelectedItemPath { get; set; }
        //public static string currentPath = String.Format("{0}{1}", Path.GetTempPath(), "UploadFileTest");
        private static MainWindow _MainWindow { get; set; }
        
        public ListFolder(MainWindow MainWindow)
        {
            _MainWindow = MainWindow;
            RestoreCommand = new UpdateCommandModel(this,SelectedItemPath);
            UpdateCommand = new UpdateTreeViewCommandModel(this);
            m_folders = new List<IFolder>();
            //TODO: listar pastas e arquivos em abas separadas
            AuthenticationMethod();
            InitTreeView();
        }

             
        private List<IFolder> m_folders;

        public List<IFolder> Folders
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
            IConnectionConfiguration _connection = new ConnectionConfiguration();
            IAuthentication _authentication = new Authentication(
                ConfigurationManager.AppSettings["bdatumNodeKey"],
                ConfigurationManager.AppSettings["bdatumPartnerKey"]
            );

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["bdatumURI"]))
                _connection.ServiceURL = ConfigurationManager.AppSettings["bdatumURI"];

            _configuration = new BDatum.SDK.REST.Configuration(_authentication, _connection);
        }

        void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand UpdateCommand { get; set; }
        public ICommand RestoreCommand { get; set; }

        internal void Download()
        {
            if (_MainWindow.TreeView1.SelectedItem!=null)
            {
                IFolder item = _MainWindow.TreeView1.SelectedItem as IFolder;
                Storage storage = new Storage(_configuration);
                storage.Download(item.FullPath,Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+@"");
            }
        }


        internal void InitTreeView()
        {
            
                IFolder item = _MainWindow.TreeView1.SelectedItem as IFolder;
                List<BDatumFiles> FileList = new List<BDatumFiles>();
                Storage storage = new Storage(_configuration);
                FileList = storage.EnumerateFilesAndFolders();

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
                        Folders.Add(new Folder { FolderName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory });
                    }
                }
            
        }

        internal void UpdateTreeView()
        {
            if (_MainWindow.TreeView1.SelectedItem != null)
            {

                IFolder item = _MainWindow.TreeView1.SelectedItem as IFolder;
                List<BDatumFiles> FileList = new List<BDatumFiles>();
                Storage storage = new Storage(_configuration);
                FileList = storage.EnumerateFilesAndFolders(item.FullPath);
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
                        Folders.Add(new Folder { FolderName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory });
                    }
                }
                
            }
         }


    }
}
