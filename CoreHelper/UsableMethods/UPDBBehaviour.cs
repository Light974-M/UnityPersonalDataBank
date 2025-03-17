using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.Usable.ObjectsLibrary;

namespace UPDB.CoreHelper.UsableMethods
{
    ///<summary>
    /// UPDB methods that does not use extensions, callable in every classes that derives from monoBehaviour
    ///</summary>
    public abstract class UPDBBehaviour : MonoBehaviour
    {
        /******************************************************NATIVE METHODS***********************************************************/

        /// <summary>
        /// call when scene is updating before rendering gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            OnScene();
#endif  
        }

        /// <summary>
        /// call when scene is updating before rendering gizmos if obhect is selected by inspector
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            OnSceneSelected();
#endif
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
        /// <returns>the state of searching, true if object was found, false if no objects were found</returns>
        public static bool TryFindObjectOfType<T>(out T variable) where T : Object
        {
            variable = FindFirstObjectByType<T>();
            return variable != null;
        }

        /// <summary>
        /// return if two lists have exactly the sames values(same number of objects and same objects at same positions)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns>if the given lists are the same or not</returns>
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
        /// <returns>the angular velocity of the given rotation delta</returns>
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
        /// <param name="obj">objetc to destroy</param>
        public static void IntelliDestroy(Object obj)
        {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        /// <summary>
        /// make a (not really optimized) find object of type, but for generic classes, it will search if any object in scene are derived from the class<T> given
        /// </summary>
        /// <typeparam name="T">parent class, if none, put System.Object</typeparam>
        /// <param name="type">generic class to search for</param>
        /// <returns>the generic class found</returns>
        public static T[] FindObjectsOfTypeGeneric<T>(System.Type type) where T : Object
        {
            T[] objList = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
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

        /// <summary>
        /// take the names of LayerMask, and return the name of each layers 
        /// </summary>
        /// <returns>array of all layers names</returns>
        public static string[] GetLayerNames()
        {
            string[] layers = new string[32];
            for (int i = 0; i < 32; i++)
            {
                layers[i] = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layers[i]))
                {
                    layers[i] = $"Layer {i}";
                }
            }
            return layers;
        }

        /// <summary>
        /// get a list of transform direction that represent a sphere ith a given number of horizontal and vertical vertice
        /// </summary>
        /// <param name="xNumber"></param>
        /// <param name="yNumber"></param>
        /// <returns></returns>
        public static Vector3[][] GetSphereVerticeDirections(int xNumber, int yNumber)
        {
            if (xNumber == 0 || yNumber == 0)
                return new Vector3[0][];

            Vector3[] yPosList = new Vector3[yNumber];
            Vector3 verticalPos = Vector3.up;

            for (int i = 0; i < yPosList.Length; i++)
            {
                yPosList[i] = verticalPos;
                verticalPos = RotateVector(verticalPos, Vector3.right, 180 / ((float)yNumber - 1));
            }

            Vector3[][] posList = new Vector3[yNumber][];

            for (int i = 0; i < yNumber; i++)
            {
                Vector3 horizontalPos = yPosList[i];
                posList[i] = new Vector3[xNumber];

                for (int j = 0; j < xNumber; j++)
                {
                    posList[i][j] = horizontalPos;
                    horizontalPos = RotateVector(horizontalPos, Vector3.up, 360 / (float)xNumber);
                }
            }

            return posList;
        }

        /// <summary>
        /// return true if gameObject has no components
        /// </summary>
        /// <param name="obj">object to test</param>
        /// <returns>if the gameObject is null</returns>
        public static bool IsGameObjectEmpty(GameObject obj)
        {
            Component[] components = obj.GetComponents<Component>();

            return components.Length <= 1;
        }

        /// <summary>
        /// return true if gameObject has no components
        /// </summary>
        /// <param name="obj">object to test</param>
        /// <returns>if the gameObject is null</returns>
        public static bool IsGameObjectEmpty(GameObject obj, Component exclude)
        {
            Component[] components = obj.GetComponents<Component>();

            foreach (Component component in components)
                if(component != obj.transform && component != exclude)
                    return false;

            return true;
        }

        /// <summary>
        /// return true if value is between the two bounds
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="min">min bound</param>
        /// <param name="max">max bound</param>
        /// <returns>if value is within min and max bounds</returns>
        public static bool IsInRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static bool IsLayerInLayerMask(int layer, LayerMask layerMask)
        {
            return (layerMask & (1 << layer)) != 0;
        }

        public static T PickRandomPointInList<T>(T[] array)
        {
            int i = Random.Range(0, array.Length);
            return array[i];
        }

        public static int GetWeightedRandomIndex(List<float> weights)
        {
            if (weights == null || weights.Count == 0)
            {
                Debug.LogError("La liste des poids ne peut pas être vide");
                return 0;
            }

            float totalWeight = 0;
            foreach (float weight in weights)
            {
                if (weight < 0)
                {
                    Debug.LogError("Les poids doivent être positifs.");
                    return 0;
                }

                totalWeight += weight;
            }

            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0;

            for (int i = 0; i < weights.Count; i++)
            {
                cumulativeWeight += weights[i];
                if (randomValue <= cumulativeWeight)
                    return i;
            }

            return weights.Count - 1; // Sécurité (ne devrait jamais arriver)
        }

        /************************************************UTILITY METHODS COLLECTIONS****************************************************/

        //LERP TOOLS

