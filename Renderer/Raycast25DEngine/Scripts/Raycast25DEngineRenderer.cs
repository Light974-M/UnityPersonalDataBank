using System.Collections.Generic;
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

        private Color[,][] _floorTexturesColorsArray;
        private Color[,][] _wallTexturesColorsArray;

        private void Awake()
        {
            _raycastRenderTexture = new Texture2D(_player.CPURayNumbers, _player.CPUVerticalPixelNumbers);
            _raycastRenderTexture.filterMode = FilterMode.Point;

            for (int y = 0; y < _raycastRenderTexture.height; y++)
                for (int x = 0; x < _raycastRenderTexture.width; x++)
                    _raycastRenderTexture.SetPixel(x, y, Color.white);

            _raycastRenderTexture.Apply();

            _imageComponent.texture = _raycastRenderTexture;

            GenerateFloorTexturesArray(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
            GenerateWallTexturesArray(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
        }
        private void FixedUpdate()
        {
            DrawTexture();
        }

        public void GenerateFloorTexturesArray(Cell[,] levelArray, Vector2Int size)
        {
            _floorTexturesColorsArray = new Color[size.x, size.y][];

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Texture2D floorText = levelArray[x, y].CellType.GroundTexture;
                    _floorTexturesColorsArray[x, y] = floorText.GetPixels();
                }
            }
        }

        public void GenerateWallTexturesArray(Cell[,] levelArray, Vector2Int size)
        {
            _wallTexturesColorsArray = new Color[size.x, size.y][];

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Texture2D wallText = levelArray[x, y].CellType.WallTexture;

                    if (!wallText)
                        continue;

                    _wallTexturesColorsArray[x, y] = wallText.GetPixels();
                }
            }
        }

        private void DrawTexture()
        {
            if (_player.RaysList == null || _player.RaysList.Count == 0)
                return;

            Color[] mainTextureToApplyColor = new Color[_raycastRenderTexture.width * _raycastRenderTexture.height];
            float screenMiddle = _raycastRenderTexture.height / 2f;
            int horizonOffset = (int)_player.VerticalLookingValue;
            float playerYPos = _player.CPUCameraPosOffset.y - 0.5f;
            float horizon = screenMiddle - horizonOffset;

            for (int x = 0; x < _raycastRenderTexture.width; x++)
                GetRayInfo(x, horizonOffset, playerYPos, horizon, ref mainTextureToApplyColor, screenMiddle);

            _raycastRenderTexture.SetPixels(mainTextureToApplyColor);
            _raycastRenderTexture.Apply();
        }

        private void GetRayInfo(int x, int horizonOffset, float playerYPos, float horizon, ref Color[] mainTextureToApplyColor, float screenMiddle)
        {
            int height = _player.RaysList[x] ? (int)(_raycastRenderTexture.height / _player.RaysList[x].distance) : 0;
            height = (int)(height * _player.CPUVerticalPixelNumbers) / _player.CPUVerticalPixelNumbers;
            int baseHeight = (((_raycastRenderTexture.height - height) / 2) - horizonOffset) + (int)(height * (-playerYPos));

            Vector2 collidedPos = Vector2.zero;
            Vector2Int collidedWall = Vector2Int.zero;
            CellData collidedCellData = null;
            Texture2D cellTexture = null;
            Color[] cellTextureColors = null;
            Vector2 textureCoords = Vector2Int.zero;
            int textureXPos = 0;
            int multipliedHeight = 0;
            Vector2 floorDir = _player.RaysDirList[x];

            if (_player.RaysList[x])
            {
                collidedPos = _player.RaysList[x].transform.position;
                collidedWall = new Vector2Int((int)(collidedPos.x), (int)(collidedPos.y));

                collidedCellData = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[collidedWall.x, collidedWall.y].CellType;
                cellTexture = collidedCellData.WallTexture;
                cellTextureColors = _wallTexturesColorsArray[collidedWall.x, collidedWall.y];

                textureCoords = (_player.RaysList[x].point - collidedPos);
                textureXPos = (int)(((textureCoords.x + textureCoords.y) % 1) * cellTexture.width);

                multipliedHeight = (int)(height * collidedCellData.WallHeight);
            }

            float invMultipliedHeight = 1f / (float)multipliedHeight;

            int floorBaseHeight = (((_raycastRenderTexture.height - height) / 2) - horizonOffset);
            float minDy = (horizon - floorBaseHeight) / horizon;
            float maxBase = 1f / minDy;
            float wallTextureIndexMultiplier = 0;
            float invHorizon = 1f / horizon;

            if (_player.RaysList[x])
                wallTextureIndexMultiplier = cellTexture.height * invMultipliedHeight;

            float floorRawDistanceHorizonMultiplier = invHorizon * screenMiddle;

            for (int y = 0; y < _raycastRenderTexture.height; y++)
                DrawYPixel(x, y, baseHeight, ref mainTextureToApplyColor, horizon, invHorizon, floorRawDistanceHorizonMultiplier, floorDir, multipliedHeight, cellTexture, textureXPos, cellTextureColors, wallTextureIndexMultiplier);
        }

        private void DrawYPixel(int x, int y, int baseHeight, ref Color[] mainTextureToApplyColor, float horizon, float invHorizon, float floorRawDistanceHorizonMultiplier, Vector2 floorDir, int multipliedHeight, Texture2D cellTexture, int textureXPos, Color[] cellTextureColors, float wallTextureIndexMultiplier)
        {
            int i = y * _raycastRenderTexture.width + x;

            if (y < baseHeight)
            {
                if (_player.CPUCameraPosOffset.y <= 0)
                {
                    mainTextureToApplyColor[i] = Color.grey;
                    return;
                }

                float dy = (horizon - y) * invHorizon;

                float rawDistance = ((_player.CPUCameraPosOffset.y + 0.5f) / dy);
                rawDistance = rawDistance * floorRawDistanceHorizonMultiplier;

                Vector2 worldPos = (Vector2)_player.transform.position + floorDir * rawDistance;

                int mapCoordsX = (int)(worldPos.x);
                int mapCoordsY = (int)(worldPos.y);

                if (worldPos.x < 0 || worldPos.y < 0 || worldPos.x >= Raycast2DLevelBuilder.Instance.LevelData.LevelSize.x || worldPos.y >= Raycast2DLevelBuilder.Instance.LevelData.LevelSize.y)
                {
                    mainTextureToApplyColor[i] = Color.grey;
                    return;
                }

                CellData cellFloor = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[mapCoordsX, mapCoordsY].CellType;

                Color[] cellFloorTextureColors = _floorTexturesColorsArray[mapCoordsX, mapCoordsY];

                if (!cellFloor.GroundTexture)
                {
                    mainTextureToApplyColor[i] = Color.grey;
                    return;
                }

                int currentFloorTextureCoordsX = (int)((worldPos.x - mapCoordsX) * cellFloor.GroundTexture.width);
                int currentFloorTextureCoordsY = (int)((worldPos.y - mapCoordsY) * cellFloor.GroundTexture.height);

                int currentFloorTextureIndex = currentFloorTextureCoordsY * cellFloor.GroundTexture.width + currentFloorTextureCoordsX;

                mainTextureToApplyColor[i] = cellFloorTextureColors[currentFloorTextureIndex];
            }
            else if (y >= multipliedHeight + baseHeight)
            {
                mainTextureToApplyColor[i] = Color.cyan;
            }
            else
            {
                int cellTextureIndex = (int)((y - baseHeight) * wallTextureIndexMultiplier) * cellTexture.width + textureXPos;
                mainTextureToApplyColor[i] = cellTextureColors[cellTextureIndex];
            }
        }
    }
}

