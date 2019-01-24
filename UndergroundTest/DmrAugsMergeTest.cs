using common.structs;
using external_tools.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    class DmrAugsMergeTest : Test
    {
        public override string TEST_SUBSUBDIR { get { return Path.Combine(TESTS_SUBDIR, "dmr_augs_merge_test"); } set { } }

        static string source_file;
        string pbbmdf_save_file;

        public DmrAugsMergeTest() : base() {
            source_file = @"E:\workspaces\LIDAR_WORKSPACE\augmentation\augmentation_resultend.txt";
            pbbmdf_save_file = Path.Combine(TEST_SUBSUBDIR, "pbbmdf.txt");
        }

        public void ExecuteTest(string[] args) {

            

            // load up some augmentables in AugmentableSampleResultFormat
            string str = File.ReadAllText(source_file);
            List<AugmentableObjectSample> samples = PointCloudiaFormatDeserializer.AugmentableSampleResultFormat(str);
            string pbbmdf = PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            File.WriteAllText(pbbmdf_save_file, pbbmdf);


            // Turn them to bounding box format

            // use python script to transform to lidar format

            // load up DMR, convert to lidar format

            // concat them both
        }


    }
}
