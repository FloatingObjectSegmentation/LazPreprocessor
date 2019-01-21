using common.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using common;

namespace external_tools.common
{
    public class PointCloudiaFormatSerializer
    {
        public static string PointBoundingBoxAndMaxDimFormat(List<AugmentableObjectSample> samples) {
            // position bbox_vertex1_position ... bbox_vertex8_position maxDim
            // x,y,z    x,y,z                     x,y,z                 double
            // space separated
            string str = "";
            for (int i = 0; i < samples.Count; i++)
            {
                str += samples[i].location.X + "," + samples[i].location.Y + "," + samples[i].location.Z + " ";

                List<Vector3> boundingPoints = BoundingBox.GetVertices(samples[i]);
                str += String.Join(" ", boundingPoints.Select((x) => { return x.X + "," + x.Y + "," + x.Z; }).ToList());
                str += " ";
                str += samples[i].maxDim + "\n";
            }

            return str;
        }

        public static string AugmentableSampleResultFormat(List<AugmentableObjectSample> samples, List<double> RbnnMinValsPerObject) {
            // id   position scale  shape airplane_position distance_from_transitive_floor
            // int  x,y,z    x,y,z  BIRD  x,y,z             double
            // space separated
            string a = "";
            int currid = 0;
            for (int i = 0; i < samples.Count; i++)
            {

                List<string> vals = new List<string>();

                string id = currid.ToString();
                vals.Add(id);
                string pos = samples[i].location.X + "," + samples[i].location.Y + "," + samples[i].location.Z;
                vals.Add(pos);
                string scale = samples[i].sizeMeters.X + "," + samples[i].sizeMeters.Y + "," + samples[i].sizeMeters.Z;
                vals.Add(scale);
                string shape = samples[i].name;
                vals.Add(shape);
                string airplane_pos = samples[i].location.X + "," + samples[i].location.Y + "," + samples[i].location.Z + 1000.0f;
                vals.Add(airplane_pos);
                string dist = RbnnMinValsPerObject[i].ToString();
                vals.Add(dist);
                a += String.Join(" ", vals) + "\n";
            }

            return a;
        }
    }
}
