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
using external_tools.underground_filter;
using external_tools.overlap_filter;
using external_tools.common;

namespace augmentation_sampler
{
    class Program
    {

        #region [config]
        // source lidar dataset
        static string TxtDatasetFileDirectory = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.LIDAR_SUBDIR);
        static string TxtDatasetFileName = GConfig.CHUNK + ".txt";

        // source dataset DMR
        static string DmrDirectory = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.DMR_SUBDIR);
        static string DmrFileName = GConfig.CHUNK;
        
        // saving location of augmentables
        static string SamplesFileDirectory = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.AUGMENTATION_SUBDIR);
        static string SamplesFileName = "augmentation_result.txt";
        #endregion

        #region [computed]
        static Vector3 minBound;
        static Vector3 maxBound;
        static List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        static List<double> RbnnMinValsPerObject;
        #endregion

        static void Main(string[] args)
        {
            Tools.Time(UnfilteredSampleObjects);
            Tools.Time(FilterOverlappingObjects);
            Tools.Time(FilterUndergroundPoints);
            Tools.Time(ComputeRbnnMinVals);
            Tools.Time(SaveResults);

            Console.WriteLine("Program finished. Press the ANY key to continue...");
            Console.ReadLine();
        }

        #region [aux]
        private static void UnfilteredSampleObjects()
        {
            minBound = Tools.FindMinimumVector(Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName));
            maxBound = Tools.FindMaximumVector(Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName));
            maxBound.Z += GConfig.candidateContextRadius;
            ObjectSampler samp = new ObjectSampler();
            samp.UnfilteredSampleObjects(GConfig.ObjectsToAdd, minBound, maxBound);
            samples = samp.samples;
        }

        private static void FilterOverlappingObjects()
        {
            List<int> discardedIndices = OverlapFilter.Execute(samples);
            Console.WriteLine($"OverlapFilter: Discard {discardedIndices.Count} examples.");
            DiscardFilteredExamples(discardedIndices);
        }

        private static void FilterUndergroundPoints()
        {
            List<int> discardedIndices = UndergroundFilter.Execute(samples, Path.Combine(DmrDirectory, DmrFileName));
            Console.WriteLine($"UndergroundFilter: Discard {discardedIndices.Count} examples.");
            DiscardFilteredExamples(discardedIndices);
        }

        private static void ComputeRbnnMinVals()
        {
            DFTFComputationProcedure proc = new DFTFComputationProcedure(samples, Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName));
            RbnnMinValsPerObject = proc.Execute();
        }

        private static void SaveResults()
        {
            string serialized_samples = PointCloudiaFormatSerializer.AugmentableSampleResultFormat(samples, RbnnMinValsPerObject);
            File.WriteAllText(Path.Combine(SamplesFileDirectory, SamplesFileName), serialized_samples);
        }
        #endregion

        #region [aux of aux]
        private static void DiscardFilteredExamples(List<int> indicesOfDiscarded)
        {
            HashSet<int> setIndicesOfDiscarded = new HashSet<int>(indicesOfDiscarded);

            // discard the overlapping examples
            List<AugmentableObjectSample> filteredSamples = new List<AugmentableObjectSample>();

            for (int i = 0; i < samples.Count; i++)
            {
                if (!setIndicesOfDiscarded.Contains(i))
                {
                    filteredSamples.Add(samples[i]);
                }
            }
            samples.Clear();
            samples = filteredSamples;
        }
        #endregion
    }
}
