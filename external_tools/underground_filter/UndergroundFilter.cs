using common;
using common.structs;
using external_tools.common;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace external_tools.underground_filter
{
    public class UndergroundFilter {
        public static List<int> Execute(List<AugmentableObjectSample> samples, string dmrfilepath) {
            string serialized = PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            string tempfilepath = Path.Combine(GConfig.TOOL_OVERLAP_COMPUTE_PATH, "temp.txt");
            File.WriteAllText(tempfilepath, serialized);
            List<int> result = UndergroundFilterDriver.Execute(dmrfilepath, tempfilepath);
            File.Delete(tempfilepath);
            return result;
        }
    }

    class UndergroundFilterDriver
    {
        public static List<int> Execute(string dmrfilepath, string samplesfilepath)
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

            string resultFileName = Path.Combine(
                                                Path.GetDirectoryName(samplesfilepath),
                                                "underground" + Path.GetFileName(samplesfilepath));

            List<int> lst = SpaceSeparatedFileParser.ParseInt(resultFileName);
            File.Delete(resultFileName);
            return lst;
        }
    }
}
