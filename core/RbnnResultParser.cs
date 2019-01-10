using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace core
{
    public class RbnnResultParser
    {
        public Dictionary<string, RbnnResult> ParseResults(string result_file_path) {
            Dictionary<string, RbnnResult> result = new Dictionary<string, RbnnResult>();
            string[] lines = File.ReadAllLines(result_file_path);
            foreach (string line in lines) {
                KeyValuePair<string, RbnnResult> res = ParseResult(line);
                result.Add(res.Key, res.Value);
            }
            return result;
        }

        private KeyValuePair<string, RbnnResult> ParseResult(string line) {
            string[] parts = line.Split(" ");
            string radius_str = parts[0];
            double radius = double.Parse(radius_str);

            // yeah... slow...
            string[] values = new string[parts.Length - 2];
            for (int i = 1; i < parts.Length - 1; i++) {
                values[i - 1] = parts[i]; 
            }

            List<int> ClusterIndices = new List<int>(values.Length);
            Array.ForEach(values, (x) => ClusterIndices.Add(int.Parse(x)));

            return new KeyValuePair<string, RbnnResult>(radius_str, new RbnnResult(radius, ClusterIndices));
        }
    }

    public class RbnnResult {

        public double radius;
        public List<int> clusterIndices; // -1 are transitive floor points, floating are positive.

        public RbnnResult(double radius, List<int> ClusterIndices)
        {
            this.radius = radius;
            this.clusterIndices = ClusterIndices;
        }
    }
}
