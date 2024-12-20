using UnityEngine;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    [System.Serializable]
    public struct Field3<T>
    {
        [SerializeField]
        private T _x;

        [SerializeField]
        private T _y;

        [SerializeField]
        private T _z;

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
        public T Z
        {
            get => _z;
            set => _z = value;
        }

        #endregion
    }
}
