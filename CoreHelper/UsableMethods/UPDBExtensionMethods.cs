using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.Usable.ObjectsLibrary;

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

        #region BARYCENTRIC COORDS TOOLS

        #region Triangle Barycentric coords to 2D Coords

        /// <summary>
        /// convert barycentric coordinate system into 2D coords, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="allowInside"></param>
        /// <param name="allowOutside"></param>
        /// <returns></returns>
        public static Vector2 FromBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c, bool allowInside, bool allowOutside)
        {
            if (!allowInside && !allowOutside)
                return Vector2.zero;

            if (allowInside && allowOutside)
                return Get2DCoordsFromBarycentricCoords(pos, a, b, c);

            if (allowInside)
            {
                pos = UPDBMath.Clamp01(pos);

                if (pos.x + pos.y > 1)
                {
                    float toDivide = pos.x + pos.y;

                    pos.x /= toDivide;
                    pos.y /= toDivide;
                }

                return Get2DCoordsFromBarycentricCoords(pos, a, b, c);
            }

            if (allowOutside)
            {
                if (pos.x + pos.y < 1 && pos.x + pos.y > 0)
                {
                    float factor = pos.x / pos.y;

                    if (pos.x + pos.y >= 5)
                    {
                        float toMultiply = 1 / (pos.x + pos.y);
                        pos.x *= toMultiply;
                        pos.y *= toMultiply;
                    }
                    else
                    {
                        if (pos.x > pos.y)
                        {
                            float toMultiply = pos.y / pos.x;
                            float toSubX = (pos.x + pos.y) * toMultiply;
                            float toSubY = (pos.x + pos.y) - toSubX;

                            pos.x -= toSubX;
                            pos.y -= toSubY;
                        }
                        if (pos.x < pos.y)
                        {
                            float toMultiply = pos.x / pos.y;
                            float toSubY = (pos.x + pos.y) * toMultiply;
                            float toSubX = (pos.x + pos.y) - toSubY;

                            pos.x -= toSubX;
                            pos.y -= toSubY;
                        }
                        if (pos.x == pos.y)
                        {
                            float toSubstract = (pos.x + pos.y) / 2;
                            pos.x -= toSubstract;
                            pos.y -= toSubstract;
                        }
                    }
                }

                return Get2DCoordsFromBarycentricCoords(pos, a, b, c);
            }

            return Get2DCoordsFromBarycentricCoords(pos, a, b, c);
        }

        /// <summary>
        /// convert barycentric coordinate system into 2D coords, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector2 FromBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return pos.FromBarycentricCoords(a, b, c, true, true);
        }

        /// <summary>
        /// convert barycentric coordinate system into 2D coords, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector2 FromInsideBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return pos.FromBarycentricCoords(a, b, c, true, false);
        }

        /// <summary>
        /// convert barycentric coordinate system into 2D coords, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector2 FromOutsideBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return pos.FromBarycentricCoords(a, b, c, false, true);
        }

        #endregion

        #region 2D Coords to Triangle Barycentric coords

        /// <summary>
        /// convert 2D coords into barycentric coordinate system, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="allowInside"></param>
        /// <param name="allowOutside"></param>
        /// <returns></returns>
        public static Vector2 ToBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c, bool allowInside, bool allowOutside)
        {
            if (allowInside && allowOutside)
                return GetBarycentricCoords(pos, a, b, c);

            if (!allowInside && !allowOutside)
                return Vector2.zero;

            if (allowInside)
            {
                Vector2 toReturn = UPDBMath.Clamp01(GetBarycentricCoords(pos, a, b, b));

                if (toReturn.x + toReturn.y > 1)
                {
                    float toDivide = toReturn.x + toReturn.y;

                    toReturn.x /= toDivide;
                    toReturn.y /= toDivide;
                }

                return toReturn;
            }

            if (allowOutside)
            {
                Vector2 toReturn = UPDBMath.Clamp01(GetBarycentricCoords(pos, a, b, b));

                if (toReturn.x + toReturn.y < 1 && toReturn.x + toReturn.y > 0)
                {
                    float factor = toReturn.x / toReturn.y;

                    if (toReturn.x + toReturn.y >= 5)
                    {
                        float toMultiply = 1 / (toReturn.x + toReturn.y);
                        toReturn.x *= toMultiply;
                        toReturn.y *= toMultiply;
                    }
                    else
                    {
                        if (toReturn.x > toReturn.y)
                        {
                            float toMultiply = toReturn.y / toReturn.x;
                            float toSubX = (toReturn.x + toReturn.y) * toMultiply;
                            float toSubY = (toReturn.x + toReturn.y) - toSubX;

                            toReturn.x -= toSubX;
                            toReturn.y -= toSubY;
                        }
                        if (toReturn.x < toReturn.y)
                        {
                            float toMultiply = toReturn.x / toReturn.y;
                            float toSubY = (toReturn.x + toReturn.y) * toMultiply;
                            float toSubX = (toReturn.x + toReturn.y) - toSubY;

                            toReturn.x -= toSubX;
                            toReturn.y -= toSubY;
                        }
                        if (toReturn.x == toReturn.y)
                        {
                            float toSubstract = (toReturn.x + toReturn.y) / 2;
                            toReturn.x -= toSubstract;
                            toReturn.y -= toSubstract;
                        }
                    }
                }

                return toReturn;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// convert 2D coords into barycentric coordinate system, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector2 ToBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return ToBarycentricCoords(pos, a, b, c, true, true);
        }

        /// <summary>
        /// convert 2D coords into barycentric coordinate system, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector2 ToInsideBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return ToBarycentricCoords(pos, a, b, c, true, false);
        }

        /// <summary>
        /// convert 2D coords into barycentric coordinate system, wich means x and y are represented inside a triangle, and if the sum of x and y are below 0, the coordinates are inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector2 ToOutsideBarycentricCoords(this Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return ToBarycentricCoords(pos, a, b, c, false, true);
        }

        #endregion

        #region Triangle barycentric coords to 3D coords

        /// <summary>
        /// convert barycentric coordinate system into 3D coords, wich means x y and z are represented inside a triangle, and if both x y and z are between 0 and 1, point is inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="allowInside"></param>
        /// <param name="allowOutside"></param>
        /// <returns></returns>
        public static Vector3 FromBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c, bool allowInside, bool allowOutside)
        {
            if (!allowInside && !allowOutside)
                return Vector3.zero;

            if (allowInside && allowOutside)
                return Get3DCoordsFromBarycentricCoords(pos, a, b, c);

            if (allowInside)
            {
                pos = UPDBMath.Clamp01(pos);

                float imprecision = 0.0000001f;
                if ((pos.x + pos.y + pos.z) - 1 >= imprecision || (pos.x + pos.y + pos.z) - 1 <= -imprecision)
                {
                    float toDivide = pos.x + pos.y + pos.z;

                    pos.x /= toDivide;
                    pos.y /= toDivide;
                    pos.z /= toDivide;
                }

                return Get3DCoordsFromBarycentricCoords(pos, a, b, c);
            }

            if (allowOutside)
            {
                if (pos.x <= 1 && pos.x >= 0 && pos.y <= 1 && pos.y >= 0 && pos.z <= 1 && pos.z >= 0)
                {
                    float distX = pos.x > 5 ? 1 - pos.x : pos.x;
                    float distY = pos.y > 5 ? 1 - pos.y : pos.y;
                    float distZ = pos.z > 5 ? 1 - pos.z : pos.z;

                    if (distX <= distY && distX <= distZ)
                    {
                        pos.x = UPDBMath.InvertClamp01(pos.x);

                        if (pos.x == 1)
                        {
                            pos.y = 0;
                            pos.z = 0;
                        }
                        else
                        {
                            float toDivide = pos.y + pos.z;
                            pos.y /= toDivide;
                            pos.z /= toDivide;
                        }
                    }

                    if (distY <= distX && distY <= distZ)
                    {
                        pos.y = UPDBMath.InvertClamp01(pos.x);

                        if (pos.y == 1)
                        {
                            pos.x = 0;
                            pos.z = 0;
                        }
                        else
                        {
                            float toDivide = pos.x + pos.z;
                            pos.x /= toDivide;
                            pos.z /= toDivide;
                        }
                    }

                    if (distZ <= distY && distZ <= distX)
                    {
                        pos.z = UPDBMath.InvertClamp01(pos.x);

                        if (pos.z == 1)
                        {
                            pos.y = 0;
                            pos.x = 0;
                        }
                        else
                        {
                            float toDivide = pos.y + pos.x;
                            pos.y /= toDivide;
                            pos.x /= toDivide;
                        }
                    }
                }

                return Get3DCoordsFromBarycentricCoords(pos, a, b, c);
            }

            return Get2DCoordsFromBarycentricCoords(pos, a, b, c);
        }

        /// <summary>
        /// convert barycentric coordinate system into 3D coords, wich means x y and z are represented inside a triangle,  and if both x y and z are between 0 and 1, point is inside the triangle 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector3 FromBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            return FromBarycentricCoords(pos, a, b, c, true, true);
        }

        /// <summary>
        /// convert barycentric coordinate system into 3D coords, wich means x y and z are represented inside a triangle, and if both x y and z are between 0 and 1, point is inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector3 FromInsideBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            return FromBarycentricCoords(pos, a, b, c, true, false);
        }

        /// <summary>
        /// convert barycentric coordinate system into 3D coords, wich means x y and z are represented inside a triangle, and if both x y and z are between 0 and 1, point is inside the triangle 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector3 FromOutsideBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            return FromBarycentricCoords(pos, a, b, c, false, true);
        }

        #endregion

        #region 3D Coords to Triangle Barycentric coords

        /// <summary>
        /// convert 3D coords into barycentric coordinate system, wich means x and y are represented inside a triangle, and if both x y and z are between 0 and 1, point is inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="allowInside"></param>
        /// <param name="allowOutside"></param>
        /// <returns></returns>
        public static Vector3 ToBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c, bool allowInside, bool allowOutside)
        {
            if (allowInside && allowOutside)
                return GetBarycentricCoords(pos, a, b, c);

            if (!allowInside && !allowOutside)
                return Vector3.zero;

            if (allowInside)
            {
                Vector3 coords = GetBarycentricCoords(pos, a, b, c);



                return coords;
            }

            if (allowOutside)
            {
                Vector3 toReturn = GetBarycentricCoords(pos, a, b, c);

                return toReturn;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// convert 3D coords into barycentric coordinate system, wich means x and y are represented inside a triangle, and if both x y and z are between 0 and 1, point is inside the triangle
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector3 ToBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            return ToBarycentricCoords(pos, a, b, c, true, true);
        }

        #endregion

        #region Tetrahedron Barycentric coords to 3D Coords

        public static Vector3 FromBarycentricCoords(this Vector4 pos, Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool allowInside, bool allowOutside)
        {
            return Get3DCoordsFromBarycentricCoords(pos, a, b, c, d);
        }

        public static Vector3 FromBarycentricCoords(this Vector4 pos, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return pos.FromBarycentricCoords(a, b, c, d, true, true);
        }

        #endregion

        #region 3D Coords to Tetrahedron Barycentric coords

        public static Vector4 ToBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool allowInside, bool allowOutside)
        {
            return GetBarycentricCoords(pos, a, b, c, d);
        }

        public static Vector4 ToBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return pos.ToBarycentricCoords(a, b, c, d, true, true);
        }

        #endregion

        #region Base Calculations Methods

        private static Vector2 Get2DCoordsFromBarycentricCoords(Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            return (1 - pos.x - pos.y) * a + pos.x * b + pos.y * c;
        }

        private static Vector2 GetBarycentricCoords(Vector2 pos, Vector2 a, Vector2 b, Vector2 c)
        {
            float denom = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            float posX = ((pos.x - a.x) * (c.y - a.y) - (pos.y - a.y) * (c.x - a.x)) / denom;
            float posY = ((b.x - a.x) * (pos.y - a.y) - (b.y - a.y) * (pos.x - a.x)) / denom;

            return new Vector2(posX, posY);
        }

        private static Vector3 Get3DCoordsFromBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            float imprecision = 0.0000001f;
            // Vérifier que la somme des coordonnées barycentriques est égale à 1
            if ((pos.x + pos.y + pos.z) - 1 >= imprecision || (pos.x + pos.y + pos.z) - 1 <= -imprecision)
            {
                Debug.LogWarning("Warning : Les coordonnées barycentriques doivent satisfaire pos.x + pos.y + pos.z = 1.");
                return Vector3.zero;
            }

            // Calculer le point en fonction des coordonnées barycentriques
            return pos.x * a + pos.y * b + pos.z * c;
        }

        private static Vector3 GetBarycentricCoords(this Vector3 pos, Vector3 a, Vector3 b, Vector3 c)
        {
            // Vecteurs du triangle
            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = pos - a;

            // Produits scalaires nécessaires
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);

            // Déterminant
            float denom = d00 * d11 - d01 * d01;
            if (Mathf.Abs(denom) < Mathf.Epsilon)
            {
                throw new System.Exception("Les sommets du triangle sont colinéaires ou très proches.");
            }

            // Coordonnées barycentriques
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }

        private static Vector3 Get3DCoordsFromBarycentricCoords(Vector4 pos, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            // Extrait les coordonnées barycentriques
            float u = pos.x;
            float v = pos.y;
            float w = pos.z;
            float t = pos.w;

            // Calcule la position dans l'espace
            Vector3 position = u * a + v * b + w * c + t * d;

            return position;
        }

        /// <summary>
        /// take coordinates and a 4 points tetrahedron, and returns the barycentric coordinates of the tetrahedron, and if every value are within 0 and 1, and the sum of the four values are equal to 1, then point is inside the tetrahedron
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private static Vector4 GetBarycentricCoords(Vector3 point, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            // Calcul des volumes barycentriques
            float volumeABCD = SignedTetrahedronVolume(a, b, c, d);
            float volumePBCD = SignedTetrahedronVolume(point, b, c, d);
            float volumeAPCD = SignedTetrahedronVolume(a, point, c, d);
            float volumeABPD = SignedTetrahedronVolume(a, b, point, d);
            float volumeABCP = SignedTetrahedronVolume(a, b, c, point);

            // Calcul des coordonnées barycentriques
            float u = volumePBCD / volumeABCD;
            float v = volumeAPCD / volumeABCD;
            float w = volumeABPD / volumeABCD;
            float t = volumeABCP / volumeABCD;

            // Vérification des contraintes des coordonnées barycentriques
            return new Vector4(u, v, w, t);
        }

        /// <summary>
        /// Calcule le volume signé d'un tétraèdre défini par quatre points.
        /// </summary>
        /// <param name="a">Premier point du tétraèdre.</param>
        /// <param name="b">Deuxième point du tétraèdre.</param>
        /// <param name="c">Troisième point du tétraèdre.</param>
        /// <param name="d">Quatrième point du tétraèdre.</param>
        /// <returns>Le volume signé du tétraèdre.</returns>
        private static float SignedTetrahedronVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Vector3.Dot(Vector3.Cross(b - a, c - a), d - a) / 6.0f;
        }

        #endregion

        #endregion

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
