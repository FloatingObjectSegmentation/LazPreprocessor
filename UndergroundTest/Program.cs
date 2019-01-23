using System;
using System.Collections.Generic;
using System.
using System.Numerics;
using common.structs;

namespace UndergroundTest
{
    class Program
    {

        static List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        static string dmr_filename = "";
        static List<bool> solutions = new List<bool>();

        static void Main(string[] args)
        {
            string dmr = String.Join("\n", GenerateDmrGrid());

            for (int i = 0; i < 5000; i++) {
                AugmentableObjectSample samp = new AugmentableObjectSample("BIRD", new Vector3(5, 5, 5), Vector3.Multiply(1000.0f, common.Numpy.RandomVector()), 5);
                samples.Add(samp);

            }
        }

        static List<string> GenerateDmrGrid() {

            List<string> vec = new List<string>();
            for (int i = 0; i < 1000; i++) {
                for (int j = 0; j < 1000; j++) {
                    vec.Add($"{i}.00000 {j}.00000 0.00000");
                }
            }

            return vec;
        }
    }
}
