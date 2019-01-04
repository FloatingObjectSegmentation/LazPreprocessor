using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using si.birokrat.next.common.shell;
using System.Linq;
using core;

namespace augmentation_sampler
{
    class Program
    {

        #region [config]
        static string TxtDatasetFileDirectory = "";
        static string TxtDatasetFileName = "";
        static string SamplesFileDirectory = "";
        static string SamplesFileName = "";
        static string overlaptool_exe_location = "";
        static string rbnn_exe_location = "";
        static int ObjectsToAdd = 500;

        // bounds should be redefined according to dataset
        static Vector3 minBound = new Vector3(0.0f, 0.0f, 0.0f);
        static Vector3 maxBound = new Vector3(1000.0f, 1000.0f, 50.0f);
        #endregion

        #region [computed]
        static List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        static List<Vector3> locations = new List<Vector3>();
        static List<float> maxDims = new List<float>();
        static List<double> RbnnMinValsPerObject;
        #endregion

        static void Main(string[] args)
        {
            UnfilteredSampleObjects();
            FilterSampleObjects();
            ComputeRbnnMinVals();
            SaveResults();
        }

        #region [aux]
        private static void UnfilteredSampleObjects()
        {
            ObjectSampler samp = new ObjectSampler();
            samp.UnfilteredSampleObjects(ObjectsToAdd, minBound, maxBound);
            samples = samp.samples;
            locations = samp.locations;
            maxDims = samp.maxDims;
        }

        private static void FilterSampleObjects()
        {
            string str = UnfilteredSamplesToString();
            FilterWithOverlapTool(str);
            DiscardFilteredExamples();
        }

        private static void ComputeRbnnMinVals()
        {
            List<string> LidarFileLines = File.ReadAllLines(Path.Combine(TxtDatasetFileDirectory, TxtDatasetFileName)).ToList();
            int nextIndex = LidarFileLines.Count;
            int lowestIndex = nextIndex;

            List<Vector3> LidarFileLinesAugmented = new List<Vector3>();
            
            Dictionary<int, int> LidarPointIndexToSampleIndex = new Dictionary<int, int>();

            nextIndex = CreateNewPointsAndIndexThemBackIntoOriginalSamples(nextIndex, LidarFileLinesAugmented, LidarPointIndexToSampleIndex);

            string filepath = StoreAllPointsToTempFile(LidarFileLines, LidarFileLinesAugmented);

            Dictionary<string, RbnnResult> results = ComputeRbnn(filepath);

            ComputeRbnnMinVals(nextIndex, lowestIndex, LidarPointIndexToSampleIndex, results);
        }

        private static void SaveResults()
        {
            string a = "";
            int currid = 0;
            for (int i = 0; i < samples.Count; i++)
            {
                List<string> vals = new List<string>();

                string id = currid.ToString();
                vals.Add(id);
                string pos = locations[i].X + "," + locations[i].Y + "," + locations[i].Z;
                vals.Add(pos);
                string scale = samples[i].sizeMeters.X + "," + samples[i].sizeMeters.Y + "," + samples[i].sizeMeters.Z;
                vals.Add(scale);
                string shape = samples[i].name;
                vals.Add(shape);
                string airplane_pos = locations[i].X + "," + locations[i].Y + "," + locations[i].Z + 1000.0f;
                vals.Add(airplane_pos);
                string dist = RbnnMinValsPerObject[i].ToString();
                vals.Add(dist);
                a += String.Join(" ", vals) + "\n";
            }
            File.WriteAllText(Path.Combine(SamplesFileDirectory, SamplesFileName), a);
        }
        #endregion

        #region [aux of aux]

        #region [rbnn]
        private static int CreateNewPointsAndIndexThemBackIntoOriginalSamples(int nextIndex, List<Vector3> ToAdd, Dictionary<int, int> LidarPointIndexToSampleIndex)
        {
            for (int i = 0; i < samples.Count; i++)
            {

                AugmentableObjectSample curr = samples[i];
                Vector3 location = locations[i];
                List<Vector3> BoundingBoxPoints = GetBoundingBoxPoints(curr, location);

                // add the bounding points both to be added and to keep track of what their object is, which indices belong to which object
                for (int j = 0; j < BoundingBoxPoints.Count; j++)
                {
                    ToAdd.Add(BoundingBoxPoints[j]);
                    LidarPointIndexToSampleIndex.Add(nextIndex++, i);
                }
            }

            return nextIndex;
        }

