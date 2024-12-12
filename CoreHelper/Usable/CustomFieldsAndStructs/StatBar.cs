using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    [System.Serializable]
    public struct StatBar
    {
        [SerializeField, Tooltip("value of stat")]
        private float _value;

        [SerializeField, Tooltip("min value of stat")]
        private float _min;

        [SerializeField, Tooltip("max value of stat")]
        private float _max;

        #region Public API

        public float Value
        {
            get
            {
                _value = Mathf.Clamp(_value, _min, _max);

                return _value;
            }

            set
            {
                _value = Mathf.Clamp(value, _min, _max);
            }
        }

        public float Min
        {
            get
            {
                return _min;
            }

            set
            {
                _min = Mathf.Clamp(value, -Mathf.Infinity, _max);
                _value = Mathf.Clamp(_value, _min, _max);
            }
        }

        public float Max
        {
            get
            {
                return _max;
            }

            set
            {
                _max = Mathf.Clamp(value, _min, Mathf.Infinity);
                _value = Mathf.Clamp(_value, _min, _max);
            }
        }

        #endregion
    } 
}
