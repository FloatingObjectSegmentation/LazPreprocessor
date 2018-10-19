using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace core
{
    public class RbnnDriver
    {

        // returns the name of the file that the result was saved to
        public static string Execute(string rbnn_exe_location, string filepath, double[] radius_values) {

            string pshcmd = String.Format("{0}\\rbnn.exe {1} {2} ",
                                                     rbnn_exe_location,
                                                     Path.GetDirectoryName(filepath),
                                                     Path.GetFileName(filepath));

            pshcmd += string.Join(" ", radius_values);
            PowerShell.Execute(pshcmd,
                               false,
                               Path.GetDirectoryName(filepath));
            return Path.Combine(Path.GetDirectoryName(filepath), "result") + Path.GetFileName(filepath);
        }
    }
}
