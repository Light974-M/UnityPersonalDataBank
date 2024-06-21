using UnityEngine;

namespace UPDB.CoreHelper.ExtensionMethods
{
    /// <summary>
    /// method bank that can be used everywhere for AnimationCurves extensions
    /// </summary>
    public static class AnimationCurveExtensionMethods
    {
        /// <summary>
        /// make fully clamped(from 0 to 1 on x and y) evaluates for float
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float FullyClampedEvaluate(this AnimationCurve curve, float time)
        {
            //normalize value of timer for curve time
            float lastKeyTime = curve.keys[curve.length - 1].time;
            float firstKeyTime = curve.keys[0].time;

            float timeScaledValue = (time * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = curve.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = curve.keys[curve.length - 1].value;
            float firstKeyValue = curve.keys[0].value;

            float evaluatedTime = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return evaluatedTime;
        }
    } 
}
