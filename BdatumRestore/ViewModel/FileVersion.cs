using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BDatum.SDK;
using BDatum.SDK.REST;

namespace BdatumRestore.ViewModel
{

    public partial class ListFolder : INotifyPropertyChanged
    {
        internal List<BDatumFilesVersion> GetVersions(string path)
        {
            Storage storage = new Storage(_configuration);
           List<BDatumFilesVersion> versions= storage.GetVersions(path);
           return versions;
        }

    }
}
