using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BdatumRestore.ViewModel;

namespace BdatumRestore.Model
{
    class VersionsCommandModel:ICommand
    {
          private ListFolder _ListFolderInstance { get; set; }

          public VersionsCommandModel(ListFolder Handler)
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
            
            _ListFolderInstance.GetFileVersion();
        }
    }
}
