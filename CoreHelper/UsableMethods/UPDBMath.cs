using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UPDB.CoreHelper.Usable;

namespace UPDB.CoreHelper.UsableMethods
{
    ///<summary>
    /// a collection of UPDB math functions
    ///</summary>
    public static class UPDBMath
    {
        /// <summary>
        /// make a root other than square roots, let you make cubic root, 4 roots, or wathever X root you want
        /// </summary>
        /// <param name="number"> number to root </param>
        /// <param name="root"> wich root is asked </param>
        /// <returns></returns>
        public static float Root(this float number, float root)
        {
            return Mathf.Pow(number, 1 / root);
        }

        /// <summary>
        /// make a square power
        /// </summary>
        /// <param name="number"> number to square </param>
        /// <returns></returns>
        public static float Sqr(this float number)
        {
            return Mathf.Pow(number, 2);
        }

        /// <summary>
        /// make a cube power
        /// </summary>
        /// <param name="number"> number to cube </param>
        /// <returns></returns>
        public static float Cbe(this float number)
        {
            return Mathf.Pow(number, 3);
        }

        /// <summary>
        /// take in argument probability, between 0 and 1, and return it, 0 if failed, 1 if succeed
        /// </summary>
        /// <param name="probability"> probability of operation </param>
        /// <returns></returns>
        public static int Proba(float probability)
        {
            return UnityEngine.Random.Range(0f, 1f) <= probability ? 1 : 0;
        }

        /// <summary>
        /// take in argument probability, between 0 and 1, and return it, false if failed, true if succeed
        /// </summary>
        /// <param name="probability"> probability of operation </param>
        /// <returns></returns>
        public static bool Probool(float probability)
        {
            return UnityEngine.Random.Range(0f, 1f) <= probability ? true : false;
        }

        public static bool IsPowerOf(int value, int power)
        {
            while (value > 1)
            {
                value = value / power;
                int modulo = value % power;

                if (modulo != 0 && value != 1)
                    return false;
            }


            return true;
        }

        public static Vector2 FindRightPerpendicularVector(Vector2 vec)
        {
            return new Vector2(vec.y, -vec.x);
        }

        public static Vector2 FindLeftPerpendicularVector(Vector2 vec)
        {
            return new Vector2(-vec.y, vec.x);
        }

        public static Vector3 FindAnyPerpendicularVectorUpType(Vector3 vec)
        {
            if (vec == Vector3.zero)
                return Vector3.zero;

            if (vec.x < 0.0000001f && vec.y < 0.0000001f)
                return new Vector3(0, 1, 0);

            Vector3 perpVec = Vector3.zero;

            perpVec.x = -vec.y;
            perpVec.y = vec.x;
            perpVec.z = 0;

            return perpVec.normalized * vec.magnitude;
        }

        /// <summary>
        /// return the third axis by default, that create a orthogonal system with the two given vectors, by default, vector length is average between the two given
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>
        public static Vector3 FindThirdAxis(Vector3 vecA, Vector3 vecB)
        {
            return Vector3.Cross(vecA, vecB).normalized * ((vecA.magnitude + vecB.magnitude) / 2f);
        }

        /// <summary>
        /// return the third axis by default, that create a orthogonal system with the two given vectors, by default, vector length is average between the two given
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <param name="isADominant">tell if magnitude of A will be used, set to false to use magnitude of B, or remove parameter to use average</param>
        /// <returns></returns>
        public static Vector3 FindThirdAxis(Vector3 vecA, Vector3 vecB, bool isADominant)
        {
            float mag = isADominant ? vecA.magnitude : vecB.magnitude;
            return Vector3.Cross(vecA, vecB).normalized * mag;
        }

        /// <summary>
        /// return the multiplication of the two vectors
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>
        public static Vector3 VecTime(Vector3 vecA, Vector3 vecB)
        {
            return new Vector3(vecA.x * vecB.x, vecA.y * vecB.y, vecA.z * vecB.z);
        }

        /// <summary>
        /// return the division of the two vectors
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>
        public static Vector3 VecDivide(Vector3 vecA, Vector3 vecB)
        {
            return new Vector3(vecA.x / vecB.x, vecA.y / vecB.y, vecA.z / vecB.z);
        }

        /// <summary>
        /// add a vector and a number for the given axis
        /// </summary>
        /// <param name="vecToAdd">the vector3 to add with float</param>
        /// <param name="toAdd">float parameter of number to add</param>
        /// <param name="axisToAdd">axis wich vector3 is gonna add with float</param>
        /// <returns></returns>
        public static Vector3 VecAndFloatAdd(Vector3 vecToAdd, float toAdd, Axis axisToAdd)
        {
            if(axisToAdd == Axis.X)
            {
                vecToAdd.x += toAdd;
                return vecToAdd;
            }
            if (axisToAdd == Axis.Y)
            {
                vecToAdd.y += toAdd;
                return vecToAdd;
            }
            if (axisToAdd == Axis.Z)
            {
                vecToAdd.z += toAdd;
                return vecToAdd;
            }
            if (axisToAdd == Axis.XY)
            {
                vecToAdd.x += toAdd;
                vecToAdd.y += toAdd;
                return vecToAdd;
            }
            if (axisToAdd == Axis.YZ)
            {
                vecToAdd.y += toAdd;
                vecToAdd.z += toAdd;
                return vecToAdd;
            }
            if (axisToAdd == Axis.XZ)
            {
                vecToAdd.x += toAdd;
                vecToAdd.z += toAdd;
                return vecToAdd;
            }
            if (axisToAdd == Axis.XYZ)
            {
                vecToAdd.x += toAdd;
                vecToAdd.y += toAdd;
                vecToAdd.z += toAdd;
                return vecToAdd;
            }

            return vecToAdd;
        }

