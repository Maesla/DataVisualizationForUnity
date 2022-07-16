using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace DataVisualization
{
    public class KDEJobs : BaseKDE
    {
        private JobHandle jobHandle;

        public KDEJobs(IList<Vector3> positions, GridTransformation.Settings settings, float bandwidth, float kernelMaxDistance) : base(positions, settings, bandwidth, kernelMaxDistance) {}

        public override void Update(IList<Vector3> positions)
        {
            int sampleCount = positions.Count;
            NativeArray<int> rowPositions = new NativeArray<int>(sampleCount, Allocator.TempJob);
            NativeArray<int> columnPositions = new NativeArray<int>(sampleCount, Allocator.TempJob);

            for (int i = 0; i < sampleCount; i++)
            {
                gridTransformation.GlobalPositionToRowColumn(positions[i], out int row, out int column);
                rowPositions[i] = row;
                columnPositions[i] = column;
            }

            NativeArray<float> rawData = grid.GetRawData();
            CalculateValue calculate_job = new CalculateValue()
            {
                rawData = rawData,
                rowPositions = rowPositions,
                columnPositions = columnPositions,
                kernel = kernel.kernelCache.GetRawData(),
                kernelRowCount = kernel.rowCount,
                kernelColumnCount = kernel.columnCount,
                bandwidth = bandwidth,
                rowCount = grid.RowCount,
                columnCount = grid.ColumnCount,
                totalSampleCount = totalSampleCount,
                sampleCount = sampleCount
            };

            jobHandle = calculate_job.Schedule(rawData.Length, 1);
            jobHandle.Complete();

            rowPositions.Dispose();
            columnPositions.Dispose();
        }

        [BurstCompile]
        private struct CalculateValue : IJobParallelFor
        {
            [WriteOnly]
            public NativeArray<float> rawData;

            [ReadOnly]
            public NativeArray<int> rowPositions;
            [ReadOnly]
            public NativeArray<int> columnPositions;

            [ReadOnly]
            public NativeArray<float> kernel;

            [ReadOnly]
            public int kernelRowCount;
            [ReadOnly]
            public int kernelColumnCount;

            [ReadOnly]
            public float bandwidth;

            [ReadOnly]
            public int rowCount;
            [ReadOnly]
            public int columnCount;

            [ReadOnly]
            public int totalSampleCount;

            [ReadOnly]
            public int sampleCount;

            public void Execute(int index)
            {
                int row = index / columnCount;
                int column = index % columnCount;

                float value = 0f;

                for (int i = 0; i < sampleCount; i++)
                {
                    value += Calculate(rowPositions[i] - row, columnPositions[i] - column);
                }

                rawData[index] = value / (totalSampleCount * bandwidth);
            }

            private float Calculate(int rowDiff, int columnDiff)
            {
                if (rowDiff < 0)
                    return Calculate(-rowDiff, columnDiff);
                if (columnDiff < 0)
                    return Calculate(rowDiff, -columnDiff);

                if (rowDiff > kernelRowCount - 1 || columnDiff > kernelColumnCount - 1)
                    return 0f;

                int linearIndex = rowDiff * kernelColumnCount + columnDiff;


                return kernel[linearIndex];
            }
        }
    }
}
