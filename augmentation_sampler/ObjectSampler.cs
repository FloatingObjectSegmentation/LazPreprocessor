using common.structs;
using common;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace augmentation_sampler
{
    class ObjectSampler
    {

        static Dictionary<int, AugmentableObject> AugmentableObjects;

        public List<AugmentableObjectSample> samples = new List<AugmentableObjectSample>();
        public Vector3 minWorldBound;
        public Vector3 maxWorldBound;

        public ObjectSampler() {
            AugmentableObjects = GConfig.GetAugmentableObjectPallette();
        }

        #region [API]
        public void UnfilteredSampleObjects(int ObjectsToAdd, Vector3 minWorldBound, Vector3 maxWorldBound)
        {
            this.minWorldBound = minWorldBound;
            this.maxWorldBound = maxWorldBound;

            for (int i = 0; i < 10 * ObjectsToAdd; i++)
            {
                AugmentableObjectSample curr = SampleObject();
                samples.Add(curr);
            }
        }
        #endregion

        #region [sampling]
        private AugmentableObjectSample SampleObject()
        {
            // sample object type, size
            int sample = new Random().Next(0, 3);
            AugmentableObject curr = AugmentableObjects[sample];

            float dimension = (float)((new Random().NextDouble()) * (curr.maxDimMeters - curr.minDimMeters) + curr.minDimMeters);
            Vector3 size = Vector3.Multiply(dimension, curr.proportions);
            Vector3 location = SampleLocation(minWorldBound, maxWorldBound);

            return new AugmentableObjectSample(curr.name, size, location, Math.Max(Math.Max(size.X, size.Y), size.Z));
        }

        private Vector3 SampleLocation(Vector3 minBound, Vector3 maxBound)
        {
            double X = new Random().NextDouble() * (maxBound.X - minBound.X) + minBound.X;
            double Y = new Random().NextDouble() * (maxBound.Y - minBound.Y) + minBound.Y;
            double Z = new Random().NextDouble() * (maxBound.Z - minBound.Z) + minBound.Z;
            return new Vector3((float)X, (float)Y, (float)Z);
        }
        #endregion
    }
}
