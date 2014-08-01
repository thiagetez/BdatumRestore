using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BDatum.SDK.REST;

namespace BdatumRestore.ViewModel
{
    public class ErrorLogs
    {
        private int _ErrorCount { get; set; }
        private int _FileCount { get; set; }
        private Dictionary<string, Exception> _ErrorLog = new Dictionary<string, Exception>();

        public void CreateLogFile(int filecount, int errorcount)
        {
            string logpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\DownloadErrorLog.txt";
            string userlogpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\bdatum\Log\";

            if (!File.Exists(logpath))
            {
                using (File.Create(logpath)) { }
            }
            if (!Directory.Exists(userlogpath))
            {
                Directory.CreateDirectory(userlogpath);
            }
            if (!File.Exists(userlogpath))
            {
                using (File.Create(userlogpath+@"\DownloadErrorLog.txt")) { }
            }

            StreamWriter writer2 = new StreamWriter(userlogpath + @"\DownloadErrorLog.txt");
            StreamWriter writer = new StreamWriter(logpath);
            writer2.WriteLine(String.Format("Numero de arquivos solicitado para baixar: {0} \n Numero de erros encontrados: {1}\n Arquivos que falharam: \n\n ", filecount, errorcount));
            
            foreach (KeyValuePair<string, Exception> error in _ErrorLog)
            {
                writer.WriteLine(String.Format("Error in File: {0}\n Exception: {1}\n ",error.Key,error.Value));
                writer2.WriteLine(String.Format("{0}", error.Key));
            }

            writer.Close();
            writer2.Close();

            _ErrorLog.Clear();
        }
        public void CreateLogFile(Exception e)
        {
            string logpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\bdatum\UnknowErrors.txt";
            if (!File.Exists(logpath))
            {
                using (File.Create(logpath)) { }
            }
            lock (logpath)
            {
                StreamWriter writer = new StreamWriter(logpath);

                writer.WriteLine(String.Format("Error!!! \n\n Exception: {0}\n\n ", e));
            }
        }

        public void AddError(Exception e,string path)
        {

            _ErrorLog.Add(path, e);
        }
    }
}
