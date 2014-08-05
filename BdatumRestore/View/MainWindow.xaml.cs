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
using System.Diagnostics;
using EQATEC.Analytics.Monitor; 

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

        public static Process SingleInstance()
    // Returns a System.Diagnostics.Process pointing to
    // a pre-existing process with the same name as the
    // current one, if any; or null if the current process
    // is unique.
    {
      Process curr = Process.GetCurrentProcess();
      Process[] procs = Process.GetProcessesByName(curr.ProcessName);
      foreach (Process p in procs)
      {
        if ((p.Id != curr.Id) &&
            (p.MainModule.FileName == curr.MainModule.FileName))
                return p;
      }
      return null;
    }



        public MainWindow()
        {
            browseDialog = new FolderBrowserDialog();
            InitializeComponent();
            CenterWindowOnScreen();

            if (SingleInstance()!=null)
            {
                System.Windows.MessageBox.Show("Você pode ter apenas um programa rodando por vez.");
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
