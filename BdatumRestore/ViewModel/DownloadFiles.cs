using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.IO;
using BDatum.SDK;
using BDatum.SDK.REST;
using System.Windows.Threading;
using System.Threading.Tasks;
using BdatumRestore.Model;

namespace BdatumRestore.ViewModel
{

    public partial class ListFolder : INotifyPropertyChanged
    {

        private List<IFolder> _RetryFiles { get; set; }
        private int _RetryCount { get; set; }

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
                this._RetryCount = 0;
                #region Atualiza o View
                Application.Current.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             _MainWindow.RestoreButton.IsEnabled = false;
                             _MainWindow.ProgressLabel.Content = I18n.MessagesResource.PreparingDownload;
                             _MainWindow.progressBar1.Value = 0;
                             _MainWindow.TreeView1.IsEnabled = false;
                             _MainWindow.FileList.IsEnabled = false;
                         }));
                #endregion

                bool fileListNotNull = false;
                bool HaveSelectedItens = false;
                Application.Current.Dispatcher.Invoke(
                     DispatcherPriority.Background,
                      (Action)(() =>
                      {
                          if (_MainWindow.FileList != null)
                              fileListNotNull = true;
                          else
                              fileListNotNull = false;

                          if (_MainWindow.FileList.SelectedItems.Count != 0)
                              HaveSelectedItens = true;
                          else
                              HaveSelectedItens = false;

                      }));

                //Mostra seleção de pasta
                _MainWindow.ShowBrowseDialog();

                if (fileListNotNull == true && HaveSelectedItens == true && _MainWindow.browseDialog.SelectedPath != "")
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

                    #region Path excedeu 260 chars?
                    if (!CheckPathSize(filelist, _MainWindow.browseDialog.SelectedPath))
                    {
                        Application.Current.Dispatcher.Invoke(
                                   DispatcherPriority.Background,
                                      (Action)(() =>
                                      {
                                          MessageBox.Show("Alguns arquivos solicitado excedem o tamanho do caminho suportado pelo windows visite: http://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx para mais informações","Caminho Muito longo.");
                                      }));
                        throw new BDatumException(I18n.MessagesResource.PathTooLong);
                    }
                    #endregion

                    DownloadCore(filelist);
                }
                else if (_SelectedFolder != null && _MainWindow.browseDialog.SelectedPath != "")
                {
                    List<RemoteFile> filelist = _EnumDir.EnumFolderAndSubFoldersCache(_SelectedFolder.FullPath, _configuration);

                    List<IFolder> files = new List<IFolder>(filelist.Count);

                    Parallel.ForEach(filelist, line =>
                    {
                        files.Add(new Folder { FullPath = line.Name, isVersion = false });
                    });

                    #region Path excedeu 260 chars?
                    if (!CheckPathSize(files, _MainWindow.browseDialog.SelectedPath))
                    {
                        Application.Current.Dispatcher.Invoke(
                                   DispatcherPriority.Background,
                                      (Action)(() =>
                                      {
                                          MessageBox.Show("Alguns arquivos solicitado excedem o tamanho do caminho suportado pelo windows visite: http://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx para mais informações", "Caminho Muito longo.");

                                      }));
                        throw new BDatumException(I18n.MessagesResource.PathTooLong);
                    }
                    #endregion

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
            }
            catch (Exception e)
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
        internal void DownloadCore(List<IFolder> fileList)
        {
            _RetryFiles = new List<IFolder>();
            _Pause = false;
            Application.Current.Dispatcher.Invoke(
             DispatcherPriority.Background,
             (Action)(() =>
             {
                 _MainWindow.progressBar1.Maximum = fileList.Count;
                 _MainWindow.progressBar1.Value = 0;
                 _MainWindow.PauseButton.IsEnabled = true;
                 _MainWindow.RestoreButton.IsEnabled = false;
                 _MainWindow.ResumeButton.IsEnabled = false;
             }));

            int filescount = 0;

            _ErrorCount = 0;
            _ErrorOcurred = false;

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
                        if (line.isVersion == false)
                            storage.Download(line.FullPath, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(line.FullPath));
                        else
                            storage.Download(line.FullPath, _MainWindow.browseDialog.SelectedPath + Path.GetDirectoryName(line.FullPath), line.Version);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                     (Action)(() =>
                                     {
                                         DetailList.Add(new DownloadDetailsProperties { FileName = Path.GetFileName(line.FullPath), Status = "Baixado" });
                                     }));
                    }
                    catch (Exception e)
                    {
                        if (line != null) { _ErrorLogs.AddError(e, line.FullPath); _ErrorOcurred = true; _ErrorCount++; if (!e.InnerException.InnerException.Message.Contains("404")) _RetryFiles.Add(line); }
                        else
                         _ErrorLogs.CreateLogFile(e);
                        _ErrorOcurred = true; _ErrorCount++;
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                            (Action)(() =>
                            {
                                DetailList.Add(new DownloadDetailsProperties { FileName = Path.GetFileName(line.FullPath), Status = "Erro no Download" });
                            }));
                    }
                    finally
                    {
                        FileList.Remove(line);
                        if (line != null)
                        {
                            if (_Pause != true)
                            {
                                _MainWindow.ProgressLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.ProgressLabel.Content = String.Format("Arquivo:{0} {1}/{2}", Path.GetFileName(line.FullPath), filescount + 1, fileList.Count)));
                                Application.Current.Dispatcher.Invoke(
                                    DispatcherPriority.Background,
                                       (Action)(() =>
                                       {
                                           _MainWindow.ProgressLabel.Content = Path.GetFileName(line.FullPath);
                                           _MainWindow.ProgressCountLabel.Content = String.Format("{0}/{1}", filescount + 1, fileList.Count);
                                           _MainWindow.progressBar1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => _MainWindow.progressBar1.Value++));

                                       }));
                            }
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

            if (_ErrorOcurred == false && _Pause == false)
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
                    if (_RetryFiles.Count > 0 && _RetryCount==0)
                    {
                        _RetryCount++;
                        _ErrorLogs.CreateLogFile(filescount, _ErrorCount);
                        DownloadCore(_RetryFiles);
                    }
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
            if (File.Exists(pausedpath) && _Pause == false)
                File.Delete(pausedpath);
            isBusy = false;

        }

        internal void PauseDownload()
        {
            MessageBoxResult result = MessageBox.Show(I18n.MessagesResource.PausedDownloadMessage, "Interromper restore?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _MainWindow.ResumeButton.IsEnabled = true;
                _Pause = true;
                _State.Stop();
                _Options.CancellationToken.ThrowIfCancellationRequested();
                _MainWindow.RestoreButton.IsEnabled = true;
                _MainWindow.PauseButton.IsEnabled = false;
                _MainWindow.TreeView1.IsEnabled = true;
                _MainWindow.FileList.IsEnabled = true;
                isBusy = false;
                List<Folder> list = new List<Folder>();
                do
                {
                    Thread.Sleep(500);
                } while (_State.IsStopped == false);
                PausedRestore paused = new PausedRestore();
                lock (FileList)
                {
                    foreach (IFolder file in FileList)
                    {
                        list.Add(new Folder { FullPath = file.FullPath, Version = file.Version, isVersion = file.isVersion });
                    }
                }
                paused.CreateRestoreFiles(list);
                isBusy = false;
            }
        }
    }
}
