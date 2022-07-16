using System;
using System.Collections.Generic;

namespace DataVisualization
{
    public abstract class BaseSpatialInterpolation : IDisposable
    {

        protected GridTransformation transformation;

        protected float p;

        protected NativeGrid grid;
        public IGrid Grid => grid;


        protected IList<ISensor> sensors;
        private float[] sensorValues;
        private int linearIndex;

        public BaseSpatialInterpolation(IList<ISensor> sensors, GridTransformation.Settings settings, float p)
        {
            transformation = new GridTransformation(settings);
            this.p = p;
            CreateSpatialInterpolation(sensors);
        }
 
        private void CreateSpatialInterpolation(IList<ISensor> sensors)
        {
            this.sensors = sensors;
            sensorValues = new float[sensors.Count];
            grid = new NativeGrid(transformation.settings.resolution.y, transformation.settings.resolution.x);
            transformation.RefreshMatrix();
            ChildInit();
        }

        protected virtual void ChildInit() { }

        public virtual void Update()
        {
            for (int i = 0; i < sensorValues.Length; i++)
            {
                sensors[i].WarmUp();
                sensorValues[i] = sensors[i].Value;
            }

            linearIndex = 0;
            for (int i = 0; i < Grid.RowCount; i++)
            {
                for (int j = 0; j < Grid.ColumnCount; j++)
                {
                    Grid[i, j] = InterpolateSpatialy(i, j);
                }
            }
        }

        private float InterpolateSpatialy(int row, int column)
        {
            float numerator = 0;
            float denominator = 0;

            for (int sensorIndex = 0; sensorIndex < sensorValues.Length; sensorIndex++)
            {
                float sensorValue = sensorValues[sensorIndex];
                float weight = GetWeight(row, column, sensorIndex, linearIndex++);
                numerator += sensorValue * weight;
                denominator += weight;
            }

            return numerator / denominator;
        }

        public abstract float GetWeight(int row, int column, int sensorIndex, int linearIndex);

        public virtual void Dispose()
        {
            grid.Dispose();
        }
    } 
}
