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
        public const string TOOL_AIRPLANE_HEIGHT_PATH = RESOURCES_FOLDER;
        #endregion

        #region [downloader]
        //// exec type
        public const TypeOfExecution TYPE_OF_EXEC = TypeOfExecution.CherryPick;

        // which chunks to get, (depending on execution type)
        public static int[] Range2D_CHUNKS = { 449, 121, 449, 121 }; //minx,miny,maxx,maxy in thousand, manualy set based on ARSO website
        public const string SINGLE_CHUNK = "460_103";
        public const string CHERRY_PICKED_CHUNKS = "462_100, 464_99, 467_99, 469_101, 469_103, 452_113, 442_110, 436_111, 428_116, 424_112, 410_97, 406_97, 386_95, 386_99," + 
                                                   "407_133, 405_132, 401_136, 453_141, 451_143, 472_134, 518_133, 601_158, 605_161, 613_156, 614_154, 549_156, 548_157," +
                                                   "534_159, 530_155, 525_147, 524_148, 524_147, 525_145, 414_125, 413_127, 408_122, 394_105, 393_105, 394_102, 396_93, " +
                                                   "401_46, 401_45, 400_45, 402_45, 395_44, 391_43, 388_43, 391_38, 390_40, 491_57";
        public static List<List<int>> CherryPicked_CHUNKS()
        {
            string[] chunks = CHERRY_PICKED_CHUNKS.Replace(" ", "").Split(new char[] { ',' });
            var parsed = chunks.Select(x =>
            {
                List<int> lst = new List<int>();
                string[] parts = x.Split(new char[] { '_' });
                lst.Add(int.Parse(parts[0]));
                lst.Add(int.Parse(parts[1]));
                return lst;
            }).ToList();
            Console.WriteLine($"Cherry picked {parsed.Count} chunks");
            return parsed;
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

        public static List<List<int>> GET_PICKED_CHUNKS() {
            List<List<int>> result = new List<List<int>>();
            if (TYPE_OF_EXEC == TypeOfExecution.Single)
                return SINGLE_CHUNK_VAL();
            else if (TYPE_OF_EXEC == TypeOfExecution.CherryPick)
                return CherryPicked_CHUNKS();
            else if (TYPE_OF_EXEC == TypeOfExecution.Range2D)
                return GetRange2D();
            else
                throw new Exception("UNKNOWN TYPE OF EXECUTION");
                
        }
        #endregion
        #endregion

        #region [core]
        public static string class_filename_suffix = "class";
        public static string intensity_filename_suffix = "intensity";
        public static string angle_filename_suffix = "angle";
        public static string direction_filename_suffix = "direction";
        public static string rbnn_core_result_prefix = "rbnnresult";
        public static double[] RBNN_R_VALUES = { 3, 4, 5, 6, 7, 8 };
        #endregion

        #region [augmentation_sampler]
        public static float candidateContextRadius = 25.0f; // in meters
        public static int ObjectsToAdd = 200;
        public static double[] AUGMENTATION_RBNN_R_SPAN = Numpy.LinSpace(0, 30, 15).ToArray();
        public static string rbnn_augmentation_result_prefix = "rbnnaugresult";
        public static Dictionary<int, AugmentableObject> GetAugmentableObjectPallette()
        {
            Dictionary<int, AugmentableObject> AugmentableObjects = new Dictionary<int, AugmentableObject>();
            AugmentableObjects = new Dictionary<int, AugmentableObject>();
            AugmentableObjects.Add(0, new AugmentableObject("BIRD", 0.3f, 0.5f, new Vector3(1.0f, 1.5f, 0.2f)));
            AugmentableObjects.Add(1, new AugmentableObject("AIRPLANE", 10.0f, 25.0f, new Vector3(1.0f, 1.2f, 0.25f)));
            AugmentableObjects.Add(2, new AugmentableObject("BALLOON", 5.0f, 10.0f, new Vector3(1.0f, 1.0f, 1.5f)));
            AugmentableObjects.Add(3, new AugmentableObject("DRONE", 0.5f, 1.5f, new Vector3(1.0f, 1.0f, 0.2f)));
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
