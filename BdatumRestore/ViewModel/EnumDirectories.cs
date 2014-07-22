using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BdatumRestore.ViewModel
{

    using BDatum.SDK;
    using BDatum.SDK.REST;
    using System.IO;
    using BdatumRestore.Model;
    public class EnumDirectories
    {
        private List<IFolder> _EnumeratedFiles = new List<IFolder>();
        private List<string> _FilesToDownload;
        private DownloadProperties _DirProperties = new DownloadProperties();

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
             Storage storage = new Storage(configuration);
             
                 FileList = storage.EnumerateFilesAndFolders(initPath);

                 //Melhorar Esse bloco de codigo!!!!!!!
                 #region  foreach block

                 do
                 {
                     FileList = storage.EnumerateFilesAndFolders(initPath);
                     if (FileList != null)
                     {

                         foreach (BDatumFiles directory in FileList)
                         {
                             if (directory.IsDirectory)
                             {
                                 _DirProperties.isDirectory = true;
                                 _DirProperties.DirPath = directory.FullName;

                             }
                             else
                                 _FilesToDownload.Add(directory.FullName);

                         }
                     }
                 } while (FileList != null );

                 #endregion

             

                return _EnumeratedFiles;
        }
    }
}
