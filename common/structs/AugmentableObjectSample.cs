using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace common.structs
{
    public class AugmentableObjectSample
    {
        public string name;
        public Vector3 sizeMeters;
        public Vector3 location;
        public double maxDim;
        public double airplaneHeight;
        public double rbnnMinVal;

        public AugmentableObjectSample(string name, Vector3 sizeMeters, Vector3 location, double maxDim)
        {
            this.name = name;
            this.sizeMeters = sizeMeters;
            this.location = location;
            this.maxDim = maxDim;
        }
    }
}
