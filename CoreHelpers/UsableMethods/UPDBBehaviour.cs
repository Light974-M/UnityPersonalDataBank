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
