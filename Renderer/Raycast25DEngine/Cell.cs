using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
    [System.Serializable]
    public class Cell
    {
        private Vector2Int _position;

        private CellData _cellType;

        public CellData CellType
        {
            get
            {
                return _cellType;
            }

            set
            {
                _cellType = value;
            }
        }

        public Cell(Vector2Int position, CellID id)
        {
            _position = position;
            _cellType = Raycast2DLevelBuilder.Instance.DataBase.CellsDictionary[id];
        }
    }

}