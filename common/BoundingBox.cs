using common.structs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace common
{
    public class BoundingBox
    {
        public static List<Vector3> GetVertices(AugmentableObjectSample curr) {
            // compute the current candidate's bounding points
            List<Vector3> BoundingBoxPoints = new List<Vector3>();
            int x = 1, y = 1, z = 1;
            for (int c = 0; c < 2; c++)
            {
                x = -x;
                for (int j = 0; j < 2; j++)
                {
                    y = -y;
                    for (int k = 0; k < 2; k++)
                    {
                        z = -z;

                        BoundingBoxPoints.Add(new Vector3(curr.location.X + x * curr.sizeMeters.X / 2,
                            curr.location.Y + y * curr.sizeMeters.Y / 2,
                            curr.location.Z + z * curr.sizeMeters.Z / 2));
                    }
                }
            }

            return BoundingBoxPoints;
        }
    }
}
