using System;
using System.Collections.Generic;
using System.Numerics;
using common.structs;

using System.IO;

using external_tools.filters;
using core;
using System.Linq;

namespace UndergroundTest
{
    class Program
    {

        static List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        static string workdir = @"E:\workspaces\LIDAR_WORKSPACE\test\";
        static string dmrfilename = "dmr";
        static string augsfilename = "augs";
        static List<int> solutions = new List<int>();

        static int dimension = 300;

        static void Main(string[] args)
        {
            
            string dmr = String.Join("\n", GenerateDmrGrid());

            // write dmr
            File.WriteAllText(Path.Combine(workdir, dmrfilename), dmr);
            new CoreProcedure(Path.Combine(workdir, dmrfilename)).Dmr2Pcd();

            for (int i = 0; i < 5000; i++) {
                Vector3 tmp = Vector3.Multiply((float)dimension, common.Numpy.RandomVector());
                tmp = Vector3.Subtract(tmp,
                                                                     new Vector3(0, 0, (float)dimension / 2.0f));
                AugmentableObjectSample samp = new AugmentableObjectSample("BIRD", new Vector3(0.1f, 0.1f, 0.1f), 
                                                    tmp,
                                                    5);
                samples.Add(samp);
                if (samp.location.Z > 0.0f)
                    solutions.Add(1);
                else
                    solutions.Add(0);
            }

            // write augs
            string serialized_augs = external_tools.common.PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            File.WriteAllText(Path.Combine(workdir, augsfilename), serialized_augs);

            // write sols
            File.WriteAllText(Path.Combine(workdir, "sols"), String.Join("\n", solutions));
            
            List<int> predictions = UndergroundFilter.Execute(samples, Path.Combine(workdir, dmrfilename));

            int[] preds = new int[samples.Count];
            foreach (int p in predictions) {
                preds[p] = 1;
            }

            for (int i = 0; i < predictions.Count; i++) {
                if (preds[i] != solutions[i])
                    Console.WriteLine(preds[i] + " " + solutions[i]);
            }
            Console.ReadLine();

        }

        static List<string> GenerateDmrGrid() {

            List<string> vec = new List<string>();
            for (int i = 0; i < dimension; i++) {
                for (int j = 0; j < dimension; j++) {
                    vec.Add($"{i}.00000;{j}.00000;0.00000");
                }
            }

            return vec;
        }
    }
}
