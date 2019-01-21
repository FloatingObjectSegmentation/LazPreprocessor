using System;
using System.IO;
using core;
using common;

namespace Executor
{
    class Program
    {
        static string folder = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.LIDAR_SUBDIR);

        static void Main(string[] args)
        {
            string[] filepaths = Directory.GetFiles(folder, "*.laz");

            foreach (string path in filepaths)
            {
                CoreProcedure procedure = new CoreProcedure(path);
                procedure.PreprocessLaz();
            }
        }
    }
}
