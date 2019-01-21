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

        public AugmentableObjectSample(string name, Vector3 sizeMeters)
        {
            this.name = name;
            this.sizeMeters = sizeMeters;
        }
    }
}
