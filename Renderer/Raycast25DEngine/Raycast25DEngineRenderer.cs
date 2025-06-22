using UnityEngine;
using UnityEngine.UI;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/Renderer/ShooterDoomLikeRenderer/SpriteDoomRenderer")]
    public class Raycast25DEngineRenderer : UPDBBehaviour
    {
        [SerializeField]
        private PlayerController _player;

        [SerializeField]
        private RawImage _imageComponent;

        private Texture2D _raycastRenderTexture;

        private void Awake()
        {
            _raycastRenderTexture = new Texture2D(_player.RayNumbers, _player.VerticalPixelNumbers);
            _raycastRenderTexture.filterMode = FilterMode.Point;

            for (int y = 0; y < _raycastRenderTexture.height; y++)
                for (int x = 0; x < _raycastRenderTexture.width; x++)
                    _raycastRenderTexture.SetPixel(x, y, Color.white);
            _raycastRenderTexture.Apply();

            _imageComponent.texture = _raycastRenderTexture;
        }
        private void FixedUpdate()
        {
            DrawTexture();
        }

        private void DrawTexture()
        {
            if (_player.RaysList == null || _player.RaysList.Count == 0)
                return;

            for (int x = 0; x < _raycastRenderTexture.width; x++)
            {
                int height = _player.RaysList[x] ? Mathf.RoundToInt(_raycastRenderTexture.height / _player.RaysList[x].distance) : 0;
                height = Mathf.RoundToInt(height * _player.VerticalPixelNumbers) / _player.VerticalPixelNumbers;
                int baseHeight = ((_raycastRenderTexture.height - height) / 2) - (_player.VerticalLookingValue);

                Vector2 collidedPos = Vector2.zero;
                Vector2Int collidedWall = Vector2Int.zero;
                CellData collidedCellData = null;
                Texture2D cellTexture = null;
                Vector2 textureCoords = Vector2Int.zero;
                int textureXPos = 0;
                int multipliedHeight = 0;

                if (_player.RaysList[x])
                {
                    collidedPos = _player.RaysList[x].transform.position;
                    collidedWall = new Vector2Int(Mathf.FloorToInt(collidedPos.x), Mathf.FloorToInt(collidedPos.y));

                    collidedCellData = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[collidedWall.x, collidedWall.y].CellType;
                    cellTexture = collidedCellData.Texture;

                    textureCoords = (_player.RaysList[x].point - collidedPos) * cellTexture.width;
                    textureXPos = Mathf.FloorToInt(textureCoords.x) + Mathf.FloorToInt(textureCoords.y);

                    multipliedHeight = Mathf.RoundToInt(height * collidedCellData.Height);
                }

                for (int y = 0; y < _raycastRenderTexture.height; y++)
                {
                    if (y < baseHeight)
                        _raycastRenderTexture.SetPixel(x, y, Color.grey);
                    else if (y >= multipliedHeight + baseHeight)
                        _raycastRenderTexture.SetPixel(x, y, Color.cyan);
                    else
                        DrawFacePixel(x, y, multipliedHeight, baseHeight, cellTexture, textureXPos);
                }
            }

            _raycastRenderTexture.Apply();
        }

        private void DrawFacePixel(int x, int y, int multipliedHeight, int baseHeight, Texture2D cellTexture, int textureXPos)
        {
            int dy = y - baseHeight;
            int textureYPos = (dy * cellTexture.height) / multipliedHeight;

            _raycastRenderTexture.SetPixel(x, y, cellTexture.GetPixel(textureXPos, textureYPos));
        }
    }
}

