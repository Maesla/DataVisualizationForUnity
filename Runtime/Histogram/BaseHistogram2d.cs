using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataVisualization
{
    public abstract class BaseHistogram2d : IDisposable
    {
        protected NativeGrid histogram;
        public IGrid Frequency => histogram;

        public GridTransformation gridTransformation;

        protected int sampleCount;
        protected bool density;
        public float BinArea { get; private set; }

        public BaseHistogram2d(IList<float> x, IList<float> y, int xBins, int yBins, bool density)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException("x and y must contain the same number of elements");
            }

            CalculateTransformation(x, y, xBins, yBins);
            CreateHistogram2d(x.Select((x, index) => new Vector3(x, 0f, y[index])).ToArray(), density);
        }


        private void CreateHistogram2d(IList<Vector3> positions, bool density)
        {
            this.sampleCount = positions.Count;
            int rowCount = gridTransformation.RowCount;
            int columnCount = gridTransformation.ColumnCount;
            this.histogram = new NativeGrid(rowCount, columnCount);
            this.density = density;
            BinArea = gridTransformation.Range.x * gridTransformation.Range.z;

            Update(positions);
        }

        public abstract void Update(IList<Vector3> positions);
           
        public BaseHistogram2d(IList<float> x, IList<float> y, int bins) : this(x, y, bins, bins, false) { }
        public BaseHistogram2d(IList<float> x, IList<float> y, int xBins, int yBins) : this(x, y, xBins, yBins, false) { }

        public BaseHistogram2d(IList<Vector3> positions, GridTransformation.Settings settings, bool density)
        {
            gridTransformation = new GridTransformation(settings);
            CreateHistogram2d(positions, density);
        }

        private void CalculateTransformation(IList<float> x, IList<float> y, int xBins, int yBins)
        {
            float minX = Mathf.Infinity;
            float minY = Mathf.Infinity;
            float maxX = Mathf.NegativeInfinity;
            float maxY = Mathf.NegativeInfinity;

            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] > maxX)
                {
                    maxX = x[i];
                }
                if (x[i] < minX)
                {
                    minX = x[i];
                }

                if (y[i] > maxY)
                {
                    maxY = y[i];
                }
                if (y[i] < minY)
                {
                    minY = y[i];
                }
            }

            Vector2 minEdge = new Vector2(minX, minY);
            Vector2 maxEdge = new Vector2(maxX, maxY);
            Vector2Int resolution = new Vector2Int(xBins, yBins);

            var settings = new GridTransformation.Settings()
            {
                minEdge = minEdge,
                maxEdge = maxEdge,
                resolution = resolution
            };

            gridTransformation = new GridTransformation(settings);

            gridTransformation.RefreshMatrix();
        }

        public void Dispose()
        {
            histogram.Dispose();
        }
    }
}
