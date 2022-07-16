using System;
using UnityEngine;

namespace DataVisualization
{
    //https://en.wikipedia.org/wiki/Kernel_density_estimation#Definition
    public class Kernel :IDisposable
    {
        private readonly float normalizationFactor;
        private readonly float maxDistance;

        internal NativeGrid kernelCache;

        internal int rowCount;
        internal int columnCount;

        public Kernel(float xRange, float yRange, float bandwidth, float maxDistance)
        {
            this.maxDistance = maxDistance;
            this.normalizationFactor = 1f / Mathf.Sqrt(2f * Mathf.PI);

            rowCount = Mathf.CeilToInt(maxDistance * bandwidth / yRange);
            columnCount = Mathf.CeilToInt(maxDistance * bandwidth / xRange);

            //kernelCache = new DenseArray<float>(new[] { rowMax, columnMax });
            kernelCache = new NativeGrid(rowCount, columnCount);

            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(row * yRange, 2f) + Mathf.Pow(column * xRange, 2f));
                    kernelCache[row, column] = NormalDistribution(distance / bandwidth);
                }
            }

        }

        public float Calculate(Vector3 x, Vector3 xi, float h)
        {
            float distance = Vector3.Distance(x, xi);
            return NormalDistribution(distance / h);
        }

        public float Calculate(int rowDiff, int columnDiff)
        {
            if (rowDiff < 0)
                return Calculate(-rowDiff, columnDiff);
            if (columnDiff < 0)
                return Calculate(rowDiff, -columnDiff);

            if (rowDiff > rowCount - 1 || columnDiff > columnCount - 1)
                return 0f;

            return kernelCache[rowDiff, columnDiff];
        }

        public void Dispose()
        {
            kernelCache.Dispose();
        }

        private float NormalDistribution(float x)
        {
            if (x > maxDistance)
                return 0f;

            return Mathf.Exp(-x * x * 0.5f) *normalizationFactor;
        }

        //public float Calculate(Vector3 x, Vector3 xi, float h)
        //{
        //    float sqrtDistance = (x - xi).sqrMagnitude;
        //    sqrtDistance = Mathf.Abs(sqrtDistance);
        //    return Mathf.Exp(-sqrtDistance / (2f * h * h)) / Mathf.Sqrt(2f * Mathf.PI);
        //}

    } 
}
