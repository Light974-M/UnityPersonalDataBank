using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
    [System.Serializable]
    public class CellSavable
    {
        [SerializeField]
        private Vector2Int _position;

        [SerializeField]
        private CellData _cellType;

        public Vector2Int Position => _position;
        public CellData CellType => _cellType;

        public CellSavable(Cell cell)
        {
            _position = cell.Position;
            _cellType = cell.CellType;
        }
    } 
}