        #region Auto Lerp

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns>the value of lerp with the current time</returns>
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
        /// <returns>the value of lerp with the current time</returns>
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
        /// <returns>the value of lerp with the current time</returns>
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
        /// <returns>the value of lerp with the current time</returns>
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
        /// <returns>the value of lerp with the current time</returns>
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
        /// <returns>the value of lerp with the current time</returns>
        public static float AutoLerp(float a, float b, float lerpTime, ref float timer, ref AnimationCurve smoothTimer)
        {
            float value = 0;

            if (timer < lerpTime)
            {
                float timerNormalized = (timer / lerpTime);
                value = Mathf.Lerp(a, b, GetShapedTime(timerNormalized, ref smoothTimer));

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
        /// <returns>the value of lerp with the current time</returns>
        public static Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer, ref AnimationCurve smoothTimer)
        {
            //create a null vector 2
            Vector2 value = Vector2.zero;

            //if timer has not reach lerp time, update lerp state, if it has, put timer to lerp time, and value to end state
            if (timer < lerpTime)
            {
                float timerNormalized = (timer / lerpTime);
                value = Vector2.Lerp(a, b, GetShapedTime(timerNormalized, ref smoothTimer));

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
        /// <returns>the value of lerp with the current time</returns>
        public static Vector3 AutoLerp(Vector3 a, Vector3 b, float lerpTime, ref float timer, ref AnimationCurve smoothTimer)
        {
            Vector3 value = Vector3.zero;

            if (timer < lerpTime)
            {
                float timerNormalized = (timer / lerpTime);
                value = Vector3.Lerp(a, b, GetShapedTime(timerNormalized, ref smoothTimer));

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
        /// <returns>the value of lerp with the current time</returns>
        public static Vector4 AutoLerp(Vector4 a, Vector4 b, float lerpTime, ref float timer, ref AnimationCurve smoothTimer)
        {
            Vector4 value = Vector4.zero;

            if (timer < lerpTime)
            {
                float timerNormalized = (timer / lerpTime);
                value = Vector4.Lerp(a, b, GetShapedTime(timerNormalized, ref smoothTimer));

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
        /// <returns>the value of lerp with the current time</returns>
        public static Quaternion AutoLerp(Quaternion a, Quaternion b, float lerpTime, ref float timer, ref AnimationCurve smoothTimer)
        {
            Quaternion value = Quaternion.identity;

            if (timer < lerpTime)
            {
                float timerNormalized = (timer / lerpTime);
                value = Quaternion.Lerp(a, b, GetShapedTime(timerNormalized, ref smoothTimer));

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

        #region Curve Lerp

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns>the lerp value at t read from shape curve</returns>
        public static float CurveLerp(float a, float b, float t, ref AnimationCurve shape)
        {
            return Mathf.Lerp(a, b, GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns>the lerp value at t read from shape curve</returns>
        public static Vector2 CurveLerp(Vector2 a, Vector2 b, float t, ref AnimationCurve shape)
        {
            return Vector2.Lerp(a, b, GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns>the lerp value at t read from shape curve</returns>
        public static Vector3 CurveLerp(Vector3 a, Vector3 b, float t, ref AnimationCurve shape)
        {
            return Vector3.Lerp(a, b, GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns>the lerp value at t read from shape curve</returns>
        public static Vector4 CurveLerp(Vector4 a, Vector4 b, float t, ref AnimationCurve shape)
        {
            return Vector4.Lerp(a, b, GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns>the lerp value at t read from shape curve</returns>
        public static Quaternion CurveLerp(Quaternion a, Quaternion b, float t, ref AnimationCurve shape)
        {
            return Quaternion.Lerp(a, b, GetShapedTime(t, ref shape));
        }

        #endregion

        #region Unbounded Lerp

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, lerp will still calculate to make value pass under a, or over b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static float UnboundedLerp(float a, float b, float t)
        {
            return a + ((b - a) * t);
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, lerp will still calculate to make value pass under a, or over b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static Vector2 UnboundedLerp(Vector2 a, Vector2 b, float t)
        {
            return a + ((b - a) * t);
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, lerp will still calculate to make value pass under a, or over b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static Vector3 UnboundedLerp(Vector3 a, Vector3 b, float t)
        {
            return a + ((b - a) * t);
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, lerp will still calculate to make value pass under a, or over b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static Vector4 UnboundedLerp(Vector4 a, Vector4 b, float t)
        {
            return a + ((b - a) * t);
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, you can't directly with t value, but you can if you set a value over final key or under initial key of curve
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static float UnboundedLerp(float a, float b, float t, ref AnimationCurve shape)
        {
            return a + ((b - a) * GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, you can't directly with t value, but you can if you set a value over final key or under initial key of curve
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static Vector2 UnboundedLerp(Vector2 a, Vector2 b, float t, ref AnimationCurve shape)
        {
            return a + ((b - a) * GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, you can't directly with t value, but you can if you set a value over final key or under initial key of curve
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static Vector3 UnboundedLerp(Vector3 a, Vector3 b, float t, ref AnimationCurve shape)
        {
            return a + ((b - a) * GetShapedTime(t, ref shape));
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, you can't directly with t value, but you can if you set a value over final key or under initial key of curve
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <returns>the lerp value</returns>
        public static Vector4 UnboundedLerp(Vector4 a, Vector4 b, float t, ref AnimationCurve shape)
        {
            return a + ((b - a) * GetShapedTime(t, ref shape));
        }

        #endregion

        #region Unbounded Shaped Lerp

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, lerp will still calculate to make value pass under a, or over b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <param name="valueShape">curve modifier of value output</param>
        /// <returns>the lerp value</returns>
        public static Vector2 UnboundedShapedLerp(Vector2 a, Vector2 b, float t, ref AnimationCurve valueShape)
        {
            Vector2 value = a + ((b - a) * t);

            return GetShapedValue(a, b, value, t, ref valueShape);
        }

        /// <summary>
        /// make a manual lerp calculation to allow overpassing bounds 0 and 1, lerp will still calculate to make value pass under a, or over b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <param name="shape">curve modifier of t</param>
        /// <param name="valueShape">curve modifier of value output</param>
        /// <returns></returns>
        public static Vector2 UnboundedShapedLerp(Vector2 a, Vector2 b, float t, ref AnimationCurve shape, ref AnimationCurve valueShape)
        {
            Vector2 value = a + ((b - a) * GetShapedTime(t, ref shape));

            return GetShapedValue(a, b, value, t, ref valueShape);
        }

        #endregion

        #region Unbounded Inverse Lerp

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(float a, float b, float value)
        {
            return (value - a) / (b - a);
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(Vector2 a, Vector2 b, Vector2 value)
        {
            Vector2 time = UPDBMath.Divide(value - a, b - a);

            if (time.x != time.y)
            {
                Debug.LogWarning("warning : given value of inverse lerp do not exist between ");
                return 0;
            }

            return time.x;
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 time = UPDBMath.Divide(value - a, b - a);

            if (time.x != time.y)
            {
                Debug.LogWarning("warning : given value of inverse lerp do not exist between ");
                return 0;
            }

            return time.x;
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(Vector4 a, Vector4 b, Vector4 value)
        {
            Vector4 time = UPDBMath.Divide(value - a, b - a);

            if (time.x != time.y)
            {
                Debug.LogWarning("warning : given value of inverse lerp do not exist between ");
                return 0;
            }

            return time.x;
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <param name="shape">shape of getted time</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(float a, float b, float value, ref AnimationCurve shape)
        {
            float t = (value - a) / (b - a);
            return GetShapedTime(t, ref shape);
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <param name="shape">shape of getted time</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(Vector2 a, Vector2 b, Vector2 value, ref AnimationCurve shape)
        {
            Vector2 time = UPDBMath.Divide(value - a, b - a);

            if (time.x != time.y)
            {
                Debug.LogWarning("warning : given value of inverse lerp do not exist between ");
                return 0;
            }

            return GetShapedTime(time.x, ref shape);
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <param name="shape">shape of getted time</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(Vector3 a, Vector3 b, Vector3 value, ref AnimationCurve shape)
        {
            Vector3 time = UPDBMath.Divide(value - a, b - a);

            if (time.x != time.y)
            {
                Debug.LogWarning("warning : given value of inverse lerp do not exist between ");
                return 0;
            }

            return GetShapedTime(time.x, ref shape);
        }

        /// <summary>
        /// make a manual inverse lerp calculation to allow overpassing bounds a and b, lerp will still calculate to make value pass under 0, or over 1
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="value">value of lerp</param>
        /// <param name="shape">shape of getted time</param>
        /// <returns>the inverse lerp value</returns>
        public static float UnboundedInverseLerp(Vector4 a, Vector4 b, Vector4 value, ref AnimationCurve shape)
        {
            Vector4 time = UPDBMath.Divide(value - a, b - a);

            if (time.x != time.y)
            {
                Debug.LogWarning("warning : given value of inverse lerp do not exist between ");
                return 0;
            }

            return GetShapedTime(time.x, ref shape);
        }

        #endregion

        #region Normalized Coordinate Lerp

        /// <summary>
        /// make a lerp that, instead of asking a value between 0 and 1, ask a position in coordinates, and return the normalized position between a and b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <param name="unlockBounds">allow t value to be inferior or superior to a and b bounds</param>
        /// <returns>the position of value normalized between a and b</returns>
        public static Vector2 NormalizedCoordinateLerp(Vector2 a, Vector2 b, Vector2 t, bool unlockBounds)
        {
            if (unlockBounds)
                return new Vector2(UnboundedLerp(a.x, b.x, t.x), UnboundedLerp(a.y, b.y, t.y));

            return new Vector2(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y));
        }

        /// <summary>
        /// make a lerp that, instead of asking a value between 0 and 1, ask a position in coordinates, and return the normalized position between a and b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <param name="unlockBounds">allow t value to be inferior or superior to a and b bounds</param>
        /// <returns>the position of value normalized between a and b</returns>
        public static Vector3 NormalizedCoordinateLerp(Vector3 a, Vector3 b, Vector3 t, bool unlockBounds)
        {
            if (unlockBounds)
                return new Vector3(UnboundedLerp(a.x, b.x, t.x), UnboundedLerp(a.y, b.y, t.y), UnboundedLerp(a.z, b.z, t.z));

            return new Vector3(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y), Mathf.Lerp(a.z, b.z, t.z));
        }

        /// <summary>
        /// make a lerp that, instead of asking a value between 0 and 1, ask a position in coordinates, and return the normalized position between a and b
        /// </summary>
        /// <param name="a">init pos</param>
        /// <param name="b">final pos</param>
        /// <param name="t">value of lerp</param>
        /// <param name="unlockBounds">allow t value to be inferior or superior to a and b bounds</param>
        /// <returns>the position of value normalized between a and b</returns>
        public static Vector4 NormalizedCoordinateLerp(Vector4 a, Vector4 b, Vector4 t, bool unlockBounds)
        {
            if (unlockBounds)
                return new Vector4(UnboundedLerp(a.x, b.x, t.x), UnboundedLerp(a.y, b.y, t.y), UnboundedLerp(a.z, b.z, t.z), UnboundedLerp(a.w, b.w, t.w));

            return new Vector4(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y), Mathf.Lerp(a.z, b.z, t.z), Mathf.Lerp(a.w, b.w, t.w));
        }

        #endregion

        #region Lerp Sub methods

        /// <summary>
        /// read t value in animation curve, first key and last key of curve gives the considered "0" and "1" both in x and y
        /// </summary>
        /// <param name="t">time value between 0 and 1(usually)</param>
        /// <param name="shape">curve to read from</param>
        /// <returns>the time read in shape curve</returns>
        public static float GetShapedTime(float t, ref AnimationCurve shape)
        {
            //if curve isn't readable or null, create a linear curve
            if (shape == null || shape.keys.Length < 2)
                shape = AnimationCurve.Linear(0, 0, 1, 1);

            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            return (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);
        }

        /// <summary>
        /// read t value in animation curve, first key and last key of curve gives the considered "0" and "1" in x, and keep y value raw
        /// </summary>
        /// <param name="t">time value between 0 and 1(usually)</param>
        /// <param name="shape">curve to read from</param>
        /// <returns>the raw value of curve at time position</returns>
        public static float GetShapedTimeRawValue(float t, ref AnimationCurve shape)
        {
            //if curve isn't readable or null, create a linear curve
            if (shape == null || shape.keys.Length < 2)
                shape = AnimationCurve.Linear(0, 0, 1, 1);

            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            return timeCurveValue;
        }

        /// <summary>
        /// return value shaped with shape animation curve
        /// </summary>
        /// <param name="start">lower bound of value</param>
        /// <param name="end">upper bound of value</param>
        /// <param name="value">value to shape</param>
        /// <param name="time">time to shape</param>
        /// <param name="shape">curve to shape with</param>
        /// <returns>shaped value</returns>
        public static Vector2 GetShapedValue(Vector2 start, Vector2 end, Vector2 value, float time, ref AnimationCurve shape)
        {
            if (time < 0 || time > 1)
                return value;

            Vector2 boundDir = end - start;
            Vector2 valueDir = value - start;
            Vector2 clampedValueDir = valueDir / boundDir.magnitude;

            float valueToAdd = GetShapedTimeRawValue(clampedValueDir.magnitude, ref shape);

            Vector2 verticalVec = FindPerpToVec(boundDir.normalized, Axis.Y);
            verticalVec *= valueToAdd;

            return value + verticalVec;
        }

        #endregion


        //VECTOR AND ROTATION CALCULATIONS TOOLS

        #region Custom LookRotation

        /// <summary>
        /// calculate rotation with a direction vector and a up vector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="up"></param>
        /// <returns>the rotation that make object look towards direction while staying upwards toward up</returns>
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
        /// get the rotation quaternion locally from another rotation(just like local rotation with parenting GameObjects)
        /// </summary>
        /// <param name="worldRotation">world rotation</param>
        /// <param name="parentRotation">rotation to parent to</param>
        /// <returns>the local rotation from parent rotation</returns>
        public static Quaternion GetLocalRotation(Quaternion worldRotation, Quaternion parentRotation)
        {
            return Quaternion.Inverse(parentRotation) * worldRotation;
        }

        /// <summary>
        /// get the world rotation quaternion from a local rotation(just like local rotation with parenting GameObjects)
        /// </summary>
        /// <param name="localRotation">locals rotation</param>
        /// <param name="parentRotation">rotation to unparent from</param>
        /// <returns>the world rotation from parent rotation</returns>
        public static Quaternion GetWorldRotation(Quaternion localRotation, Quaternion parentRotation)
        {
            return localRotation * parentRotation;
        }

        /// <summary>
        /// give a vector wich as the vector a rotated along axis with theta angle
        /// </summary>
        /// <param name="a">vector to rotate from</param>
        /// <param name="axis">vector that a vector rotate from, imagine this vector "turning" on itself and taking a vector with it</param>
        /// <param name="theta">angle between a and returned vector</param>
        /// <returns>vector that has theta angle with a and same angle with axis than a/returns>
        public static Vector3 RotateVector(Vector3 a, Vector3 axis, float theta)
        {
            Quaternion rotation = Quaternion.AngleAxis(theta, axis.normalized);
            return rotation * a;
        }

        #endregion

        #region Vector2CoordinateConversion

        #region Vector type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns>local vector</returns>
        public static Vector2 Vec2WorldToLocal(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns>local vector</returns>
        public static Vector2 Vec2WorldToLocal(Vector2 vecToConvert, Vector2 axis, Axis axisToFind)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local vector</returns>
        public static Vector2 Vec2WorldToLocal(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local vector</returns>
        public static Vector2 Vec2WorldToLocal(Vector2 vecToConvert, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns>world vector</returns>
        public static Vector2 Vec2LocalToWorld(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns>world vector</returns>
        public static Vector2 Vec2LocalToWorld(Vector2 vecToConvert, Vector2 axis, Axis axisToFind)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returnsworld vector></returns>
        public static Vector2 Vec2LocalToWorld(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world vector</returns>
        public static Vector2 Vec2LocalToWorld(Vector2 vecToConvert, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <returns>vector converted in system B with given system B</returns>
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
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 axisOfB, Axis axisToFind)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

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
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 xAxisOfB, Vector2 yAxisOfB, bool ignoreScale)
        {
            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

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
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 axisOfB, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <returns>vector converted in system B with given system A</returns>
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
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 axisOfA, Axis axisToFind)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

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
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 xAxisOfA, Vector2 yAxisOfA, bool ignoreScale)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? vecToConvert : vecToConvert * axisMag;

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
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 axisOfA, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? vecToConvert : vecToConvert * axisMag;

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
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns>local position</returns>
        public static Vector2 Point2WorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns>local position</returns>
        public static Vector2 Point2WorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">point to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local position</returns>
        public static Vector2 Point2WorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local position</returns>
        public static Vector2 Point2WorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns>world position</returns>
        public static Vector2 Point2LocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns>world position</returns>
        public static Vector2 Point2LocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world position</returns>
        public static Vector2 Point2LocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world position</returns>
        public static Vector2 Point2LocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <returns>position converted in system B with given system B</returns>
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
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns>position converted in system B with given system B</returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 axisOfB, Axis axisToFind)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

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
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted in system B with given system B</returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfB, Vector2 yAxisOfB, bool ignoreScale)
        {
            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

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
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted in system B with given system B</returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 axisOfB, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <returns>position converted in system B with given system A</returns>
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
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns>position converted in system B with given system A</returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 axisOfA, Axis axisToFind)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

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
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted in system B with given system A</returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfA, Vector2 yAxisOfA, bool ignoreScale)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? posToConvert : posToConvert * axisMag;

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
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted in system B with given system A</returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 axisOfA, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? posToConvert : posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        #endregion

        #endregion

        #endregion

        #region Vector3CoordinateConversion

        #region Vector type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns>local Vector</returns>
        public static Vector3 Vec3WorldToLocal(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns>local Vector</returns>
        public static Vector3 Vec3WorldToLocal(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local Vector</returns>
        public static Vector3 Vec3WorldToLocal(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local Vector</returns>
        public static Vector3 Vec3WorldToLocal(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <returns>world Vector</returns>
        public static Vector3 Vec3LocalToWorld(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns>world Vector</returns>
        public static Vector3 Vec3LocalToWorld(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world Vector</returns>
        public static Vector3 Vec3LocalToWorld(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world Vector</returns>
        public static Vector3 Vec3LocalToWorld(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB)
        {
            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis)
        {
            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        //EXPERIMENTAL
        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 axisOfB, Axis givenAxis)
        {
            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            //Vector3 originalAxisOfB = axisOfB;
            //axisOfB = new Vector3(Mathf.Abs(axisOfB.x), Mathf.Abs(axisOfB.y), Mathf.Abs(axisOfB.z));
            Vector3 axisTwoOfB = UPDBMath.FindAnyPerpendicularVectorUpType(axisOfB);

            if (givenAxis == Axis.X)
            {
                FindPerpToVecs(axisOfB, axisTwoOfB, Axis.X, Axis.Y, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);
            }
            if (givenAxis == Axis.Y)
            {
                FindPerpToVecs(axisOfB, axisTwoOfB, Axis.Y, Axis.X, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            }
            if (givenAxis == Axis.Z)
            {
                FindPerpToVecs(axisOfB, axisTwoOfB, Axis.Z, Axis.Y, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            }


            Debug.DrawRay(Vector3.zero, xAxisOfB);
            Debug.DrawRay(Vector3.zero, yAxisOfB);
            //Debug.DrawRay(Vector3.zero, zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB, bool ignoreScale)
        {
            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system B</returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.Multiply(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.Multiply(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA, bool ignoreScale)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? vecToConvert : UPDBMath.Multiply(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>vector converted in system B with given system A</returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? vecToConvert : UPDBMath.Multiply(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        #endregion

        #endregion

        #region Position Type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns>local position</returns>
        public static Vector3 Point3WorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns>local position</returns>
        public static Vector3 Point3WorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local position</returns>
        public static Vector3 Point3WorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>local position</returns>
        public static Vector3 Point3WorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <returns>world position</returns>
        public static Vector3 Point3LocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns>world position</returns>
        public static Vector3 Point3LocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world position</returns>
        public static Vector3 Point3LocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>world position</returns>
        public static Vector3 Point3LocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <returns>position converted to system B with given system B</returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns>position converted to system B with given system B</returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted to system B with given system B</returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB, bool ignoreScale)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted to system B with given system B</returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.Divide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <returns>position converted to system B with given system A</returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.Multiply(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns>position converted to system B with given system A</returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.Multiply(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted to system B with given system A</returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA, bool ignoreScale)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? pointToConvert : UPDBMath.Multiply(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns>position converted to system B with given system A</returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? pointToConvert : UPDBMath.Multiply(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        #endregion

        #endregion

        #endregion

        #region FindPerpendicularVector

        #region 2D

        /// <summary>
        /// find the vector perpendicular to the given one
        /// </summary>
        /// <param name="givenAxis">vec to dearch for</param>
        /// <param name="axisToFind">wich axis you give ? basically means that : x</param>
        /// <returns>vector perpendiculat to the given vector</returns>
        public static Vector2 FindPerpToVec(Vector2 givenAxis, Axis axisToFind)
        {
            if (axisToFind != Axis.X && axisToFind != Axis.Y)
            {
                Debug.LogError("ERROR : you're using an axis with a 2D typed vector wich is not X or Y, please select either X or Y axis to give");
                return Vector2.up;
            }

            if (axisToFind == Axis.Y)
                return UPDBMath.FindLeftPerpendicularVector(givenAxis);

            if (axisToFind == Axis.X)
                return UPDBMath.FindRightPerpendicularVector(givenAxis);

            Debug.LogError("ERROR : method " + nameof(FindPerpToVec) + "return no axis possible to give");
            return Vector3.up;
        }

        /// <summary>
        /// find the vector perpendicular to the given one
        /// </summary>
        /// <param name="givenAxis">vec to dearch for</param>
        /// <param name="axisToFind">wich axis you give ? basically means that : x</param>
        public static void FindPerpToVec(Vector2 givenAxis, Axis axisToFind, ref Vector2 x, ref Vector2 y)
        {
            if (axisToFind != Axis.X && axisToFind != Axis.Y)
            {
                Debug.LogError("ERROR : you're using an axis with a 2D typed vector wich is not X or Y, please select either X or Y axis to give");
                return;
            }

            if (axisToFind == Axis.Y)
            {
                x = givenAxis;
                y = UPDBMath.FindLeftPerpendicularVector(givenAxis);
                return;
            }

            if (axisToFind == Axis.X)
            {
                x = UPDBMath.FindRightPerpendicularVector(givenAxis);
                y = givenAxis;
                return;
            }

            Debug.LogError("ERROR : method " + nameof(FindPerpToVec) + "return no axis possible to give");
        }

        #endregion

        #region 3D

        /// <summary>
        /// find the vector perpendicular to the given ones
        /// </summary>
        /// <param name="givenAxisA">vec A</param>
        /// <param name="givenAxisB">vec B perpendicular to A</param>
        /// <param name="axisToFind">wich axis you give ? basically means that : x</param>
        /// <returns>vector perpendicular to the two other found</returns>
        public static void FindPerpToVecs(Vector3 givenAxisA, Vector3 givenAxisB, Axis firstGivenAxis, Axis secondGivenAxis, ref Vector3 x, ref Vector3 y, ref Vector3 z)
        {
            if (firstGivenAxis == secondGivenAxis || (firstGivenAxis != Axis.X && firstGivenAxis != Axis.Y && firstGivenAxis != Axis.Z) || (secondGivenAxis != Axis.X && secondGivenAxis != Axis.Y && secondGivenAxis != Axis.Z))
            {
                Debug.LogError("ERROR : you setted something wrong with given axis, either first and second are equal, or one of them does not represent either x y or z");
                return;
            }

            if (firstGivenAxis == Axis.X && secondGivenAxis == Axis.Y)
            {
                x = givenAxisA;
                y = givenAxisB;
                z = UPDBMath.FindThirdAxis(x, y);
                return;
            }
            if (firstGivenAxis == Axis.Y && secondGivenAxis == Axis.X)
            {
                x = givenAxisB;
                y = givenAxisA;
                z = UPDBMath.FindThirdAxis(x, y);
                return;
            }

            if (firstGivenAxis == Axis.Y && secondGivenAxis == Axis.Z)
            {
                y = givenAxisA;
                z = givenAxisB;
                x = UPDBMath.FindThirdAxis(y, z);
                return;
            }
            if (firstGivenAxis == Axis.Z && secondGivenAxis == Axis.Y)
            {
                y = givenAxisB;
                z = givenAxisA;
                x = UPDBMath.FindThirdAxis(y, z);
                return;
            }

            if (firstGivenAxis == Axis.X && secondGivenAxis == Axis.Z)
            {
                x = givenAxisA;
                z = givenAxisB;
                y = -UPDBMath.FindThirdAxis(x, z);

                return;
            }
            if (firstGivenAxis == Axis.Z && secondGivenAxis == Axis.X)
            {
                x = givenAxisB;
                z = givenAxisA;
                y = -UPDBMath.FindThirdAxis(x, z);
                return;
            }

            Debug.LogError("ERROR : method " + nameof(FindPerpToVec) + "return no axis possible to give");
        }

        #endregion

        #endregion


        //DEBUG TOOLS

        #region Custom Debug

        #region Debug Arrows

        /// <summary>
        /// draw a simple arrow going towards direction
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        public static void DebugDrawArrow(Vector3 origin, Vector3 direction)
        {
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = origin + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB);
            Debug.DrawLine(crossMinPosB, crossMaxPosA);
            Debug.DrawLine(crossMaxPosB, crossMinPosA);

            //draw pin
            Debug.DrawLine(crossMinPosA, origin + direction);
            Debug.DrawLine(crossMaxPosA, origin + direction);
            Debug.DrawLine(crossMinPosB, origin + direction);
            Debug.DrawLine(crossMaxPosB, origin + direction);
        }

        /// <summary>
        /// draw a simple arrow going towards direction
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color)
        {
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = origin + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB, color);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB, color);
            Debug.DrawLine(crossMinPosB, crossMaxPosA, color);
            Debug.DrawLine(crossMaxPosB, crossMinPosA, color);

            //draw pin
            Debug.DrawLine(crossMinPosA, origin + direction, color);
            Debug.DrawLine(crossMaxPosA, origin + direction, color);
            Debug.DrawLine(crossMinPosB, origin + direction, color);
            Debug.DrawLine(crossMaxPosB, origin + direction, color);
        }

        /// <summary>
        /// draw a simple arrow going towards direction
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        /// <param name="color">color of arrow</param>
        /// <param name="arrowSize">between 0 and 1</param>
        public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowSize)
        {
            arrowSize = direction.magnitude * arrowSize;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = origin + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB, color);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB, color);
            Debug.DrawLine(crossMinPosB, crossMaxPosA, color);
            Debug.DrawLine(crossMaxPosB, crossMinPosA, color);

            //draw pin
            Debug.DrawLine(crossMinPosA, origin + direction, color);
            Debug.DrawLine(crossMaxPosA, origin + direction, color);
            Debug.DrawLine(crossMinPosB, origin + direction, color);
            Debug.DrawLine(crossMaxPosB, origin + direction, color);
        }

        /// <summary>
        /// draw an arrow from point start to end
        /// </summary>
        /// <param name="start">starting point</param>
        /// <param name="end">end point</param>
        public static void DebugDrawArrowLine(Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = start + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(start, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB);
            Debug.DrawLine(crossMinPosB, crossMaxPosA);
            Debug.DrawLine(crossMaxPosB, crossMinPosA);

            //draw pin
            Debug.DrawLine(crossMinPosA, end);
            Debug.DrawLine(crossMaxPosA, end);
            Debug.DrawLine(crossMinPosB, end);
            Debug.DrawLine(crossMaxPosB, end);
        }

        /// <summary>
        /// draw an arrow from point start to end
        /// </summary>
        /// <param name="start">starting point</param>
        /// <param name="end">end point</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrowLine(Vector3 start, Vector3 end, Color color)
        {
            Vector3 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = start + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(start, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB, color);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB, color);
            Debug.DrawLine(crossMinPosB, crossMaxPosA, color);
            Debug.DrawLine(crossMaxPosB, crossMinPosA, color);

            //draw pin
            Debug.DrawLine(crossMinPosA, end, color);
            Debug.DrawLine(crossMaxPosA, end, color);
            Debug.DrawLine(crossMinPosB, end, color);
            Debug.DrawLine(crossMaxPosB, end, color);
        }

        /// <summary>
        /// draw an arrow from point start to end
        /// </summary>
        /// <param name="start">starting point</param>
        /// <param name="end">end point</param>
        /// <param name="color">color of arrow</param>
        /// <param name="arrowSize">between 0 and 1</param>
        public static void DebugDrawArrowLine(Vector3 start, Vector3 end, Color color, float arrowSize)
        {
            Vector3 direction = end - start;
            arrowSize = direction.magnitude * arrowSize;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = start + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(start, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB, color);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB, color);
            Debug.DrawLine(crossMinPosB, crossMaxPosA, color);
            Debug.DrawLine(crossMaxPosB, crossMinPosA, color);

            //draw pin
            Debug.DrawLine(crossMinPosA, end, color);
            Debug.DrawLine(crossMaxPosA, end, color);
            Debug.DrawLine(crossMinPosB, end, color);
            Debug.DrawLine(crossMaxPosB, end, color);
        }

        /// <summary>
        /// draw a simple arrow going towards direction, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="direction">direction of arrow</param>
        public static void DebugDrawArrow(Vector2 start, Vector2 direction)
        {
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos);

            //draw pin
            Debug.DrawLine(crossMinPos, start + direction);
            Debug.DrawLine(crossMaxPos, start + direction);

        }

        /// <summary>
        /// draw a simple arrow going towards direction, in 2D
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrow(Vector2 origin, Vector2 direction, Color color)
        {
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = origin + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos, color);

            //draw pin
            Debug.DrawLine(crossMinPos, origin + direction, color);
            Debug.DrawLine(crossMaxPos, origin + direction, color);

        }

        /// <summary>
        /// draw a simple arrow going towards direction, in 2D
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        /// <param name="color">color of arrow</param>
        /// <param name="arrowSize">between 0 and 1</param>
        public static void DebugDrawArrow(Vector2 origin, Vector2 direction, Color color, float arrowSize)
        {
            arrowSize = direction.magnitude * arrowSize;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = origin + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos, color);

            //draw pin
            Debug.DrawLine(crossMinPos, origin + direction, color);
            Debug.DrawLine(crossMaxPos, origin + direction, color);

        }

        /// <summary>
        /// draw an arrow from point start to end, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="end">end pos</param>
        public static void DebugDrawArrowLine(Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos);

            //draw pin
            Debug.DrawLine(crossMinPos, end);
            Debug.DrawLine(crossMaxPos, end);

        }

        /// <summary>
        /// draw an arrow from point start to end, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="end">end pos</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrowLine(Vector2 start, Vector2 end, Color color)
        {
            Vector2 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos, color);

            //draw pin
            Debug.DrawLine(crossMinPos, end, color);
            Debug.DrawLine(crossMaxPos, end, color);

        }

        /// <summary>
        /// draw an arrow from point start to end, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="end">end pos</param>
        /// <param name="color">color of arrow</param>
        /// <param name="arrowSize">between 0 and 1</param>
        public static void DebugDrawArrowLine(Vector2 start, Vector2 end, Color color, float arrowSize)
        {
            Vector2 direction = end - start;
            arrowSize = direction.magnitude * arrowSize;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos, color);

            //draw pin
            Debug.DrawLine(crossMinPos, end, color);
            Debug.DrawLine(crossMaxPos, end, color);

        }

        #endregion

        #region Debug Points

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        public static void DebugDrawPoint(Vector3 position)
        {
            float size = .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
            Debug.DrawLine(position - forward, position + forward);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        public static void DebugDrawPoint(Vector3 position, Color color)
        {
            float size = .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
            Debug.DrawLine(position - forward, position + forward, color);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector3 position, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, Camera.current.transform.position) * .025f : .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
            Debug.DrawLine(position - forward, position + forward);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector3 position, Color color, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, Camera.current.transform.position) * .025f : .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
            Debug.DrawLine(position - forward, position + forward, color);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        public static void DebugDrawPoint(Vector2 position)
        {
            float size = .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        public static void DebugDrawPoint(Vector2 position, Color color)
        {
            float size = .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector2 position, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, new Vector3(position.x, position.y, Camera.current.transform.position.z)) * .025f : .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector2 position, Color color, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, new Vector3(position.x, position.y, Camera.current.transform.position.z)) * .025f : .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
        }

        #endregion

        #region DrawGrid

        #region 2DOverrides

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCount, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="localFrom">what is the object representing right, up, and forward axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCount, Transform localFrom, Color gridColor)
        {
            Vector3 right = localFrom.right;
            Vector3 up = localFrom.up;
            Vector3 forward = Vector3.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="scale">what are the scale of the three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector3 scale, Color gridColor)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward * scale.z;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="scale">what are the scale of the two axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector2 scale, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector3 right, Vector3 up, Color gridColor)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, Vector3.forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, Vector3.forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector3 right, Vector3 up, Color gridColor, PositionRendredType positionType)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, Vector3.forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, Vector3.forward, gridColor, positionType);
        }

        #endregion

        #region DrawGrid3D

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCount, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="localFrom">what is the object representing right, up, and forward axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCount, Transform localFrom, Color gridColor)
        {
            Vector3 right = localFrom.right;
            Vector3 up = localFrom.up;
            Vector3 forward = localFrom.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, positionType);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="scale">what are the scale of the three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 scale, Color gridColor)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward * scale.z;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="scale">what are the scale of the three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 scale, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward * scale.z;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, positionType);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="forward">represent z axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 right, Vector3 up, Vector3 forward, Color gridColor)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="forward">represent z axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 right, Vector3 up, Vector3 forward, Color gridColor, PositionRendredType positionType)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, positionType);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, positionType);
        }

        #endregion

        /// <summary>
        /// take given parameters, and draw a section of lines in the axisToDraw direction
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountToDraw">how many edges are rendered in to draw axis ?</param>
        /// <param name="edgeCountOne">how many edges are rendered in first axis ?</param>
        /// <param name="edgeCountTwo">how many edges are rendered in second axis ?</param>
        /// <param name="axisToDraw">axis to draw towards</param>
        /// <param name="axisOne">first axis reprensenting second dimention axis</param>
        /// <param name="axisTwo">second axis reprensenting third dimention axis</param>
        /// <param name="gridColor"></param>
        /// <param name="positionType"></param>
        public static void DrawLineSection(Vector3 origin, int edgeCountToDraw, int edgeCountOne, int edgeCountTwo, Vector3 axisToDraw, Vector3 axisOne, Vector3 axisTwo, Color gridColor, PositionRendredType positionType)
        {
            Vector3 offset = TestGridOffsetType(edgeCountToDraw, edgeCountOne, edgeCountTwo, axisToDraw, axisOne, axisTwo, positionType);

            for (int i = 0; i < edgeCountOne + 1; i++)
            {
                for (int j = 0; j < edgeCountTwo + 1; j++)
                {
                    Vector3 axisToDrawLength = axisToDraw * edgeCountToDraw;
                    Vector3 lowPoint = origin;
                    Vector3 highPoint = origin + axisToDrawLength;
                    Vector3 firstAxisILength = axisOne * i;
                    Vector3 secondAxisILength = axisTwo * j;
                    Vector3 firstAndSecondAxisOffset = firstAxisILength + secondAxisILength;
                    Vector3 A = lowPoint + firstAndSecondAxisOffset;
                    Vector3 B = highPoint + firstAndSecondAxisOffset;

                    Debug.DrawLine(A - offset, B - offset, gridColor);
                }
            }
        }

        /// <summary>
        /// take the numbers of count lines to draw, and the third axis, and return the offset depending on the position type
        /// </summary>
        /// <param name="edgeCountToDraw">how many edges are rendered in to draw axis ?</param>
        /// <param name="edgeCountOne">how many edges are rendered in first axis ?</param>
        /// <param name="edgeCountTwo">how many edges are rendered in second axis ?</param>
        /// <param name="axisToDraw">axis to draw towards</param>
        /// <param name="axisOne">first axis reprensenting second dimention axis</param>
        /// <param name="axisTwo">second axis reprensenting third dimention axis</param>
        /// <param name="positionType"></param>
        /// <returns></returns>
        private static Vector3 TestGridOffsetType(int edgeCountToDraw, int edgeCountOne, int edgeCountTwo, Vector3 axisToDraw, Vector3 axisOne, Vector3 axisTwo, PositionRendredType positionType)
        {
            if (positionType == PositionRendredType.IntCentered)
            {
                Vector3 toDrawOffset = axisToDraw * (edgeCountToDraw / 2);
                Vector3 firstOffset = axisOne * (edgeCountOne / 2);
                Vector3 secondOffset = axisTwo * (edgeCountTwo / 2);

                return toDrawOffset + firstOffset + secondOffset;
            }
            if (positionType == PositionRendredType.Edged)
            {
                return Vector3.zero;
            }
            if (positionType == PositionRendredType.Centered)
            {
                Vector3 toDrawOffset = axisToDraw * (edgeCountToDraw / 2f);
                Vector3 firstOffset = axisOne * (edgeCountOne / 2f);
                Vector3 secondOffset = axisTwo * (edgeCountTwo / 2f);

                return toDrawOffset + firstOffset + secondOffset;
            }

            Debug.LogError("EXCEPTION : grid offset type takes a position type that doesn't exist !");
            return Vector3.zero;
        }

        #endregion

        #region DrawCube

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="right">right face of cube direction</param>
        /// <param name="up">up face of cube direction</param>
        /// <param name="forward">forward face of cube direction</param>
        /// <param name="color">color of drawn cube</param>
        /// <param name="sideWireOne">draw first side wires</param>
        /// <param name="sideWireTwo">draw second side wires</param>
        /// <param name="internalWireOne">draw first inside the cube wire</param>
        /// <param name="internalWireTwo">draw second inside the cube wire</param>
        /// <param name="internalWireThree">draw third inside the cube wire</param>
        /// <param name="internalWireFour">draw fourth inside the cube wire</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Vector3 right, Vector3 up, Vector3 forward, Color color, bool sideWireOne, bool sideWireTwo, bool internalWireOne, bool internalWireTwo, bool internalWireThree, bool internalWireFour)
        {
            Vector3 halfScale = scale / 2;

            Vector3 localPointA = new Vector3(-halfScale.x, -halfScale.y, -halfScale.z);
            Vector3 localPointB = new Vector3(+halfScale.x, -halfScale.y, -halfScale.z);
            Vector3 localPointC = new Vector3(-halfScale.x, -halfScale.y, +halfScale.z);
            Vector3 localPointD = new Vector3(+halfScale.x, -halfScale.y, +halfScale.z);
            Vector3 localPointE = new Vector3(-halfScale.x, +halfScale.y, -halfScale.z);
            Vector3 localPointF = new Vector3(+halfScale.x, +halfScale.y, -halfScale.z);
            Vector3 localPointG = new Vector3(-halfScale.x, +halfScale.y, +halfScale.z);
            Vector3 localPointH = new Vector3(+halfScale.x, +halfScale.y, +halfScale.z);

            Vector3 globalPointA = Point3LocalToWorld(localPointA, position, right, up, forward);
            Vector3 globalPointB = Point3LocalToWorld(localPointB, position, right, up, forward);
            Vector3 globalPointC = Point3LocalToWorld(localPointC, position, right, up, forward);
            Vector3 globalPointD = Point3LocalToWorld(localPointD, position, right, up, forward);
            Vector3 globalPointE = Point3LocalToWorld(localPointE, position, right, up, forward);
            Vector3 globalPointF = Point3LocalToWorld(localPointF, position, right, up, forward);
            Vector3 globalPointG = Point3LocalToWorld(localPointG, position, right, up, forward);
            Vector3 globalPointH = Point3LocalToWorld(localPointH, position, right, up, forward);

            Debug.DrawLine(globalPointA, globalPointB, color);
            Debug.DrawLine(globalPointA, globalPointC, color);
            Debug.DrawLine(globalPointB, globalPointD, color);
            Debug.DrawLine(globalPointC, globalPointD, color);
            Debug.DrawLine(globalPointE, globalPointF, color);
            Debug.DrawLine(globalPointE, globalPointG, color);
            Debug.DrawLine(globalPointF, globalPointH, color);
            Debug.DrawLine(globalPointG, globalPointH, color);
            Debug.DrawLine(globalPointA, globalPointE, color);
            Debug.DrawLine(globalPointB, globalPointF, color);
            Debug.DrawLine(globalPointC, globalPointG, color);
            Debug.DrawLine(globalPointD, globalPointH, color);

            if (sideWireOne)
            {
                Debug.DrawLine(globalPointB, globalPointC, color);
                Debug.DrawLine(globalPointE, globalPointH, color);
                Debug.DrawLine(globalPointB, globalPointH, color);
                Debug.DrawLine(globalPointC, globalPointE, color);
                Debug.DrawLine(globalPointA, globalPointF, color);
                Debug.DrawLine(globalPointD, globalPointG, color);
            }

            if (sideWireTwo)
            {
                Debug.DrawLine(globalPointA, globalPointD, color);
                Debug.DrawLine(globalPointF, globalPointG, color);
                Debug.DrawLine(globalPointF, globalPointD, color);
                Debug.DrawLine(globalPointA, globalPointG, color);
                Debug.DrawLine(globalPointB, globalPointE, color);
                Debug.DrawLine(globalPointC, globalPointH, color);
            }

            if (internalWireOne)
                Debug.DrawLine(globalPointA, globalPointH, color);

            if (internalWireTwo)
                Debug.DrawLine(globalPointB, globalPointG, color);

            if (internalWireThree)
                Debug.DrawLine(globalPointC, globalPointF, color);

            if (internalWireFour)
                Debug.DrawLine(globalPointD, globalPointE, color);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="right">right face of cube direction</param>
        /// <param name="up">up face of cube direction</param>
        /// <param name="forward">forward face of cube direction</param>
        /// <param name="color">color of drawn cube</param>
        /// <param name="sideWireOne">draw first side wires</param>
        /// <param name="sideWireTwo">draw second side wires</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Vector3 right, Vector3 up, Vector3 forward, Color color, bool sideWireOne, bool sideWireTwo)
        {
            DebugDrawCube(position, scale, right, up, forward, color, sideWireOne, sideWireTwo, false, false, false, false);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="right">right face of cube direction</param>
        /// <param name="up">up face of cube direction</param>
        /// <param name="forward">forward face of cube direction</param>
        /// <param name="color">color of drawn cube</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Vector3 right, Vector3 up, Vector3 forward, Color color)
        {
            DebugDrawCube(position, scale, right, up, forward, color, false, false, false, false, false, false);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="right">right face of cube direction</param>
        /// <param name="up">up face of cube direction</param>
        /// <param name="forward">forward face of cube direction</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Vector3 right, Vector3 up, Vector3 forward)
        {
            DebugDrawCube(position, scale, right, up, forward, Color.white, false, false, false, false, false, false);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="spaceRefTransform">transform that gives cube direction</param>
        /// <param name="sideWireOne">draw first side wires</param>
        /// <param name="sideWireTwo">draw second side wires</param>
        /// <param name="color">color of drawn cube</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Transform spaceRefTransform, Color color, bool sideWireOne, bool sideWireTwo)
        {
            DebugDrawCube(position, scale, spaceRefTransform.right, spaceRefTransform.up, spaceRefTransform.forward, color, sideWireOne, sideWireTwo, false, false, false, false);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="spaceRefTransform">transform that gives cube direction</param>
        /// <param name="color">color of drawn cube</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Transform spaceRefTransform, Color color)
        {
            DebugDrawCube(position, scale, spaceRefTransform.right, spaceRefTransform.up, spaceRefTransform.forward, color, false, false, false, false, false, false);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="spaceRefTransform">transform that gives cube direction</param>
        public static void DebugDrawCube(Vector3 position, Vector3 scale, Transform spaceRefTransform)
        {
            DebugDrawCube(position, scale, spaceRefTransform.right, spaceRefTransform.up, spaceRefTransform.forward, Color.white, false, false, false, false, false, false);
        }

        /// <summary>
        /// draw a cube, but allow user to face it the side he wants
        /// </summary>
        /// <param name="position">position of cube center</param>
        /// <param name="scale">scale of cube, length of each edges</param>
        /// <param name="right">right face of cube direction</param>
        /// <param name="up">up face of cube direction</param>
        /// <param name="forward">forward face of cube direction</param>
        /// <param name="color">color of drawn cube</param>
        public static void DebugDrawMinMaxCube(Vector3 posMin, Vector3 posMax, Vector3 right, Vector3 up, Vector3 forward, Color color)
        {
            Vector3 scale = posMax - posMin;
            Vector3 position = Vector3.Lerp(posMin, posMax, 0.5f);
            DebugDrawCube(position, scale, right, up, forward, color, false, false, false, false, false, false);
        }

        #endregion

        #region DrawCone

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="scale">scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="ringVerticeNumber">number of vertice to make the base, and all the lines linked up to the center</param>
        /// <param name="loopVerticeNumber">number of vertice that forms the "circles" arround the sphere part</param>
        /// <param name="drawRings">draw ring edges(do not include base)</param>
        /// <param name="drawLoops">draw loops edges(do not include the first loop for base circle)</param>
        /// <param name="drawBase">draw base(if not, it will draw up to 3 edges)</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, Vector3 scale, float length, float angle, Color color, int ringVerticeNumber, int loopVerticeNumber, bool drawRings, bool drawLoops, bool drawBase)
        {
            if (loopVerticeNumber <= 0 || ringVerticeNumber <= 0)
                return;

            angle = Mathf.Clamp(angle, 0, 180);

            List<List<Vector3>> ringBaseVertices = new List<List<Vector3>>();

            float angleToSet = angle;

            //register all vertices to use
            for (int j = 0; j < loopVerticeNumber; j++)
            {
                Vector3 flattenA = forward;
                ringBaseVertices.Add(new List<Vector3>());

                for (int i = 0; i < ringVerticeNumber; i++)
                {
                    Vector3 vertex = GetConeBaseVertex(flattenA, angleToSet);
                    Vector3 localVertex = Point3WorldToLocal(vertex, position, right, up, forward);
                    Vector3 scaledLocalVertex = new Vector3(localVertex.x * scale.x, localVertex.y * scale.y, localVertex.z * scale.z);
                    Vector3 scaledVertex = Point3LocalToWorld(scaledLocalVertex, position, right, up, forward);

                    ringBaseVertices[j].Add(scaledVertex);

                    flattenA = RotateVector(flattenA, up, 360f / (float)ringVerticeNumber);
                }

                angleToSet -= angle / (float)loopVerticeNumber;
            }

            //link base ring edges
            if (drawBase || ringBaseVertices[0].Count < 3)
            {
                for (int i = 0; i < ringBaseVertices[0].Count; i++)
                    Debug.DrawLine(position, ringBaseVertices[0][i], color);
            }
            else
            {
                int oneThird = ringBaseVertices[0].Count / 3;

                for (int i = 0; i < 3; i++)
                    Debug.DrawLine(position, ringBaseVertices[0][(oneThird * (i + 1)) - 1], color);
            }

            if (drawRings)
            {
                //link core ring edges
                for (int j = 0; j < ringBaseVertices.Count - 1; j++)
                    for (int i = 0; i < ringBaseVertices[j].Count; i++)
                        Debug.DrawLine(ringBaseVertices[j][i], ringBaseVertices[j + 1][i], color);

                //link last ring to center of sphere
                Vector3 upPosRanged = position + (up * length * scale.y);
                int lastIndex = ringBaseVertices.Count - 1;

                for (int i = 0; i < ringBaseVertices[lastIndex].Count; i++)
                    Debug.DrawLine(ringBaseVertices[lastIndex][i], upPosRanged, color);
            }

            //link loop edges
            for (int j = 0; j < (drawLoops ? ringBaseVertices.Count : Mathf.Clamp(ringBaseVertices.Count, 0, 1)); j++)
                for (int i = 0; i < ringBaseVertices[j].Count; i++)
                    Debug.DrawLine(ringBaseVertices[j][i], ringBaseVertices[j][UPDBMath.LoopClamp(i + 1, 0, ringBaseVertices[j].Count - 1)], color);


            Vector3 GetConeBaseVertex(Vector3 forward, float angle)
            {
                return position + (RotateVector(up, forward, angle).normalized * length);
            }
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="scale">scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="ringVerticeNumber">number of vertice to make the base, and all the lines linked up to the center</param>
        /// <param name="loopVerticeNumber">number of vertice that forms the "circles" arround the sphere part</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, Vector3 scale, float length, float angle, Color color, int ringVerticeNumber, int loopVerticeNumber)
        {
            DebugDrawConeSphere(position, forward, up, right, scale, length, angle, color, ringVerticeNumber, loopVerticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="ringVerticeNumber">number of vertice to make the base, and all the lines linked up to the center</param>
        /// <param name="loopVerticeNumber">number of vertice that forms the "circles" arround the sphere part</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, float length, float angle, Color color, int ringVerticeNumber, int loopVerticeNumber)
        {
            DebugDrawConeSphere(position, forward, up, right, Vector3.one, length, angle, color, ringVerticeNumber, loopVerticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="transform">transform that give rotation and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="ringVerticeNumber">number of vertice to make the base, and all the lines linked up to the center</param>
        /// <param name="loopVerticeNumber">number of vertice that forms the "circles" arround the sphere part</param>
        public static void DebugDrawConeSphere(Vector3 position, Transform transform, float length, float angle, Color color, int ringVerticeNumber, int loopVerticeNumber)
        {
            DebugDrawConeSphere(position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, color, ringVerticeNumber, loopVerticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="transform">transform that give position, rotation, and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="ringVerticeNumber">number of vertice to make the base, and all the lines linked up to the center</param>
        /// <param name="loopVerticeNumber">number of vertice that forms the "circles" arround the sphere part</param>
        public static void DebugDrawConeSphere(Transform transform, float length, float angle, Color color, int ringVerticeNumber, int loopVerticeNumber)
        {
            DebugDrawConeSphere(transform.position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, color, ringVerticeNumber, loopVerticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="scale">scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="verticeNumber">number of vertice to make the cone and sphere</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, Vector3 scale, float length, float angle, Color color, int verticeNumber)
        {
            DebugDrawConeSphere(position, forward, up, right, scale, length, angle, color, verticeNumber, verticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="verticeNumber">number of vertice to make the cone and sphere</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, float length, float angle, Color color, int verticeNumber)
        {
            DebugDrawConeSphere(position, forward, up, right, Vector3.one, length, angle, color, verticeNumber, verticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="transform">transform that give rotation and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="verticeNumber">number of vertice to make the cone and sphere</param>
        public static void DebugDrawConeSphere(Vector3 position, Transform transform, float length, float angle, Color color, int verticeNumber)
        {
            DebugDrawConeSphere(position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, color, verticeNumber, verticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="transform">transform that give position, rotation, and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        /// <param name="verticeNumber">number of vertice to make the cone and sphere</param>
        public static void DebugDrawConeSphere(Transform transform, float length, float angle, Color color, int verticeNumber)
        {
            DebugDrawConeSphere(transform.position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, color, verticeNumber, verticeNumber, true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="scale">scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, Vector3 scale, float length, float angle, Color color)
        {
            DebugDrawConeSphere(position, forward, up, right, scale, length, angle, color, 20, Mathf.RoundToInt(Mathf.Clamp(angle, 0, 180) / 10), true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="forward">forward of virtual transform of cone</param>
        /// <param name="up">up of virtual transform of cone</param>
        /// <param name="right">right of virtual transform of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        public static void DebugDrawConeSphere(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, float length, float angle, Color color)
        {
            DebugDrawConeSphere(position, forward, up, right, Vector3.one, length, angle, color, 20, Mathf.RoundToInt(Mathf.Clamp(angle, 0, 180) / 10), true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="position">position of the circle center</param>
        /// <param name="transform">transform that give rotation and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        public static void DebugDrawConeSphere(Vector3 position, Transform transform, float length, float angle, Color color)
        {
            DebugDrawConeSphere(position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, color, 20, Mathf.RoundToInt(Mathf.Clamp(angle, 0, 180) / 10), true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="transform">transform that give position, rotation, and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        /// <param name="color">color of cone</param>
        public static void DebugDrawConeSphere(Transform transform, float length, float angle, Color color)
        {
            DebugDrawConeSphere(transform.position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, color, 20, Mathf.RoundToInt(Mathf.Clamp(angle, 0, 180) / 10), true, true, true);
        }

        /// <summary>
        /// draw a sphere cone, so a cone with it's base made with a part of sphere that has the sommet of the cone for center
        /// </summary>
        /// <param name="transform">transform that give position, rotation, and scale of cone</param>
        /// <param name="length">basically the radius of circle, or the height of the cone</param>
        /// <param name="angle">the angle between the center and the border of cone</param>
        public static void DebugDrawConeSphere(Transform transform, float length, float angle)
        {
            DebugDrawConeSphere(transform.position, transform.forward, transform.up, transform.right, transform.localScale, length, angle, Color.white, 20, Mathf.RoundToInt(Mathf.Clamp(angle, 0, 180) / 10), true, true, true);
        }

        #endregion

        #region DrawCustomShape

        /// <summary>
        /// draw a custom wire shape with given vertice and edges
        /// </summary>
        /// <param name="vertice"></param>
        /// <param name="edges"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="right"></param>
        /// <param name="up"></param>
        /// <param name="forward"></param>
        /// <param name="color"></param>
        public static void DebugDrawWireShape(Vector3[] vertice, Vector2Int[] edges, Vector3 position, Vector3 scale, Vector3 right, Vector3 up, Vector3 forward, Color color)
        {
            if (vertice.Length == 0 || edges.Length == 0)
                return;

            for (int i = 0; i < edges.Length; i++)
            {
                if (edges[i].x < 0 || edges[i].x >= vertice.Length || edges[i].y < 0 || edges[i].y >= vertice.Length)
                    continue;

                Debug.DrawLine(Point3LocalToWorld(vertice[edges[i].x], position, right * scale.x, up * scale.y, forward * scale.z), Point3LocalToWorld(vertice[edges[i].y], position, right * scale.x, up * scale.y, forward * scale.z), color);
            }
        }

        /// <summary>
        /// draw a custom wire shape with given vertice and edges
        /// </summary>
        /// <param name="vertice"></param>
        /// <param name="edges"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="right"></param>
        /// <param name="up"></param>
        /// <param name="forward"></param>
        public static void DebugDrawWireShape(Vector3[] vertice, Vector2Int[] edges, Vector3 position, Vector3 scale, Vector3 right, Vector3 up, Vector3 forward)
        {
            DebugDrawWireShape(vertice, edges, position, scale, right, up, forward, Color.yellow);
        }

        /// <summary>
        /// draw a custom wire shape with given vertice and edges
        /// </summary>
        /// <param name="vertice"></param>
        /// <param name="edges"></param>
        /// <param name="spaceRelative"></param>
        /// <param name="color"></param>
        public static void DebugDrawWireShape(Vector3[] vertice, Vector2Int[] edges, Transform spaceRelative, Color color)
        {
            DebugDrawWireShape(vertice, edges, spaceRelative.position, spaceRelative.localScale, spaceRelative.right, spaceRelative.up, spaceRelative.forward, color);
        }

        /// <summary>
        /// draw a custom wire shape with given vertice and edges
        /// </summary>
        /// <param name="vertice"></param>
        /// <param name="edges"></param>
        /// <param name="spaceRelative"></param>
        public static void DebugDrawWireShape(Vector3[] vertice, Vector2Int[] edges, Transform spaceRelative)
        {
            DebugDrawWireShape(vertice, edges, spaceRelative.position, spaceRelative.localScale, spaceRelative.right, spaceRelative.up, spaceRelative.forward, Color.yellow);
        }

        /// <summary>
        /// draw a custom wire shape with given vertice and edges
        /// </summary>
        /// <param name="vertice"></param>
        /// <param name="edges"></param>
        /// <param name="color"></param>
        public static void DebugDrawWireShape(Vector3[] vertice, Vector2Int[] edges, Color color)
        {
            DebugDrawWireShape(vertice, edges, Vector3.zero, Vector3.one, Vector3.right, Vector3.up, Vector3.forward, color);
        }

        /// <summary>
        /// draw a custom wire shape with given vertice and edges
        /// </summary>
        /// <param name="vertice"></param>
        /// <param name="edges"></param>
        public static void DebugDrawWireShape(Vector3[] vertice, Vector2Int[] edges)
        {
            DebugDrawWireShape(vertice, edges, Vector3.zero, Vector3.one, Vector3.right, Vector3.up, Vector3.forward, Color.yellow);
        }

        #endregion

        #endregion


        //OTHERS

        #region Binary Conversions

        /// <summary>
        /// convert a given binary input into decimal, from string to float, using BitArray for calculations
        /// </summary>
        /// <param name="input">binary input</param>
        /// <returns>decimal output</returns>
        public static float BinaryToDecimal(string input)
        {
            string allocatedMemoryKey = "32bit";

            if (!DictionaryLib.BitAllocatedMemory.TryGetValue(allocatedMemoryKey, out long allocatedMemoryTry))
            {
                Debug.LogError($"error : no value named {allocatedMemoryKey} found in {nameof(DictionaryLib.BitAllocatedMemory)}");
                return 0;
            }

            int allocatedMemory = (int)allocatedMemoryTry;

            //get a BitArray out of binary input
            BitArray inputRaw = BinaryInputToBitArray(input, allocatedMemoryKey);

            float output = 0f;

            for (int i = 1; i < inputRaw.Length; i++)
            {
                if (!inputRaw[i])
                    continue;

                int e = (allocatedMemory / 2) - i;

                output += Mathf.Pow(2f, e);
            }

            return output;
        }

        /// <summary>
        /// convert a binary input given in string into a real BitArray binary input
        /// </summary>
        /// <param name="input">binary input in string</param>
        /// <param name="allocatedMemoryKey">allocated memory bits</param>
        /// <returns>bit array of binary input</returns>
        public static BitArray BinaryInputToBitArray(string input, string allocatedMemoryKey)
        {
            if (!DictionaryLib.BitAllocatedMemory.TryGetValue(allocatedMemoryKey, out long allocatedMemoryTry))
            {
                Debug.LogError($"error : no value named {allocatedMemoryKey} found in {nameof(DictionaryLib.BitAllocatedMemory)}");
                return new BitArray(0, false);
            }

            int allocatedMemory = (int)allocatedMemoryTry;

            BitArray convertedInput = new BitArray(allocatedMemory, false);

            if (input == string.Empty)
                return convertedInput;

            bool isNegative = input[0] == DictionaryLib.BaseNumerationAPIReader["Substractor"];
            convertedInput.Set(0, isNegative);
            string normalizedInput = input;

            //get positive or negative informations and remove the - symbol
            if (isNegative)
            {
                normalizedInput = string.Empty;

                for (int i = 1; i < input.Length; i++)
                    normalizedInput += input[i];
            }

            string integerPart = string.Empty;
            string decimalPart = string.Empty;

            //isolate integer part
            int j = 0;

            for (int i = 0; i < normalizedInput.Length; i++)
            {
                if (normalizedInput[i] == DictionaryLib.BaseNumerationAPIReader["Separator"] || normalizedInput[i] == DictionaryLib.BaseNumerationAPIReader["AlternativeSeparator"])
                {
                    if (i == 0)
                        integerPart += DictionaryLib.BaseNumerationCaractersList.KeyByValue(0);

                    j = i + 1;
                    break;
                }

                integerPart += normalizedInput[i];
                j = i + 1;
            }

            //isolate decimal part(fraction part)
            int init = j;
            for (j = init; j < normalizedInput.Length; j++)
            {
                decimalPart += normalizedInput[j];

                if (decimalPart.Length >= (allocatedMemory / 2) - 1)
                    break;
            }

            //remove useless 0 of integer part
            string shortenIntegerPart = string.Empty;

            for (int i = 0; i < integerPart.Length; i++)
            {
                if (ConvertCharToBaseNumerationValue(integerPart[i]) != 0 || i == integerPart.Length - 1)
                {
                    shortenIntegerPart += integerPart[i];
                    continue;
                }

                if (shortenIntegerPart != string.Empty)
                    shortenIntegerPart += integerPart[i];

                if (shortenIntegerPart.Length >= allocatedMemory / 2)
                    break;
            }

            //remove useless 0 of decimal part
            string invertedShortenDecimalPart = string.Empty;

            for (int i = decimalPart.Length - 1; i >= 0; i--)
            {
                if (ConvertCharToBaseNumerationValue(decimalPart[i]) != 0)
                {
                    invertedShortenDecimalPart += decimalPart[i];
                    continue;
                }

                if (invertedShortenDecimalPart != string.Empty)
                    invertedShortenDecimalPart += decimalPart[i];
            }

            string shortenDecimalPart = string.Empty;

            for (int i = invertedShortenDecimalPart.Length - 1; i >= 0; i--)
                shortenDecimalPart += invertedShortenDecimalPart[i];


            //add integer part to bitArray
            for (int i = allocatedMemory / 2; i > 0; i--)
            {
                int integerPartIndex = (i - 1) - ((allocatedMemory / 2) - shortenIntegerPart.Length);

                if (integerPartIndex < 0)
                    break;

                convertedInput.Set(i, ConvertCharToBaseNumerationValue(shortenIntegerPart[integerPartIndex]) != 0);
            }

            //add decimal part to bitArray
            for (int i = (allocatedMemory / 2) + 1; i < convertedInput.Length; i++)
            {
                int decimalPartIndex = i - ((allocatedMemory / 2) + 1);

                if (decimalPartIndex >= shortenDecimalPart.Length)
                    break;

                convertedInput.Set(i, ConvertCharToBaseNumerationValue(shortenDecimalPart[decimalPartIndex]) != 0);
            }

            return convertedInput;
        }

        /// <summary>
        /// convert a given decimal input into binary, from float to string, using BitArray for calculations
        /// </summary>
        /// <param name="input">decimal input in float</param>
        /// <returns>binary output</returns>
        public static string DecimalToBinary(float input)
        {
            string allocatedMemoryKey = "32bit";

            BitArray bitArrayOutput = DecimalInputToBitArray(input, allocatedMemoryKey);

            string output = BitArrayToBinaryOutput(bitArrayOutput);

            return output;
        }

        /// <summary>
        /// convert a decimal input into a bit array binary output
        /// </summary>
        /// <param name="input">decimal input in float</param>
        /// <param name="allocatedMemoryKey">allocated memory bits</param>
        /// <returns>bit array of converted decimal(to binary)</returns>
        public static BitArray DecimalInputToBitArray(float input, string allocatedMemoryKey)
        {
            if (!DictionaryLib.BitAllocatedMemory.TryGetValue(allocatedMemoryKey, out long allocatedMemoryTry))
            {
                Debug.LogError($"error : no value named {allocatedMemoryKey} found in {nameof(DictionaryLib.BitAllocatedMemory)}");
                return new BitArray(0, false);
            }

            int allocatedMemory = (int)allocatedMemoryTry;

            BitArray convertedInput = new BitArray(allocatedMemory, false);

            //set value of negative/positive in bitArray
            convertedInput.Set(0, input < 0);

            //initialize values
            float normalizedInput = Mathf.Abs(input);
            int integerPart = Mathf.FloorToInt(normalizedInput);
            float decimalPart = normalizedInput - integerPart;

            //get binary value of integer part
            string reversedIntegerValue = string.Empty;

            do
            {
                reversedIntegerValue += DictionaryLib.BaseNumerationCaractersList.KeyByValue(integerPart % 2);
                integerPart = integerPart / 2;
            } while (integerPart > 0);

            string formatedIntegerValue = string.Empty;

            for (int i = reversedIntegerValue.Length - 1; i >= 0; i--)
            {
                formatedIntegerValue += reversedIntegerValue[i];

                if (formatedIntegerValue.Length >= allocatedMemory / 2)
                    break;
            }

            //add integer part to BitArray
            for (int i = allocatedMemory / 2, j = formatedIntegerValue.Length - 1; i > 0 && j >= 0; i--, j--)
            {
                convertedInput.Set(i, ConvertCharToBaseNumerationValue(formatedIntegerValue[j]) != 0);
            }

            for (int i = (allocatedMemory / 2) + 1; i < convertedInput.Length; i++)
            {
                decimalPart *= 2f;
                int flooredValue = Mathf.FloorToInt(decimalPart);
                decimalPart -= flooredValue;

                convertedInput.Set(i, flooredValue != 0);
            }

            return convertedInput;
        }

        /// <summary>
        /// convert a bitArray binary input into a readable and displayable string output
        /// </summary>
        /// <param name="input">bit array binary input</param>
        /// <returns>binary output(string)</returns>
        public static string BitArrayToBinaryOutput(BitArray input)
        {
            if (!UPDBMath.IsPowerOf(input.Length, 2))
            {
                Debug.LogError("error : given BitArray doesn't fit the size for fixed decimal render, please insert an array with a length that is a power of 2");
                return string.Empty;
            }

            string reversedOutput = string.Empty;

            for (int i = input.Length - 1; i > 0; i--)
            {
                if (i == (input.Length / 2))
                    reversedOutput += DictionaryLib.BaseNumerationAPIReader["Separator"];

                if (!input[i] && reversedOutput == string.Empty)
                    continue;

                reversedOutput += input[i] ? DictionaryLib.BaseNumerationCaractersList.KeyByValue(1) : DictionaryLib.BaseNumerationCaractersList.KeyByValue(0);

            }

            string output = string.Empty;
            int endIndex = reversedOutput[0] == DictionaryLib.BaseNumerationAPIReader["Separator"] ? 1 : 0;

            for (int i = reversedOutput.Length - 1; i >= endIndex; i--)
            {
                if (i > endIndex && ConvertCharToBaseNumerationValue(reversedOutput[i]) == 0 && output == string.Empty)
                    continue;

                output += reversedOutput[i];
            }

            string substractor = input[0] ? DictionaryLib.BaseNumerationAPIReader["Substractor"].ToString() : string.Empty;
            output = substractor + output;

            return output;
        }

        /// <summary>
        /// take a char and return it's int value of baseNumeration dictionnary, this is used if you want, for example, the 1 character, to not be taken as a 1 value
        /// </summary>
        /// <param name="input">character</param>
        /// <returns>value of char in dictionnary</returns>
        public static int ConvertCharToBaseNumerationValue(char input)
        {
            if (DictionaryLib.BaseNumerationCaractersList.TryGetValue(input, out int output))
                return output;

            return -1;
        }

        #endregion

        #region RayIntersectionCalculations

        /// <summary>
        /// take a ray in arguments, and a height, and return if the ray intersect the height at a certain point, and if true, calculate the intersection direction multiplier t
        /// </summary>
        /// <param name="origin">origin of ray</param>
        /// <param name="direction">direction of ray</param>
        /// <param name="h">height to intersect</param>
        /// <param name="t">the intersection direction multiplier, multiply this to direction to get the vector that intersect the height</param>
        /// <returns>if the ray intersect height</returns>
        public static bool FindIntersectionWithHeight(Vector3 origin, Vector3 direction, float h, out float t)
        {
            t = 0f;
            if (direction.y == 0)
            {
                // Si la direction en y est nulle, la ligne est parall�le au plan y = h
                if (origin.y == h)
                {
                    // La ligne est dans le plan y = h, il y a une infinit� de solutions
                    t = 0f; // t peut �tre n'importe quel nombre
                    return true;
                }
                else
                {
                    // La ligne ne traverse jamais y = h
                    return false;
                }
            }

            // Calculer t
            t = (h - origin.y) / direction.y;
            return true;
        }

        /// <summary>
        /// given the t value of direction, calculate the intersection point
        /// </summary>
        /// <param name="origin">origin of ray</param>
        /// <param name="direction">direction of ray</param>
        /// <param name="t">t value of direction calculated by FindIntersectionWithHeight</param>
        /// <returns>get the point of intersection of ray</returns>
        public static Vector3 GetIntersectionPointWithHeight(Vector3 origin, Vector3 direction, float t)
        {
            return origin + t * direction;
        }

        /// <summary>
        /// automatically search for a point, with the two methods above, an return vector3.zero if no intersection found
        /// </summary>
        /// <param name="origin">origin of ray</param>
        /// <param name="direction">direction of ray</param>
        /// <param name="height">height to intersect</param>
        /// <returns>point of ray that intersect with height</returns>
        public static Vector3 GetPointOfVectorWithHeight(Vector3 origin, Vector3 direction, float height)
        {
            if (FindIntersectionWithHeight(origin, direction, height, out float t))
                return GetIntersectionPointWithHeight(origin, direction, t);

            Debug.LogWarning("No intersection found.");
            return Vector3.zero;
        }

        #endregion

        #region Procedural Mesh Drawing

        #region No Saved Vertices

        /// <summary>
        /// create a new serie of vertices and one triangle
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="pos1">pos of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        public static void CreateMeshTriangle(ref List<Vector3> vertices, ref List<int> triangles, Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            vertices.Add(pos1);
            vertices.Add(pos2);
            vertices.Add(pos3);
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);
        }

        /// <summary>
        /// create two new vertices and one triangle attached to the two and one given vertex in list
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        public static void AddMeshTriangleOneVertexLinked(ref List<Vector3> vertices, ref List<int> triangles, int linkedPos1, Vector3 pos2, Vector3 pos3)
        {
            triangles.Add(linkedPos1);

            vertices.Add(pos2);
            vertices.Add(pos3);

            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);
        }

        /// <summary>
        /// create one new vertex and one triangle attached to this vertex and two given vertex in list
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="linkedPos2">index of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        public static void AddMeshTriangleTwoVertexLinked(ref List<Vector3> vertices, ref List<int> triangles, int linkedPos1, int linkedPos2, Vector3 pos3)
        {
            triangles.Add(linkedPos1);
            triangles.Add(linkedPos2);

            vertices.Add(pos3);

            triangles.Add(vertices.Count - 1);
        }

        /// <summary>
        /// create a new triangle attached to three given vertex in list
        /// </summary>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="linkedPos2">index of second vertex</param>
        /// <param name="linkedPos3">index of third vertex</param>
        public static void AddMeshTriangleThreeVertexLinked(ref List<int> triangles, int linkedPos1, int linkedPos2, int linkedPos3)
        {
            triangles.Add(linkedPos1);
            triangles.Add(linkedPos2);
            triangles.Add(linkedPos3);
        }

        /// <summary>
        /// create a new serie of vertices and two triangle to form a quad
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="pos1">pos of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        /// <param name="pos4">pos of fourth vertex</param>
        public static void CreateQuad(ref List<Vector3> vertices, ref List<int> triangles, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
        {
            CreateMeshTriangle(ref vertices, ref triangles, pos1, pos2, pos3);
            AddMeshTriangleTwoVertexLinked(ref vertices, ref triangles, vertices.Count - 1, vertices.Count - 2, pos4);
        }

        /// <summary>
        /// create two new triangle attached to four given vertex in list to form a quad
        /// </summary>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="linkedPos2">index of second vertex</param>
        /// <param name="linkedPos3">index of third vertex</param>
        /// <param name="linkedPos4">index of fourth vertex</param>
        public static void AddQuadFourVerticesLinked(ref List<int> triangles, int linkedPos1, int linkedPos2, int linkedPos3, int linkedPos4)
        {
            triangles.Add(linkedPos1);
            triangles.Add(linkedPos2);
            triangles.Add(linkedPos3);

            triangles.Add(linkedPos2);
            triangles.Add(linkedPos4);
            triangles.Add(linkedPos3);
        }

        /// <summary>
        /// create a new serie of vertices and three triangles to form a pentagone
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="pos1">pos of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        /// <param name="pos4">pos of fourth vertex</param>
        /// <param name="pos5">pos of fifth vertex</param>
        public static void CreatePentagone(ref List<Vector3> vertices, ref List<int> triangles, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, Vector3 pos5)
        {
            CreateMeshTriangle(ref vertices, ref triangles, pos1, pos2, pos3);
            AddMeshTriangleTwoVertexLinked(ref vertices, ref triangles, vertices.Count - 3, vertices.Count - 1, pos4);
            AddMeshTriangleTwoVertexLinked(ref vertices, ref triangles, vertices.Count - 4, vertices.Count - 1, pos5);
        }

        /// <summary>
        /// create three new triangles attached to five given vertex in list to form a pentagone
        /// </summary>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="linkedPos2">index of second vertex</param>
        /// <param name="linkedPos3">index of third vertex</param>
        /// <param name="linkedPos4">index of fourth vertex</param>
        /// <param name="linkedPos5">index of fifth vertex</param>
        public static void AddPentagoneFiveVerticesLinked(ref List<int> triangles, int linkedPos1, int linkedPos2, int linkedPos3, int linkedPos4, int linkedPos5)
        {
            triangles.Add(linkedPos1);
            triangles.Add(linkedPos2);
            triangles.Add(linkedPos3);

            triangles.Add(linkedPos1);
            triangles.Add(linkedPos3);
            triangles.Add(linkedPos4);

            triangles.Add(linkedPos1);
            triangles.Add(linkedPos4);
            triangles.Add(linkedPos5);
        }

        #endregion

        #region Vertices Library Save

        /// <summary>
        /// create a new serie of vertices and one triangle
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="verticesLibrary">vertices library to write on and read from</param>
        /// <param name="pos1">pos of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        public static void CreateMeshTriangle(ref List<Vector3> vertices, ref List<int> triangles, ref Dictionary<Vector3, int> verticesLibrary, Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            vertices.Add(pos1);
            vertices.Add(pos2);
            vertices.Add(pos3);
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            if (verticesLibrary == null)
                verticesLibrary = new Dictionary<Vector3, int>();

            verticesLibrary.Add(pos1, vertices.Count - 3);
            verticesLibrary.Add(pos2, vertices.Count - 2);
            verticesLibrary.Add(pos3, vertices.Count - 1);
        }

        /// <summary>
        /// create two new vertices and one triangle attached to the two and one given vertex in list
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="verticesLibrary">vertices library to write on and read from</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        public static void AddMeshTriangleOneVertexLinked(ref List<Vector3> vertices, ref List<int> triangles, ref Dictionary<Vector3, int> verticesLibrary, int linkedPos1, Vector3 pos2, Vector3 pos3)
        {
            triangles.Add(linkedPos1);

            vertices.Add(pos2);
            vertices.Add(pos3);

            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            if (verticesLibrary == null)
                verticesLibrary = new Dictionary<Vector3, int>();

            verticesLibrary.Add(pos2, vertices.Count - 2);
            verticesLibrary.Add(pos3, vertices.Count - 1);
        }

        /// <summary>
        /// create one new vertex and one triangle attached to this vertex and two given vertex in list
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="verticesLibrary">vertices library to write on and read from</param>
        /// <param name="linkedPos1">index of first vertex</param>
        /// <param name="linkedPos2">index of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        public static void AddMeshTriangleTwoVertexLinked(ref List<Vector3> vertices, ref List<int> triangles, ref Dictionary<Vector3, int> verticesLibrary, int linkedPos1, int linkedPos2, Vector3 pos3)
        {
            triangles.Add(linkedPos1);
            triangles.Add(linkedPos2);

            vertices.Add(pos3);

            triangles.Add(vertices.Count - 1);

            if (verticesLibrary == null)
                verticesLibrary = new Dictionary<Vector3, int>();

            verticesLibrary.Add(pos3, vertices.Count - 1);
        }

        /// <summary>
        /// create a new serie of vertices and two triangle to form a quad
        /// </summary>
        /// <param name="vertices">vertices list to write</param>
        /// <param name="triangles">triangles list to write</param>
        /// <param name="verticesLibrary">vertices library to write on and read from</param>
        /// <param name="pos1">pos of first vertex</param>
        /// <param name="pos2">pos of second vertex</param>
        /// <param name="pos3">pos of third vertex</param>
        /// <param name="pos4">pos of fourth vertex</param>
        public static void CreateQuad(ref List<Vector3> vertices, ref List<int> triangles, ref Dictionary<Vector3, int> verticesLibrary, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
        {
            CreateMeshTriangle(ref vertices, ref triangles, ref verticesLibrary, pos1, pos2, pos3);
            AddMeshTriangleTwoVertexLinked(ref vertices, ref triangles, ref verticesLibrary, vertices.Count - 1, vertices.Count - 2, pos4);
        }

        #endregion

        #endregion

        #region Make Non Nullable

        /// <summary>
        /// make sure value of reference is never null, by searching, and if needed, creating new components
        /// </summary>
        /// <typeparam name="T">type of reference tested</typeparam>
        /// <typeparam name="W">type of object to Add if no object exist</typeparam>
        /// <param name="component">reference to test</param>
        /// <param name="targetObj">object to search in and create component</param>
        /// <param name="isGlobal">is the method searching in all the scene with FindObjectOfType ? use this for unique classes only</param>
        /// <returns>component after assuring it's not null</returns>
        public static T MakeNonNullable<T, W>(ref T component, GameObject targetObj, bool isGlobal) where T : Component where W : T
        {
            //if reference is not null
            if (component)
                return component;

            //if reference is null, but a component exist in the object or scene(depending on args)
            if (targetObj && targetObj.TryGetComponent(out component))
                return component;

            if (isGlobal && TryFindObjectOfType(out component))
                return component;

            //no component exist, method has to generate one
            return component = targetObj ? targetObj.AddComponent<W>() : new GameObject(component.name).AddComponent<W>();
        }

        /// <summary>
        /// make sure value of reference is never null, by searching, and if needed, creating new components
        /// </summary>
        /// <typeparam name="T">type of reference tested</typeparam>
        /// <typeparam name="W">type of object to Add if no object exist</typeparam>
        /// <param name="component">reference to test</param>
        /// <param name="targetObj">object to search in and create component</param>
        /// <returns>component after assuring it's not null</returns>
        public static T MakeNonNullable<T, W>(ref T component, GameObject targetObj) where T : Component where W : T
        {
            return MakeNonNullable<T, W>(ref component, targetObj, false);
        }

        /// <summary>
        /// make sure value of reference is never null, by searching, and if needed, creating new components
        /// </summary>
        /// <typeparam name="T">type of reference tested</typeparam>
        /// <param name="component">reference to test</param>
        /// <param name="targetObj">object to search in and create component</param>
        /// <param name="isGlobal">is the method searching in all the scene with FindObjectOfType ? use this for unique classes only</param>
        /// <returns>component after assuring it's not null</returns>
        public static T MakeNonNullable<T>(ref T component, GameObject targetObj, bool isGlobal) where T : Component
        {
            return MakeNonNullable<T, T>(ref component, targetObj, isGlobal);
        }

        /// <summary>
        /// make sure value of reference is never null, by searching, and if needed, creating new components
        /// </summary>
        /// <typeparam name="T">type of reference tested</typeparam>
        /// <param name="component">reference to test</param>
        /// <param name="targetObj">object to search in and create component</param>
        /// <returns>component after assuring it's not null</returns>
        public static T MakeNonNullable<T>(ref T component, GameObject targetObj) where T : Component
        {
            return MakeNonNullable<T, T>(ref component, targetObj, false);
        }

        #endregion

        #region Free Shape Contain and Drawing Tools

        #region 2D

        /// <summary>
        /// return if a point is inside a custom 2D polygon
        /// </summary>
        /// <param name="position">position to test</param>
        /// <param name="shapeVertice">array of vertices of polygon</param>
        /// <param name="algoUsed">what algorithm is used to determine if point is inside shape</param>
        /// <returns>if position is inside custom shape</returns>
        public static bool Volume2DContain(Vector2 position, Vector2[] shapeVertice, VolumeTestAlgo algoUsed)
        {
            if (shapeVertice.Length == 0)
                return false;

            if (algoUsed == VolumeTestAlgo.WindingNumber)
            {
                int windingNumber = 0;

                for (int i = 0; i < shapeVertice.Length; i++)
                {
                    Vector2 v1 = shapeVertice[i];
                    Vector2 v2 = shapeVertice[(i + 1) % shapeVertice.Length]; // Boucle vers le premier point

                    // Si le segment traverse la ligne horizontale passant par le point
                    if (v1.y <= position.y)
                    {
                        if (v2.y > position.y) // Le segment monte
                            if (IsLeft(v1, v2, position) > 0) // Point � gauche du segment
                                windingNumber++;
                    }
                    else
                    {
                        if (v2.y <= position.y) // Le segment descend
                            if (IsLeft(v1, v2, position) < 0) // Point � gauche du segment
                                windingNumber--;
                    }
                }

                // Si le winding number est diff�rent de 0, le point est dans le polygone
                return windingNumber != 0;
            }

            if (algoUsed == VolumeTestAlgo.BarycentricTest)
            {
                // triangulate polygon(work only with convex shapes)
                Vector2[][] triangles = TriangulatePolygon(shapeVertice);

                for (int i = 0; i < triangles.Length; i++)
                {
                    Vector2 barCoords = position.ToBarycentricCoords(triangles[i][0], triangles[i][1], triangles[i][2]);
                    float sum = barCoords.x + barCoords.y;
                    bool xInside = barCoords.x <= 1 && barCoords.x >= 0;
                    bool yInside = barCoords.y <= 1 && barCoords.y >= 0;

                    if (xInside && yInside && sum >= 0 && sum <= 1)
                        return true;
                }

                return false;
            }

            Debug.LogWarning("warning, please select a valid algorithm to use");
            return false;
        }

        /// <summary>
        /// return if a point is inside a custom 2D polygon
        /// </summary>
        /// <param name="position">position to test</param>
        /// <param name="shapeVertice">array of vertices of polygon</param>
        /// <returns>if position is inside custom shape</returns>
        public static bool Volume2DContain(Vector2 position, Vector2[] shapeVertice)
        {
            return Volume2DContain(position, shapeVertice, VolumeTestAlgo.WindingNumber);
        }

        /// <summary>
        /// return a random point inside a custom volume 2D polygon
        /// </summary>
        /// <param name="shapeVertice">polygon shape</param>
        /// <returns>the generated point inside the polygon</returns>
        public static Vector2 GenerateRandomPosInVolume2D(Vector2[] shapeVertice)
        {
            // triangulate polygon(work only with convex shapes)
            Vector2[][] triangles = TriangulatePolygon(shapeVertice);

            // calculate triangles area
            float[] areas = new float[triangles.Length];
            float totalArea = 0;
            for (int i = 0; i < triangles.Length; i++)
            {
                areas[i] = TriangleArea(triangles[i][0], triangles[i][1], triangles[i][2]);
                totalArea += areas[i];
            }

            // select random triangle ponderated to the area of triangle
            float randomValue = Random.value * totalArea;
            int selectedTriangle = 0;
            float cumulativeArea = 0;

            for (int i = 0; i < triangles.Length; i++)
            {
                cumulativeArea += areas[i];
                if (randomValue <= cumulativeArea)
                {
                    selectedTriangle = i;
                    break;
                }
            }

            // generate a point in barycentric coords within the triangle
            return GeneratePointInTriangle(triangles[selectedTriangle][0], triangles[selectedTriangle][1], triangles[selectedTriangle][2]);
        }

        #region Sub Methods

        /// <summary>
        /// generate a barycentric coord that is always inside triangle without clamping but looping(to avoid weight issues in randomness)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static Vector2 GeneratePointInTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            float u = Random.value;
            float v = Random.value;

            // Assurer que les coordonn�es barycentriques sont valides
            if (u + v > 1)
            {
                u = 1 - u;
                v = 1 - v;
            }

            // Calculer le point � l'int�rieur du triangle
            return (1 - u - v) * a + u * b + v * c;
        }

        /// <summary>
        /// transform polygon shape into a serie of triangles
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Vector2[][] TriangulatePolygon(Vector2[] polygon)
        {
            Vector2[][] triangles = new Vector2[polygon.Length - 2][];

            for (int i = 1; i < polygon.Length - 1; i++)
            {
                triangles[i - 1] = new Vector2[] { polygon[0], polygon[i], polygon[i + 1] };
            }

            return triangles;
        }

        /// <summary>
        /// calculate the area of a triangle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
        {
            return Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
        }

        /// <summary>
        /// get square that circle the shape (with the pattern vector4 : minX, minY, maxX, maxY)
        /// </summary>
        /// <param name="shapeVertice">shape to get square from</param>
        /// <returns>square that circle shape</returns>
        public static Vector4 GetCerclingSquare(Vector2[] shapeVertice)
        {
            if (shapeVertice.Length == 0)
                return Vector4.zero;

            float minX = shapeVertice[0].x;
            float minY = shapeVertice[0].y;
            float maxX = shapeVertice[0].x;
            float maxY = shapeVertice[0].y;

            for (int i = 1; i < shapeVertice.Length; i++)
            {
                if (shapeVertice[i].x < minX)
                    minX = shapeVertice[i].x;
                if (shapeVertice[i].x > maxX)
                    maxX = shapeVertice[i].x;

                if (shapeVertice[i].y < minY)
                    minY = shapeVertice[i].y;
                if (shapeVertice[i].y > maxY)
                    maxY = shapeVertice[i].y;
            }

            return new Vector4(minX, minY, maxX, maxY);
        }

        /// <summary>
        /// check if p is to the left or the right of the line created by a and b
        /// </summary>
        /// <param name="a">a position of line</param>
        /// <param name="b">b position of line</param>
        /// <param name="p">point to test</param>
        /// <returns>the cross product, if superior to 0, point is to the left</returns>
        private static float IsLeft(Vector2 a, Vector2 b, Vector2 p)
        {
            return (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
        }

        #endregion

        #endregion

        #region 3D

        public static bool VolumeContain(Vector3 position, Vector3[] vertice, Vector2Int[] edges)
        {
            Vector3[][] tetrahedronConvertedShape = GenerateTetrahedrons(vertice, edges);


            foreach (Vector3[] tetrahedron in tetrahedronConvertedShape)
            {
                if (TetrahedronContain(position, tetrahedron))
                    return true;
            }

            return false;
        }

        public static Vector3 GenerateRandomPosInVolume(Vector3[] vertice, Vector2Int[] edges)
        {
            Vector3[][] tetrahedronConvertedShape = GenerateTetrahedrons(vertice, edges);

            //calculate tetrahedrons volume
            float[] tetrahedronVolumes = new float[tetrahedronConvertedShape.Length];
            float totalVolume = 0;

            for (int i = 0; i < tetrahedronConvertedShape.Length; i++)
            {
                tetrahedronVolumes[i] = CalculateTetrahedronVolume(tetrahedronConvertedShape[i][0], tetrahedronConvertedShape[i][1], tetrahedronConvertedShape[i][2], tetrahedronConvertedShape[i][3]);
                totalVolume += tetrahedronVolumes[i];
            }

            // select random triangle ponderated to the area of triangle
            float randomValue = Random.value * totalVolume;
            int selectedTetrahedron = 0;
            float cumulativeVolume = 0;

            for (int i = 0; i < tetrahedronConvertedShape.Length; i++)
            {
                cumulativeVolume += tetrahedronVolumes[i];
                if (randomValue <= cumulativeVolume)
                {
                    selectedTetrahedron = i;
                    break;
                }
            }

            return GenerateRandomPosInTetrahedron(tetrahedronConvertedShape[selectedTetrahedron][0], tetrahedronConvertedShape[selectedTetrahedron][1], tetrahedronConvertedShape[selectedTetrahedron][2], tetrahedronConvertedShape[selectedTetrahedron][3]);
        }

        #region Sub Methods

        private static bool TetrahedronContain(Vector3 position, Vector3[] tetrahedronVertice)
        {
            Vector4 coords = position.ToBarycentricCoords(tetrahedronVertice[0], tetrahedronVertice[1], tetrahedronVertice[2], tetrahedronVertice[3]);

            return coords.x >= 0 && coords.y >= 0 && coords.z >= 0 && coords.w >= 0 && coords.x <= 1 && coords.y <= 1 && coords.z <= 1 && coords.w <= 1;
        }

        /// <summary>
        /// check if p is to the left or the right of the line created by a and b
        /// </summary>
        /// <param name="a">a position of line</param>
        /// <param name="b">b position of line</param>
        /// <param name="p">point to test</param>
        /// <returns>the cross product, if superior to 0, point is to the left</returns>
        private static Vector3 IsLeft(Vector3 a, Vector3 b, Vector3 p)
        {
            // Vector from a to b
            Vector3 ab = b - a;

            // Vector from a to p
            Vector3 ap = p - a;

            // Cross product of ab and ap
            return Vector3.Cross(ab, ap);
        }

        /// <summary>
        /// unadviced, generate as many tetrahedrons as possible, high performance consomation
        /// </summary>
        /// <param name="vertices">Liste des sommets de la forme.</param>
        /// <param name="edges">Liste des ar�tes de la forme, repr�sent�es par des indices de sommets.</param>
        /// <returns>Liste des t�tra�dres, chaque t�tra�dre �tant une liste de 4 sommets.</returns>
        private static Vector3[][] ConvertToTetrahedraAlways(Vector3[] vertices, Vector2Int[] edges)
        {
            // Liste pour stocker les t�tra�dres r�sultants
            List<Vector3[]> tetrahedra = new List<Vector3[]>();

            // V�rifie qu'il y a au moins 4 sommets pour former un t�tra�dre
            if (vertices.Length < 4)
            {
                Debug.LogError("La liste de sommets contient moins de 4 points. Impossible de cr�er des t�tra�dres.");
                return tetrahedra.ToArray();
            }

            // Prend le premier point comme sommet de r�f�rence (Vr)
            Vector3 referenceVertex = vertices[0];

            // Parcourt les edges pour trouver des triangles
            for (int i = 0; i < edges.Length; i++)
            {
                for (int j = i + 1; j < edges.Length; j++)
                {
                    // Si les deux edges partagent un sommet, ils forment un triangle
                    int sharedVertex = -1;
                    int unique1 = -1, unique2 = -1;

                    if (edges[i].x == edges[j].x || edges[i].x == edges[j].y)
                        sharedVertex = edges[i].x;
                    else if (edges[i].y == edges[j].x || edges[i].y == edges[j].y)
                        sharedVertex = edges[i].y;

                    // Si un sommet partag� est trouv�
                    if (sharedVertex != -1)
                    {
                        // Identifier les sommets uniques pour le triangle
                        unique1 = edges[i].x == sharedVertex ? edges[i].y : edges[i].x;
                        unique2 = edges[j].x == sharedVertex ? edges[j].y : edges[j].x;

                        // Ajouter un t�tra�dre form� par (Vr, sharedVertex, unique1, unique2)
                        tetrahedra.Add(new Vector3[] { referenceVertex, vertices[sharedVertex], vertices[unique1], vertices[unique2] });
                    }
                }
            }

            return tetrahedra.ToArray();
        }

        /// <summary>
        /// G�n�re des t�tra�dres non-chevauchants � partir d'une liste de sommets et d'ar�tes.
        /// </summary>
        /// <param name="vertices">Liste des sommets de la forme.</param>
        /// <param name="edges">Liste des ar�tes de la forme.</param>
        /// <returns>Liste des t�tra�dres non-chevauchants.</returns>
        public static Vector3[][] GenerateTetrahedrons(Vector3[] vertices, Vector2Int[] edges)
        {
            List<Vector3[]> tetrahedra = new List<Vector3[]>();

            // �tape 1 : Calculer un point central de r�f�rence
            Vector3 center = Vector3.zero;
            foreach (var vertex in vertices)
            {
                center += vertex;
            }

            center /= vertices.Length; // Point moyen des sommets

            // �tape 2 : Trouver les faces de la forme � partir des edges
            Vector3Int[] faces = FindFaces(vertices, edges);

            // �tape 3 : G�n�rer des t�tra�dres en connectant le centre � chaque face
            foreach (var face in faces)
            {
                tetrahedra.Add(new Vector3[] { center, vertices[face.x], vertices[face.y], vertices[face.z] });
            }

            return tetrahedra.ToArray();
        }

        /// <summary>
        /// Trouve toutes les faces triangul�es de la forme � partir des edges.
        /// </summary>
        private static Vector3Int[] FindFaces(Vector3[] vertices, Vector2Int[] edges)
        {
            List<Vector3Int> faces = new List<Vector3Int>();

            // Parcourir chaque combinaison d'edges pour trouver les triangles
            for (int i = 0; i < edges.Length; i++)
            {
                for (int j = i + 1; j < edges.Length; j++)
                {
                    for (int k = j + 1; k < edges.Length; k++)
                    {
                        // Trouver si ces trois ar�tes partagent les sommets n�cessaires pour former une face
                        HashSet<int> uniqueVertices = new HashSet<int>
                    {
                        edges[i].x, edges[i].y,
                        edges[j].x, edges[j].y,
                        edges[k].x, edges[k].y
                    };

                        if (uniqueVertices.Count == 3)
                        {
                            // Ces trois ar�tes forment un triangle
                            var verticesArray = new List<int>(uniqueVertices);
                            faces.Add(new Vector3Int(verticesArray[0], verticesArray[1], verticesArray[2]));
                        }
                    }
                }
            }

            return faces.ToArray();
        }

        private static float CalculateTetrahedronVolume(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
        {
            // Vecteurs du t�tra�dre
            Vector3 AB = B - A;
            Vector3 AC = C - A;
            Vector3 AD = D - A;

            // Produit vectoriel AC x AD
            Vector3 crossProduct = Vector3.Cross(AC, AD);

            // Produit scalaire AB . (AC x AD)
            float scalarTripleProduct = Vector3.Dot(AB, crossProduct);

            // Volume du t�tra�dre
            float volume = Mathf.Abs(scalarTripleProduct) / 6.0f;

            return volume;
        }

        private static Vector3 GenerateRandomPosInTetrahedron(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
        {
            Vector4 randomBarycentricPos = new Vector4(Random.value, Random.value, Random.value, Random.value);

            float toDivide = randomBarycentricPos.x + randomBarycentricPos.y + randomBarycentricPos.z + randomBarycentricPos.w;

            randomBarycentricPos.x /= toDivide;
            randomBarycentricPos.y /= toDivide;
            randomBarycentricPos.z /= toDivide;
            randomBarycentricPos.w /= toDivide;

            return randomBarycentricPos.FromBarycentricCoords(A, B, C, D);
        }

        #endregion

        #endregion

        #endregion

        #region Custom Rigidbody Methods

        public static Vector3 AddForce(Vector3 velocity, float valueX, float valueY, float valueZ, float mass)
        {
            return velocity += (new Vector3(valueX, valueY, valueZ) / mass) * Time.fixedDeltaTime;
        }

        public static Vector3 AddForce(Vector3 velocity, Vector3 value, float mass)
        {
            return velocity += (value / mass) * Time.fixedDeltaTime;
        }

        public static Vector3 AddRelativeForce(Transform space, Vector3 velocity, Vector3 value, float mass)
        {
            float massMultiplier = 1 / mass;
            Vector3 vel = space.InverseTransformDirection(velocity);
            vel += value * massMultiplier * Time.fixedDeltaTime;
            vel = space.TransformDirection(vel);
            return vel;
        }

        #endregion

    }
}
