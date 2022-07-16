using UnityEngine;
using System;

namespace DataVisualization
{
    [Serializable]
    public struct GridTransformation : ISerializationCallbackReceiver
    {
        [Serializable]
        public struct Settings
        {
            public Vector2Int resolution;

            public Vector2 minEdge;
            public Vector2 maxEdge;
        }

        public Settings settings;

        private Matrix4x4 gridToPositionTransformation;
        private Matrix4x4 positionToGridTransformation;

        private Vector3 range;
        public Vector3 Range { get => range; }

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public GridTransformation(Settings settings) : this()
        {
            this.settings = settings;
            RefreshMatrix();
        }

        public void RefreshMatrix()
        {
            gridToPositionTransformation = CalculateTrasformation();
            positionToGridTransformation = gridToPositionTransformation.inverse;
        }

        private Matrix4x4 CalculateTrasformation()
        {
            float scaleX = (settings.maxEdge.y - settings.minEdge.y) / (settings.resolution.y);
            float scaleZ = (settings.maxEdge.x - settings.minEdge.x) / (settings.resolution.x);
            Vector3 scale = new Vector3(-scaleX, 1f, scaleZ);

            Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.up);
            range = rotation * scale;
            range.y = 0f;

            Vector3 translation = settings.minEdge.y * Vector3.forward + settings.minEdge.x * Vector3.right;

            Matrix4x4 m = Matrix4x4.TRS(translation, rotation, scale);

            RowCount = settings.resolution.y;
            ColumnCount = settings.resolution.x;

            return m;
        }

        public void RowColumnToGlobalPosition(int row, int column, out Vector3 globalPosition)
        {
            Vector3 nodeLocalPosition = new Vector3(row, 0f, column);
            globalPosition = gridToPositionTransformation.MultiplyPoint3x4(nodeLocalPosition);
        }

        public void GlobalPositionToRowColumn(Vector3 globalPosition, out int row, out int column, bool rightHandMostCloseInterval = true)
        {
            Vector3 localPosition = positionToGridTransformation.MultiplyPoint3x4(globalPosition);
            if (rightHandMostCloseInterval && globalPosition.x == settings.maxEdge.x)
            {
                column = ColumnCount - 1;
            }
            else
            {
                column = (int)localPosition.z;
            }

            if (rightHandMostCloseInterval && globalPosition.z == settings.maxEdge.y)
            {
                row = RowCount - 1;
            }
            else
            {
                row = (int)localPosition.x;
            }


            //row = rightHandMostCloseInterval && localPosition.x == maxEdge.y ? (int)resolution.y - 1 : (int)localPosition.x;
            //column = rightHandMostCloseInterval && localPosition.z == resolution.x ? (int)resolution.x - 1 : (int)localPosition.z;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            RefreshMatrix();
        }
    } 
}
