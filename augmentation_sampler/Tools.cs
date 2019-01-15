using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace augmentation_sampler
{
    public static class Tools
    {

        public static Vector3 FindMinimumVector(List<string> LidarFileLines)
        {
            Vector3 minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            LidarFileLines.ForEach((l) =>
            {
                float[] parts = l.Split(" ").Select((k) => float.Parse(k)).ToArray();
                float x = parts[0], y = parts[1], z = parts[2];
                if (x < minCoord.X) minCoord.X = x;
                if (y < minCoord.Y) minCoord.Y = y;
                if (z < minCoord.Z) minCoord.Z = z;
            });
            return minCoord;
        }

        public static Vector3 FindMinimumVector(string lidarPath)
        {
            List<string> LidarFileLines = File.ReadAllLines(lidarPath).ToList();
            Vector3 minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            LidarFileLines.ForEach((l) =>
            {
                float[] parts = l.Split(" ").Select((k) => float.Parse(k)).ToArray();
                float x = parts[0], y = parts[1], z = parts[2];
                if (x < minCoord.X) minCoord.X = x;
                if (y < minCoord.Y) minCoord.Y = y;
                if (z < minCoord.Z) minCoord.Z = z;
            });
            return minCoord;
        }

        public static Vector3 FindMaximumVector(List<string> LidarFileLines)
        {
            Vector3 maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            LidarFileLines.ForEach((l) =>
            {
                float[] parts = l.Split(" ").Select((k) => float.Parse(k)).ToArray();
                float x = parts[0], y = parts[1], z = parts[2];
                if (x > maxCoord.X) maxCoord.X = x;
                if (y > maxCoord.Y) maxCoord.Y = y;
                if (z > maxCoord.Z) maxCoord.Z = z;
            });
            return maxCoord;
        }

        public static Vector3 FindMaximumVector(string lidarPath)
        {
            List<string> LidarFileLines = File.ReadAllLines(lidarPath).ToList();
            Vector3 maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            LidarFileLines.ForEach((l) =>
            {
                float[] parts = l.Split(" ").Select((k) => float.Parse(k)).ToArray();
                float x = parts[0], y = parts[1], z = parts[2];
                if (x > maxCoord.X) maxCoord.X = x;
                if (y > maxCoord.Y) maxCoord.Y = y;
                if (z > maxCoord.Z) maxCoord.Z = z;
            });
            return maxCoord;
        }

    }
}
