﻿using common;
using common.structs;
using external_tools.common;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace external_tools.filters
{
    public class UndergroundFilter : Filter {
        public List<int> Execute(List<AugmentableObjectSample> samples, string dmrfilepath) {
            string serialized = PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            string tempfilepath = Path.Combine(GConfig.TOOL_UNDERGROUND_FILTER_PATH, "temp.txt" + Guid.NewGuid().ToString());
            File.WriteAllText(tempfilepath, serialized);
            List<int> result = new UndergroundFilterDriver().Execute(dmrfilepath, tempfilepath);
            File.Delete(tempfilepath);
            return result;
        }
    }

    class UndergroundFilterDriver
    {
        public List<int> Execute(string dmrfilepath, string samplesfilepath)
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
