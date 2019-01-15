using System;
using System.IO;
using core;


namespace Executor
{
    class Program
    {
        static string folder = @"C:\Users\km\Desktop\MAG\FloatingObjectFilter\data";

        static void Main(string[] args)
        {

            string path = @"C:\Users\km\Desktop\LIDAR_WORKSPACE\dmr\449_121";
            CoreProcedure procedure = new CoreProcedure(path);
            procedure.Dmr2Pcd();

            /*
            string[] filepaths = Directory.GetFiles(folder, "*.laz");

            foreach (string path in filepaths)
            {
                CoreProcedure procedure = new CoreProcedure(path);
                procedure.PreprocessLaz();
            }
            */
        }
    }
}
