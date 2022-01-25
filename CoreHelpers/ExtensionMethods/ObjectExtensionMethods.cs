using UnityEngine;

namespace UPDB
{
    /// <summary>
    /// method bank that can be used everywhere for Object extensions
    /// </summary>
    public static class ObjectExtensionMethods
    {
        /// <summary>
        /// WIP : avoid null values for variables by searching object type in all scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="var"></param>
        public static void ReplaceDefaultFindObjectOfType<T>(this Object owner, out T var) where T : Object
        {
            var = null;
        }
    } 
}
