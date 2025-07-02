using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class Cell
    {
        private Vector2Int _position;

        private CellData _cellType;

        public Vector2Int Position => _position;

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
            if (!Raycast2DLevelBuilder.Instance.DataBase)
                return;

            _position = position;
            Raycast2DLevelBuilder.Instance.DataBase.CellsDictionary.TryGetValue(id, out _cellType);
        }

        public Cell(CellSavable cellSavable)
        {
            _position = cellSavable.Position;
            _cellType = cellSavable.CellType;
        }
    }

}