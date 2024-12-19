using UnityEngine;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    ///<summary>
    /// representation of a five-dimensional vector
    ///</summary>
    public struct Vector5
    {
        private float x;
        private float y;
        private float z;
        private float w;
        private float v;

        /// <summary>
        /// constructor for 5 parameters
        /// </summary>
        /// <param name="x"> x parameter</param>
        /// <param name="y"> y parameter</param>
        /// <param name="z"> z parameter</param>
        /// <param name="w"> w parameter</param>
        /// <param name="v"> v parameter</param>
        public Vector5(float x, float y, float z, float w, float v)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.v = v;
        }

        /// <summary>
        /// constructor for 4 parameters
        /// </summary>
        /// <param name="x"> x parameter</param>
        /// <param name="y"> y parameter</param>
        /// <param name="z"> z parameter</param>
        /// <param name="w"> w parameter</param>
        /// <param name="v"> v parameter</param>
        public Vector5(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            v = 0;
        }

        /// <summary>
        /// constructor for 3 parameters
        /// </summary>
        /// <param name="x"> x parameter</param>
        /// <param name="y"> y parameter</param>
        /// <param name="z"> z parameter</param>
        /// <param name="w"> w parameter</param>
        /// <param name="v"> v parameter</param>
        public Vector5(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0;
            v = 0;
        }

        /// <summary>
        /// constructor for 2 parameters
        /// </summary>
        /// <param name="x"> x parameter</param>
        /// <param name="y"> y parameter</param>
        /// <param name="z"> z parameter</param>
        /// <param name="w"> w parameter</param>
        /// <param name="v"> v parameter</param>
        public Vector5(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0;
            w = 0;
            v = 0;
        }

        /// <summary>
        /// shortcut for Vector5(0, 0, 0, 0, 0)
        /// </summary>
        public static Vector5 zero
        {
            get { return new Vector5(0, 0, 0, 0, 0); }
        }

        /// <summary>
        /// shortcut for Vector5(1, 1, 1, 1, 1)
        /// </summary>
        public static Vector5 one
        {
            get { return new Vector5(1, 1, 1, 1, 1); }
        }
    }

}