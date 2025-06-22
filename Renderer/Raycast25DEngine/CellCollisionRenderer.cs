using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class CellCollisionRenderer : UPDBBehaviour
    {
        private Vector2Int _linkedCellPos = Vector2Int.zero;

        public Vector2Int LinkedCellPos
        {
            get => _linkedCellPos; 
            set => _linkedCellPos = value;
        }
    } 
}
