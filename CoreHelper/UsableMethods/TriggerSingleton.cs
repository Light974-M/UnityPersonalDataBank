using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.UsableMethods
{
    public class TriggerSingleton : Singleton<TriggerSingleton>
    {
        private bool _value = false;

        public bool Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool Read
        {
            get
            {
                bool value = _value;
                _value = false;
                return value;
            }
        }

        public void Activate()
        {
            _value = true;
        }
    } 
}
