using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration;


namespace BdatumRestore.ViewModel
{
    using Newtonsoft.Json;
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
    using System.Threading.Tasks;

    //Classe que lista as pastas

    public partial class ListFolder:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region private properties
        /// <summary>
        /// Instancia da classe ErrorLogs
        /// </summary>
        private ErrorLogs _ErrorLogs = new ErrorLogs();

        /// <summary>
        /// Lista de Arquivos
        /// </summary>
        private ObservableCollection<IFolder> m_files;

        private ParallelOptions _Options=new ParallelOptions();

        private ParallelLoopState _State;

        /// <summary>
        ///Lista de diretórios
        /// </summary>
        private ObservableCollection<IFolder> m_folders;

        /// <summary>
        /// Path do folder que foi selecionado
        /// </summary>
        private string _FolderSelected { get; set; }

        /// <summary>
        /// Caminho do arquivo selecionado
        /// </summary>
        private IFolder _SelectedFile { get; set; }

        /// <summary>
        /// Instancia da interface IConfiguration para configurar a conexão
        /// </summary>
        private static IConfiguration _configuration { get; set; }

        /// <summary>
        /// Instancia do EnumDirectories para fazer a enumeração dos arquivos
        /// nas pastas e subpastas
        /// </summary>
        private EnumDirectories _EnumDir { get; set; }

        /// <summary>
        /// Contador de diretórios
        /// </summary>
        private int _FoldersCount { get; set; }

        /// <summary>
        /// Contador de arquivos com erros
        /// </summary>
        private int _ErrorCount { get; set; }

        /// <summary>
        /// Contem todas as informações do item da TreeView selecionada
        /// </summary>
        private IFolder _SelectedFolder { get; set; }

        /// <summary>
        /// Contador de itens
        /// </summary>
        private int _ItensCount { get; set; }

        /// <summary>
        /// Define se o usuario pausou a operação
        /// </summary>
        private bool _Pause { get; set; }

        /// <summary>
        /// Define se um erro ocorreu
        /// </summary>
        private bool _ErrorOcurred { get; set; }

        /// <summary>
        /// Instancia do Window
        /// </summary>
        private static MainWindow _MainWindow { get; set; }

        /// <summary>
        /// Lista dos detalhes
        /// </summary>
        private ObservableCollection<DownloadDetailsProperties> _DetailList { get; set; }

        #endregion

        #region public properties

        /// <summary>
        /// Propriedade com as informações dos arquivos
        /// </summary>
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
        /// Propriedade com as informações da pasta selecionada
        /// </summary>
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

        public ObservableCollection<IFolder> Folders
        {
            get { return m_folders; }
            set
            {
                m_folders = value;
                NotifiyPropertyChanged("Folders");
            }
        }

        /// <summary>
        /// Propriedade da pasta selecionada
        /// </summary>
        public string FolderSelected
        {
            get
            {
                return _FolderSelected;
            }
            set
            {

                _FolderSelected = "Pasta selecionada: " + value;
                NotifiyPropertyChanged("FolderSelected");
            }
        }
        
        /// <summary>
        /// Lista de arquivos
        /// </summary>
        public List<IFolder> FileList;

        /// <summary>
        /// Define se o restore esta sendo executado
        /// </summary>
        public bool isBusy { get; set; }

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
        /// Comando de Versão
        /// </summary>
        public ICommand VersionCommand { get; set; }

        /// <summary>
        /// Comando de Resume
        /// </summary>
        public ICommand ResumeCommand { get; set; }

        /// <summary>
        /// Detalhes do status do arquivo
        /// </summary>
        public ObservableCollection<DownloadDetailsProperties> DetailList
        {
            get
            {
                return _DetailList;
            }
            set
            {
                _DetailList = value;
                NotifiyPropertyChanged("DetailList");
            }
        }

