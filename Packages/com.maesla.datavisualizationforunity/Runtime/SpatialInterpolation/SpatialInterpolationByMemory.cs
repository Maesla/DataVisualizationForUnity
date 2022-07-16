using System.Collections.Generic;
using UnityEngine;

namespace DataVisualization
{
    public class SpatialInterpolationByMemory : BaseSpatialInterpolation
    {
        private DenseArray<float> weights;

        public SpatialInterpolationByMemory(IList<ISensor> sensors, GridTransformation.Settings settings, float p) : base(sensors, settings, p){}

        protected override void ChildInit()
        {
            weights = new DenseArray<float>(new int[] { transformation.settings.resolution.y, transformation.settings.resolution.x, sensors.Count });
            CalculateWeights();
        }

        public void RefreshWeights()
        {
            CalculateWeights();
        }

        private void CalculateWeights()
        {
            foreach (var sensor in sensors)
            {
                sensor.WarmUp();
            }

            int index = 0;
            for (int i = 0; i < Grid.RowCount; i++)
            {
                for (int j = 0; j < Grid.ColumnCount; j++)
                {
                    transformation.RowColumnToGlobalPosition(i, j, out Vector3 nodeGlobalPosition);
                    for (int z = 0; z < sensors.Count; z++)
                    {
                        float distance = Vector3.Distance(nodeGlobalPosition, sensors[z].Position);
                        float weight = Mathf.Pow(1f / distance, p);

                        //weights[i, j, z] = weight;
                        weights.rawData[index++] = weight;
                    }
                }
            }
        }

        public override float GetWeight(int row, int column, int sensorIndex, int linearIndex)
        {
            //return weights[row, column, sensorIndex];
            return weights.rawData[linearIndex];
        }

        public override void Dispose()
        {
            weights.Dispose();
            base.Dispose();
        }
    } 
}
