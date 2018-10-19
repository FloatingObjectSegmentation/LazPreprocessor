using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace core
{
    class Las2Txt
    {
        public static void Exec(string folder_name, string filepath, string outpath, string attributes)
        {
            PowerShell.Execute(String.Format("las2txt -i {0} -o {1} -parse {2}",
                                                     filepath,
                                                     outpath,
                                                     attributes),
                                       false,
                                       folder_name);
        }
    }
}
