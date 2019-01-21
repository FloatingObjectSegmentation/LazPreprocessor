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
            List<int> list = new List<int>((result.Split(" ").Select((x) => int.Parse(x))));
            return list;
        }
    }
}
