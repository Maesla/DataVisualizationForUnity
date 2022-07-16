using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace DataVisualization
{
    public class SpatialInterpolationByJobs : BaseSpatialInterpolation
    {
        private JobHandle jobHandle;

        public SpatialInterpolationByJobs(IList<ISensor> sensors, GridTransformation.Settings settings, float p) : base(sensors, settings, p){}

        public override void Update()
        {
            NativeArray<float> values = new NativeArray<float>(sensors.Count, Allocator.TempJob);
            NativeArray<Vector3> positions = new NativeArray<Vector3>(sensors.Count, Allocator.TempJob);

            for (int i = 0; i < sensors.Count; i++)
            {
                sensors[i].WarmUp();
                values[i] = sensors[i].Value;
                positions[i] = sensors[i].Position;
            }

            NativeArray<float> rawData = grid.GetRawData();
            CalculateValue calculate_job = new CalculateValue()
            {
                values = values,
                positions = positions,
                rawData = rawData,
                transformation = transformation,
                p = p,
                rowCount = grid.RowCount,
                columnCount = grid.ColumnCount
            };

            jobHandle = calculate_job.Schedule(rawData.Length, grid.RowCount);
            jobHandle.Complete();

            values.Dispose();
            positions.Dispose();
        }

        public override float GetWeight(int row, int column, int sensorIndex, int linearIndex)
        {
            return -1f;
        }


        [BurstCompile]
        private struct CalculateValue : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float> values;
            [ReadOnly]
            public NativeArray<Vector3> positions;
            
            [WriteOnly]
            public NativeArray<float> rawData;

            [ReadOnly]
            public GridTransformation transformation;

            [ReadOnly]
            public float p;

            [ReadOnly]
            public int rowCount;
            [ReadOnly]
            public int columnCount;

            public void Execute(int index)
            {
                float numerator = 0;
                float denominator = 0;

                int i = index/columnCount;
                int j = index%columnCount;
                transformation.RowColumnToGlobalPosition(i, j, out Vector3 nodeGlobalPosition);

                for (int sensorIndex = 0; sensorIndex < values.Length; sensorIndex++)
                {
                    float sensorValue = values[sensorIndex];

                    float distance = Vector3.Distance(nodeGlobalPosition, positions[sensorIndex]);
                    float weight = Mathf.Pow(1f / distance, p);
                    numerator += sensorValue * weight;
                    denominator += weight;
                }

                float value = numerator / denominator;
                rawData[index] = value;
            }
        }
    }
}
