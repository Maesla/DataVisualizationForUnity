using System;
using System.Linq;
using Unity.Collections;

namespace DataVisualization
{
    public class DenseArray<T> : IDisposable where T : struct
    {
        private readonly int[] size;
        public int Dimension { get; private set; }
        public int Lenght { get; private set; }
        public NativeArray<T> rawData;


        public DenseArray(int[] size)
        {
            this.size = size.ToArray();
            Dimension = this.size.Length;

            Lenght = 1;

            for (int i = 0; i < Dimension; i++)
            {
                Lenght *= size[i];
            }

            rawData = new NativeArray<T>(Lenght, Allocator.Persistent);
        }

        //https://eli.thegreenplace.net/2015/memory-layout-of-multi-dimensional-arrays
        //https://en.wikipedia.org/wiki/Row-_and_column-major_order#Address_calculation_in_general
        public int CalculateIndex(int[] indices)
        {
            int index = 0;
            for (int k = 0; k < Dimension; k++)
            {
                int p = 1;
                for (int l = k + 1; l < Dimension; l++)
                {
                    p *= size[l];
                }

                index += p * indices[k];
            }

            return index;
        }

        public void Set(T value, int[] indices)
        {
            int index = CalculateIndex(indices);
            rawData[index] = value;
        }

        public T Get(int[] indices)
        {
            int index = CalculateIndex(indices);
            return rawData[index];
        }

        public void Dispose()
        {
            rawData.Dispose();
        }

        public T this[params int[] indices]
        {
            set => Set(value, indices);
            get => Get(indices);
        }
    }

}