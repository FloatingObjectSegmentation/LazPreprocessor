using common.structs;
using external_tools.common;
using si.birokrat.next.common.shell;
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
        string dmrlidar;

        public DmrAugsMergeTest() : base() {
            source_file = @"E:\workspaces\LIDAR_WORKSPACE\augmentation\augmentation_resultend.txt";
            dmrlidar = @"E:\workspaces\LIDAR_WORKSPACE\tests\dmr_augs_merge_test\lidar_dmr";
            pbbmdf_save_file = Path.Combine(TEST_SUBSUBDIR, "pbbmdf.txt");
        }

        public void ExecuteTest(string[] args) {

            

            // load up some augmentables in AugmentableSampleResultFormat
            string str = File.ReadAllText(source_file);
            List<AugmentableObjectSample> samples = PointCloudiaFormatDeserializer.AugmentableSampleResultFormat(str);
            string pbbmdf = PointCloudiaFormatSerializer.PointBoundingBoxAndMaxDimFormat(samples);
            File.WriteAllText(pbbmdf_save_file, pbbmdf);

            string pythonscriptsdir = @"C:\Users\km\Desktop\playground\pycharmprojects\floating_object_segmentation_scripts\other\";
            string pbmdftolidar_path = Path.Combine(pythonscriptsdir, "PBBMDF_to_lidar.py");
            string lidar_to_number_per_line = Path.Combine(pythonscriptsdir, "lidar_to_number_per_line.py");
            string lidar_to_dummy_rbnn_results = Path.Combine(pythonscriptsdir, "lidar_to_dummy_rbnn_results.py");
            string merge_two_lidars = Path.Combine(pythonscriptsdir, "merge_two_lidars.py");

            // use python script to transform to lidar format
            PowerShell.Execute($"python {pbmdftolidar_path} {dmrlidar} {pbbmdf_save_file} {Path.Combine(TEST_SUBSUBDIR, "lidar_pbbmdf")} 1");
            PowerShell.Execute($"python {merge_two_lidars} {dmrlidar} {Path.Combine(TEST_SUBSUBDIR, "lidar_pbbmdf")} {Path.Combine(TEST_SUBSUBDIR, "result.txt")}");
            PowerShell.Execute($"python {lidar_to_number_per_line} {Path.Combine(TEST_SUBSUBDIR, "result.txt")} {Path.Combine(TEST_SUBSUBDIR, "resultintensity.txt")} 0");
            PowerShell.Execute($"python {lidar_to_number_per_line} {Path.Combine(TEST_SUBSUBDIR, "result.txt")} {Path.Combine(TEST_SUBSUBDIR, "resultclass.txt")} 0");
            PowerShell.Execute($"python {lidar_to_dummy_rbnn_results} {Path.Combine(TEST_SUBSUBDIR, "result.txt")} {Path.Combine(TEST_SUBSUBDIR, "resultrbnn.txt")}");
        }


    }
}
