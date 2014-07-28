using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BdatumRestore.ViewModel;
using System.Threading;

namespace BdatumRestore.Model
{
    /// <summary>
    /// Comando do download
    /// </summary>
    class DownloadCommandModel:ICommand
    {
        public delegate void DownloadStart();

        public DownloadCommandModel(ListFolder ListFolderInstance)
        {
            _ListFolder = ListFolderInstance;            
        }

        private ListFolder _ListFolder { get; set; }

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
            Thread thread = new Thread(new ThreadStart(() => _ListFolder.Download()));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            //_ListFolder.Download();            
        }
    }
}
