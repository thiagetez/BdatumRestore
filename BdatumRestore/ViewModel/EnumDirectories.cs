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
        private List<string> _FilesToDownload=new List<string>();
        private List<DownloadProperties> _DirProperties = new List<DownloadProperties>();

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

        public List<string> EnumFolderAndSubFolders(string initPath, IConfiguration configuration)
        {
            List<BDatumFiles> FileList=new List<BDatumFiles>();
             Storage storage = new Storage(configuration);
             
                 FileList = storage.EnumerateFilesAndFolders(initPath);

                 //Melhorar Esse bloco de codigo!!!!!!!
                 #region  foreach block

                 do
                 {
                     
                     if (FileList != null && FileList.Count != 0)
                     {

                         for(int i=0;i<FileList.Count;i++)
                         {
                             BDatumFiles directory = (BDatumFiles)FileList[i];
                             if (directory.IsDirectory)
                             {
                                 _DirProperties.Add(new DownloadProperties {isDirectory=true, DirPath=directory.FullName });
                              
                             }
                             else
                                 _FilesToDownload.Add(directory.FullName);
                         }
                     }
                     FileList.Clear();
                     if(_DirProperties.Count>0)
                     {
                         FileList = storage.EnumerateFilesAndFolders(_DirProperties[0].DirPath);
                         _DirProperties.Remove(_DirProperties[0]);
                     }

                 } while (FileList.Count != 0 );

                 #endregion

             

                return _FilesToDownload;
        }
    }
}
