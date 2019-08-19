using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace bitmap
{
    class Program
    {
        static void Main(string[] args)
        {

            string source = @"E:\workspaces\LIDAR_WORKSPACE\lidar\521_126.txt";

            Bitmap bmp = new Bitmap(3000, 3000);

            string[] lines = File.ReadAllLines(source);
            List<Tuple<double, double>> values = new List<Tuple<double, double>>();


            // find list of values
            foreach (string line in lines) {
                string[] parts = line.Split(" ");
                double x = double.Parse(parts[0]);
                double y = double.Parse(parts[1]);
                values.Add(new Tuple<double, double>(x, y));
            }

            // find mins
            double minx = int.MaxValue, miny = int.MaxValue;
            for (int i = 0; i < values.Count; i++) {
                double x = values[i].Item1;
                double y = values[i].Item2;
                if (x < minx) minx = x;
                if (y < miny) miny = y;
            }

            // fill bitmap
            for (int i = 0; i < values.Count; i++) {
                double x = values[i].Item1 - minx;
                double y = values[i].Item2 - miny;

                try {
                    int xidx = (int)(x * 3);
                    int yidx = (int)(y * 3);
                    bmp.SetPixel(xidx, yidx, Color.FromArgb(255, 0, 0));
                } catch (Exception ex) { }
            }

            bmp.Save(source + ".jpg");

        }
    }
}
