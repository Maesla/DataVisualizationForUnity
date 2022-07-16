using System.Collections.Generic;
using UnityEngine;

namespace DataVisualization
{
    public class KDE : BaseKDE
    {
        public KDE(IList<Vector3> positions, GridTransformation.Settings settings, float bandwidth, float kernelMaxDistance) : base(positions, settings, bandwidth, kernelMaxDistance) {}

        public override void Update(IList<Vector3> positions)
        {
            int sampleCount = positions.Count;
            int[] rowPositions = new int[sampleCount];
            int[] columnPositions = new int[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                gridTransformation.GlobalPositionToRowColumn(positions[i], out int row, out int column);
                rowPositions[i] = row;
                columnPositions[i] = column;
            }

            for (int row = 0; row < Grid.RowCount; row++)
            {
                for (int column = 0; column < Grid.ColumnCount; column++)
                {
                    float value = 0f;

                    for (int i = 0; i < positions.Count; i++)
                    {
                        value += kernel.Calculate(rowPositions[i] - row, columnPositions[i] - column);
                    }

                    Grid[row, column] = value / (totalSampleCount * bandwidth);
                }
            }
        }
    }
}
