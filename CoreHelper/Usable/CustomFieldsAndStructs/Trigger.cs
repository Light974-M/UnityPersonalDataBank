using UnityEngine;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    /// <summary>
    /// act like a boolean when set, but automatically reseted when getted
    /// </summary>
    [System.Serializable]
    public class Trigger
    {
        [SerializeField]
        private bool _value;

        public bool Read
        {
            get
            {
                if (_value)
                {
                    _value = false;
                    Debug.Log(_value);
                    return true;
                }

                return false;
            }
        }

        public void SetTrigger()
        {
            _value = true;
        }

        public void ResetTrigger()
        {
            _value = false;
        }

        public Trigger(bool value)
        {
            _value = value;
        }
    }
}
