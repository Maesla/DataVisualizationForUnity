namespace DataVisualization
{
    public class Grid : IGrid
    {
        private readonly float [,] data;

        public Grid(int rowCount, int columnCount)
        {
            data = new float[rowCount, columnCount];
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public float this[int index]
        {
            get
            {
                int row = index / ColumnCount;
                int column = index % ColumnCount;
                return this[row, column];
            }
            set 
            {
                int row = index / ColumnCount;
                int column = index % ColumnCount;
                this[row, column] = value;
            }
        }
        public float this[int row, int column]
        {
            get => data[row, column];
            set => data[row, column] = value;
        }

        public int RowCount { get; private set; }

        public int ColumnCount { get; private set; }

        public int Length => RowCount*ColumnCount;
    } 
}
