using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataVisualization
{
    internal class ValueToIndexTransformation
    {
        private readonly int n;
        private float min;
        private float max;
        private readonly Func<float, float> linearTrasformation;
        public float Range { get; private set; }

        public ValueToIndexTransformation(IList<float> data, int n)
        {
            this.n = n;
            GetMinMax(data);
            float scale = (max - min) / n;
            linearTrasformation = value => (value - min) / scale;
            Range = scale;
        }

        public int ValueToIndex(float value)
        {
            if (value == min) //lower limit
            {
                return 0;
            }
            if (value == max) //upper limit. Righthand-most are half open but the righthand-most, which is closed interval
            {
                return n - 1;
            }

            return (int)linearTrasformation(value);
        }

        private void GetMinMax(IList<float> data)
        {
            max = Mathf.NegativeInfinity;
            min = Mathf.Infinity;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
                if (data[i] < min)
                {
                    min = data[i];
                }
            }
        }
    } 
}
