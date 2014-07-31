using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        public int Version { get; set; }

        public bool isVersion { get; set; }

        public string FileName { get; set; }
        
        public bool isFolder { get; set; }
        
        public DateTime Date { get; set; }

        public string Size { get; set; }

        public bool IsExpanded { get; set; }

        public bool isExpanded
        {
            get { return IsExpanded; }
            set
            {
                IsExpanded = value;
                NotifiyPropertyChanged("isExpanded");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public ObservableCollection<IFolder> m_folders;

        public ObservableCollection<IFolder> Folders
        {
            get { return m_folders; }
        }

        public bool isChildren { get; set; }

        public string Type { get; set; }
    }
}
