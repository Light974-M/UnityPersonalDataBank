using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPDB.CoreHelper.UsableMethods
{
	public static class UPDBExtensionMethods
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

        /// <summary>
        /// WIP : cancel fully Nullref, avoid null values for variables, replace it by AddComponent.
        /// </summary>
        /// <typeparam name="T"> type parameter of variable to edit </typeparam>
        /// <param name="owner"> method extension for Component class </param>
        /// <param name="var"> variable to edit </param>
        public static void ReplaceDefaultComponentValue<T>(this Component owner, out T var) where T : Component
        {
            //if (!var.gameObject.TryGetComponent(out var))
            //    var = obj.AddComponent<T>();
            var = null;
        }

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
