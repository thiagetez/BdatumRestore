using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdatumRestore.ViewModel
{
    public partial class ListFolder
    {
        public bool CheckPathSize(List<IFolder> fileList,string localPath)
        {
            int longPathsCount = 0;

            Parallel.ForEach(fileList, _Options, (line, State) =>
            {
                string path = localPath + line.FullPath;
                if (path.Length > 260)
                {
                    longPathsCount++;
                }
            });

            if (longPathsCount > 0)
                return false;

            return true;
        }
    }
}
