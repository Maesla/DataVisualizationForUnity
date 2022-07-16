using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataVisualization
{
    public abstract class BaseKDE : IDisposable
    {
        protected NativeGrid grid;
        public IGrid Grid => grid;

        public GridTransformation gridTransformation;

        public float BinArea { get; private set; }

        protected Kernel kernel;
        protected float bandwidth;
        protected int totalSampleCount;

        public BaseKDE(IList<Vector3> positions, GridTransformation.Settings settings, float bandwidth, float kernelMaxDistance)
        {
            gridTransformation = new GridTransformation(settings);
            CreateKDE(positions, bandwidth, kernelMaxDistance);
        }

        private void CreateKDE(IList<Vector3> positions, float bandwidth, float kernelMaxDistance)
        {
            this.bandwidth = bandwidth;
            int rowCount = gridTransformation.RowCount;
            int columnCount = gridTransformation.ColumnCount;
            grid = new NativeGrid(rowCount, columnCount);
            BinArea = gridTransformation.Range.x * gridTransformation.Range.z;
            kernel = new Kernel(gridTransformation.Range.x, gridTransformation.Range.z, bandwidth, kernelMaxDistance);
            totalSampleCount = positions.Count;
            Update(positions);
        }

        public abstract void Update(IList<Vector3> positions);

        public void Dispose()
        {
            grid.Dispose();
            kernel.Dispose();
        }

        //https://en.wikipedia.org/wiki/Kernel_density_estimation#A_rule-of-thumb_bandwidth_estimator
        public static float BandwithEstimator(IList<Vector3> positions)
        {
            Vector3 variance = Statistics.Variance(positions);
            float unitVariance = variance.magnitude;
            float standardDeviation = Mathf.Sqrt(unitVariance);
            float count = positions.Count;
            return 1.06f * standardDeviation / Mathf.Pow(count, 1 / 5f);
        }
    }
}
