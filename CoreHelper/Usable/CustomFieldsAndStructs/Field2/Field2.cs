using UnityEngine;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    [System.Serializable]
    public struct Field2<T>
    {
        [SerializeField]
        private T _x;

        [SerializeField]
        private T _y;

        #region Public API

        public T X
        {
            get => _x;
            set => _x = value;
        }
        public T Y
        {
            get => _y;
            set => _y = value;
        }

        #endregion
    } 
}
