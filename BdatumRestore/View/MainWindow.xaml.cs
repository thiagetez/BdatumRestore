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

namespace BdatumRestore.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListFolder _Update { get; set; }
        public FolderBrowserDialog browseDialog { get; set; }
        public System.Windows.Forms.IWin32Window win32Handle { get; set; }
        
        public MainWindow()
        {
            browseDialog = new FolderBrowserDialog();
            InitializeComponent();
            CenterWindowOnScreen();
            DataContext=new ListFolder(this);
            FilesExistLabel.Content = "";
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

    }
}
