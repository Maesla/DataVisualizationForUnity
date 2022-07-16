namespace DataVisualization
{
    public interface IGrid
    {
        public int RowCount { get; }
        public int ColumnCount { get; }
        public int Length { get; }
        float this[int index] { get; set; }
        float this[int row, int colum] { get; set; }
    }
}
