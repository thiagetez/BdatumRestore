using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading;

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
            Thread thread = new Thread(new ThreadStart(() => _ListFolderInstance.UpdateTreeView(parameter)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
