using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BdatumRestore.ViewModel
{
    public interface IFolder
    {
        string FullPath { get; }
        string FolderName { get; }
        bool isFolder { get; }
        List<IFolder> Folders { get; } 
    }
}
