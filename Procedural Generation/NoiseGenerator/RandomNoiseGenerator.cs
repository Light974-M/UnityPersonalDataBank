using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.ProceduralGeneration.NoiseGenerator
{
    ///<summary>
    /// 
    ///</summary>
    [CreateAssetMenu(fileName = "NewRandomNoiseGenerator", menuName = NamespaceID.UPDB + "/" + NamespaceID.ProceduralGeneration + "/NoiseGenerator")]
    public class RandomNoiseGenerator : NoiseTextureGenerator
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

            texture.Apply();
            return texture;
        }
    } 
}