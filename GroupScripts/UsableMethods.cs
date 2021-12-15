using UnityEngine;
using UPDB.physic.RAPhysic;

namespace UPDB
{
    public class UsableMethods : MonoBehaviour
    {
        /// <summary>
        /// try to find Object, and, if not, let an exception parameter
        /// </summary>
        /// <param name="variable">variable that will assign the TryFindObjectOfType value</param>
        /// <returns></returns>
        public static bool TryFindObjectOfType(out GlobalValuesManager variable)
        {
            if (FindObjectOfType<GlobalValuesManager>() != null)
            {
                //if usable GlobalValuesManager exist, affect it and return true
                variable = FindObjectOfType<GlobalValuesManager>();
                return true;
            }
            else
            {
                //if there is no usable Globalvaluesmanager, returen false
                variable = null;
                return false;
            }
        }
    } 
}
