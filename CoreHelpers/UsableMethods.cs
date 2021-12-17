using UnityEngine;
using UPDB.physic.RAPhysic;

namespace UPDB
{
    /// <summary>
    /// method bank that can be used everywhere
    /// </summary>
    public static class UsableMethods
    {
        /// <summary>
        /// try to find Object, and, if not, let an exception parameter
        /// </summary>
        /// <param name="variable">variable that will assign the TryFindObjectOfType value</param>
        /// <returns></returns>
        public static bool TryFindObjectOfType<T>(this Object owner, out T variable) where T : Object
        {
            variable = Object.FindObjectOfType<T>();
            return variable != null;
        }
    } 
}