        #endregion


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
            VersionCommand = new VersionsCommandModel(this);
            ResumeCommand = new ResumeCommandModel(this);
            _EnumDir=new EnumDirectories();
            m_folders = new ObservableCollection<IFolder>();
            m_files = new ObservableCollection<IFolder>();
            DetailList =new ObservableCollection<DownloadDetailsProperties>();
            //TODO: listar pastas e arquivos em abas separadas
            AuthenticationMethod();
            CheckPausedRestore();
            InitTreeView();
        }

        //Refazer esse metodo para baixar por versao tbm
        private void CheckPausedRestore()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            if (File.Exists(path))
            {
                
                MessageBoxResult msg= MessageBox.Show(I18n.MessagesResource.PendingRestore, "Restore pendente encontrado", MessageBoxButton.YesNo);

                if (msg == MessageBoxResult.Yes)
                {
                    PausedRestore pause = new PausedRestore();

                    List<IFolder> list = pause.RestoreFiles();

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
        /// Método de Autenticação
        /// </summary>
        public static void AuthenticationMethod()
        {
            try
            {
                IConnectionConfiguration _connection = new ConnectionConfiguration();
                _connection.UserAgent = String.Format("{0} ({1})", "BDatum-Restore/1.0.0", Environment.OSVersion.ToString());
                //_connection.Proxy = new System.Net.WebProxy("127.0.0.1", 8888);

                string AuthPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum";
                FileStream fs=new FileStream(AuthPath+@"\settings.json",FileMode.Open,FileAccess.Read);
                var reader=new StreamReader(fs);
                string json=reader.ReadToEnd();
                AuthenticationProperties auth = JsonConvert.DeserializeObject<AuthenticationProperties>(json);

                ////////TEST ONLY
                //IAuthentication _authentication = new Authentication("NEKSP7b6hoktg4nIg5u4", "XGzzNadCLjXFpQjTuJYF");

                //IAuthentication _authentication = new Authentication("6uAazfsYFez56nRYGyfZ", "XGzzNadCLjXFpQjTuJYF");

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
                        Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2], FullPath = file.FullName, isFolder = file.IsDirectory,isExpanded=true });
                    }
                    else
                    {
                        string[] dirname = file.FullName.Split('/');
                        Files.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory, Size =file.Size.ToString(), Date = file.TimeSpan, Type = file.MimeType });
                    }

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(I18n.MessagesResource.ConectionOrAuthError,"Fatal Error");
                ErrorLogs error = new ErrorLogs();
                error.CreateLogFile(e);
                _MainWindow.Close();
            }
            
        }

        /// <summary>
        /// Atualiza a TreeView com as pastas e/ou arquivos
        /// </summary>
        /// <param name="fileItem">Folder para navegar</param>
        internal void UpdateTreeView(object fileItem)
        {
                bool isListed = false;
                //Folder test = fileItem as Folder;

            if(fileItem!=null)
                SelectedFolder = fileItem as IFolder;

              #region TODO: Arrumar a expansão dos itens da TreeView
                if (SelectedFolder.isExpanded == true)
                    SelectedFolder.isExpanded = false;
                else
                    SelectedFolder.isExpanded = true;
             #endregion  

                
                if (SelectedFolder.Folders.Count != 0)
                {
                    isListed = true;
                }
                    List<BDatumFiles> FileList = new List<BDatumFiles>();
                    Storage storage = new Storage(_configuration);
                    FileList = storage.EnumerateFilesAndFolders(SelectedFolder.FullPath);
                    _FoldersCount = Folders.Count();
                    int index = Folders.IndexOf(SelectedFolder);
                    //m_folders.Clear();

                    #region remove os arquivos da lista

                    FileListClear();

                    #endregion

                    SizeConverter converter = new SizeConverter();
                    foreach (BDatumFiles file in FileList)
                    {
                        if (file.IsDirectory)
                        {
                            if (isListed == false)
                            {
                                string[] dirname = file.FullName.Split('/');
                                //Folders[index].Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2]+@"\", FullPath = file.FullName, isFolder = file.IsDirectory,isChildren=true});
                                SelectedFolder.Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2], FullPath = file.FullName, isFolder = file.IsDirectory, isChildren = true });
                            }
                        }
                        else
                        {
                            _ItensCount++;
                            string[] dirname = file.FullName.Split('/');
                            //Folders[index].Folders.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory,isChildren=true });
                            Files.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory, Size = converter.Conversor(file.Size.ToString()), Date = file.TimeSpan, Type = file.MimeType });
                        }

                    }
                    if (_ItensCount == 0)
                    {
                        _MainWindow.FilesExistLabel.Content = I18n.MessagesResource.NoFileFound;
                    }
                    else
                        _MainWindow.FilesExistLabel.Visibility = Visibility.Hidden;
                    _MainWindow.BackButton.Visibility = Visibility.Hidden;
            
            
         }

        /// <summary>
        /// Mostra os detalhes do arquivo selecionado   
        /// </summary>
        /// <param name="sender">File passado pelo item da ListBox</param>
        /// <param name="e">Evento de seleção</param>
        public void SelectionChanged(ListBox sender, SelectionChangedEventArgs e)
        {
            IFolder desc = sender.SelectedItem as IFolder;
            SizeConverter converter=new SizeConverter();

            if (desc != null)
            {
                _MainWindow.FileType.Content = "Tipo: " + desc.Type;
                _MainWindow.FileSize.Content = "Tamanho: "+converter.Conversor(desc.Size);
                _MainWindow.FileDate.Content = "Data: " + desc.Date.ToString();
                _MainWindow.VersionButton.Visibility = Visibility.Visible;
                _SelectedFile =desc;
            }
            else
            {
                _MainWindow.FileType.Content = "";
                _MainWindow.FileSize.Content = "";
                _MainWindow.FileDate.Content = "";
                _MainWindow.VersionButton.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Mostra as versões do arquivo selecionado
        /// </summary>
        public void GetFileVersion()
        {
           List<BDatumFilesVersion> versions= GetVersions(_SelectedFile.FullPath);
           Files.Clear();
            _ItensCount=1;
           foreach (BDatumFilesVersion file in versions)
           {       
                   //folders[index].folders.add(new folder { filename = dirname[dirname.length - 1], fullpath = file.fullname, isfolder = file.isdirectory,ischildren=true });
                   Files.Add(new Folder { FileName =_ItensCount.ToString() , FullPath = _SelectedFile.FullPath, isFolder = false, Size = file.Size.ToString(), Date = file.TimeStamp, Type = _SelectedFile.Type,Version=(int)file.Version,isVersion=true });
                   _ItensCount++;
           }
           _MainWindow.VersionButton.Visibility = Visibility.Hidden;
           _MainWindow.BackButton.Visibility = Visibility.Visible;
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
                _MainWindow.FilesExistLabel.Content = I18n.MessagesResource.NoFileFound;
                _MainWindow.FilesExistLabel.Visibility = Visibility.Visible;
            }
        }


        
    }
}
