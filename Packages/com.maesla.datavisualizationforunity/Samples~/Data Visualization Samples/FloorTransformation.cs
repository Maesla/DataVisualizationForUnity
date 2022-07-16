using UnityEngine;

namespace DataVisualization.Debug
{
    public class FloorTransformation : MonoBehaviour
    {
        public void SetTexture(Texture2D texture)
        {
            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }

        public void Set(GridTransformation.Settings settings)
        {
            Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.right);
            transform.rotation = rotation;
            Vector3 scale = (settings.maxEdge - settings.minEdge);
            transform.localScale = scale + Vector3.forward;
            transform.position = rotation * ((settings.maxEdge - settings.minEdge) * 0.5f + settings.minEdge);
        }
    } 
}
