using UnityEngine;

namespace UPDB.CoreHelper.ExtensionMethods
{
    /// <summary>
    /// method bank that can be used everywhere for float extensions
    /// </summary>
    public static class Vector3ExtensionMethods
    {
        public static Vector3 Forward(this Vector3 eulerAngles)
        {
            return Quaternion.Euler(eulerAngles) * Vector3.forward;
        }

        public static Vector3 Forward(this Quaternion rotation)
        {
            return rotation * Vector3.forward;
        }

        public static Vector3 Right(this Vector3 eulerAngles)
        {
            return Quaternion.Euler(eulerAngles) * Vector3.right;
        }

        public static Vector3 Right(this Quaternion rotation)
        {
            return rotation * Vector3.right;
        }

        public static Vector3 Up(this Vector3 eulerAngles)
        {
            return Quaternion.Euler(eulerAngles) * Vector3.up;
        }

        public static Vector3 Up(this Quaternion rotation)
        {
            return rotation * Vector3.up;
        }

    } 
}
