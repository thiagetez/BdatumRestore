using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BdatumRestore.ViewModel
{
    /// <summary>
    /// Interface dos Folder
    /// </summary>
    public interface IFolder
    {
        [JsonProperty(PropertyName = "FullPath")]
        string FullPath { get; }
        string FolderName { get; }
        string FileName { get; }
        bool isFolder { get; }
        ObservableCollection<IFolder> Folders { get; }
        bool isChildren { get; }
        DateTime Date { get; }
        string Size { get; }
        string Type { get; }
         [JsonProperty(PropertyName = "Version")]
        int Version { get; }
        bool isVersion { get; }

    }
}
