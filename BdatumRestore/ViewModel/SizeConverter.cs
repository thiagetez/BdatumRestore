using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BdatumRestore.ViewModel
{
    public class SizeConverter
    {
        public string Conversor(string size)
        {
            if (!size.Contains("B") && !size.Contains("b"))
            {
                double sourceSize = double.Parse(size);

                if (sourceSize < 1024)
                    return String.Format("{0} bytes", sourceSize);
                else if (sourceSize < 1048576)
                {
                    string kb = (sourceSize / 1024).ToString();
                    kb = kb.Substring(0, kb.IndexOf(",") + 2);
                    return String.Format("{0} KB", kb);
                }
                else if (sourceSize < 1073741824)
                {
                    string mb = (sourceSize / 1048576).ToString();
                    mb = mb.Substring(0,mb.IndexOf(",")+2);
                    return String.Format("{0} MB", mb);
                }
                else
                {
                    string gb = (sourceSize / 1073741824).ToString();
                    gb = gb.Substring(0, gb.IndexOf(",") + 2);
                    return String.Format("{0} GB", gb);
                }
            }

            return size;
        }
    }
}
