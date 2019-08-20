using System;
using System.IO;
using core;
using common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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

            // partition to batch size of 8 for 8 core processors.
            List<List<Task>> batches = MyCollections.Partition<Task>(tasks.ToArray(), 8).ToList();
            foreach (var batch in batches) {
                foreach (Task t in batch)
                {
                    t.Start();
                }
                foreach (Task t in batch)
                {
                    t.Wait();
                }
                foreach (Task t in batch)
                {
                    t.Dispose();
                }
            }


            


            tasks = new List<Task>();
            foreach (string dmr in dmrpaths)
            {
                if (dmr.Contains("pcd")) continue;
                string current_dmr = dmr;
                Task task = new Task((object dmr_state) =>
                {
                    CoreProcedure procedure = new CoreProcedure((string)dmr_state);
                    procedure.Dmr2Pcd();
                }, current_dmr);
                tasks.Add(task);
            }

            batches = MyCollections.Partition<Task>(tasks.ToArray(), 8).ToList();
            foreach (var batch in batches)
            {
                foreach (Task t in batch)
                {
                    t.Start();
                }
                foreach (Task t in batch)
                {
                    t.Wait();
                }
            }

        }
    }
}
