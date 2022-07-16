using System;
using Unity.Collections;
using UnityEngine;

namespace DataVisualization
{
    public struct GradientStruct : IDisposable
    {
        public NativeArray<GradientColorKey> keys;

        public GradientStruct(GradientColorKey[] keys)
        {
            this.keys = new NativeArray<GradientColorKey>(keys, Allocator.Persistent);
        }

        public Color Evaluate(float t)
        {
            int i = 0;
            for (; i < keys.Length - 1; i++)
            {
                float currentTime = keys[i].time;
                float nextTime = keys[i + 1].time;

                if (t < nextTime)
                {
                    float localT = Mathf.InverseLerp(currentTime, nextTime, t);
                    return Color.Lerp(keys[i].color, keys[i + 1].color, localT);
                }
            }

            return keys[i].color;
        }

        public void Dispose()
        {
            keys.Dispose();
        }
    } 
}
