using System.Collections.Generic;
using UnityEngine;

namespace DataVisualization
{
    public class Histogram2d : BaseHistogram2d
    {
        public Histogram2d(IList<Vector3> positions, GridTransformation.Settings settings, bool density) : base(positions, settings, density){}

        public Histogram2d(IList<float> x, IList<float> y, int xBins, int yBins, bool density) : base(x, y, xBins, yBins, density){}

        public override void Update(IList<Vector3> positions)
        {
            this.sampleCount = positions.Count;

            for (int i = 0; i < histogram.Length; i++)
            {
                histogram[i] = 0f;
            }

            float densityFactor = 1.0f;

            if (density)
            {
                densityFactor = 1f / (BinArea * sampleCount);
            }


            for (int i = 0; i < sampleCount; i++)
            {
                Vector3 globalPosition = positions[i];
                gridTransformation.GlobalPositionToRowColumn(globalPosition, out int rowIndex, out int columnIndex);
                histogram[rowIndex, columnIndex] += 1f * densityFactor; //counts 1 if normal behaviour. Counts 1/sampleCount/binArea if density behaviour
            }
        }
    }
}
