using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace BdatumRestore.ViewModel
{
    /// <summary>
    /// Comando de Update da TreeView
    /// </summary>
    public class UpdateTreeViewCommandModel:ICommand
    {
        private ListFolder _ListFolderInstance { get; set; }

        public UpdateTreeViewCommandModel(ListFolder Handler)
        {
            _ListFolderInstance = Handler;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            //TreeViewItem tv = new TreeViewItem();
            //tv.IsExpanded = true;
            _ListFolderInstance.UpdateTreeView(parameter);
        }
    }
}