        /// <summary>
        /// add a vector and a number for the given axis
        /// </summary>
        /// <param name="vecToRemove">the vector3 to add with float</param>
        /// <param name="toRemove">float parameter of number to add</param>
        /// <param name="axisToRemove">axis wich vector3 is gonna add with float</param>
        /// <returns></returns>
        public static Vector3 VecAndFloatRemove(Vector3 vecToRemove, float toRemove, Axis axisToRemove)
        {
            if (axisToRemove == Axis.X)
            {
                vecToRemove.x -= toRemove;
                return vecToRemove;
            }
            if (axisToRemove == Axis.Y)
            {
                vecToRemove.y -= toRemove;
                return vecToRemove;
            }
            if (axisToRemove == Axis.Z)
            {
                vecToRemove.z -= toRemove;
                return vecToRemove;
            }
            if (axisToRemove == Axis.XY)
            {
                vecToRemove.x -= toRemove;
                vecToRemove.y -= toRemove;
                return vecToRemove;
            }
            if (axisToRemove == Axis.YZ)
            {
                vecToRemove.y -= toRemove;
                vecToRemove.z -= toRemove;
                return vecToRemove;
            }
            if (axisToRemove == Axis.XZ)
            {
                vecToRemove.x -= toRemove;
                vecToRemove.z -= toRemove;
                return vecToRemove;
            }
            if (axisToRemove == Axis.XYZ)
            {
                vecToRemove.x -= toRemove;
                vecToRemove.y -= toRemove;
                vecToRemove.z -= toRemove;
                return vecToRemove;
            }

            return vecToRemove;
        }

        public static float ToDegrees(float radianValue)
        {
            return (radianValue / Mathf.PI) * 180;
        }

        public static float ToRadians(float degreesValue)
        {
            return (degreesValue / 180) * Mathf.PI;
        }

        /// <summary>
        /// make a clamp with vector2
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns>clamped vector2</returns>
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
        }

