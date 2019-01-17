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
        public List<Vector3> locations = new List<Vector3>();
        public List<float> maxDims = new List<float>();

        public ObjectSampler() {
            Setup();
        }

        #region [API]
        public void UnfilteredSampleObjects(int ObjectsToAdd, Vector3 minWorldBound, Vector3 maxWorldBound)
        {
            for (int i = 0; i < 10 * ObjectsToAdd; i++)
            {
                AugmentableObjectSample curr = SampleObject();

                // sample point
                Vector3 location = SampleLocation(minWorldBound, maxWorldBound);

                samples.Add(curr);
                locations.Add(location);
                maxDims.Add(Math.Max(Math.Max(curr.sizeMeters.X, curr.sizeMeters.Y), curr.sizeMeters.Z));
            }
        }
        #endregion

        #region [sampling]
        private void Setup()
        {
            AugmentableObjects = new Dictionary<int, AugmentableObject>();
            AugmentableObjects.Add(0, new AugmentableObject("BIRD", 3f, 10f, new Vector3(1.0f, 1.5f, 0.2f)));
            AugmentableObjects.Add(1, new AugmentableObject("AIRPLANE", 30.0f, 50.0f, new Vector3(1.0f, 1.2f, 0.2f)));
            AugmentableObjects.Add(2, new AugmentableObject("BALLOON", 50.0f, 70.0f, new Vector3(1.0f, 1.0f, 2.5f)));
        }

        private AugmentableObjectSample SampleObject()
        {
            // sample object type, size
            int sample = new Random().Next(0, 3);
            AugmentableObject curr = AugmentableObjects[sample];

            float dimension = (float)((new Random().NextDouble()) * (curr.maxDimMeters - curr.minDimMeters) + curr.minDimMeters);
            Vector3 size = Vector3.Multiply(dimension, curr.proportions);

            return new AugmentableObjectSample(curr.name, size);
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

    #region [classes]
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
    #endregion
}
