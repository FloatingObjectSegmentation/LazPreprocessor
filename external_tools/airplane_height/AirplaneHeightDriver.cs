using common;
using common.structs;
using external_tools.common;
using external_tools.filters;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace external_tools.airplane_height
{
    
    public class AirplaneHeight: Filter
    {
        public List<double> Execute(List<AugmentableObjectSample> samples, string dmr_filepath)
        {
            string serialized = PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            string tempfilepath = Path.Combine(GConfig.TOOL_AIRPLANE_HEIGHT_PATH, "temp.txt" + Guid.NewGuid().ToString());
            File.WriteAllText(tempfilepath, serialized);
            List<double> result = new AirplaneHeightDriver().Execute(tempfilepath, dmr_filepath);
            File.Delete(tempfilepath);
            return result;
        }
    }

    class AirplaneHeightDriver
    {

        const string resultfile_prefix = "airplaneheights";

        public List<double> Execute(string samplesfilepath, string dmrpath)
        {

            string pshcmd = String.Format("{0}\\airplane_height.exe {1} {2} {3} {4}",
                                                        GConfig.TOOL_AIRPLANE_HEIGHT_PATH,
                                                        Path.GetDirectoryName(dmrpath),
                                                        Path.GetFileName(dmrpath),
                                                        Path.GetDirectoryName(samplesfilepath),
                                                        Path.GetFileName(samplesfilepath)
                                                        );

            PowerShell.Execute(pshcmd,
                                false,
                                Path.GetDirectoryName(samplesfilepath));

            string resultFileName = Path.Combine(
                                                Path.GetDirectoryName(samplesfilepath),
                                                resultfile_prefix + Path.GetFileName(samplesfilepath));

            try
            {
                List<double> lst = SpaceSeparatedFileParser.ParseDouble(resultFileName);
                File.Delete(resultFileName);
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
