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
            DataContext=new ListFolder(this);
        }

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
    
                _Update = DataContext as ListFolder;
                _Update.UpdateCommand.Execute(null);
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
                _Update = DataContext as ListFolder;
                _Update.UpdateCommand.Execute(null);
            
        }

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

       
       
    }
}
