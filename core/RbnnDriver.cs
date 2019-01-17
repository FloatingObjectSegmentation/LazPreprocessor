using si.birokrat.next.common.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace core
{
    public class RbnnDriver
    {
        

        public static string ExecuteParallelTxt(string rbnn_exe_location, string filepath, double[] radius_values) {

            Barrier barrier = new Barrier(participantCount: 5);

            Task[] tasks = new Task[5];

            for (int i = 0; i < 5; ++i)
            {
                int j = i;
                tasks[j] = Task.Factory.StartNew(() =>
                {

                    Console.WriteLine("Getting data from server: " + j);
                    Thread.Sleep(TimeSpan.FromSeconds(2));

                    barrier.SignalAndWait();

                    Console.WriteLine("Send data to Backup server: " + j);

                    barrier.SignalAndWait();
                });
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Backup completed");
            Console.ReadLine();

            return "";
        }

        public static string ExecuteParallel() {
            return "";
        }

        // returns the name of the file that the result was saved to

        public static string ExecuteTxt(string rbnn_exe_location, string filepath, string result_prefix, double[] radius_values) {
            string newFileName = Txt2Pcd.ExecXYZ(filepath);
            Execute(rbnn_exe_location, newFileName, result_prefix, radius_values);
            File.Delete(newFileName);
            
            return Path.Combine(Path.GetDirectoryName(filepath), result_prefix) + Path.GetFileName(filepath).Replace(".txt", ".pcd");
        }

        /// <summary>
        /// accepts a .pcd file, returns the result filepath
        /// </summary>
        public static string Execute(string rbnn_exe_location, string filepath, string result_prefix, double[] radius_values) {

            string pshcmd = String.Format("{0}\\rbnn.exe {1} {2} {3} ",
                                                     rbnn_exe_location,
                                                     Path.GetDirectoryName(filepath),
                                                     Path.GetFileName(filepath),
                                                     result_prefix);

            pshcmd += string.Join(" ", radius_values);
            PowerShell.Execute(pshcmd,
                               false,
                               Path.GetDirectoryName(filepath));
            return Path.Combine(Path.GetDirectoryName(filepath), result_prefix) + Path.GetFileName(filepath);
        }
    }
}
