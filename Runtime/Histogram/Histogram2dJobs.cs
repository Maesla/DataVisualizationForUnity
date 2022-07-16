using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace DataVisualization
{
    public class Histogram2dJobs : BaseHistogram2d
    {
        private JobHandle jobHandle;

        public Histogram2dJobs(IList<Vector3> positions, GridTransformation.Settings settings, bool density) : base(positions, settings, density){}

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

            float densityFactor = 1.0f;

            if (density)
            {
                densityFactor = 1f / (BinArea * sampleCount);
            }

            NativeArray<float> rawData = histogram.GetRawData();
            CalculateValue calculate_job = new CalculateValue()
            {
                rawData = rawData,
                rowPositions = rowPositions,
                columnPositions = columnPositions,
                rowCount = histogram.RowCount,
                columnCount = histogram.ColumnCount,
                sampleCount = sampleCount,
                densityFactor = densityFactor
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
            public int rowCount;
            [ReadOnly]
            public int columnCount;
            [ReadOnly]
            public int sampleCount;

            public float densityFactor;

            public void Execute(int index)
            {
                int row = index / columnCount;
                int column = index % columnCount;

                float value = 0;
                for (int i = 0; i < sampleCount; i++)
                {
                    if (row == rowPositions[i] && column == columnPositions[i])
                    {
                        value += 1f * densityFactor;
                    }
                }

                rawData[index] = value;
            }

        }
    }
}
