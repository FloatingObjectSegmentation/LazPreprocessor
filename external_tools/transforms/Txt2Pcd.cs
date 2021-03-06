﻿using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using common;

namespace external_tools.transforms
{
    public class Txt2Pcd
    {
        /*
        ASSUMPTION: input file is xyzRGB where scope of RGB is 0 - 65536 
        */

        private static string header = "# .PCD v.7 - Point Cloud Data file format" + "\n" +
                        "VERSION .7" + "\n" +
                        "FIELDS x y z" + "\n" +
                        "SIZE 4 4 4" + "\n" +
                        "TYPE F F F" + "\n" +
                        "COUNT 1 1 1" + "\n" +
                        "WIDTH {0}" + "\n" +
                        "HEIGHT 1" + "\n" +
                        "VIEWPOINT 0 0 0 1 0 0 0" + "\n" +
                        "POINTS {0}" + "\n" +
                        "DATA ascii" + "\n";

        public static string ExecXYZ(string filepath, string separator = " ", string pathaddition = "some") {

            string filename = Filename.FromFullPath(filepath);
            string filedir = Filename.FolderFromFullPath(filepath);
            string new_name = Path.Combine(filedir, pathaddition + filename).Replace(".txt", ".pcd");

            StreamWriter output = new StreamWriter(new_name);
            output.Write(string.Format(header, CountLines.CountLinesReader(new FileInfo(filepath))));

            using (var input = new StreamReader(filepath)) {
                string input_line = "";
                while ((input_line = input.ReadLine()) != null)
                {
                    if (input_line.Contains("# .PCD v.7 - Point Cloud Data file format"))
                    {
                        return "";
                    }

                    string[] parts = input_line.Split(separator);
                    double x = double.Parse(parts[0]);
                    double y = double.Parse(parts[1]);
                    double z = double.Parse(parts[2]);

                    input_line = string.Format("{0:F5} {1:F5} {2:F5}", x, y, z);
                    output.WriteLine(input_line);

                }
                input.Close();
            }
            output.Close();
            return new_name;

                /*string[] lines = File.ReadAllLines(filepath);

            if (lines[0].Contains("# .PCD v.7 - Point Cloud Data file format")) {
                return ""; // the file has already been processed
            }

            for (int i = 0; i < lines.Length; i++) {
                string[] parts = lines[i].Split(separator);
                double x = double.Parse(parts[0]);
                double y = double.Parse(parts[1]);
                double z = double.Parse(parts[2]);
                // double colour = threeValRgbToOneVal(parts[3], parts[4], parts[5]);
                lines[i] = string.Format("{0:F5} {1:F5} {2:F5}", x,y,z);
            }
            File.WriteAllText(filepath.Replace(".txt", ".pcd"), string.Format(header, lines.Length, string.Join("\n", lines)));
            return filepath.Replace(".txt", ".pcd");*/
        }

        #region [auxiliary]
        private static double threeValRgbToOneVal(string r, string g, string b) {
            double colour = 0;
            colour += (int)(float.Parse(r) / 256.0) << 16;
            colour += (int)(float.Parse(g) / 256.0) << 8;
            colour += (int)(float.Parse(b) / 256.0);
            return colour;
        }
        #endregion
    }
}
