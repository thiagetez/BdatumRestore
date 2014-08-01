using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BdatumRestore.View;
using BdatumRestore.ViewModel;
using System.Windows.Forms;
using BdatumRestore.Model;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;

namespace BdatumRestore.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListFolder _Listfolder { get; set; }
        public FolderBrowserDialog browseDialog { get; set; }
        public System.Windows.Forms.IWin32Window win32Handle { get; set; }
        private static String _mutexID = "{a8b65a4f-9ffb-46fd-a432-bdd3338c423e}";

        public MainWindow()
        {
            browseDialog = new FolderBrowserDialog();
            InitializeComponent();
            CenterWindowOnScreen();

            Mutex mutex = new Mutex(true, _mutexID);
            bool wait = false;
            wait = mutex.WaitOne(TimeSpan.Zero, wait);
            if (!wait)
            {
                System.Windows.MessageBox.Show("Você so pode ter um programa rodando por vez.");
                this.Close();
            }
            DataContext=new ListFolder(this);
            FilesExistLabel.Content = "";
            PauseButton.IsEnabled = false;
            VersionButton.Visibility = Visibility.Hidden;
            ResumeButton.IsEnabled = false;
            BackButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Método para mostrar o FolderBrowserDialog
        /// </summary>
        public void ShowBrowseDialog()
        {
            
                System.Windows.Application.Current.Dispatcher.Invoke(
                         DispatcherPriority.Background,
                         (Action)(() =>
                         {
                             win32Handle = new WindowWrapper(new WindowInteropHelper(this).Handle);
                         }));
                browseDialog.Description = "Escolha a pasta para salvar os arquivos.";
            browseDialog.ShowDialog(win32Handle);
        }
        /// <summary>
        /// Método para centralizar a tela
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext != null)
            {
                _Listfolder = DataContext as ListFolder;

                if (_Listfolder.isBusy == true)
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Deseja pausar o download e sair?", "Pausar download?", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        _Listfolder.PauseDownload();

                    }
                }
            }
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Listfolder = DataContext as ListFolder;
            System.Windows.Controls.ListBox listbox = sender as System.Windows.Controls.ListBox;
            _Listfolder.SelectionChanged(listbox, e);
        }

    }
}
