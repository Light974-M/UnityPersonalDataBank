using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    ///<summary>
    /// representation of a five-dimensional vector
    ///</summary>
    public struct Properties
    {
        private string name;
        private float value;
        private int index;
        private string state;
        private bool condition;

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
            this.name = name;
            this.value = value;
            this.index = index;
            this.state = state;
            this.condition = condition;
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
            this.name = name;
            this.value = value;
            this.index = index;
            this.state = state;
            this.condition = true;
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
            this.name = name;
            this.value = value;
            this.index = index;
            this.state = "";
            this.condition = condition;
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
            this.name = name;
            this.value = value;
            this.index = 0;
            this.state = "";
            this.condition = condition;
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
            this.name = name;
            this.value = 0;
            this.index = index;
            this.state = state;
            this.condition = condition;
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
            this.name = name;
            this.value = value;
            this.index = index;
            this.state = "";
            this.condition = true;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, float value, string state)
        {
            this.name = name;
            this.value = value;
            this.index = 0;
            this.state = state;
            this.condition = true;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> index parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, int index, string state)
        {
            this.name = name;
            this.value = 0;
            this.index = index;
            this.state = state;
            this.condition = true;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="value"> value parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, float value, bool condition)
        {
            this.name = name;
            this.value = value;
            this.index = 0;
            this.state = "";
            this.condition = condition;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> value parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, int index, bool condition)
        {
            this.name = name;
            this.value = 0;
            this.index = index;
            this.state = "";
            this.condition = condition;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="state"> value parameter</param>
        /// <param name="condition"> state parameter</param>
        public Properties(string name, string state, bool condition)
        {
            this.name = name;
            this.value = 0;
            this.index = 0;
            this.state = state;
            this.condition = condition;
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
            this.name = name;
            this.value = value;
            this.index = 0;
            this.state = "";
            this.condition = true;
        }

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="index"> index parameter</param>
        public Properties(string name, int index)
        {
            this.name = name;
            this.value = 0;
            this.index = index;
            this.state = "";
            this.condition = true;
        }

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="state"> state parameter</param>
        public Properties(string name, string state)
        {
            this.name = name;
            this.value = 0;
            this.index = 0;
            this.state = state;
            this.condition = true;
        }

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        /// <param name="condition"> condition parameter</param>
        public Properties(string name, bool condition)
        {
            this.name = name;
            this.value = 0;
            this.index = 0;
            this.state = "";
            this.condition = condition;
        }

        #endregion

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="name"> name parameter</param>
        public Properties(string name)
        {
            this.name = name;
            this.value = 0;
            this.index = 0;
            this.state = "";
            this.condition = true;
        }

    } 
}
