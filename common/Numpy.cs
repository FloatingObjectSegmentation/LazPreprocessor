using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace common
{
    public static class Numpy
    {

        public static float MaxDimension(Vector3 vec) {
            return Math.Max(Math.Max(vec.X, vec.Y), vec.Z);
        }

        public static float MinDimension(Vector3 vec)
        {
            return Math.Min(Math.Min(vec.X, vec.Y), vec.Z);
        }

        public static Vector3 RandomVector() {
            float x = (float) new Random().NextDouble();
            float y = (float) new Random().NextDouble();
            float z = (float) new Random().NextDouble();
            return new Vector3(x, y, z);
        }

        public static IEnumerable<double> Arange(double start, int count)
        {
            return Enumerable.Range((int)start, count).Select(v => (double)v);
        }

        public static IEnumerable<double> Power(IEnumerable<double> exponents, double baseValue = 10.0d)
        {
            return exponents.Select(v => Math.Pow(baseValue, v));
        }

        public static IEnumerable<double> LinSpace(double start, double stop, int num, bool endpoint = true)
        {
            var result = new List<double>();
            if (num <= 0)
            {
                return result;
            }

            if (endpoint)
            {
                if (num == 1)
                {
                    return new List<double>() { start };
                }

                var step = (stop - start) / ((double)num - 1.0d);
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }
            else
            {
                var step = (stop - start) / (double)num;
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }

            return result;
        }

        public static IEnumerable<double> LogSpace(double start, double stop, int num, bool endpoint = true, double numericBase = 10.0d)
        {
            var y = LinSpace(start, stop, num: num, endpoint: endpoint);
            return Power(y, numericBase);
        }
    }
}
