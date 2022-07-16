using UnityEngine;

namespace DataVisualization
{
    public static class Colormap
    {
        public static Gradient Standard = 
            Factory(new Color[] { new Color32(49, 122, 173, 255), new Color32(137, 193, 135, 255), new Color32(251, 252, 169, 255), new Color32(253, 181, 101, 255), new Color32(202, 40, 62, 255) });

        private static Gradient Factory(Color [] colors)
        {
            GradientColorKey[] colorKey = new GradientColorKey[colors.Length]; 
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[colors.Length];


            for (int i = 0; i < colorKey.Length; i++)
            {
                colorKey[i].color = colors[i];
                colorKey[i].time = i / (colors.Length - 1f);

                alphaKey[i].alpha = 1f;
                alphaKey[i].time = i / (colors.Length - 1f);
            }

            var gradient = new Gradient();
            gradient.SetKeys(colorKey, alphaKey);
            return gradient;
        }
    }
}
