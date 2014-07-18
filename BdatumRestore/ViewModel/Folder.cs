using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BdatumRestore.ViewModel
{
    /// <summary>
    /// Classe que implementa a interface IFolder
    /// </summary>
    public class Folder:IFolder
    {
        public Folder()
        {
            m_folders = new List<IFolder>();
        }
        public string FullPath { get; set; }

        public string FolderName { get; set; }

        public bool isFolder { get; set; }

        private readonly List<IFolder> m_folders;
        public List<IFolder> Folders
        {
            get { return m_folders; }
        }
    }
}
