using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class Raycast25DEngineRenderer : UPDBBehaviour
    {
        [SerializeField]
        private PlayerController _player;

        [SerializeField]
        private RawImage _imageComponent;

        public float _test = 1;
        private Texture2D _raycastRenderTexture;

        private void Awake()
        {
            _raycastRenderTexture = new Texture2D(_player.CPURayNumbers, _player.CPUVerticalPixelNumbers);
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

            // Color[] mainTextureToApplyColor = new Color[];
            float screenMiddle = _raycastRenderTexture.height / 2f;
            int horizonOffset = (int)_player.VerticalLookingValue;
            float playerYPos = _player.CPUCameraPosOffset.y - 0.5f;
            float horizon = screenMiddle - horizonOffset;

            for (int x = 0; x < _raycastRenderTexture.width; x++)
            {
                int height = _player.RaysList[x] ? (int)(_raycastRenderTexture.height / _player.RaysList[x].distance) : 0;
                height = (int)(height * _player.CPUVerticalPixelNumbers) / _player.CPUVerticalPixelNumbers;
                int baseHeight = (((_raycastRenderTexture.height - height) / 2) - horizonOffset) + (int)(height * (-playerYPos));

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
                    collidedWall = new Vector2Int((int)(collidedPos.x), (int)(collidedPos.y));

                    collidedCellData = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[collidedWall.x, collidedWall.y].CellType;
                    cellTexture = collidedCellData.WallTexture;

                    textureCoords = (_player.RaysList[x].point - collidedPos) * cellTexture.width;
                    textureXPos = (int)(textureCoords.x) + (int)(textureCoords.y);

                    multipliedHeight = (int)(height * collidedCellData.WallHeight);
                }

                float invMultipliedHeight = 1f / (float)multipliedHeight;

                Vector2 floorDir = (_player.RaysList[x].point - (Vector2)_player.transform.position).normalized;

                int floorBaseHeight = (((_raycastRenderTexture.height - height) / 2) - horizonOffset);
                float minDy = (horizon - floorBaseHeight) / horizon;
                float maxBase = 1f / minDy;

                for (int y = 0; y < _raycastRenderTexture.height; y++)
                {
                    if (y < baseHeight)
                    {
                        if(_player.CPUCameraPosOffset.y <= 0)
                        {
                            _raycastRenderTexture.SetPixel(x, y, Color.grey);
                            continue;
                        }

                        float dy = (horizon - y) / horizon;
                        //float rawBase = 1f / dy;

                        //float t = (rawBase - 1f) / (maxBase - 1f);
                        float rowDistance = ((_player.CPUCameraPosOffset.y + 0.5f) / dy);
                        rowDistance = (rowDistance / horizon) * screenMiddle;

                        //if (_player.CPUCameraPosOffset.y + 0.5f >= maxBase)
                        //{
                        //    _raycastRenderTexture.SetPixel(x, y, Color.grey);
                        //    continue;
                        //}

                        //float rowDistance = (Mathf.Lerp(_player.CPUCameraPosOffset.y + 0.5f, maxBase, t) / horizon) * screenMiddle;

                        Vector2 worldPos = (Vector2)_player.transform.position + floorDir * rowDistance;

                        Vector2Int mapCoords = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));

                        if (mapCoords.x < 0 || mapCoords.y < 0 || mapCoords.x >= Raycast2DLevelBuilder.Instance.LevelData.LevelSize.x || mapCoords.y >= Raycast2DLevelBuilder.Instance.LevelData.LevelSize.y)
                        {
                            _raycastRenderTexture.SetPixel(x, y, Color.grey);
                            continue;
                        }

                        CellData cellFloor = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[mapCoords.x, mapCoords.y].CellType;

                        if (!cellFloor.GroundTexture)
                        {
                            _raycastRenderTexture.SetPixel(x, y, Color.grey);
                            continue;
                        }

                        Vector2Int currentFloorTextureCoords = new Vector2Int(Mathf.FloorToInt(Mathf.Repeat(worldPos.x, 1) * cellFloor.GroundTexture.width), Mathf.FloorToInt(Mathf.Repeat(worldPos.y, 1) * cellFloor.GroundTexture.height));
                        _raycastRenderTexture.SetPixel(x, y, cellFloor.GroundTexture.GetPixel(currentFloorTextureCoords.x, currentFloorTextureCoords.y));
                    }
                    else if (y >= multipliedHeight + baseHeight)
                    {
                        _raycastRenderTexture.SetPixel(x, y, Color.cyan);
                    }
                    else
                    {
                        int textureYPos = (int)(((y - baseHeight) * cellTexture.height) * invMultipliedHeight);

                        _raycastRenderTexture.SetPixel(x, y, cellTexture.GetPixel(textureXPos, textureYPos));
                    }
                }
            }

            _raycastRenderTexture.Apply();
        }
    }
}

