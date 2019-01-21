using System;
using System.IO;
using core;
using common;

namespace Executor
{
    class Program
    {
        static string lazfolder = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.LIDAR_SUBDIR);
        static string dmrfolder = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.DMR_SUBDIR);

        static void Main(string[] args)
        {
            string[] filepaths = Directory.GetFiles(lazfolder, "*.laz");
            string[] dmrpaths = Directory.GetFiles(dmrfolder);

            foreach (string path in filepaths)
            {
                CoreProcedure procedure = new CoreProcedure(path);
                procedure.PreprocessLaz();
            }
            foreach (string dmr in dmrpaths)
            {
                CoreProcedure procedure = new CoreProcedure(dmr);
                procedure.Dmr2Pcd();
            }
        }
    }
}
