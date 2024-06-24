using UnityEngine;

namespace UPDB.CoreHelper.ExtensionMethods
{
    /// <summary>
    /// method bank that can be used everywhere for Transform extensions
    /// </summary>
    public static class TransformExtensionMethods
    {
        /// <summary>
        /// return a vector3 direction normalized pointing on target
        /// </summary>
        /// <param name="origin"> origin parameter, where vector start </param>
        /// <param name="target"> target, where vector3 is directed to </param>
        /// <returns></returns>
        public static Vector3 Direction(this Transform origin, Transform target)
        {
            return (target.position - origin.position).normalized;
        }

        /// <summary>
        /// Add position locally to self object(without parenting)
        /// </summary>
        /// <param name="transform"> transform been modified </param>
        /// <param name="x"> value to add on x axis </param>
        /// <param name="y"> value to add on y axis </param>
        /// <param name="z"> value to add on z axis </param>
        public static void AddSelfLocalPosition(this Transform transform, float x, float y, float z)
        {
            transform.position += (transform.right * x) + (transform.forward * y) + (transform.up * z);
        }

        /// <summary>
        /// Add position locally to self object(without parenting)
        /// </summary>
        /// <param name="transform"> transform been modified </param>
        /// <param name="pos"> value to add on xyz axis </param>
        public static void AddSelfLocalPosition(this Transform transform, Vector3 pos)
        {
            transform.position += (transform.right * pos.x) + (transform.forward * pos.y) + (transform.up * pos.z);
        }

        /// <summary>
        /// set position locally to self object(without parenting)
        /// </summary>
        /// <param name="transform"> transform been modified </param>
        /// <param name="x"> value to set on x axis </param>
        /// <param name="y"> value to set on y axis </param>
        /// <param name="z"> value to set on z axis </param>
        public static void SetSelfLocalPosition(this Transform transform, float x, float y, float z)
        {
            transform.position = (transform.right * x) + (transform.forward * y) + (transform.up * z);
        }

        /// <summary>
        /// Set position locally to self object(without parenting)
        /// </summary>
        /// <param name="transform"> transform been modified </param>
        /// <param name="pos"> value to set on xyz axis </param>
        public static void SetSelfLocalPosition(this Transform transform, Vector3 pos)
        {
            transform.position = (transform.right * pos.x) + (transform.forward * pos.y) + (transform.up * pos.z);
        }
    } 
}
