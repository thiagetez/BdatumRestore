using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BdatumRestore.ViewModel
{

    using BDatum.SDK;
    using BDatum.SDK.REST;
    using System.IO;

    public class EnumDirectories
    {
        private List<IFolder> _EnumeratedFiles = new List<IFolder>();
        private List<IFolder> EnumeratedFiles
        {
            get
            {
                return _EnumeratedFiles;
            }
            set
            {
                _EnumeratedFiles = value;
            }
        }

        public List<IFolder> EnumFolderAndSubFolders(string initPath, IConfiguration configuration)
        {
            List<BDatumFiles> FileList=new List<BDatumFiles>();
            Dictionary<string,BDatumFiles> dir = new Dictionary<string,BDatumFiles>();
             Storage storage = new Storage(configuration);
             
                 FileList = storage.EnumerateFilesAndFolders(initPath);

                 //Melhorar Esse bloco de codigo!!!!!!!
                 #region  foreach block

                 foreach (BDatumFiles directory in FileList)
                 {
                     if (directory.IsDirectory)
                         dir.Add(directory.FullName,directory);
                 }

                 //remover os diretorios da lista
                 foreach(KeyValuePair<string,BDatumFiles> removeDir in dir)
                 {
                     FileList.Remove(removeDir.Value);
                 }

                 foreach (BDatumFiles file in FileList)
                 {
                     _EnumeratedFiles.Add((IFolder)file);
                 }

                 while (dir.Count > 0)
                 {
                     FileList.Clear();
                     FileList = storage.EnumerateFilesAndFolders(dir.Keys.ToString());
                     foreach (BDatumFiles file in FileList)
                     {
                         _EnumeratedFiles.Add((IFolder)file);
                     }
                 }
                   

                 #endregion

             

                return _EnumeratedFiles;
        }
    }
}
