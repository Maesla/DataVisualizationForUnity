using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using HeatmapImplementation = DataVisualization.HeatmapJob;

namespace DataVisualization.Debug
{
    public class SpatialInterpolationDebug : MonoBehaviour
    {

        public GridTransformation.Settings settings;
        public float p = 2;


        public Gradient gradient;
        public RawImage image;

        public BaseSpatialInterpolation spatialInterpolation;

        public Dot[] sensors;

        public HeatmapImplementation heatmap;

        public bool isDirty;

        public FloorTransformation floor;


        private void OnEnable()
        {
            spatialInterpolation = new SpatialInterpolationByJobs(sensors, settings, p);
            heatmap = new HeatmapImplementation(spatialInterpolation.Grid, gradient, 0f, 45f);
            floor.SetTexture(heatmap.Texture);
        }

        public void Update()
        {
            if (sensors.Select(sensor => sensor.t).Any(transform => transform.hasChanged))
            {
                if (spatialInterpolation is SpatialInterpolationByMemory spatialInterpolationByMemory)
                {
                    spatialInterpolationByMemory.RefreshWeights();
                }

                isDirty = true;
            }


            if (isDirty)
            {
                spatialInterpolation.Update();
                heatmap.Update(spatialInterpolation.Grid);

                image.texture = heatmap.Texture;

                sensors.All(s => { s.t.hasChanged = false; return true; });

                SetFloor();

                isDirty = false;
            }
        }

        private void SetFloor()
        {
            floor.Set(settings);
        }


        private void OnValidate()
        {
            isDirty = true;
        }

        private void OnApplicationQuit()
        {
            spatialInterpolation.Dispose();
            heatmap.Dispose();
        }
    } 
}
