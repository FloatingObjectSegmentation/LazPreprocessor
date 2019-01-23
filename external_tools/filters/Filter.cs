using common.structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace external_tools.filters
{
    public class Filter
    {
        public static List<AugmentableObjectSample> DiscardFilteredExamples(List<int> indicesOfDiscarded, List<AugmentableObjectSample> samples)
        {
            HashSet<int> setIndicesOfDiscarded = new HashSet<int>(indicesOfDiscarded);

            // discard the overlapping examples
            List<AugmentableObjectSample> filteredSamples = new List<AugmentableObjectSample>();

            for (int i = 0; i < samples.Count; i++)
            {
                if (!setIndicesOfDiscarded.Contains(i))
                {
                    filteredSamples.Add(samples[i]);
                }
            }
            samples.Clear();
            samples = filteredSamples;
            return samples;
        }
    }
}
