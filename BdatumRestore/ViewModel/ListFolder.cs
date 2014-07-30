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

        #region private properties
        /// <summary>
        /// Instancia da classe ErrorLogs
        /// </summary>
        private ErrorLogs _ErrorLogs = new ErrorLogs();

        private ParallelOptions _Options=new ParallelOptions();

        private ParallelLoopState _State;

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

        #endregion

        #region public properties
        
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
        
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        //public static string currentPath = String.Format("{0}{1}", Path.GetTempPath(), "UploadFileTest");
        

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
            _EnumDir=new EnumDirectories();
            m_folders = new ObservableCollection<IFolder>();
            m_files = new ObservableCollection<IFolder>();
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

                _FolderSelected = "Pasta selecionada: "+value;
                NotifiyPropertyChanged("FolderSelected");
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

                ////////TEST ONLY
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
                this._Pause = false;
                Application.Current.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                         (Action)(() =>
                            {
                              _MainWindow.RestoreButton.IsEnabled = false;
                              _MainWindow.ProgressLabel.Content =I18n.MessagesResource.PreparingDownload;
                              _MainWindow.progressBar1.Value =0;
                              _MainWindow.TreeView1.IsEnabled = false;
                              _MainWindow.FileList.IsEnabled = false;
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
                    List<IFolder> filelist = new List<IFolder>();

                        Application.Current.Dispatcher.Invoke(
                         DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             foreach (IFolder item in _MainWindow.FileList.SelectedItems)
                             {

                                 filelist.Add(item);
                             }

                         }));

                        DownloadCore(filelist);
                }
                else if (_SelectedFolder != null && _MainWindow.browseDialog.SelectedPath != "")
                {
                    List<RemoteFile> filelist = _EnumDir.EnumFolderAndSubFoldersCache(_SelectedFolder.FullPath, _configuration);

                    List<IFolder> files = new List<IFolder>();

                    Parallel.ForEach(filelist, line =>
                    {
                        files.Add(new Folder {FullPath=line.Name,isVersion=false });
                    });

                    DownloadCore(files);
                }
                else
                {
                    _MainWindow.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => MessageBox.Show(_MainWindow, I18n.MessagesResource.NoDirOrFileSelected, I18n.MessagesResource.DownloadErrorGeneric)));
                    _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = I18n.MessagesResource.DownloadErrorGeneric));
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                    _MainWindow.TreeView1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.TreeView1.IsEnabled = true));
                    _MainWindow.FileList.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.FileList.IsEnabled = true));
                    isBusy = false;
                }
          }catch (Exception e)
             {
                 _ErrorLogs.CreateLogFile(e);
                 _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = I18n.MessagesResource.DownloadErrorGeneric));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                _MainWindow.TreeView1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.TreeView1.IsEnabled = true));
                _MainWindow.FileList.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.FileList.IsEnabled = true));
                isBusy = false; 
          }
        }

        /// <summary>
        /// Faz Download dos arquivos
        /// </summary>
        /// <param name="Files"></param>
        private void DownloadCore(List<IFolder> fileList )
        {

            Application.Current.Dispatcher.Invoke(
             DispatcherPriority.Background,
             (Action)(() =>
             {
                 _MainWindow.progressBar1.Maximum = fileList.Count;
                 _MainWindow.PauseButton.IsEnabled = true;
                 _MainWindow.RestoreButton.IsEnabled = false;
             }));

            int filescount = 0;
            

            //Cria lista de arquivos para salvar caso ocorra algum erro
            //ou a operação seja pausada

            FileList = new List<IFolder>(fileList);

            Parallel.ForEach(fileList, _Options, (line, State) =>
            {
                _State = State;
                if (_Pause == false)
                {
                Storage storage = new Storage(_configuration);
               
                    //Usar try para pegar a exceção sem parar a fila de download.
                    try 
                    {
                        if(line.isVersion==false)
                        storage.Download(line.FullPath, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(line.FullPath));
                        else
                            storage.Download(line.FullPath, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(line.FullPath), line.Version);
                    }
                    catch (Exception e) { _ErrorLogs.AddError(e, line.FullPath); _ErrorOcurred = true; _ErrorCount++; }
                    finally
                    {
                        FileList.Remove(line);
                        if (_Pause != true)
                        {
                            _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = String.Format("Arquivo:{0} {1}/{2}", Path.GetFileName(line.FullPath), filescount + 1, fileList.Count)));
                            Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Background,
                                   (Action)(() =>
                                   {

                                       _MainWindow.progressBar1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.progressBar1.Value++));

                                   }));
                        }
                        filescount++;
                    }
                }
                else
                {
                    _State.Break(); 
                    _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download Pausado"));                  
                }
            });                      

            if (_ErrorOcurred == false && _Pause==false )
            {
                _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download completo"));
                _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                _MainWindow.TreeView1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.TreeView1.IsEnabled = true));
                _MainWindow.FileList.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.FileList.IsEnabled = true));
                _MainWindow.PauseButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.PauseButton.IsEnabled = false));
                isBusy = false;
            }
            else
            {
                if (_Pause == false)
                {
                    _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download incompleto, verifique o log em Documents/bdatum/Logs ou contacte o suporte."));
                    _MainWindow.RestoreButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.RestoreButton.IsEnabled = true));
                    _MainWindow.TreeView1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.TreeView1.IsEnabled = true));
                    _MainWindow.FileList.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.FileList.IsEnabled = true));
                    _ErrorLogs.CreateLogFile(filescount, _ErrorCount);
                    isBusy = false;
                }
                else
                {
                    isBusy = false;
                    _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = "Download Pausado"));
                }
             }

            string pausedpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            if (File.Exists(pausedpath) && _Pause==false)
                File.Delete(pausedpath);
            isBusy = false;
            
        }

        internal void PauseDownload()
        {
            MessageBoxResult result = MessageBox.Show(I18n.MessagesResource.PausedDownloadMessage, "Interromper restore?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _Pause = true;                
                _State.Stop();
                _Options.CancellationToken.ThrowIfCancellationRequested();
                List<Folder> list= new List<Folder>();
                _MainWindow.RestoreButton.IsEnabled = true;
                _MainWindow.PauseButton.IsEnabled = false;
                _MainWindow.TreeView1.IsEnabled = true;
                _MainWindow.FileList.IsEnabled = true;
                isBusy = false;
                do
                {
                    Thread.Sleep(500);
                } while (_State.IsStopped == false);
                PausedRestore paused = new PausedRestore();
                lock (FileList)
                {
                    foreach (IFolder file in FileList)
                    {
                        list.Add(new Folder{FullPath=file.FullPath,Version=file.Version,isVersion=file.isVersion});
                    }
                }
                    paused.CreateRestoreFiles(list);
                    isBusy = false;
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
                        Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2], FullPath = file.FullName, isFolder = file.IsDirectory });
                    }
                    else
                    {
                        string[] dirname = file.FullName.Split('/');
                        Files.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory, Size = "Tamanho: " + file.Size.ToString(), Date = file.TimeSpan, Type = file.MimeType });
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(I18n.MessagesResource.ConectionOrAuthError,"Fatal Error");
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
                                item.Folders.Add(new Folder { FolderName = dirname[dirname.Length - 2], FullPath = file.FullName, isFolder = file.IsDirectory, isChildren = true });
                            }
                        }
                        else
                        {
                            _ItensCount++;
                            string[] dirname = file.FullName.Split('/');
                            //Folders[index].Folders.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory,isChildren=true });
                            Files.Add(new Folder { FileName = dirname[dirname.Length - 1], FullPath = file.FullName, isFolder = file.IsDirectory, Size ="Tamanho: "+file.Size.ToString(), Date = file.TimeSpan, Type = file.MimeType });
                        }

                    }
                    if (_ItensCount == 0)
                    {
                        _MainWindow.FilesExistLabel.Content = I18n.MessagesResource.NoFileFound;
                    }
                    else
                        _MainWindow.FilesExistLabel.Visibility = Visibility.Hidden;
                
            
            
         }
        public void SelectionChanged(ListBox sender, SelectionChangedEventArgs e)
        {
            IFolder desc = sender.SelectedItem as IFolder;
            if (desc != null)
            {
                _MainWindow.FileType.Content = "Tipo: " + desc.Type;
                _MainWindow.FileSize.Content = desc.Size;
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

        public void GetFileVersion()
        {
           List<BDatumFilesVersion> versions= GetVersions(_SelectedFile.FullPath);
           Files.Clear();
            _ItensCount=1;
           foreach (BDatumFilesVersion file in versions)
           {       
                   //folders[index].folders.add(new folder { filename = dirname[dirname.length - 1], fullpath = file.fullname, isfolder = file.isdirectory,ischildren=true });
                   Files.Add(new Folder { FileName =_ItensCount.ToString() , FullPath = _SelectedFile.FullPath, isFolder = false, Size = "tamanho: " + file.Size.ToString(), Date = file.TimeStamp, Type = _SelectedFile.Type,Version=(int)file.Version,isVersion=true });
                   _ItensCount++;
           }
           _MainWindow.VersionButton.Visibility = Visibility.Hidden;
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
