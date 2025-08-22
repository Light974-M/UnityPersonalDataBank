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

        //overrides
        [SerializeField]
        private int _selectorAreaSize = 3;

        [SerializeField]
        private Vector2 _selectorBounds = new Vector2(-1, 1);

        [SerializeField]
        private bool _displayClampMagAndNormalize = false;

        [SerializeField]
        private bool _displayDebugLine = false;

        [SerializeField]
        private bool _displayVector2Field = false;

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

        public int SelectorAreaSize
        {
            get => _selectorAreaSize;
            set => _selectorAreaSize = value;
        }

        public bool DisplayClampMagAndNormalize
        {
            get => _displayClampMagAndNormalize;
            set => _displayClampMagAndNormalize = value;
        }

        public bool DisplayDebugLine
        {
            get => _displayDebugLine;
            set => _displayDebugLine = value;
        }

        public bool DisplayVector2Field
        {
            get => _displayVector2Field;
            set => _displayVector2Field = value;
        }

        public Vector2 SelectorBounds
        {
            get => _selectorBounds;
            set => _selectorBounds = value;
        }

        #endregion

        public Vector2DirectionSelectorAttribute()
        {
            _displayClampMagAndNormalize = false;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _clampMagnitude = false;
            _normalize = false;
            _drawDebugLine = false;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;
        }

        public Vector2DirectionSelectorAttribute(SelectorClampType clampType)
        {
            _displayClampMagAndNormalize = false;
            _displayDebugLine = true;
            _displayVector2Field = true;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = false;
        }

        public Vector2DirectionSelectorAttribute(SelectorDebugLine debugLine)
        {
            _displayClampMagAndNormalize = true;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;
        }

        public Vector2DirectionSelectorAttribute(int selectorAreaSize)
        {
            _displayClampMagAndNormalize = false;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _clampMagnitude = false;
            _normalize = false;
            _drawDebugLine = false;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(SelectorAreaSizeToken customSize, float selectorBoundsX, float selectorBoundsY)
        {
            Vector2 selectorBounds = new Vector2(selectorBoundsX, selectorBoundsY);
            _displayClampMagAndNormalize = false;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _clampMagnitude = false;
            _normalize = false;
            _drawDebugLine = false;

            _selectorBounds = selectorBounds;
        }

        public Vector2DirectionSelectorAttribute(SelectorClampType clampType, SelectorDebugLine debugLine)
        {
            _displayClampMagAndNormalize = false;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;
        }

        public Vector2DirectionSelectorAttribute(SelectorClampType clampType, int selectorAreaSize)
        {
            _displayClampMagAndNormalize = false;
            _displayDebugLine = true;
            _displayVector2Field = true;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = false;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(int selectorAreaSize, SelectorDebugLine debugLine)
        {
            _displayClampMagAndNormalize = true;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(SelectorClampType clampType, SelectorDebugLine debugLine, int selectorAreaSize)
        {
            _displayClampMagAndNormalize = false;
            _displayDebugLine = false;
            _displayVector2Field = true;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, SelectorClampType clampType)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = false;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, SelectorDebugLine debugLine)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, int selectorAreaSize)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _clampMagnitude = false;
            _normalize = false;
            _drawDebugLine = false;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, SelectorClampType clampType, SelectorDebugLine debugLine)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, SelectorClampType clampType, int selectorAreaSize)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = false;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, int selectorAreaSize, SelectorDebugLine debugLine)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;

            _selectorAreaSize = selectorAreaSize;
        }

        public Vector2DirectionSelectorAttribute(SelectorDisplayParameters displayParameters, SelectorClampType clampType, SelectorDebugLine debugLine, int selectorAreaSize)
        {
            _displayClampMagAndNormalize = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.NormalizeAndClampMag || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine;
            _displayDebugLine = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.DebugLine || displayParameters == SelectorDisplayParameters.NormalizeAndClampMagAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine;
            _displayVector2Field = displayParameters == SelectorDisplayParameters.All || displayParameters == SelectorDisplayParameters.Vector2Field || displayParameters == SelectorDisplayParameters.Vector2FieldAndDebugLine || displayParameters == SelectorDisplayParameters.Vector2FieldAndNormalizeAndClampMag;

            _clampMagnitude = clampType == SelectorClampType.ClampMagnitude;
            _normalize = clampType == SelectorClampType.Normalize;
            _drawDebugLine = debugLine == SelectorDebugLine.GreenDebugLine;

            _selectorAreaSize = selectorAreaSize;
        }
    }

    public enum SelectorClampType
    {
        SelectorRange,
        ClampMagnitude,
        Normalize,
    }

    public enum SelectorDisplayParameters
    {
        None,
        All,
        Vector2Field,
        NormalizeAndClampMag,
        DebugLine,
        NormalizeAndClampMagAndDebugLine,
        Vector2FieldAndDebugLine,
        Vector2FieldAndNormalizeAndClampMag,
    }

    public enum SelectorDebugLine
    {
        None,
        GreenDebugLine,
    }

    public enum SelectorAreaSizeToken
    {
        CustomSelectorArea,
    }
}
