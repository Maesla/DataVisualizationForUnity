using System;
using Unity.Collections;

namespace DataVisualization
{
    public class NativeGrid : IGrid, IDisposable
    {
        private readonly DenseArray<float> data;

        public NativeGrid(int rowCount, int columnCount)
        {
            data = new DenseArray<float>(new[] { rowCount, columnCount });
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public float this[int index]
        {
            get
            {
                return data.rawData[index];
            }
            set
            {
                data.rawData[index] = value;
            }
        }
        public float this[int row, int column]
        {
            get => this[row*ColumnCount + column];
            set => this[row * ColumnCount + column] = value;
        }

        public int RowCount { get; private set; }

        public int ColumnCount { get; private set; }

        public int Length => data.Lenght;

        public NativeArray<float> GetRawData()
        {
            return data.rawData;
        }

        public void Dispose()
        {
            data.Dispose();
        }
    }
}
