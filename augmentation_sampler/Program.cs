using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using si.birokrat.next.common.shell;
using System.Linq;
using core;
using common;
using common.structs;
using external_tools.rbnn;
using external_tools.filters;
using external_tools.common;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace augmentation_sampler
{
    class Program
    {

        #region [config]
        // source lidar dataset
        string TxtDatasetFileDirectory = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.LIDAR_SUBDIR);
        string TxtDatasetFileName = GConfig.SINGLE_CHUNK + ".txt";

        // source dataset DMR
        string DmrDirectory = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.DMR_SUBDIR);
        string DmrFileName = GConfig.SINGLE_CHUNK;
        
        // saving location of augmentables
        string SamplesFileDirectory = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.AUGMENTATION_SUBDIR);
        string SamplesFileName = "augmentation_result.txt";
        #endregion

        #region [computed]
        Vector3 minBound;
        Vector3 maxBound;
        List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        List<double> RbnnMinValsPerObject;
        #endregion

        static void Main(string[] args)
        {
            List<List<int>> chunks = GConfig.GET_PICKED_CHUNKS();

            List<List<int>> list = new List<List<int>>();
            string[] augs = Directory.GetFiles(Path.Combine(GConfig.WORKSPACE_DIR, GConfig.AUGMENTATION_SUBDIR));
            foreach (string s in augs) {
                if (File.Exists(s)) {

                    Regex rx = new Regex(@"[0-9]{3}[_]{1}[0-9]{2,3}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    Match matches = rx.Match(s);
                    string chunk = matches.Value;
                    
                    string[] parts = chunk.Split("_");
                    List<int> element = new List<int>();
                    element.Add(int.Parse(parts[0]));
                    element.Add(int.Parse(parts[1]));
                    list.Add(element);
                }
            }

            List<List<int>> filteredChunks = new List<List<int>>();
            foreach (List<int> chunk in chunks) {

                bool some = true;
                for (int i = 0; i < list.Count; i++) {
                    if (list[i][0] == chunk[0] && list[i][1] == chunk[1]) {
                        some = false;
                        break;
                    }
                }
                if (!some) continue;

                filteredChunks.Add(chunk);
            }
            chunks = filteredChunks;
            


            List<Task> tasks = new List<Task>();

            Console.WriteLine($"Processing {chunks.Count} chunks.");
            foreach (List<int> chunk in chunks) {
                Program prg = new Program();
                prg.TxtDatasetFileName = chunk[0] + "_" + chunk[1] + ".txt";
                prg.DmrFileName = $"pcd{chunk[0]}_{chunk[1]}";
                prg.SamplesFileName = $"{chunk[0]}_{chunk[1]}augmentation_result.txt";

                Task t = new Task((exec_env) => {
                    try
                    {
                        Program exec_env_prg = (Program)exec_env;
                        Tools.Time(exec_env_prg.UnfilteredSampleObjects);
                        Tools.Time(exec_env_prg.FilterOverlappingObjects);
                        Tools.Time(exec_env_prg.FilterUndergroundPoints);

                        exec_env_prg.samples = exec_env_prg.samples.Take(GConfig.ObjectsToAdd).ToList();

                        Tools.Time(exec_env_prg.ComputeRbnnMinVals);
                        Tools.Time(exec_env_prg.SaveResults);
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Failed executing " + ((Program)exec_env).TxtDatasetFileName + ex.StackTrace + ex.Message);
                    }
                }, prg);
                tasks.Add(t);
            }

            List<List<Task>> batches = MyCollections.Partition<Task>(tasks.ToArray(), 5).ToList();
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

            Console.WriteLine("Program finished. Press the ANY key to continue...");
            Console.ReadLine();
        }

        #region [aux]
        private void UnfilteredSampleObjects()
        {
            minBound = Tools.FindMinimumVector(Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName));
            maxBound = Tools.FindMaximumVector(Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName));
            maxBound.Z += GConfig.candidateContextRadius; 
            ObjectSampler samp = new ObjectSampler();
            samp.UnfilteredSampleObjects(GConfig.ObjectsToAdd, minBound, maxBound);
            samples = samp.samples;
        }

        private void FilterOverlappingObjects()
        {
            List<int> discardedIndices = new OverlapFilter().Execute(samples);
            Console.WriteLine($"OverlapFilter: Discard {discardedIndices.Count} examples.");
            samples = OverlapFilter.DiscardFilteredExamples(discardedIndices, samples);
        }

        private void FilterUndergroundPoints()
        {
            List<int> discardedIndices = new UndergroundFilter().Execute(samples, Path.Combine(DmrDirectory, DmrFileName));
            Console.WriteLine($"UndergroundFilter: Discard {discardedIndices.Count} examples.");
            samples = UndergroundFilter.DiscardFilteredExamples(discardedIndices, samples);
        }

        private void ComputeRbnnMinVals()
        {
            DFTFComputationProcedure proc = new DFTFComputationProcedure(samples, Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName));
            RbnnMinValsPerObject = proc.Execute();
        }

        private void SaveResults()
        {

            // remove 0 entries
            List<Tuple<AugmentableObjectSample, double>> some = samples.Zip(RbnnMinValsPerObject, (x, y) =>
            {
                if (y != 0) return new Tuple<AugmentableObjectSample, double>(x, y); else return null;
            }).Where(x => x != null).ToList();

            // save to disk
            string serialized_samples = PointCloudiaFormatSerializer.AugmentableSampleResultFormat(some.Select(x => x.Item1).ToList(), some.Select(x => x.Item2).ToList());
            File.WriteAllText(Path.Combine(SamplesFileDirectory, SamplesFileName), serialized_samples);
        }
        #endregion
    }
}
