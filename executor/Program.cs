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

            string[] filepaths = Directory.GetFiles(folder, "*.laz");

            foreach (string path in filepaths)
            {
                CoreProcedure procedure = new CoreProcedure(path);
                procedure.PreprocessLaz();
            }
        }
    }
}
