using UnityEngine;

namespace UPDB.ProceduralGeneration.NoiseGenerator
{
    ///<summary>
    /// 
    ///</summary>
    public class CustomPerlinNoiseGenerator : NoiseTextureGenerator
    {

        [SerializeField, Range(0, 1)]
        private float _minValue = 0f;
        public float MinValue => _minValue;

        [SerializeField, Range(0, 1)]
        private float _maxValue = 1f;
        public float MaxValue => _maxValue;


        public override Texture2D Generate()
        {
            Texture2D texture = new Texture2D(TextureSize.x, TextureSize.y);
            texture.filterMode = FilterMode.Point;

            for (int y = 0; y < texture.height; y++)
                for (int x = 0; x < texture.width; x++)
                    texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, Random.Range(_minValue, _maxValue)));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float value = texture.GetPixel(x, y).r;
                    if(x > 0) { value += texture.GetPixel(x - 1, y).r; }
                    if(y > 0) { value += texture.GetPixel(x, y - 1).r; }
                    if(x < texture.width - 1) { value += texture.GetPixel(x + 1, y).r; }
                    if(x < texture.height - 1) { value += texture.GetPixel(x, y + 1).r; }

                    texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, value / 5));
                }
            }
            texture.Apply();
            return texture;
        }
    } 
}