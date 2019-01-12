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
        // source lidar dataset
        static string TxtDatasetFileDirectory = @"C:\Users\km\Desktop\MAG\floatingObjectFilter\data";
        static string TxtDatasetFileName = "459_99.txt";

        
        ///  augmentation
        static int ObjectsToAdd = 500;
        // saving location of augmentables
        static string SamplesFileDirectory = @"C:\Users\km\Desktop\MAG\floatingObjectFilter\data\augmentables";
        static string SamplesFileName = "kurac.txt";

        // required tool locations
        static string overlaptool_exe_location = @"C:\Users\km\source\repos\LazPreprocessor\augmentation_sampler\resources";
        static string rbnn_exe_location = @"C:\Users\km\source\repos\LazPreprocessor\core\resources";
        
        

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

            double minX = 9999999999.0;
            double minY = 9999999999.0;
            double minZ = 9999999999.0;
            for (int i = 0; i < LidarFileLines.Count; i++)
            {
                String[] parts = LidarFileLines[i].Split(" ");
                double x = Double.Parse(parts[0]), y = Double.Parse(parts[1]), z = Double.Parse(parts[2]);
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ) minZ = z;
            }
            Console.WriteLine("Before add: " + minX + " " + minY + " " + minZ);

            Dictionary<int, int> LidarPointIndexToSampleIndex = new Dictionary<int, int>();

            nextIndex = CreateNewPointsAndIndexThemBackIntoOriginalSamples(nextIndex, LidarFileLinesAugmented, LidarPointIndexToSampleIndex);

            LidarFileLinesAugmented = AugmentedPointsToLidarDataCoordinateFrame(LidarFileLines, LidarFileLinesAugmented);



            string filepath = StoreAllPointsToTempFile(LidarFileLines, LidarFileLinesAugmented);


            // debug only
            int point_idx = 12517757;
            AugmentableObjectSample s = samples[LidarPointIndexToSampleIndex[point_idx]];
            Vector3 locs = locations[LidarPointIndexToSampleIndex[point_idx]];
            String[] some = File.ReadAllLines(filepath);
            Console.WriteLine(some[point_idx]);
            Console.WriteLine(s.name + s.sizeMeters + locs.ToString());

            minX = 9999999999.0;
            minY = 9999999999.0;
            minZ = 9999999999.0;
            int minZidx = 0;
            for (int i = 0; i < LidarFileLinesAugmented.Count; i++)
            {
                Vector3 curr = LidarFileLinesAugmented[i];
                double x = curr.X, y = curr.Y, z = curr.Z;
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ)
                {
                    minZ = z;
                    minZidx = i;
                }
            }
            Console.WriteLine("After add: " + minX + " " + minY + " " + minZ);
            Console.WriteLine("Type of minimum: " + samples[LidarPointIndexToSampleIndex[lowestIndex + minZidx]].name);
            Console.WriteLine("Type of minimum: " + samples[LidarPointIndexToSampleIndex[lowestIndex + minZidx]].sizeMeters.ToString());
            Console.WriteLine("Location of minimum: " + locations[LidarPointIndexToSampleIndex[lowestIndex + minZidx]]);

            Dictionary<string, RbnnResult> results = ComputeRbnn(filepath);

            foreach (KeyValuePair<String, RbnnResult> q in results)
            {

                RbnnResult r = q.Value;
                for (int i = 0; i < r.clusterIndices.Count; i++)
                {
                    if (r.clusterIndices[i] != -1)
                        Console.WriteLine(i + " " + r.clusterIndices[i]);
                }

            }
            // end debug only

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
                    if (res.clusterIndices[j] == -1)
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
            filepath = RbnnDriver.ExecuteTxt(rbnn_exe_location, filepath, common.Numpy.LinSpace(2, 15, 10).ToArray());

            // retrieve results
            RbnnResultParser parser = new RbnnResultParser();
            Dictionary<string, RbnnResult> results = parser.ParseResults(filepath);
            return results;
        }

        private static List<Vector3> AugmentedPointsToLidarDataCoordinateFrame(List<string> LidarFileLines, List<Vector3> LidarFileLinesAugmented)
        {
            Vector3 minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            LidarFileLines.ForEach((l) => {
                float[] parts = l.Split(" ").Select((k) => float.Parse(k)).ToArray();
                float x = parts[0], y = parts[1], z = parts[2];
                if (x < minCoord.X) minCoord.X = x;
                if (y < minCoord.Y) minCoord.Y = y;
                if (z < minCoord.Z) minCoord.Z = z;
            });
            LidarFileLinesAugmented = LidarFileLinesAugmented.Select((x) => Vector3.Add(x, minCoord)).ToList();
            return LidarFileLinesAugmented;
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
            for (int c = 0; c < 2; c++)
            {
                x = -x;
                for (int j = 0; j < 2; j++)
                {
                    y = -y;
                    for (int k = 0; k < 2; k++)
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

            PowerShell.Execute(overlaptool_exe_location + "\\overlap_compute.exe " + SamplesFileDirectory + " " + SamplesFileName);

            string result_file = Path.Combine(SamplesFileDirectory, "result" + SamplesFileName);
            return result_file;
        }

        private static string UnfilteredSamplesToString()
        {
            string str = "";
            for (int i = 0; i < locations.Count; i++)
            {
                str += locations[i].X + "," + locations[i].Y + "," + locations[i].Z + " ";

                List<Vector3> boundingPoints = GetBoundingBoxPoints(samples[i], locations[i]);
                str += String.Join(" ", boundingPoints.Select((x) => { return x.X + "," + x.Y + "," + x.Z; }).ToList());
                str += " ";
                str += maxDims[i] + "\n";
            }

            return str;
        }
        #endregion

        #endregion
    }
}
