using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Data.NativeTools.SimpleGridLevel
{
    ///<summary>
    /// parameter of level that will be used between scene loads
    ///</summary>
    [CreateAssetMenu(fileName = "NewLevelParameters", menuName = NamespaceID.UPDB + "/" + NamespaceID.Data + "/" + NamespaceID.NativeTools + "/" + NamespaceID.SimpleGridLevel + "/LevelParameters")]
    public class LevelParameters : ScriptableObject
    {
        [SerializeField, Tooltip("width of level in x")]
        private int _width = 3;

        [SerializeField, Tooltip("height of level in y")]
        private int _height = 3;


        #region public API

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        #endregion
    }
}