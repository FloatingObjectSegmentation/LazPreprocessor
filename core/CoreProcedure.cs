using common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using external_tools.rbnn;
using external_tools.transforms;

namespace core
{
    public class CoreProcedure
    {
        /*
        Takes a laz file. it will produce the following files in the 
        directory in which the laz file resides:
            - xyzRGB txt file
            - c txt file (the classes to which each point belongs to)
            - floating object class txt file (in each line only one entry
              -1 = not a floating objects, anything else is the cluster to
              which the floating object belongs)

        Also takes a dmr file and creates a pcd dmr file.
        */

        #region [config]
        static double[] radius_values = GConfig.RBNN_R_VALUES;
        #endregion

        #region [members]
        string filepath;
        string txt_path;
        string txt_class_path;
        string pcd_path;
        static string attributes_basic = "xyzRGB";
        static string attributes_class = "c";
        #endregion

        public CoreProcedure(string filepath) {
            this.filepath = filepath;
        }

        public void Dmr2Pcd() {
            TransformTxtFileToPcdFile(filepath, ";", "pcd");
        }
        
        public void PreprocessLaz()
        {
            CreateRGBClassIntensityTxtFilesFromLaz();
            TransformTxtFileToPcdFile(txt_path);
            ExecuteRbnnFromPcdFile();
            File.Delete(pcd_path);
        }

        #region [auxiliary]
        void CreateRGBClassIntensityTxtFilesFromLaz() {
           txt_path = filepath.Replace(".laz", ".txt").Replace(".las", ".txt");
           txt_class_path = filepath.Replace(".laz", $"{GConfig.class_filename_suffix}.txt").Replace(".las", $"{GConfig.class_filename_suffix}.txt");
           string txt_intensity_path = filepath.Replace(".laz", $"{GConfig.intensity_filename_suffix}.txt").Replace(".las", $"{GConfig.intensity_filename_suffix}.txt");
           Las2Txt.Exec(Path.GetDirectoryName(filepath), filepath, txt_path, attributes_basic);
           Las2Txt.Exec(Path.GetDirectoryName(filepath), filepath, txt_class_path, attributes_class);
           Las2Txt.Exec(Path.GetDirectoryName(filepath), filepath, txt_intensity_path, "i");
           pcd_path = txt_path.Replace(".txt", ".pcd");

        }

        void TransformTxtFileToPcdFile(string txt_path, string separator = " ", string pathappend = "")
        {
            Txt2Pcd.ExecXYZ(txt_path, separator, pathappend);
        }

        private void ExecuteRbnnFromPcdFile()
        {
            string rbnn_result_filepath = RbnnDriver.Execute(pcd_path, GConfig.rbnn_core_result_prefix, radius_values);
        }
        #endregion
    }
}
