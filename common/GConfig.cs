using System;
using System.Collections.Generic;
using System.Text;

namespace common
{
    public static class GConfig
    {
        // workspace dirs
        public const string WORKSPACE_DIR = @"C:\Users\km\Desktop\LIDAR_WORKSPACE";
        public const string LIDAR_SUBDIR = "lidar";
        public const string AUGMENTATION_SUBDIR = "augmentation";
        public const string DMR_SUBDIR = "dmr";
        
        // tool paths
        public const string TOOL_RBNN_PATH = @"C:\Users\km\source\repos\LazPreprocessor\core\resources";
        public const string TOOL_OVERLAP_COMPUTE_PATH = @"C:\Users\km\source\repos\LazPreprocessor\augmentation_sampler\resources";
        public const string TOOL_UNDERGROUND_FILTER_PATH = @"C:\Users\km\source\repos\LazPreprocessor\augmentation_sampler\resources";
        
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
            some[0] = int.Parse(CHUNK.Split('_')[0]);
            some[1] = int.Parse(CHUNK.Split('_')[1]);
            List<List<int>> outer = new List<List<int>>();
            outer.Add(some);
            return outer;
        }
    }

    public enum TypeOfExecution
    {
        Range2D = 1,
        CherryPick = 2,
        Single = 3
    }
}
