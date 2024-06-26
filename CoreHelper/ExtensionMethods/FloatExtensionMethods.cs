using UnityEngine;

namespace UPDB.CoreHelper.ExtensionMethods
{
    /// <summary>
    /// method bank that can be used everywhere for float extensions
    /// </summary>
    public static class FloatExtensionMethods
    {
        /// <summary>
        /// make a root other than square roots, let you make cubic root, 4 roots, or wathever X root you want
        /// </summary>
        /// <param name="number"> number to root </param>
        /// <param name="root"> wich root is asked </param>
        /// <returns></returns>
        public static float Root(this float number, float root)
        {
            return Mathf.Pow(number, 1 / root);
        }

        /// <summary>
        /// make a square power
        /// </summary>
        /// <param name="number"> number to square </param>
        /// <returns></returns>
        public static float Sqr(this float number)
        {
            return Mathf.Pow(number, 2);
        }

        /// <summary>
        /// make a cube power
        /// </summary>
        /// <param name="number"> number to cube </param>
        /// <returns></returns>
        public static float Cbe(this float number)
        {
            return Mathf.Pow(number, 3);
        }
    } 
}
