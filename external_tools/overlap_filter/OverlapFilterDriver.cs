using common;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace external_tools.overlap_filter
{
    public class OverlapFilterDriver
    {
        public static void Execute(string samplesfilepath)
        {

            string pshcmd = String.Format("{0}\\overlap_compute.exe {1} {2}",
                                                     GConfig.TOOL_OVERLAP_COMPUTE_PATH,
                                                     Path.GetDirectoryName(samplesfilepath),
                                                     Path.GetFileName(samplesfilepath));

            PowerShell.Execute(pshcmd,
                               false,
                               Path.GetDirectoryName(samplesfilepath));
        }
    }
}