        private static void ComputeRbnnMinVals(int nextIndex, int lowestIndex, Dictionary<int, int> LidarPointIndexToSampleIndex, Dictionary<string, RbnnResult> results)
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
                    if (!res.isPointFloating[j])
                    {
                        int sample_idx = LidarPointIndexToSampleIndex[j];
                        if (RbnnMinValsPerObject[sample_idx] == 0.0)
                            RbnnMinValsPerObject[sample_idx] = ks[i - 1]; // the previous rbnn value was its boundary
                    }
                }
            }
        }

        private static Dictionary<string, RbnnResult> ComputeRbnn(string filepath)
        {
            RbnnDriver.Execute(rbnn_exe_location, filepath, common.Numpy.LinSpace(5, 50, 10).ToArray());

            // retrieve results
            RbnnResultParser parser = new RbnnResultParser();
            Dictionary<string, RbnnResult> results = parser.ParseResults(RbnnDriver.GetResultFileName(filepath));
            return results;
        }

        private static string StoreAllPointsToTempFile(List<string> LidarFileLines, List<Vector3> ToAdd)
        {
            foreach (var x in ToAdd)
                LidarFileLines.Add(x.X + " " + x.Y + " " + x.Z + " 0 0 0");

            // store it
            string filepath = Path.Combine(TxtDatasetFileDirectory, "temp" + TxtDatasetFileName);
            File.WriteAllLines(filepath, LidarFileLines);
            return filepath;
        }

        private static List<Vector3> GetBoundingBoxPoints(AugmentableObjectSample curr, Vector3 location)
        {

            // compute the current candidate's bounding points
            List<Vector3> BoundingBoxPoints = new List<Vector3>();
            int x = 1, y = 1, z = 1;
            for (int c = 0; c < 1; c++)
            {
                x = -x;
                for (int j = 0; j < 1; j++)
                {
                    y = -y;
                    for (int k = 0; k < 1; k++)
                    {
                        z = -z;

                        BoundingBoxPoints.Add(new Vector3(location.X + x * curr.sizeMeters.X / 2,
                            location.Y + y * curr.sizeMeters.Y / 2,
                            location.Z + z * curr.sizeMeters.Z / 2));
                    }
                }
            }

            return BoundingBoxPoints;
        }

        private static void DiscardFilteredExamples()
        {
            string result = File.ReadAllText(Path.Combine(SamplesFileDirectory, "result" + SamplesFileName));
            List<int> indicesOfDiscarded = new List<int>((result.Split(" ").Select((x) => int.Parse(x))));
            HashSet<int> setIndicesOfDiscarded = new HashSet<int>(indicesOfDiscarded);

            // discard the overlapping examples
            List<AugmentableObjectSample> filteredSamples = new List<AugmentableObjectSample>();
            List<Vector3> filteredLocations = new List<Vector3>();

            for (int i = 0; i < samples.Count; i++)
            {
                if (!setIndicesOfDiscarded.Contains(i))
                {
                    filteredSamples.Add(samples[i]);
                    filteredLocations.Add(locations[i]);
                }
            }
            samples.Clear();
            locations.Clear();
            samples = filteredSamples;
            locations = filteredLocations;
        }
        #endregion

        #region [filtering]
        private static string FilterWithOverlapTool(string str)
        {
            File.WriteAllText(Path.Combine(SamplesFileDirectory, SamplesFileName), str);

            PowerShell.Execute("./" + overlaptool_exe_location + " " + SamplesFileDirectory + " " + SamplesFileName);

            string result_file = Path.Combine(SamplesFileDirectory, "result" + SamplesFileName);
            return result_file;
        }

        private static string UnfilteredSamplesToString()
        {
            string str = "";
            for (int i = 0; i < locations.Count; i++)
            {
                str += locations[i].X + "," + locations[i].Y + "," + locations[i].Z + " ";
                str += maxDims[i] + "\n";
            }

            return str;
        }
        #endregion

        #endregion
    }
}
