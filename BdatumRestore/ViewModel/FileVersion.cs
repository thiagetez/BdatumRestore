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
        /// <summary>
        /// Retorna as versões de um arquivo
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal List<BDatumFilesVersion> GetVersions(string path)
        {
            Storage storage = new Storage(_configuration);
           List<BDatumFilesVersion> versions= storage.GetVersions(path);
           return versions;
        }

    }
}
