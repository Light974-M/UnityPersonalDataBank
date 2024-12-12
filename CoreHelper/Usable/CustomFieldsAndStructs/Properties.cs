using UnityEngine;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    ///<summary>
    /// give properties to an object, like name, or state
    ///</summary>
    [System.Serializable]
    public struct Properties
    {
        [SerializeField, Tooltip("")]
        private string _name;

        [SerializeField, Tooltip("")]
        private float _value;

        [SerializeField, Tooltip("")]
        private int _index;

        [SerializeField, Tooltip("")]
        private string _state;

        [SerializeField, Tooltip("")]
        private bool _condition;

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="index"> index parameter</param>
        /// <param name="state"> state parameter</param>
        /// <param name="condition"> condition parameter</param>
        public Properties(string name, float value, int index, string state, bool condition)
        {
            this._name = name;
            this._value = value;
            this._index = index;
            this._state = state;
            this._condition = condition;
        }


        #region 4 parameter constructors

        /// <summary>
        /// constructor for 4 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="index"> index parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, float value, int index, string state)
        {
            this._name = name;
            this._value = value;
            this._index = index;
            this._state = state;
            this._condition = true;
        }

        /// <summary>
        /// constructor for 4 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="index"> index parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, float value, int index, bool condition)
        {
            this._name = name;
            this._value = value;
            this._index = index;
            this._state = "";
            this._condition = condition;
        }

        /// <summary>
        /// constructor for 4 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="state"> index parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, float value, string state, bool condition)
        {
            this._name = name;
            this._value = value;
            this._index = 0;
            this._state = "";
            this._condition = condition;
        }

        /// <summary>
        /// constructor for 4 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> index parameter</param>
        /// <param name="state"> state parameter</param>
        /// <param name="condition"> condition parameter</param>
        public Properties(string name, int index, string state, bool condition)
        {
            this._name = name;
            this._value = 0;
            this._index = index;
            this._state = state;
            this._condition = condition;
        }

        #endregion


        #region 3 paramater constructors

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="index"> index parameter</param>
        public Properties(string name, float value, int index)
        {
            this._name = name;
            this._value = value;
            this._index = index;
            this._state = "";
            this._condition = true;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, float value, string state)
        {
            this._name = name;
            this._value = value;
            this._index = 0;
            this._state = state;
            this._condition = true;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> index parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, int index, string state)
        {
            this._name = name;
            this._value = 0;
            this._index = index;
            this._state = state;
            this._condition = true;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, float value, bool condition)
        {
            this._name = name;
            this._value = value;
            this._index = 0;
            this._state = "";
            this._condition = condition;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> value parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, int index, bool condition)
        {
            this._name = name;
            this._value = 0;
            this._index = index;
            this._state = "";
            this._condition = condition;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="state"> value parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, string state, bool condition)
        {
            this._name = name;
            this._value = 0;
            this._index = 0;
            this._state = state;
            this._condition = condition;
        }

        #endregion

        #region 2 parameter constructors

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        public Properties(string name, float value)
        {
            this._name = name;
            this._value = value;
            this._index = 0;
            this._state = "";
            this._condition = true;
        }

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> index parameter</param>
        public Properties(string name, int index)
        {
            this._name = name;
            this._value = 0;
            this._index = index;
            this._state = "";
            this._condition = true;
        }

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, string state)
        {
            this._name = name;
            this._value = 0;
            this._index = 0;
            this._state = state;
            this._condition = true;
        }

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="condition"> condition parameter</param>
        public Properties(string name, bool condition)
        {
            this._name = name;
            this._value = 0;
            this._index = 0;
            this._state = "";
            this._condition = condition;
        }

        #endregion

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        public Properties(string name)
        {
            this._name = name;
            this._value = 0;
            this._index = 0;
            this._state = "";
            this._condition = true;
        }

    } 
}
