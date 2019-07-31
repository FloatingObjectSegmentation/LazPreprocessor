using common.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace common
{
    public static class GConfig
    {
        #region [global]
        // workspace dirs
        public const string WORKSPACE_DIR = @"E:\workspaces\LIDAR_WORKSPACE";
        public const string LIDAR_SUBDIR = "lidar";
        public const string AUGMENTATION_SUBDIR = "augmentation";
        public const string DMR_SUBDIR = "dmr";
        public const string TESTS_SUBDIR = "tests";

        // tool paths
        public const string RESOURCES_FOLDER = @"C:\Users\km\Desktop\playground\LazPreprocessor\external_tools\resources";
        public const string TOOL_RBNN_PATH = RESOURCES_FOLDER;
        public const string TOOL_OVERLAP_COMPUTE_PATH = RESOURCES_FOLDER;
        public const string TOOL_UNDERGROUND_FILTER_PATH = RESOURCES_FOLDER;
        #endregion

        #region [downloader]
        //// exec type
        public const TypeOfExecution TYPE_OF_EXEC = TypeOfExecution.CherryPick;

        // which chunks to get, (depending on execution type)
        public static int[] Range2D_CHUNKS = { 449, 121, 449, 121 }; //minx,miny,maxx,maxy in thousand, manualy set based on ARSO website
        public const string SINGLE_CHUNK = "449_121";
        public static List<List<int>> CherryPicked_CHUNKS()
        {
            List<List<int>> CherryPicked = new List<List<int>>();
            CherryPicked.Add(new List<int> { 464, 99 });
            CherryPicked.Add(new List<int> { 463, 99 });
            /*CherryPicked.Add(new List<int> { 464, 132 });
            CherryPicked.Add(new List<int> { 509, 124 });
            CherryPicked.Add(new List<int> { 521, 126 });
            CherryPicked.Add(new List<int> { 401, 46 });
            CherryPicked.Add(new List<int> { 413, 47 });
            CherryPicked.Add(new List<int> { 414, 47 });
            CherryPicked.Add(new List<int> { 410, 127 });*/
            return CherryPicked;
        }
        
        #region helper methods (DO NOT CONFIGURE)
        public static List<List<int>> GetRange2D()
        {
            List<List<int>> DesiredChunks = new List<List<int>>();
            for (var x = GConfig.Range2D_CHUNKS[0]; x <= GConfig.Range2D_CHUNKS[2]; x++)
            {
                for (var y = GConfig.Range2D_CHUNKS[1]; y <= GConfig.Range2D_CHUNKS[3]; y++)
                {
                    DesiredChunks.Add(new List<int>() { x, y });
                }
            }
            return DesiredChunks;
        }
        public static List<List<int>> SINGLE_CHUNK_VAL()
        {
            List<int> some = new List<int>(2);
            string[] a = SINGLE_CHUNK.Split('_');
            some.Add(int.Parse(a[0]));
            some.Add(int.Parse(a[1]));
            List<List<int>> outer = new List<List<int>>();
            outer.Add(some);
            return outer;
        }
        #endregion
        #endregion

        #region [core]
        public static string class_filename_suffix = "class";
        public static string intensity_filename_suffix = "intensity";
        public static string rbnn_core_result_prefix = "rbnnresult";
        public static double[] RBNN_R_VALUES = { 3 };
        #endregion

        #region [augmentation_sampler]
        public static float candidateContextRadius = 25.0f; // in meters
        public static int ObjectsToAdd = 500;
        public static double[] AUGMENTATION_RBNN_R_SPAN = Numpy.LinSpace(2, 30, 4).ToArray();
        public static string rbnn_augmentation_result_prefix = "rbnnaugresult";
        public static Dictionary<int, AugmentableObject> GetAugmentableObjectPallette()
        {
            Dictionary<int, AugmentableObject> AugmentableObjects = new Dictionary<int, AugmentableObject>();
            AugmentableObjects = new Dictionary<int, AugmentableObject>();
            AugmentableObjects.Add(0, new AugmentableObject("BIRD", 3f, 10f, new Vector3(1.0f, 1.5f, 0.2f)));
            AugmentableObjects.Add(1, new AugmentableObject("AIRPLANE", 30.0f, 50.0f, new Vector3(1.0f, 1.2f, 0.2f)));
            AugmentableObjects.Add(2, new AugmentableObject("BALLOON", 50.0f, 70.0f, new Vector3(1.0f, 1.0f, 2.5f)));
            return AugmentableObjects;
        }
        #endregion
    }

    public enum TypeOfExecution
    {
        Range2D = 1,
        CherryPick = 2,
        Single = 3
    }
}
