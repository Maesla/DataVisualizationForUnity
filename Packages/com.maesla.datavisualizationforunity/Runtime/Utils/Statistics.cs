using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataVisualization
{
    public static class Statistics
    {
        public static Vector3 Average(IList<Vector3> values)
        {
            Vector3 mean = Vector3.zero;
            for (int i = 0; i < values.Count; i++)
            {
                mean = mean + values[i];
            }
            mean = mean / values.Count;
            return mean;
        }

        public static Vector3 Variance(IList<Vector3> values)
        {
            Vector3 mean = Average(values);
            Vector3 variance = Vector3.zero;

            for (int i = 0; i < values.Count; i++)
            {
                var difference = values[i] - mean;
                var squaredDifference = Vector3.Scale(difference, difference);
                variance = variance + squaredDifference;
            }

            variance = variance / values.Count;

            return variance;
        }
    }
}
