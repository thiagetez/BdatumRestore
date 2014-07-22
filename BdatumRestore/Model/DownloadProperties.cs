using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BdatumRestore.Model
{
    public class DownloadProperties
    {
        public bool isDirectory { get; set; }
        public string DirPath { get; set; }
        public List<DownloadProperties> DirectoryList { get; set; }
    }
}
