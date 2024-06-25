using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    ///<summary>
    /// UPDB methods that does not use extensions, callable in every classes that derives from monoBehaviour
    ///</summary>
    public class UPDBBehaviour : MonoBehaviour
    {
        /******************************************************NATIVE METHODS***********************************************************/

        /// <summary>
        /// call when scene is updating before rendering gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                OnScene();
        }

        /// <summary>
        /// call when scene is updating before rendering gizmos if obhect is selected by inspector
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                OnSceneSelected();
        }


        /******************************************************CUSTOM METHODS***********************************************************/

        /// <summary>
        /// called on scene update in editor only
        /// </summary>
        protected virtual void OnScene()
        {

        }

        /// <summary>
        /// called on scene update in editor only, when object is selected by inspector
        /// </summary>
        protected virtual void OnSceneSelected()
        {

        }

        /******************************************************UTILITY METHODS**********************************************************/

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
        public static bool AreSameList<T>(List<T> list1, List<T> list2) where T : UnityEngine.Object
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

        /// <summary>
        /// make the value given "clamp" but instead of clamping to borders, it is looping, so it's clamping with "inverse borns" usefull when using arrays and want to loop without going outside the bounds
        /// </summary>
        /// <param name="value">value to clamp</param>
        /// <param name="min">min born(0 for a list)</param>
        /// <param name="max">max born(length - 1 for a list)</param>
        /// <returns></returns>
        public static int LoopClamp(int value, int min, int max)
        {
            bool isLastValueOutbound = value > max;
            bool isFirstValueOutboud = value < min;

            if (isLastValueOutbound)
            {
                return min;
            }

            if (isFirstValueOutboud)
            {
                return max;
            }

            if (!isFirstValueOutboud && !isLastValueOutbound)
            {
                return value;
            }

            return value;
        }

        /// <summary>
        /// make a (not really optimized) find object of type, but for generic classes, it will search if any object in scene are derived from the class<T> given
        /// </summary>
        /// <typeparam name="T">parent class, if none, put System.Object</typeparam>
        /// <param name="type">generic class to search for</param>
        /// <returns></returns>
        public static T[] FindObjectsOfTypeGeneric<T>(System.Type type) where T : Object
        {
            T[] objList = FindObjectsOfType<T>();
            List<T> matchList = new List<T>();

            foreach (T obj in objList)
            {
                System.Type objTypeRaw = obj.GetType();

                while (objTypeRaw != typeof(System.Object))
                {
                    objTypeRaw = objTypeRaw.BaseType;
                    System.Type objType = objTypeRaw.IsGenericType ? objTypeRaw.GetGenericTypeDefinition() : objTypeRaw;

                    if (objType == type)
                    {
                        matchList.Add(obj);
                        break;
                    }
                }
            }

            return matchList.ToArray();
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

        #region Custom LookRotation

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

        #endregion

        #region Vector2CoordinateConversion

        #region Vector type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 VecWorldToLocal(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisGiven">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 VecWorldToLocal(Vector2 vecToConvert, Vector2 axis, GivenAxis axisGiven)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axis, axisGiven);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 VecLocalToWorld(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisGiven">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 VecLocalToWorld(Vector2 vecToConvert, Vector2 axis, GivenAxis axisGiven)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axis, axisGiven);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 xAxisOfB, Vector2 yAxisOfB)
        {
            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisGiven">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 axisOfB, GivenAxis axisGiven)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            if(axisGiven == GivenAxis.Z)
            {
                Debug.LogError("ERROR : you're using Z axis with a 2D typed vector, please select either X or Y axis to give");
                return Vector2.zero;
            }

            if(axisGiven == GivenAxis.X)
            {
                xAxisOfB = axisOfB;
                yAxisOfB = new Vector2(-axisOfB.y, axisOfB.x);
            }

            if (axisGiven == GivenAxis.Y)
            {
                yAxisOfB = axisOfB;
                xAxisOfB = new Vector2(axisOfB.y, -axisOfB.x);
            }

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 xAxisOfA, Vector2 yAxisOfA)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = vecToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisGiven">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 axisOfA, GivenAxis axisGiven)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            if (axisGiven == GivenAxis.Z)
            {
                Debug.LogError("ERROR : you're using Z axis with a 2D typed vector, please select either X or Y axis to give");
                return Vector2.zero;
            }

            if (axisGiven == GivenAxis.X)
            {
                xAxisOfA = axisOfA;
                yAxisOfA = new Vector2(-axisOfA.y, axisOfA.x);
            }

            if (axisGiven == GivenAxis.Y)
            {
                yAxisOfA = axisOfA;
                xAxisOfA = new Vector2(axisOfA.y, -axisOfA.x);
            }

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = vecToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec;
        }

        #endregion

        #endregion

        #region Position Type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">point to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 PosWorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisGiven">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 PosWorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 axis, GivenAxis axisGiven)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, axis, axisGiven);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 PosLocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisGiven">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 PosLocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 axis, GivenAxis axisGiven)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, axis, axisGiven);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <returns></returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfB, Vector2 yAxisOfB)
        {
            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisGiven">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 axisOfB, GivenAxis axisGiven)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            if (axisGiven == GivenAxis.Z)
            {
                Debug.LogError("ERROR : you're using Z axis with a 2D typed vector, please select either X or Y axis to give");
                return Vector2.zero;
            }

            if (axisGiven == GivenAxis.X)
            {
                xAxisOfB = axisOfB;
                yAxisOfB = new Vector2(-axisOfB.y, axisOfB.x);
            }

            if (axisGiven == GivenAxis.Y)
            {
                yAxisOfB = axisOfB;
                xAxisOfB = new Vector2(axisOfB.y, -axisOfB.x);
            }

            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <returns></returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfA, Vector2 yAxisOfA)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisGiven">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 axisOfA, GivenAxis axisGiven)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            if (axisGiven == GivenAxis.Z)
            {
                Debug.LogError("ERROR : you're using Z axis with a 2D typed vector, please select either X or Y axis to give");
                return Vector2.zero;
            }

            if (axisGiven == GivenAxis.X)
            {
                xAxisOfA = axisOfA;
                yAxisOfA = new Vector2(-axisOfA.y, axisOfA.x);
            }

            if (axisGiven == GivenAxis.Y)
            {
                yAxisOfA = axisOfA;
                xAxisOfA = new Vector2(axisOfA.y, -axisOfA.x);
            }

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        #endregion

        #endregion

        #endregion

        public enum GivenAxis
        {
            X,
            Y,
            Z,
        }
    }
}
