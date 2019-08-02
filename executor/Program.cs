using System;
using System.IO;
using core;
using common;
using System.Threading.Tasks;
using System.Collections.Generic;

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


            List<Task> tasks = new List<Task>();
            foreach (string path in filepaths)
            {
                string current_path = path;
                Task task = new Task(() =>
                {
                    CoreProcedure procedure = new CoreProcedure(current_path);
                    procedure.PreprocessLaz();
                });
                tasks.Add(task);
            }
            foreach (Task t in tasks)
            {
                t.Start();
            }
            foreach (Task t in tasks)
            {
                t.Wait();
            }
            foreach (Task t in tasks)
            {
                t.Dispose();
            }


            tasks = new List<Task>();
            foreach (string dmr in dmrpaths)
            {
                string current_dmr = dmr;
                Task task = new Task(() =>
                {
                    CoreProcedure procedure = new CoreProcedure(current_dmr);
                    procedure.Dmr2Pcd();
                });
                tasks.Add(task);
            }
            foreach (Task t in tasks)
            {
                t.Start();
            }
            foreach (Task t in tasks)
            {
                t.Wait();
            }

        }
    }
}
