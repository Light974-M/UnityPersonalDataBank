using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
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
        /// return if two lists have exactly the sames values(same number of objects and same objects at same positions)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        private bool AreSameList<T>(List<T> list1, List<T> list2) where T : UnityEngine.Object
        {
            bool isSame = true;

            if (list1.Count == list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                    if (list1[i] != list2[i])
                        isSame = false;
            }
            else
            {
                isSame = false;
            }

            return isSame;
        }

        /// <summary>
        /// calculate rotation with a direction vector and a up vector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Quaternion MyLookRotation(Vector3 dir, Vector3 up)
        {
            if (dir == Vector3.zero)
            {
                Debug.Log("Zero direction in MyLookRotation");
                return Quaternion.identity;
            }

            if (up != dir)
            {
                up.Normalize();
                var v = dir + up * -Vector3.Dot(up, dir);
                var q = Quaternion.FromToRotation(Vector3.forward, v);
                return Quaternion.FromToRotation(v, dir) * q;
            }
            else
            {
                return Quaternion.FromToRotation(Vector3.forward, dir);
            }
        }

        /// <summary>
        /// make a lookAt rotation with up instead(turn a pleyr to ave feet pointing direction)
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="dir"></param>
        public static void LookRotationUp(Transform transform, Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.right, dir);
            transform.Rotate(0, -90, 0);
            transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        }

        /// <summary>
        /// calculate an angularVelocity from transform rotation of a moving object
        /// </summary>
        /// <param name="rotation">actual rotation</param>
        /// <param name="previousRotation">last rotation(only need a variable, value is automatically updated</param>
        /// <param name="timeDelta">is deltaTime, fixedDeltaTime, or other ?</param>
        /// <returns></returns>
        public static Vector3 GetAngularVelocity(Quaternion rotation, ref Quaternion previousRotation, float timeDelta)
        {
            Quaternion deltaRotation = rotation * Quaternion.Inverse(previousRotation);

            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

            angle *= Mathf.Deg2Rad;

            Vector3 angularVelocity = (1.0f / timeDelta) * angle * axis;

            previousRotation = rotation;

            return angularVelocity;
        }

        /// <summary>
        /// destroy instance in play or in editor intelligently
        /// </summary>
        /// <param name="obj"></param>
        public static void IntelliDestroy(Object obj)
        {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        #region AutoLerp

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        public static float AutoLerp(float a, float b, float lerpTime, ref float timer)
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
        public static Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer)
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
        public static Vector3 AutoLerp(Vector3 a, Vector3 b, float lerpTime, ref float timer)
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
        public static Vector4 AutoLerp(Vector4 a, Vector4 b, float lerpTime, ref float timer)
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
        public static Quaternion AutoLerp(Quaternion a, Quaternion b, float lerpTime, ref float timer)
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

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static float AutoLerp(float a, float b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            float value = 0;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Mathf.Lerp(a, b, timerCurveValueNormalized);
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
        /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time, with an addition of choosing the shape of a non linear lerp
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            //create a null vector 2
            Vector2 value = Vector2.zero;

            //if timer has not reach lerp time, update lerp state, if it has, put timer to lerp time, and value to end state
            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                //update value to a lerp between a and b, with timer / lerp timer for T (value between 0 and 1) applyied to the animation curve value
                value = Vector2.Lerp(a, b, timerCurveValueNormalized);

                //update timer value
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            //return value updated
            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector3, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector3 AutoLerp(Vector3 a, Vector3 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            Vector3 value = Vector3.zero;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Vector3.Lerp(a, b, timerCurveValueNormalized);
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
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector4 AutoLerp(Vector4 a, Vector4 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            Vector4 value = Vector4.zero;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Vector4.Lerp(a, b, timerCurveValueNormalized);

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
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Quaternion AutoLerp(Quaternion a, Quaternion b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            Quaternion value = Quaternion.identity;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Quaternion.Lerp(a, b, timerCurveValueNormalized);

                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        #endregion

        #region CurveLerp

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static float CurveLerp(float a, float b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Mathf.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector2 CurveLerp(Vector2 a, Vector2 b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Vector2.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector3 CurveLerp(Vector3 a, Vector3 b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Vector3.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector4 CurveLerp(Vector4 a, Vector4 b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Vector4.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Quaternion CurveLerp(Quaternion a, Quaternion b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Quaternion.Lerp(a, b, timerCurveValueNormalized);
        }

        #endregion
    }
}
