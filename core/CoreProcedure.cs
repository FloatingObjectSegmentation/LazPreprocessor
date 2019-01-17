using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace core
{
    public class CoreProcedure
    {
        /*
        Takes a folder full of laz files. For each of the
        laz files it will produce the following files in the 
        directory in which the laz files reside:
            - xyzRGB txt file
            - c txt file (the classes to which each point belongs to)
            - floating object class txt file (in each line only one entry
              -1 = not a floating objects, anything else is the cluster to
              which the floating object belongs)
        */

        #region [config]
        static double[] radius_values = { 1, 2, 3, 4, 5, 8 };
        static string rbnn_exe_location = @"C:\Users\km\source\repos\LazPreprocessor\core\resources";
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
            TransformTxtFileToPcdFile(filepath, ";");
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
           txt_class_path = filepath.Replace(".laz", "class.txt").Replace(".las", "class.txt");
           string txt_intensity_path = filepath.Replace(".laz", "intensity.txt").Replace(".las", "intensity.txt");
           Las2Txt.Exec(Path.GetDirectoryName(filepath), filepath, txt_path, attributes_basic);
           Las2Txt.Exec(Path.GetDirectoryName(filepath), filepath, txt_class_path, attributes_class);
           Las2Txt.Exec(Path.GetDirectoryName(filepath), filepath, txt_intensity_path, "i");
           pcd_path = txt_path.Replace(".txt", ".pcd");

        }

        void TransformTxtFileToPcdFile(string txt_path, string separator = " ")
        {
            Txt2Pcd.ExecXYZ(txt_path, separator);
        }

        private void ExecuteRbnnFromPcdFile()
        {
            string rbnn_result_filepath = RbnnDriver.Execute(rbnn_exe_location, pcd_path, "result", radius_values);
        }
        #endregion
    }
}
