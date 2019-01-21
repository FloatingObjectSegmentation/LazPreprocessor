using common;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace external_tools.underground_filter
{
    public class UndergroundFilterDriver
    {
        public static void Execute(string dmrfilepath, string samplesfilepath)
        {

            string pshcmd = String.Format("{0}\\underground_filter.exe {1} {2} {3} {4}",
                                                     GConfig.TOOL_UNDERGROUND_FILTER_PATH,
                                                     Path.GetDirectoryName(dmrfilepath),
                                                     Path.GetFileName(dmrfilepath),
                                                     Path.GetDirectoryName(samplesfilepath),
                                                     Path.GetFileName(samplesfilepath));
            
            PowerShell.Execute(pshcmd,
                               false,
                               Path.GetDirectoryName(samplesfilepath));
        }
    }
}
