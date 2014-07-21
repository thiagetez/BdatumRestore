using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BdatumRestore.ViewModel
{
    /// <summary>
    /// Classe que implementa a interface IFolder
    /// </summary>
    public class Folder:IFolder
    {
        public Folder()
        {
            m_folders = new ObservableCollection<IFolder>();
        }
        public string FullPath { get; set; }

        public string FolderName { get; set; }
        public string FileName { get; set; }
        public bool isFolder { get; set; }

        public ObservableCollection<IFolder> m_folders;

        public ObservableCollection<IFolder> Folders
        {
            get { return m_folders; }
        }
    }
}
