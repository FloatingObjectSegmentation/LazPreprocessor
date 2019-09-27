using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace external_tools.common
{
    public class SpaceSeparatedFileParser
    {
        public static List<int> ParseInt(string filename) {
            string result = File.ReadAllText(filename);
            if (result.Length == 0)
                throw new Exception("FileParserError: The input file was empty!");
            List<int> list = new List<int>((result.Split(" ").Select((x) => int.Parse(x))));
            return list;
        }

        public static List<double> ParseDouble(string filename)
        {
            string result = File.ReadAllText(filename);
            if (result.Length == 0)
                throw new Exception("FileParserError: The input file was empty!");
            List<double> list = new List<double>((result.Split(" ").Select((x) => double.Parse(x))));
            return list;
        }
    }
}
