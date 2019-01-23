using System;
using System.Collections.Generic;
using System.Numerics;
using common.structs;

using System.IO;

using external_tools.underground_filter;
using core;

namespace UndergroundTest
{
    class Program
    {

        static List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        static string dmr_filepath = @"E:\workspaces\LIDAR_WORKSPACE\dmr\testdmr";
        static List<int> solutions = new List<int>();

        static void Main(string[] args)
        {
            
            string dmr = String.Join("\n", GenerateDmrGrid());
            File.WriteAllText(dmr_filepath, dmr);
            new CoreProcedure(dmr_filepath).Dmr2Pcd();

            for (int i = 0; i < 5000; i++) {
                AugmentableObjectSample samp = new AugmentableObjectSample("BIRD", new Vector3(0.1f, 0.1f, 0.1f), 
                                                    Vector3.Subtract(Vector3.Multiply(1000.0f, common.Numpy.RandomVector()),
                                                                     new Vector3(0,0,-500)),
                                                    5);
                samples.Add(samp);
                if (samp.location.Z > 0.0f)
                    solutions.Add(1);
                else
                    solutions.Add(0);
            }

            List<int> predictions = UndergroundFilter.Execute(samples, dmr_filepath);

            for (int i = 0; i < predictions.Count; i++) {
                Console.WriteLine(predictions[i] + " " + solutions[i]);
            }
            Console.ReadLine();

        }

        static List<string> GenerateDmrGrid() {

            List<string> vec = new List<string>();
            for (int i = 0; i < 1000; i++) {
                for (int j = 0; j < 1000; j++) {
                    vec.Add($"{i}.00000;{j}.00000;0.00000");
                }
            }

            return vec;
        }
    }
}
