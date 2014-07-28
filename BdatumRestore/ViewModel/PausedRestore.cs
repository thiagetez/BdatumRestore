using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace BdatumRestore.ViewModel
{
    /// <summary>
    /// Cria estrutura para fazer retomar o restore
    /// </summary>
    public class PausedRestore
    {

        public void CreateRestoreFiles(List<string> Files)
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

        public List<string> RestoreFiles()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
            StreamReader reader = new StreamReader(path);
            List<string> listFiles = JsonConvert.DeserializeObject<List<string>>(reader.ReadToEnd());

            return listFiles;
        }
    }
}
