using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;
using UPDB.CoreHelper.Usable;

namespace UPDB.CoreHelper.UsableMethods
{
    public static class UPDBExtensionMethods
    {
        /******************************************************UTILITY METHODS**********************************************************/

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
        /// WIP : avoid null values for variables by searching object type in all scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="var"></param>
        public static void UnnullableFindObjectOfType<T>(this Object owner, out T var) where T : Object
        {
            var = null;
        }

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

        /// <summary>
        /// try getting the key of a dictionnary given it's value
        /// </summary>
        /// <typeparam name="T">key type</typeparam>
        /// <typeparam name="W">value type</typeparam>
        /// <param name="dict">dictionary</param>
        /// <param name="val">value to search</param>
        /// <returns></returns>
        public static T KeyByValue<T, W>(this Dictionary<T, W> dict, W val)
        {
            T key = default;

            foreach (KeyValuePair<T, W> pair in dict)
            {
                if (EqualityComparer<W>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    break;
                }
            }

            return key;
        }

        /******************************************************UTILITY METHOD COLLECTIONS**********************************************************/

        #region CHECK DIRTY TOOLS

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this float value, ref float compareValue)
        {
            float toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this int value, ref int compareValue)
        {
            int toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this bool value, ref bool compareValue)
        {
            bool toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this string value, ref string compareValue)
        {
            string toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this char value, ref char compareValue)
        {
            char toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this Vector2 value, ref Vector2 compareValue)
        {
            Vector2 toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this Vector3 value, ref Vector3 compareValue)
        {
            Vector3 toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this Vector2Int value, ref Vector2Int compareValue)
        {
            Vector2Int toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty(this Vector3Int value, ref Vector3Int compareValue)
        {
            Vector3Int toCompare = compareValue;
            compareValue = value;

            return toCompare != value;
        }

        /// <summary>
        /// check if a value has been modified since last time it was tested, need a second variable to compare value
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="compareValue">value to compare with</param>
        /// <returns>if value and compareValue are different</returns>
        public static bool IsDirty<T>(this T value, ref T compareValue) where T : System.Enum
        {
            T toCompare = compareValue;
            compareValue = value;

            return System.Convert.ToInt32(toCompare) != System.Convert.ToInt32(value);
        }

        #endregion

        #region VALUE TESTS AND RESET FOR TIMERS AND OTHERS

        /// <summary>
        /// read the value of a number, while reseting it to a given number
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="reinitValue">number to reinit to</param>
        /// <param name="condition">value will reset only if true</param>
        /// <param name="comparisonType">select a comparison to make a test with a value and reinit only if condition succeed</param>
        /// <param name="comparedNumber">value to compare with</param>
        /// <returns>if conditions where true and value has been reseted</returns>
        public static bool TestAndReset(ref this float value, bool condition, Comparison comparisonType, float comparedNumber, float reinitValue)
        {
            if (!condition)
                return false;

            if (comparisonType == Comparison.Equal && value != comparedNumber)
                return false;
            if (comparisonType == Comparison.NotEqual && value == comparedNumber)
                return false;
            if (comparisonType == Comparison.Greater && value <= comparedNumber)
                return false;
            if (comparisonType == Comparison.Less && value >= comparedNumber)
                return false;
            if (comparisonType == Comparison.GreaterOrEqual && value < comparedNumber)
                return false;
            if (comparisonType == Comparison.LessOrEqual && value > comparedNumber)
                return false;

            value = reinitValue;
            return true;
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="reinitValue">number to reinit to</param>
        /// <param name="comparisonType">select a comparison to make a test with a value and reinit only if condition succeed</param>
        /// <param name="comparedNumber">value to compare with</param>
        /// <returns>if conditions where true and value has been reseted</returns>
        public static bool TestAndReset(ref this float value, Comparison comparisonType, float comparedNumber, float reinitValue)
        {
            return value.TestAndReset(true, comparisonType, comparedNumber, reinitValue);
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="condition">value will reset only if true</param>
        /// <param name="comparisonType">select a comparison to make a test with a value and reinit only if condition succeed</param>
        /// <param name="comparedNumber">value to compare with</param>
        /// <returns>if conditions where true and value has been reseted</returns>
        public static bool TestAndReset(ref this float value, bool condition, Comparison comparisonType, float comparedNumber)
        {
            return value.TestAndReset(condition, comparisonType, comparedNumber, 0);
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="comparisonType">select a comparison to make a test with a value and reinit only if condition succeed</param>
        /// <param name="comparedNumber">value to compare with</param>
        /// <returns>if conditions where true and value has been reseted</returns>
        public static bool TestAndReset(ref this float value, Comparison comparisonType, float comparedNumber)
        {
            return value.TestAndReset(true, comparisonType, comparedNumber, 0);
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="condition">value will reset only if true</param>
        /// <param name="reinitValue">number to reinit to</param>
        /// <returns>if conditions where true and value has been reseted</returns>
        public static bool TestAndReset(ref this float value, bool condition, float reinitValue)
        {
            return value.TestAndReset(condition, Comparison.None, 0, reinitValue);
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="condition">value will reset only if true</param>
        /// <returns>if conditions where true and value has been reseted</returns>
        public static bool TestAndReset(ref this float value, bool condition)
        {
            return value.TestAndReset(condition, Comparison.None, 0, 0);
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <param name="reinitValue">number to reinit to</param>
        /// <returns>the value of number before getting reseted</returns>
        public static float TestAndReset(ref this float value, float reinitValue)
        {
            float toReturn = value;
            value = reinitValue;
            return toReturn;
        }

        /// <summary>
        /// read the value of a number, while reseting it to 0
        /// </summary>
        /// <param name="value">number to reinit</param>
        /// <returns>the value of number before getting reseted</returns>
        public static float TestAndReset(ref this float value)
        {
            float toReturn = value;
            value = 0;
            return toReturn;
        }

        #endregion

        #region ENUM TESTS TOOLS

        /// <summary>
        /// test if a layer is in a LayerMask
        /// </summary>
        /// <param name="layer">layer to test</param>
        /// <param name="layerMask">LayerMask to test layer in</param>
        /// <returns>if layer is in LayerMask</returns>
        public static bool IsInLayerMask(this int layer, int layerMask)
        {
            return (layerMask | (1 << layer)) == layerMask;
        }

        /// <summary>
        /// test if the specified layer is in LayerMask AND every other layers are not
        /// </summary>
        /// <param name="layer">layer to test</param>
        /// <param name="layerMask">LayerMask to test layer in</param>
        /// <returns>true if layer and only layer is in LayerMask</returns>
        public static bool IsInLayerMaskExclusive(this int layer, int layerMask)
        {
            return (layerMask & (1 << layer)) == layerMask;
        }

        /// <summary>
        /// test if an enum value is in an enumFlag
        /// </summary>
        /// <typeparam name="T">type of enum tested</typeparam>
        /// <param name="element">enum value to test</param>
        /// <param name="enumflagValue">enumFlag value to test element in</param>
        /// <returns>if enum value is in enumFlag</returns>
        public static bool IsInEnumFlag<T>(this T element, T enumflagValue) where T : System.Enum
        {
            return ((int)(object)enumflagValue & (int)(object)element) == (int)(object)element;
        }

        /// <summary>
        /// test if an enum value is in an enumFlag and every other enum values are not
        /// </summary>
        /// <typeparam name="T">type of enum tested</typeparam>
        /// <param name="element">enum value to test</param>
        /// <param name="enumflagValue">enumFlag value to test element in</param>
        /// <returns>true if enum value and enum value only is in enumFlag</returns>
        public static bool IsInEnumFlagExclusive<T>(this T element, T enumflagValue) where T : System.Enum
        {
            return (int)(object)enumflagValue == (int)(object)element;
        }

        /// <summary>
        /// test if enum values are all in an enumFlag and every other enum values are not
        /// </summary>
        /// <typeparam name="T">type of enum tested</typeparam>
        /// <param name="elements">enum values to test</param>
        /// <param name="enumflagValue">enumFlag value to test element in</param>
        /// <returns>true if all enum values and all enum values only are in enumFlag</returns>
        public static bool IsInEnumFlagsANDExclusive<T>(this T[] elements, T enumflagValue) where T : System.Enum
        {
            if (elements.Length == 0)
                return false;

            int value = (int)(object)elements[0];

            for (int i = 1; i < elements.Length; i++)
                value = value | (int)(object)elements[i];

            return (int)(object)enumflagValue == value;
        }

        #endregion

        #region Direction From Rotation

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

        #endregion

        #region simple local pos setter

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

        #endregion

        #region Text Defil

        /// <summary>
        /// make a string value smoothly appear character by character in another variable
        /// </summary>
        /// <param name="text"> text to defil </param>
        /// <param name="defil"> where defil text will be stocked </param>
        public static string TextDefil(this string text, ref string defil)
        {
            if (defil == null)
                defil = string.Empty;

            if (text.Length != 0 && defil.Length < text.Length)
                defil += text[defil.Length];

            return defil;
        }

        /// <summary>
        /// make a string value smoothly appear character by character in another variable
        /// </summary>
        /// <param name="text"> text to defil </param>
        /// <param name="defil"> where defil text will be stocked </param>
        /// <param name="defilSpeed"> number of characters per second </param>
        /// <param name="deltaTime"> what time is passing between two calls of the method ? </param>
        public static string TextDefil(this string text, ref string defil, float defilSpeed, float deltaTime, ref float timer)
        {
            if (defil == null)
                defil = string.Empty;

            if (timer < 1f / defilSpeed)
            {
                timer += deltaTime;
                return defil;
            }

            while (timer >= 1f / defilSpeed)
            {
                timer -= 1f / defilSpeed;

                if (text.Length != 0 && defil.Length < text.Length)
                    defil += text[defil.Length];
            }

            timer = 0;

            return defil;
        }

        #endregion

        #region Procedural Mesh Generation

        public static void CalculateNormals(this Mesh mesh)
        {
            Vector3[] normals = new Vector3[mesh.vertices.Length];
            int triangleCount = mesh.triangles.Length / 3;

            for (int i = 0; i < triangleCount; i++)
            {
                int triangleIndex = i * 3;

                int triangleVertexA = mesh.triangles[triangleIndex];
                int triangleVertexB = mesh.triangles[triangleIndex + 1];
                int triangleVertexC = mesh.triangles[triangleIndex + 2];

                Vector3 triangleNormal = mesh.GetNormalOfTriangleByVertices(triangleVertexA, triangleVertexB, triangleVertexC);

                normals[triangleVertexA] += triangleNormal;
                normals[triangleVertexB] += triangleNormal;
                normals[triangleVertexC] += triangleNormal;
            }

            for (int i = 0; i < normals.Length; i++)
                normals[i].Normalize();

            mesh.normals = normals;
        }

        private static Vector3 GetNormalOfTriangleByVertices(this Mesh mesh, int vertA, int vertB, int vertC)
        {
            Vector3 posA = mesh.vertices[vertA];
            Vector3 posB = mesh.vertices[vertB];
            Vector3 posC = mesh.vertices[vertC];

            Vector3 AB = posB - posA;
            Vector3 AC = posC - posA;

            return Vector3.Cross(AB, AC).normalized;
        }

        #endregion
    }
}
