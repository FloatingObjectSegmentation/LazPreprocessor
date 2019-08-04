using common;
using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using external_tools.transforms;

namespace external_tools.rbnn
{
    public class RbnnDriver
    {

        #region [API]
        public static string ExecuteParallelTxt(string filepath, string resultfile_prefix, double[] radius_values, int cores)
        {

            List<List<double>> rbnn_r_batches = MyCollections.Split<double>(
                                        radius_values,
                                        (int)Math.Ceiling((decimal)radius_values.Length / (decimal)cores))
                                        .Select(x => x.ToList()).ToList();
            
            ExecEachValInOwnThread(GConfig.TOOL_RBNN_PATH, filepath, rbnn_r_batches);
            
            string resultfilepath = CombineToOneFileAndDeleteOthers(filepath, resultfile_prefix, cores);
            return resultfilepath;
        }
        
        public static string ExecuteTxt(string filepath, string resultfile_prefix, double[] radius_values) {
            string newFileName = Txt2Pcd.ExecXYZ(filepath);
            Execute(newFileName, resultfile_prefix, radius_values);
            File.Delete(newFileName);
            
            return Path.Combine(Path.GetDirectoryName(filepath), resultfile_prefix) + Path.GetFileName(filepath).Replace(".txt", ".pcd");
        }

        /// <summary>
        /// accepts a .pcd file, returns the result filepath
        /// </summary>
        public static string Execute(string filepath, string resultfile_prefix, double[] radius_values) {

            string pshcmd = String.Format("{0}\\rbnn.exe {1} {2} {3} ",
                                                     GConfig.TOOL_RBNN_PATH,
                                                     Path.GetDirectoryName(filepath),
                                                     Path.GetFileName(filepath),
                                                     resultfile_prefix);

            pshcmd += string.Join(" ", radius_values);
            PowerShell.Execute(pshcmd,
                               true,
                               Path.GetDirectoryName(filepath));
            return Path.Combine(Path.GetDirectoryName(filepath), resultfile_prefix) + Path.GetFileName(filepath);
        }
        #endregion

        #region [auxiliary]
        private static int ExecEachValInOwnThread(string rbnn_exe_location, string filepath, List<List<double>> rbnn_batches)
        {
            int num_tasks = rbnn_batches.Count;
            Barrier barrier = new Barrier(participantCount: num_tasks);
            Task[] tasks = new Task[num_tasks];
            
            List<int> forloop = Enumerable.Range(0, num_tasks).ToList();

            Console.WriteLine("ParallelRbnn: Copying files...");
            forloop.Select(i => get_result_filepath(i.ToString(), filepath)).ToList()
                .ForEach(x => {
                    if (File.Exists(x))
                        File.Delete(x);
                    File.Copy(filepath, x);
                });

            Console.WriteLine("ParallelRbnn: Executing on {0} threads...", num_tasks);
            tasks = tasks.Select((x, i) =>
                Task.Factory.StartNew(() =>
                {
                    ExecuteTxt(get_result_filepath(i.ToString(), filepath), "", rbnn_batches[i].ToArray());
                    barrier.SignalAndWait();
                })).ToArray();
            
            Task.WaitAll(tasks);

            return num_tasks;
        }

        private static string CombineToOneFileAndDeleteOthers(string filepath, string resultfile_prefix, int num_tasks)
        {
            string result_filepath = get_result_filepath(resultfile_prefix, filepath);

            List<string> allLines = new List<string>();
            for (int i = 0; i < num_tasks; ++i)
            {
                string[] lines = File.ReadAllLines(get_result_filepath(i.ToString(), filepath));
                allLines.AddRange(lines);
            }
            File.WriteAllLines(result_filepath, allLines);

            Enumerable.Range(0, num_tasks).ToList().ForEach(
                (x) => File.Delete(get_result_filepath(x.ToString(), filepath))
            );
            return result_filepath;
        }

        private static string get_result_filepath(string prefix, string filepath)
        {
            return Path.Combine(Path.GetDirectoryName(filepath), prefix + Path.GetFileName(filepath));
        }
        #endregion
    }
}
