using System;
using UnityEngine;

namespace DataVisualization.Debug
{
    [Serializable]
    public class Dot : ISensor
    {
        public float value;
        public Transform t;

        public Vector3 Position { get; private set; }

        public float Value => value;

        public void WarmUp()
        {
            Position = t.position;
        }

        public Vector3 velocity;

        public void UpdatePosition(float dt)
        {
            t.transform.position = t.transform.position + velocity * dt;
        }
    } 
}