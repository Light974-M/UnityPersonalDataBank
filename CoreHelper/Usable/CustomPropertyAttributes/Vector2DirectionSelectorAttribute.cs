using UnityEngine;

namespace UPDB.CoreHelper.CustomPropertyAttributes
{
    public class Vector2DirectionSelectorAttribute : PropertyAttribute
    {
        [SerializeField]
        private bool _clampMagnitude = false;

        [SerializeField]
        private bool _normalize = false;

        [SerializeField]
        private bool _drawDebugLine = false;

        #region Public API

        public bool ClampMagnitude
        {
            get => _clampMagnitude;
            set => _clampMagnitude = value;
        }

        public bool Normalize
        {
            get => _normalize;
            set => _normalize = value;
        }

        public bool DrawDebugLine
        {
            get => _drawDebugLine;
            set => _drawDebugLine = value;
        }

        #endregion

        public Vector2DirectionSelectorAttribute()
        {

        }
    }
}
