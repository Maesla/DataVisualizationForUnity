using System;
using System.Linq;
using UnityEngine;

namespace DataVisualization.Debug
{
    public class GradientStructDebug : MonoBehaviour
    {
        public Gradient gradient;
        public Color c;

        [Range(0f, 1f)]
        public float t;

        public void Update()
        {
            Func<float, Color> getColor = gradient.Evaluate;
            GradientColorKey[] keys = Enumerable.Range(0, 10).Select(i => { float t = i / 9f; return new GradientColorKey(getColor(t), t); }).ToArray();
            GradientStruct gradientStruct = new GradientStruct(keys);
            c = gradientStruct.Evaluate(t);
        }
    } 
}
