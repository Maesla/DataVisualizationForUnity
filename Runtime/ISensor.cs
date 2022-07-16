
using UnityEngine;

namespace DataVisualization
{
    public interface ISensor
    {
        Vector3 Position { get; }
        float Value { get; }

        void WarmUp();
    } 
}
