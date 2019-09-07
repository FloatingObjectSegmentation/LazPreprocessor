using common;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using external_tools.common;
using common.structs;

namespace external_tools.filters
{

    public class OverlapFilter : Filter {
        public List<int> Execute(List<AugmentableObjectSample> samples)
        {
            string serialized = PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            string tempfilepath = Path.Combine(GConfig.TOOL_OVERLAP_COMPUTE_PATH, "temp.txt" + Guid.NewGuid().ToString());
            File.WriteAllText(tempfilepath, serialized);
            List<int> result = new OverlapFilterDriver().Execute(tempfilepath);
            File.Delete(tempfilepath);
            return result;
        }
    }

    class OverlapFilterDriver
    {

        const string resultfile_prefix = "result";

        public List<int> Execute(string samplesfilepath)
        {

            string pshcmd = String.Format("{0}\\overlap_compute.exe {1} {2}",
                                                     GConfig.TOOL_OVERLAP_COMPUTE_PATH,
                                                     Path.GetDirectoryName(samplesfilepath),
                                                     Path.GetFileName(samplesfilepath));

            PowerShell.Execute(pshcmd,
                               false,
                               Path.GetDirectoryName(samplesfilepath));

            string resultFileName = Path.Combine(
                                                Path.GetDirectoryName(samplesfilepath),
                                                resultfile_prefix + Path.GetFileName(samplesfilepath));

            try
            {
                List<int> lst = SpaceSeparatedFileParser.ParseInt(resultFileName);
                return lst;
            }
            catch (Exception ex)
            {
                File.Delete(resultFileName);
                throw ex;
            }
        }
    }
}
