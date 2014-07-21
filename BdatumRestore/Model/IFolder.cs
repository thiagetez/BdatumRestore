using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BdatumRestore.ViewModel
{
    public interface IFolder
    {
        string FullPath { get; }
        string FolderName { get; }
        string FileName { get; }
        bool isFolder { get; }
        ObservableCollection<IFolder> Folders { get; } 
    }
}
