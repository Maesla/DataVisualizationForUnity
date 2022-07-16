using System.Collections.Generic;
using UnityEngine;

namespace DataVisualization
{
    public class SpatialInterpolationByCPU : BaseSpatialInterpolation
    {
        public SpatialInterpolationByCPU(IList<ISensor> sensors, GridTransformation.Settings settings, float p) : base(sensors, settings, p){}

        public override float GetWeight(int row, int column, int sensorIndex, int linearIndex)
        {
            ISensor sensor = sensors[sensorIndex];
            transformation.RowColumnToGlobalPosition(row, column, out Vector3 nodeGlobalPosition);

            float distance = Vector3.Distance(nodeGlobalPosition, sensor.Position);
            return Mathf.Pow(1f / distance, p);
        }
    } 
}
