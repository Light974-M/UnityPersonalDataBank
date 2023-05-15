using UnityEngine;

namespace UPDB.ProceduralGeneration.NoiseGenerator
{
    ///<summary>
    /// 
    ///</summary>
    public abstract class NoiseTextureGenerator : ScriptableObject
    {
        [SerializeField]
        private Vector2Int _textureSize = new Vector2Int(100, 100);
        public Vector2Int TextureSize => _textureSize;

        public abstract Texture2D Generate();
    }
}

