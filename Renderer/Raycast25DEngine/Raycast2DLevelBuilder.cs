using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UPDB.CoreHelper.Usable;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class Raycast2DLevelBuilder : Singleton<Raycast2DLevelBuilder>
    {
        [SerializeField]
        private LevelData _levelData;

        [SerializeField]
        private CellDataBase _cellDataBase;

        [SerializeField]
        private GameObject _cellColliderPrefab;

        [SerializeField]
        private bool _debug = true;

        [SerializeField]
        private bool _saveLevel = false;

        [SerializeField]
        private bool _loadLevel = false;

        [SerializeField]
        private Vector2Int _operationCoords;

        [SerializeField]
        private bool _setCellType = false;

        [SerializeField]
        private CellData _cellToSet;

        [SerializeField]
        private bool _createCollider = false;

        private bool _callMouseDownSwitch = false;

        #region Public API

        public LevelData LevelData
        {
            get => _levelData;
        }

        public CellDataBase DataBase
        {
            get => _cellDataBase;
        }

        #endregion

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DrawDebugGrid();

            LevelEditorFeatures();

            SaveAndLoadMethod();
        }

        private void DrawDebugGrid()
        {
            if (!_debug || !_levelData)
                return;

            for (int y = 0; y < _levelData.LevelSize.y; y++)
            {
                for (int x = 0; x < _levelData.LevelSize.x; x++)
                {
                    if (!ReferenceEquals(_levelData.LevelArray[x, y], null) && _levelData.LevelArray[x, y].CellType && _levelData.LevelArray[x, y].CellType.Id != CellID.BlankGround)
                        continue;

                    Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y), Color.white);
                    Debug.DrawLine(new Vector2(x, y), new Vector2(x, y + 1), Color.white);
                    Debug.DrawLine(new Vector2(x + 1, y), new Vector2(x + 1, y + 1), Color.white);
                    Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y + 1), Color.white);
                }
            }

            for (int y = 0; y < _levelData.LevelSize.y; y++)
            {
                for (int x = 0; x < _levelData.LevelSize.x; x++)
                {
                    if (ReferenceEquals(_levelData.LevelArray[x, y], null) || !_levelData.LevelArray[x, y].CellType || _levelData.LevelArray[x, y].CellType.Id == CellID.BlankGround)
                        continue;

                    Color colorToSet = Color.red;

                    if (_levelData.LevelArray[x, y].CellType.Id == CellID.BrickWallTall)
                        colorToSet = new Color(0.5f, 0, 0);

                    Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y), colorToSet);
                    Debug.DrawLine(new Vector2(x, y), new Vector2(x, y + 1), colorToSet);
                    Debug.DrawLine(new Vector2(x + 1, y), new Vector2(x + 1, y + 1), colorToSet);
                    Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y + 1), colorToSet);
                }
            }

            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            int xMousePos = Mathf.FloorToInt(mousePos.x);
            int yMousePos = Mathf.FloorToInt(mousePos.y);

            for (int y = 0; y < _levelData.LevelSize.y; y++)
            {
                for (int x = 0; x < _levelData.LevelSize.x; x++)
                {
                    bool isSelected = x == xMousePos && y == yMousePos;

                    if (isSelected)
                    {
                        Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y), Color.blue);
                        Debug.DrawLine(new Vector2(x, y), new Vector2(x, y + 1), Color.blue);
                        Debug.DrawLine(new Vector2(x + 1, y), new Vector2(x + 1, y + 1), Color.blue);
                        Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y + 1), Color.blue);
                    }
                }
            }

            if (!Application.isPlaying)
                SceneView.lastActiveSceneView.Repaint();
        }

        private void LevelEditorFeatures()
        {
            if (!_debug && Application.isPlaying)
                return;

            if (_setCellType)
            {
                _setCellType = false;

                _levelData.LevelArray[_operationCoords.x, _operationCoords.y].CellType = _cellToSet;

                if (_createCollider)
                {
                    GameObject obj = Instantiate(_cellColliderPrefab, transform);
                    obj.transform.position = new Vector3(_operationCoords.x, _operationCoords.y, 0);
                }
            }

            if ((Event.current.type == EventType.ExecuteCommand && Event.current.button == 0) || _callMouseDownSwitch)
            {
                _callMouseDownSwitch = false;

                Vector2 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                int x = (int)mousePos.x;
                int y = (int)mousePos.y;

                if (x < 0 || y < 0 || x >= _levelData.LevelSize.x || y >= _levelData.LevelSize.y)
                {
                    _callMouseDownSwitch = true;
                    return;
                }

                Cell selectedCell = _levelData.LevelArray[x, y];

                selectedCell.CellType = _cellToSet;

                bool isAlreadyCell = false;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Vector2 pos = transform.GetChild(i).transform.position;

                    if ((int)pos.x == x && (int)pos.y == y)
                    {
                        isAlreadyCell = true;

                        if (!selectedCell.CellType.HasWall)
                            IntelliDestroy(transform.GetChild(i).gameObject);

                        break;
                    }
                }

                if (!isAlreadyCell && _cellColliderPrefab && selectedCell.CellType.HasWall)
                {
                    GameObject obj = Instantiate(_cellColliderPrefab, transform);
                    obj.transform.position = new Vector3(x, y, 0);
                }

                LevelData.Save();
            }
        }

        private void SaveAndLoadMethod()
        {
            if (Application.isPlaying)
                return;

            if (_saveLevel)
            {
                _saveLevel = false;

                _levelData.Save();
            }

            if (_loadLevel)
            {
                _loadLevel = false;

                _levelData.Load();
            }
        }
    }
}
