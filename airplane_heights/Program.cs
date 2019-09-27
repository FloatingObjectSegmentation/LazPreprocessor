using common;
using common.structs;
using external_tools.airplane_height;
using external_tools.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace airplane_heights
{
    class Program
    {

        static string augmentable_dir = Path.Combine(GConfig.WORKSPACE_DIR, GConfig.AUGMENTATION_SUBDIR + "\\augmentables");
        static string destination = "C:\\Users\\km\\Desktop\\kurac.txt";

        static void Main(string[] args)
        {
            foreach (List<int> s in GConfig.GET_PICKED_CHUNKS())
            {
                string chunk_name = s[0] + "_" + s[1];
                string path = Path.Combine(augmentable_dir, chunk_name + "augmentation_result_transformed.txt");
                string content = File.ReadAllText(path);
                List<AugmentableObjectSample> samples = PointCloudiaFormatDeserializer.AugmentableSampleResultFormat(content);
                List<double> results = new AirplaneHeight().Execute(samples, Path.Combine(Path.Combine(GConfig.WORKSPACE_DIR, GConfig.DMR_SUBDIR), "pcd" + chunk_name));

                samples = samples.Zip(results, (x, y) =>
                {
                   x.airplaneHeight = y + 1000.0f;
                   return x;
                }).ToList();

                List<string> scan_directions_to_append = PointCloudiaFormatDeserializer.GetDirectionDefiningPoints(content);
                List<string> basic = PointCloudiaFormatSerializer.AugmentableSampleResultFormat(samples).Split('\n').ToList();

                string final_result = 
                    string.Join('\n',
                    basic.Zip(scan_directions_to_append, (x, y) =>
                    {
                        return x + " " + y;
                    }).ToList());
                File.WriteAllText(Path.Combine(augmentable_dir, chunk_name + "augmentation_result_transformed_airplane_heights.txt"), final_result);

            }                
        }
    }
}
