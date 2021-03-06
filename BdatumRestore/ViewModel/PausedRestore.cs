﻿using System;
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
        /// <summary>
        /// Cria o arquivo de restore
        /// </summary>
        /// <param name="Files"></param>
        public void CreateRestoreFiles(List<Folder> Files)
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

        public List<IFolder> RestoreFiles()
        {
            try
            {

                string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\RestoreFileList.json";
                StreamReader reader = new StreamReader(path);
                List<Folder> listFiles = JsonConvert.DeserializeObject<List<Folder>>(reader.ReadToEnd());
                List<IFolder> files = new List<IFolder>();
                foreach (Folder list in listFiles)
                {
                    files.Add(new Folder { FullPath = list.FullPath, Version = list.Version, isVersion = list.isVersion });
                }
                
                return files;
            }
            catch (Exception e)
            {
                ErrorLogs error = new ErrorLogs();
                error.CreateLogFile(e);
                return null;
            }
        }
    }
}
