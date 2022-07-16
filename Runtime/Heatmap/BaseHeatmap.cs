using System;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataVisualization
{
    public class BaseHeatmap : IDisposable
    {
        public Texture2D Texture { get => texture; }

        protected Texture2D texture;
        protected float min;
        protected float max;
        private Func<float, Color> getColor;

        protected int width;
        protected int height;

        private Color32 color32;

        public BaseHeatmap(IGrid data, Gradient gradient)
        {
            int rowCount = data.RowCount;
            int columnCount = data.ColumnCount;

            float max = Mathf.NegativeInfinity;
            float min = Mathf.Infinity;
            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    float value = data[row, column];
                    if (value > max) max = value;
                    if (value < min) min = value;
                }
            }

            CreateHeightmap(data, gradient, min, max);
        }

        public BaseHeatmap(IGrid data, Gradient gradient, float min, float max)
        {
            CreateHeightmap(data, gradient, min, max);
        }

        protected virtual void CreateHeightmap(IGrid data, Gradient gradient, float min, float max)
        {
            this.getColor = gradient.Evaluate;
            this.min = min;
            this.max = max;

            int rowCount = data.RowCount;
            int columnCount = data.ColumnCount;

            this.width = columnCount;
            this.height = rowCount;

            texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false)
            {
                filterMode = FilterMode.Point,
                hideFlags = HideFlags.HideAndDontSave
            };

            color32 = new Color32(255, 255, 255, 255);

            Update(data);
        }

        public virtual void Update(IGrid source)
        {
            NativeArray<Color32> textureData = texture.GetRawTextureData<Color32>();

            int count = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = source[count]; // grid is linearly aligned with texture
                    value = Mathf.InverseLerp(min, max, value);
                    Color color = getColor(value);
                    FastConversion(color);

                    textureData[count] = color32;
                    count++;
                }
            }

            texture.Apply(false);
        }

        public void FastConversion(Color c)
        {
            color32.r = (byte)(c.r * 255f);
            color32.g = (byte)(c.g * 255f);
            color32.b = (byte)(c.b * 255f);
        }

        public virtual void Dispose()
        {
            Object.Destroy(texture);
        }
    }
}