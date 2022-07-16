using UnityEngine;

namespace DataVisualization.Debug
{
    public class GridTransformationDebug : MonoBehaviour
    {
        public GridTransformation transformation;

        public int row;
        public int column;
        public Vector3 resultGlobalPosition;


        public Vector3 globalPosition;
        public int expectedRow;
        public int expectedColumn;

        private void Update()
        {

            transformation.RefreshMatrix();

            transformation.RowColumnToGlobalPosition(row, column, out resultGlobalPosition);
            transformation.GlobalPositionToRowColumn(globalPosition, out expectedRow, out expectedColumn);
        }
    } 
}
