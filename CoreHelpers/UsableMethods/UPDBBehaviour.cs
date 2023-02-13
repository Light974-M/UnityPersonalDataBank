using UnityEngine;

namespace UPDB
{
    ///<summary>
    /// UPDB methods that does not use extensions, callable in every classes that derives from monoBehaviour
    ///</summary>
    public class UPDBBehaviour : MonoBehaviour
    {
        /// <summary>
        /// try to find Object, and, if not, let an exception parameter
        /// </summary>
        /// <param name="variable">variable that will assign the TryFindObjectOfType value</param>
        /// <returns></returns>
        public static bool TryFindObjectOfType<T>(out T variable) where T : Object
        {
            variable = FindObjectOfType<T>();
            return variable != null;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private static float AutoLerp(float a, float b, float lerpTime, ref float timer)
        {
            float value = 0;

            if (timer < lerpTime)
            {
                value = Mathf.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private static Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer)
        {
            Vector2 value = Vector2.zero;

            if (timer < lerpTime)
            {
                value = Vector2.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector3, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private static Vector3 AutoLerp(Vector3 a, Vector3 b, float lerpTime, ref float timer)
        {
            Vector3 value = Vector3.zero;

            if (timer < lerpTime)
            {
                value = Vector3.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector4, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private static Vector4 AutoLerp(Vector4 a, Vector4 b, float lerpTime, ref float timer)
        {
            Vector4 value = Vector4.zero;

            if (timer < lerpTime)
            {
                value = Vector4.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a rotation lerp automatically(based on delta time update rate) with Quaternions, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private static Quaternion AutoLerp(Quaternion a, Quaternion b, float lerpTime, ref float timer)
        {
            Quaternion value = Quaternion.identity;

            if (timer < lerpTime)
            {
                value = Quaternion.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

    } 
}
