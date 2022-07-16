using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace DataVisualization
{
    public class HeatmapJob : BaseHeatmap
    {

        private GradientStruct gradientStruct;
        private JobHandle jobHandle;

        public HeatmapJob(IGrid data, Gradient gradient):base(data, gradient){}

        public HeatmapJob(IGrid data, Gradient gradient, float min, float max): base(data, gradient, min, max){}

        protected override void CreateHeightmap(IGrid data, Gradient gradient, float min, float max)
        {
            gradientStruct = new GradientStruct(gradient.colorKeys);
            base.CreateHeightmap(data, gradient, min, max);
        }

        public override void Update(IGrid source)
        {

            NativeArray<Color32> PixelArray = texture.GetRawTextureData<Color32>();
            NativeArray<float> gridData = ((NativeGrid)source).GetRawData();

            CalculatePixel calculate_job = new CalculatePixel()
            {
                Pixels = PixelArray,
                data = gridData,
                min = min,
                max = max,
                height = height,
                width = width,
                gradient = gradientStruct
            };

            jobHandle = calculate_job.Schedule(PixelArray.Length, 32);
            jobHandle.Complete();
            texture.Apply(false);
        }

        public override void Dispose()
        {
            gradientStruct.Dispose();
            base.Dispose();
        }

        [BurstCompile]
        struct CalculatePixel : IJobParallelFor
        {
            [WriteOnly]
            public NativeArray<Color32> Pixels;
            
            [ReadOnly]
            public NativeArray<float> data;
            public float min;
            public float max;
            public int width;
            public int height;

            [ReadOnly]
            public GradientStruct gradient;

            public void Execute(int index)
            {
                float value = data[index];
                float t = Mathf.InverseLerp(min, max, value);
                Pixels[index] = FastConversion(gradient.Evaluate(t));
            }

            private Color32 FastConversion(Color c)
            {
                return new Color32((byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), 255);
            }
        }

    }
}