        /// <summary>
        /// make a clamp with vector3
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns>clamped vector3</returns>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
        }

        /// <summary>
        /// make a lerp between 0 and 1
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>the clamped value between 0 and 1</returns>
        public static Vector2 Clamp01(Vector2 value)
        {
            return Clamp(value, Vector2.zero, Vector2.one);
        }

        /// <summary>
        /// make a lerp between 0 and 1
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>the clamped value between 0 and 1</returns>
        public static Vector3 Clamp01(Vector3 value)
        {
            return Clamp(value, Vector3.zero, Vector3.one);
        }

        /// <summary>
        /// return a value that isn't between min and max
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns></returns>
        public static float InvertClamp(float value, float min, float max)
        {
            float clampedValue = 0;

            if(value >= (min + ((max - min) / 2)))
                clampedValue = Mathf.Clamp(Mathf.Clamp(value, -Mathf.Infinity, min), max, Mathf.Infinity);
            else
                clampedValue = Mathf.Clamp(Mathf.Clamp(value, max, Mathf.Infinity), -Mathf.Infinity, min);

            return clampedValue;
        }

        /// <summary>
        /// make an inverted clamp with vector2
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns>inverted clamped vector2</returns>
        public static Vector2 InvertClamp(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2(InvertClamp(value.x, min.x, max.x), InvertClamp(value.y, min.y, max.y));
        }

        /// <summary>
        /// make an inverted clamp with vector3
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns>inverted clamped vector3</returns>
        public static Vector3 InvertClamp(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(InvertClamp(value.x, min.x, max.x), InvertClamp(value.y, min.y, max.y), InvertClamp(value.z, min.z, max.z));
        }

        /// <summary>
        /// make an inverted lerp between 0 and 1
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>the clamped value not between 0 and 1</returns>
        public static float InvertClamp01(float value)
        {
            return InvertClamp(value, 0, 1);
        }

        /// <summary>
        /// make an inverted lerp between 0 and 1
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>the clamped value not between 0 and 1</returns>
        public static Vector2 InvertClamp01(Vector2 value)
        {
            return InvertClamp(value, Vector2.zero, Vector2.one);
        }

        /// <summary>
        /// make an inverted lerp between 0 and 1
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>the clamped value not between 0 and 1</returns>
        public static Vector3 InvertClamp01(Vector3 value)
        {
            return InvertClamp(value, Vector3.zero, Vector3.one);
        }

        #region Constants

        /// <summary>
        /// represent infinity with int value, return 2147483647, wich is max value of int type
        /// </summary>
        public static int Intfinity
        {
            get { return 2147483647; }
        }

        /// <summary>
        /// return square root of 2
        /// </summary>
        public static float V2
        {
            get { return Mathf.Sqrt(2); }
        }

        /// <summary>
        /// return the golden ratio
        /// </summary>
        public static float Gr
        {
            get { return 1.618033988749894848204586834365638117720309179805762862135448622705260462818902449707207204f; }
        }

        /// <summary>
        /// return the speed of light in void in m/s
        /// </summary>
        public static float C
        {
            get { return 299792458f; }
        }

        /// <summary>
        /// return the gravitationnal constant G = 6.6743 � 10-11 m3 kg-1 s-2
        /// </summary>
        public static float G
        {
            get { return 0.000000000066743f; }
        }

        #endregion

        #region BinaryOperators

        public static BitArray Add(BitArray a, BitArray b)
        {
            int smallestLength = a.Length <= b.Length ? a.Length : b.Length;

            if (!IsPowerOf(smallestLength, 2))
            {
                if (!IsPowerOf(a.Length, 2))
                    Debug.LogError($"error : given BitArray {a.Length} doesn't fit the size for fixed decimal render, please insert an array with a length that is a power of 2");

                if (!IsPowerOf(b.Length, 2))
                    Debug.LogError($"error : given BitArray {b.Length} doesn't fit the size for fixed decimal render, please insert an array with a length that is a power of 2");

                return new BitArray(0, false);
            }

            if (a.Length != b.Length)
            {
                BitArray smallestArray = a.Length <= b.Length ? a : b;
                BitArray biggestArray = a.Length <= b.Length ? b : a;

                BitArray biggestReplacer = new BitArray(smallestLength, false);
                string integerPartReplacer = string.Empty;

                for (int i = 1; i < biggestArray.Length; i++)
                {
                    if (integerPartReplacer == string.Empty && !biggestArray[i])
                        continue;

                    integerPartReplacer += biggestArray[i] ? '1' : '0';

                    if (integerPartReplacer.Length >= smallestLength / 2)
                        break;
                }

                for (int i = smallestLength / 2; i > 0; i--)
                    biggestReplacer.Set(i, integerPartReplacer[i - 1] != '0');

                for (int i = (smallestLength / 2) + 1, j = (biggestArray.Length / 2) + 1; i < biggestReplacer.Length && j < biggestArray.Length; i++, j++)
                    biggestReplacer.Set(i, biggestArray[j]);

                //add final values of clamped arrays
                if (smallestArray.Length == a.Length)
                    b = biggestReplacer;
                else
                    a = biggestReplacer;
            }

            BitArray result = new BitArray(smallestLength, false);

            string decimalAddition = string.Empty;
            BitAddResult adder = BitAdditionTable(false, false, false);

            for (int i = result.Length - 1; i > result.Length / 2; i--)
            {
                if (a[i] == false && b[i] == false && decimalAddition == string.Empty)
                    continue;

                adder = BitAdditionTable(adder.Carry, a[i], b[i]);

                char toAdd = adder.Value ? '1' : '0';
                decimalAddition = toAdd + decimalAddition;
            }

            for (int i = (result.Length / 2) + 1, j = 0; i < result.Length && j < decimalAddition.Length; i++, j++)
                result.Set(i, decimalAddition[j] != '0');

            int endIndex = 0;

            for (int i = 1; i <= result.Length / 2; i++)
            {
                if(a[i] || b[i] || i >= (result.Length / 2))
                {
                    endIndex = i; 
                    break;
                }
            }

            string integerAddition = string.Empty;
            
            for (int i = result.Length / 2; i >= endIndex; i--)
            {
                adder = BitAdditionTable(adder.Carry, a[i], b[i]);

                char toAdd = adder.Value ? '1' : '0';
                integerAddition = toAdd + integerAddition;
            }

            if(adder.Carry)
                integerAddition = '1' + integerAddition;


            string clampedIntegerAddition = string.Empty;

            for (int i = 0; i < integerAddition.Length; i++)
            {
                clampedIntegerAddition += integerAddition[i];

                if (clampedIntegerAddition.Length >= result.Length / 2)
                    break;
            }

            for (int i = result.Length / 2, j = clampedIntegerAddition.Length - 1; i > 0 && j >= 0; i--, j--)
            {
                result.Set(i, clampedIntegerAddition[j] != '0');
            }

            return result;
        }

        public static BitAddResult BitAdditionTable(bool a, bool b, bool c)
        {
            if (a && b && c)
                return new BitAddResult(true, true);

            if ((!a && b && c) || (a && !b && c) || (a && b && !c))
                return new BitAddResult(true, false);

            if (a || b || c)
                return new BitAddResult(false, true);

            return new BitAddResult(false, false);
        }

        public struct BitAddResult
        {
            private bool _value;
            private bool _carry;

            #region Public API

            public bool Value
            {
                get => _value; 
                set => _value = value;
            }

            public bool Carry
            {
                get => _carry;
                set => _carry = value;
            }

            #endregion

            public BitAddResult(bool carry, bool value)
            {
                _value = value;
                _carry = carry;
            }
        }

        #endregion
    }

}