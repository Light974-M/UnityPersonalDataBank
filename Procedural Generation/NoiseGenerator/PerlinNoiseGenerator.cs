using UnityEngine;

namespace UPDB.ProceduralGeneration.NoiseGenerator
{
    ///<summary>
    /// 
    ///</summary>
    public class PerlinNoiseGenerator : NoiseTextureGenerator
    {

        [SerializeField, Range(0, 1)]
        private float _minValue = 0f;
        public float MinValue => _minValue;

        [SerializeField, Range(0, 1)]
        private float _maxValue = 1f;
        public float MaxValue => _maxValue;

        [SerializeField]
        private Vector2 _scale = Vector2.zero;

        [SerializeField]
        private Vector2 _offset = Vector2.zero;


        public override Texture2D Generate()
        {
            Texture2D texture = new Texture2D(TextureSize.x, TextureSize.y);
            texture.filterMode = FilterMode.Point;

            for (int y = 0; y < texture.height; y++)
                for (int x = 0; x < texture.width; x++)                                 
                    texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, Mathf.PerlinNoise((((float)x + _offset.x) / texture.width * _scale.x), ((float)y + _offset.y) / texture.height * _scale.y)));
           
            texture.Apply();
            return texture;
        }
    } 
}