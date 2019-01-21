using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace common.structs
{
    public class AugmentableObject
    {

        public string name;
        public float minDimMeters;
        public float maxDimMeters;
        public Vector3 proportions;

        public AugmentableObject(string name, float minDimMeters, float maxDimMeters, Vector3 proportions)
        {
            this.name = name;
            this.minDimMeters = minDimMeters;
            this.maxDimMeters = maxDimMeters;
            this.proportions = proportions;
        }
    }
}
