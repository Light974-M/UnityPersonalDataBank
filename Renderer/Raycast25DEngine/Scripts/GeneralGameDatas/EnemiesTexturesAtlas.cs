using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class EnemiesTexturesAtlas : UPDBBehaviour
    {
        [SerializeField]
        private Texture2DArray _enemiesAtlas;

        [SerializeField]
        private Vector2Int _enemyTextureMaxSize;

        private void Awake()
        {
            //List<Texture2D> _textureList = new List<Texture2D>();

            //foreach (EnemyController controller in FindObjectsByType<EnemyController>(FindObjectsSortMode.InstanceID))
            //    _textureList.Add(controller.Sprite);

            //_enemiesAtlas = new Texture2DArray(_enemyTextureMaxSize.x, _enemyTextureMaxSize.y, _textureList.Count, TextureFormat.ARGB32, false);

            //_enemiesAtlas.filterMode = FilterMode.Point;
            //_enemiesAtlas.wrapMode = TextureWrapMode.Repeat;

            //for (int i = 0; i < _textureList.Count; i++)
            //    if (_textureList[i])
            //        Graphics.CopyTexture(_textureList[i], 0, 0, _enemiesAtlas, i, 0);

            //_enemiesAtlas.Apply();
        }
    }
}
