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

            if (!_debug)
                return;
            
            for (int y = 0; y < _levelData.LevelSize.y; y++)
            {
                for (int x = 0; x < _levelData.LevelSize.x; x++)
                {
                    if (_levelData.LevelArray[x, y].CellType.Id != CellID.BlankGround)
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
                    if (_levelData.LevelArray[x, y].CellType.Id == CellID.BlankGround)
                        continue;

                    Color colorToSet = Color.red;

                    if(_levelData.LevelArray[x, y].CellType.Id == CellID.BrickWallTall)
                        colorToSet = new Color(0.5f, 0, 0);

                    Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y), colorToSet);
                    Debug.DrawLine(new Vector2(x, y), new Vector2(x, y + 1), colorToSet);
                    Debug.DrawLine(new Vector2(x + 1, y), new Vector2(x + 1, y + 1), colorToSet);
                    Debug.DrawLine(new Vector2(x, y + 1), new Vector2(x + 1, y + 1), colorToSet);
                }
            }

            if (_setCellType)
            {
                _setCellType = false;

                _levelData.LevelArray[_operationCoords.x, _operationCoords.y].CellType = _cellToSet;

                if (_createCollider)
                {
                    GameObject obj = Instantiate(_cellColliderPrefab, transform);
                    obj.transform.position = new Vector3(_operationCoords.x, _operationCoords.y, 0);
                    obj.GetComponent<CellCollisionRenderer>().LinkedCellPos = _operationCoords; 
                }
            }

            if(_saveLevel)
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

        private void OnValidate()
        {
            _levelData.Load();
        }
    }
}
