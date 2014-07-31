using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BdatumRestore.ViewModel;
using System.Threading;

namespace BdatumRestore.Model
{
    class ResumeCommandModel:ICommand
    {
        public ResumeCommandModel(ListFolder ListFolderInstance)
        {
            _ListFolder = ListFolderInstance;
            _PausedRestore = new PausedRestore();
        }

        private ListFolder _ListFolder { get; set; }
        private PausedRestore _PausedRestore { get; set; }


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
            List<IFolder> FilesToRestore= _PausedRestore.RestoreFiles();
            Thread thread = new Thread(new ThreadStart(() => _ListFolder.DownloadCore(FilesToRestore)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            
        }
    }
}
