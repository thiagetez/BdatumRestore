using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using BDatum.SDK.REST;

namespace BdatumRestore.ViewModel
{
    /// <summary>
    /// Cria estrutura para fazer retomar o restore
    /// </summary>
    public class PausedRestore
    {

        public void CreateRestoreFiles(List<RemoteFile> Files)
        {
            string output = JsonConvert.SerializeObject(Files);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            if (!File.Exists(path))
            {
                using (File.Create(path)) { }                
            }
            else
            {
                File.Delete(path);
                using (File.Create(path)) { }
                
            }
            StreamWriter file = new StreamWriter(path);
            file.WriteLine(output);
            file.Close();
        }

        public List<RemoteFile> RestoreFiles()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            StreamReader reader = new StreamReader(path);
            List<RemoteFile> listFiles = JsonConvert.DeserializeObject<List<RemoteFile>>(reader.ReadToEnd());

            return listFiles;
        }
    }
}
