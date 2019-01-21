using common.structs;
using System;
using System.Collections.Generic;
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
        
        // tool paths
        public const string TOOL_RBNN_PATH = @"C:\Users\km\source\repos\LazPreprocessor\core\resources";
        public const string TOOL_OVERLAP_COMPUTE_PATH = @"C:\Users\km\source\repos\LazPreprocessor\augmentation_sampler\resources";
        public const string TOOL_UNDERGROUND_FILTER_PATH = @"C:\Users\km\source\repos\LazPreprocessor\augmentation_sampler\resources";
        #endregion

        #region [downloader]
        //// exec type
        public const TypeOfExecution TYPE_OF_EXEC = TypeOfExecution.Single;

        // cherry picked
        public static List<List<int>> CherryPicked()
        {
            List<List<int>> CherryPicked = new List<List<int>>();
            CherryPicked.Add(new List<int> { 449, 121 });
            return CherryPicked;
        }

        // range2D
        public static int[] Range2D = { 449, 121, 449, 121 }; //minx,miny,maxx,maxy in thousand, manualy set based on ARSO website
        public static List<List<int>> GetRange2D()
        {
            List<List<int>> DesiredChunks = new List<List<int>>();
            for (var x = GConfig.Range2D[0]; x <= GConfig.Range2D[2]; x++)
            {
                for (var y = GConfig.Range2D[1]; y <= GConfig.Range2D[3]; y++)
                {
                    DesiredChunks.Add(new List<int>() { x, y });
                }
            }
            return DesiredChunks;
        }

        // single chunk
        public const string CHUNK = "449_121";
        public static List<List<int>> CHUNK_VAL()
        {
            List<int> some = new List<int>(2);
            string[] a = CHUNK.Split('_');
            some.Add(int.Parse(a[0]));
            some.Add(int.Parse(a[1]));
            List<List<int>> outer = new List<List<int>>();
            outer.Add(some);
            return outer;
        }
        #endregion

        #region [core]
        public static double[] RBNN_R_VALUES = { 3 };
        #endregion

        #region [augmentation_sampler]
        public static float candidateContextRadius = 25.0f; // in meters
        public static int ObjectsToAdd = 500;
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
