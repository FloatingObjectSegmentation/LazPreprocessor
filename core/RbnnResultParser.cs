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
            string values = parts[1];

            List<bool> isPointFloating = new List<bool>(values.Length);
            Array.ForEach(values.ToCharArray(), (x) => isPointFloating.Add(x == '1' ? true : false) );

            return new KeyValuePair<string, RbnnResult>(radius_str, new RbnnResult(radius, isPointFloating));
        }
    }

    public class RbnnResult {

        public double radius;
        public List<bool> isPointFloating;

        public RbnnResult(double radius, List<bool> isPointFloating)
        {
            this.radius = radius;
            this.isPointFloating = isPointFloating;
        }
    }
}
