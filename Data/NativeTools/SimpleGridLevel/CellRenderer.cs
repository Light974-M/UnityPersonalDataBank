using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Data.NativeTools.SimpleGridLevel
{
    ///<summary>
    /// renderer for every game cells, making textures, and collision detections
    ///</summary>
    [AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.Data + "/" + NamespaceID.NativeTools + "/" + NamespaceID.SimpleGridLevel + "/CellRenderer")]
    public class CellRenderer : UPDBBehaviour
    {
        [Header("INPUT TEXTURES\n")]

        [SerializeField]
        private SpriteRenderer _cellTile;

        private Cell _linkedCell = null;


        #region Public API
        
        public Cell LinkedCell
        {
            get { return _linkedCell; }
            set { _linkedCell = value; }
        }

        #endregion

        public void GraphicUpdate()
        {
            //insert action when tile need to be graphical updated
        }
    }
}
