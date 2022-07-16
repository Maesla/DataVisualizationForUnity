using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DataVisualization.Debug
{
    public class KDEDebug : MonoBehaviour
    {
        public Method method;
        public Vector2 minMax;
        public RawImage image;

        public Gradient gradient;
        public Vector2[] data;

        public List<Dot> sensors;
        private BaseHeatmap heatmap;

        public FloorTransformation floor;

        public GridTransformation.Settings settings;

        private BaseKDE kde;

        public int count = 1000;
        public float bandwidth = 0.2f;
        public float kernelMaxDistance = 3f;

        private void Start()
        {
            CreateSensors();
            CreateControllers();
            image.texture = heatmap.Texture;
            floor.SetTexture(heatmap.Texture);
        }

        private void CreateSensors()
        {
            sensors = new List<Dot>(count);

            for (int i = 0; i < count; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(go.GetComponent<Collider>());
                go.transform.parent = transform;
                go.transform.localScale = 0.1f * Vector3.one;

                go.transform.position = RandomPosition(); ;
                Dot newDot = new Dot() { t = go.transform, velocity = RandomVelocity() };
                sensors.Add(newDot);
            }
        }

        private void CreateControllers()
        {
            var positions = sensors.Select(sensor => sensor.t.position).ToArray();
            print(BaseKDE.BandwithEstimator(positions));


            kde = method == Method.Simple ? (BaseKDE)new KDE(positions, settings, bandwidth, kernelMaxDistance) : new KDEJobs(positions, settings, bandwidth, kernelMaxDistance);
            heatmap = method == Method.Simple ? new BaseHeatmap(kde.Grid, gradient, minMax.x, minMax.y) : new HeatmapJob(kde.Grid, gradient, minMax.x, minMax.y);
        }

        private Vector3 RandomPosition()
        {
            Func<float, float, float> randFunc = RandomGaussian;
            float x = randFunc(settings.minEdge.x, settings.maxEdge.x);
            float z = randFunc(settings.minEdge.y, settings.maxEdge.y);
            return new Vector3(x, 0f, z);
        }
        private Vector3 RandomVelocity()
        {
            Vector2 velocity2 = UnityEngine.Random.insideUnitCircle;
            Vector3 velocity = new Vector3(velocity2.x, 0f, velocity2.y);
            return velocity * 1;
        }

        private void Update()
        {
            UpdateElements(Time.deltaTime);
            UpdateGrids();
        }

        private void UpdateElements(float dt)
        {
            for (int i = sensors.Count - 1; i >= 0; i--)
            {
                sensors[i].UpdatePosition(dt);
                Vector3 pos = sensors[i].t.position;

                if (pos.x > settings.maxEdge.x || pos.x < settings.minEdge.x || pos.z > settings.maxEdge.y || pos.z < settings.minEdge.y)
                {
                    Destroy(sensors[i].t.gameObject);
                    sensors.RemoveAt(i);
                }
            }
        }



        private void UpdateGrids()
        {
            UpdateKDE();
            heatmap.Update(kde.Grid);

            floor.Set(kde.gridTransformation.settings);
        }


        private void UpdateKDE()
        {
            kde.Update(sensors.Select(sensor => sensor.t.position).ToArray());
        }

        public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
        {
            float u, v, S;

            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                v = 2.0f * UnityEngine.Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);

            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            float mean = (minValue + maxValue) / 2.0f;
            float sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }

        [ContextMenu("Fill")]
        private void Fill()
        {
            sensors =
                transform
                .Cast<Transform>()
                .Select(
                    (child, index) =>
                        {
                            child.position = new Vector3(data[index].x, 0f, data[index].y);
                            return new Dot() { t = child, value = 0f };
                        })
                .ToList();
        }

        [ContextMenu("Save")]
        private void Save()
        {
            byte[] bytes = heatmap.Texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/../SaveImages/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        }

        private void OnApplicationQuit()
        {
            heatmap.Dispose();
            kde.Dispose();
        }
    }

}