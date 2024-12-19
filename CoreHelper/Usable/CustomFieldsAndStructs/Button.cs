using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
	[Serializable]
	public struct Button
	{
        [SerializeField]
        private bool _value;

        #region Public API

        public bool Value
        {
            get
            {
                if( _value )
                {
                    _value = false;
                    return true;
                }
                return _value; 
            }
            set { _value = value; }
        }

        #endregion

        public Button(bool value)
        {
            _value = value;
        }
    } 
}
