﻿using common;
using common.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace external_tools.common
{
    public class PointCloudiaFormatDeserializer
    {
        // only returns the aug_object_samples and not the rbnn min vals!
        public static List<AugmentableObjectSample> AugmentableSampleResultFormat(string augmentable_format_content)
        {
            // id   position scale  shape airplane_position distance_from_transitive_floor
            // int  x,y,z    x,y,z  BIRD  x,y,z             double
            // space separated
            List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
            string[] lines = augmentable_format_content.Split("\n");
            foreach (var line in lines)
            {
                if (line.Length < 5)
                    continue;

                string[] parts = line.Split(" ");

                int id = int.Parse(parts[0]);
                string[] pos_string = parts[1].Split(",");
                Vector3 pos = new Vector3(float.Parse(pos_string[0]), float.Parse(pos_string[1]), float.Parse(pos_string[2]));

                string[] scale_string = parts[2].Split(",");
                Vector3 scale = new Vector3(float.Parse(scale_string[0]), float.Parse(scale_string[1]), float.Parse(scale_string[2]));

                string name = parts[3];


                double rbnnminval = double.Parse(parts[5]);
                AugmentableObjectSample samp = new AugmentableObjectSample(name, scale, pos, Numpy.MaxDimension(scale));
                samp.rbnnMinVal = rbnnminval;

                samples.Add(samp);
            }
            return samples;
        }

        public static List<string> GetDirectionDefiningPoints(string augmentable_format_content) {

            List<string> samples = new List<string>();

            string[] lines = augmentable_format_content.Split("\n");

            foreach (var line in lines)
            {
                string[] parts = line.Split(" ");

                if (parts.Length < 7)
                    throw new Exception("At least one of the augmentations' scan directions is not labeled");

                samples.Add(string.Join(' ', parts.Skip(6).ToArray()));
                
            }
            return samples;
        }
    }
}
