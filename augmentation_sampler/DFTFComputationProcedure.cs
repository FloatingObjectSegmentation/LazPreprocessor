using external_tools.rbnn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

using common.structs;
using System.Linq;
using common;

namespace augmentation_sampler
{
    // distance from transitive floor computation procedure
    class DFTFComputationProcedure
    {

        List<double> RbnnMinValsPerObject;
        List<AugmentableObjectSample> samples;
        string lidar_data_filepath;

        public DFTFComputationProcedure(List<AugmentableObjectSample> samples, string lidar_data_filepath) {
            this.samples = samples;
            this.lidar_data_filepath = lidar_data_filepath;
        }

        public List<double> Execute() {
            ComputeRbnnMinVals();
            return RbnnMinValsPerObject;
        }

        private void ComputeRbnnMinVals()
        {
            List<string> LidarFileLines = File.ReadAllLines(lidar_data_filepath).ToList();
            int nextIndex = LidarFileLines.Count;
            int lowestIndex = nextIndex;

            List<Vector3> LidarFileLinesAugmented = new List<Vector3>();
            Dictionary<int, int> LidarPointIndexToSampleIndex = new Dictionary<int, int>();

            nextIndex = CreateNewPointsAndIndexThemBackIntoOriginalSamples(nextIndex, LidarFileLinesAugmented, LidarPointIndexToSampleIndex);

            string filepath = StoreAllPointsToTempFile(LidarFileLines, LidarFileLinesAugmented);
            Dictionary<string, RbnnResult> results = ComputeRbnn(filepath);
            File.Delete(filepath);

            ComputeRbnnMinVals(nextIndex, lowestIndex, LidarPointIndexToSampleIndex, results);
        }

        private int CreateNewPointsAndIndexThemBackIntoOriginalSamples(int nextIndex, List<Vector3> ToAdd, Dictionary<int, int> LidarPointIndexToSampleIndex)
        {
            for (int i = 0; i < samples.Count; i++)
            {
                AugmentableObjectSample curr = samples[i];
                Vector3 location = samples[i].location;
                List<Vector3> BoundingBoxPoints = BoundingBox.GetVertices(curr);

                // add the bounding points both to be added and to keep track of what their object is, which indices belong to which object
                for (int j = 0; j < BoundingBoxPoints.Count; j++)
                {
                    ToAdd.Add(BoundingBoxPoints[j]);
                    LidarPointIndexToSampleIndex.Add(nextIndex++, i);
                }
            }

            return nextIndex;
        }

        private void ComputeRbnnMinVals(int nextIndex, int lowestIndex, Dictionary<int, int> LidarPointIndexToSampleIndex, Dictionary<string, RbnnResult> results)
        {
            List<float> ks = results.Keys.ToList().Select(x => float.Parse(x)).ToList();
            ks.Sort();
            RbnnMinValsPerObject = samples.Select(x => 0.0).ToList();

            for (int i = 0; i < ks.Count; i++)
            {

                string a = ks[i].ToString();
                RbnnResult res = results[a];
                int highestIndex = nextIndex;
                for (int j = lowestIndex; j < highestIndex; j++)
                {
                    if (res.clusterIndices[j] == -1)
                    {
                        int sample_idx = LidarPointIndexToSampleIndex[j];
                        if (RbnnMinValsPerObject[sample_idx] == 0.0 && i != 0)
                            RbnnMinValsPerObject[sample_idx] = ks[i - 1]; // the previous rbnn value was its boundary
                    }
                }
            }
        }

        private Dictionary<string, RbnnResult> ComputeRbnn(string filepath)
        {
            filepath = RbnnDriver.ExecuteTxt(filepath, GConfig.rbnn_augmentation_result_prefix, GConfig.AUGMENTATION_RBNN_R_SPAN);

            // retrieve results
            RbnnResultParser parser = new RbnnResultParser();
            Dictionary<string, RbnnResult> results = parser.ParseResults(filepath);
            File.Delete(filepath);

            return results;
        }

        private string StoreAllPointsToTempFile(List<string> LidarFileLines, List<Vector3> ToAdd)
        {
            foreach (var x in ToAdd)
                LidarFileLines.Add(x.X + " " + x.Y + " " + x.Z + " 0 0 0");
            
            string filepath = Path.Combine(Path.GetDirectoryName(lidar_data_filepath), "temp" + Path.GetFileName(lidar_data_filepath));
            File.WriteAllLines(filepath, LidarFileLines);
            return filepath;
        }
    }
}
