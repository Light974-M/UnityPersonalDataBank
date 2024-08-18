using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    ///<summary>
    /// a collection of UPDB math functions
    ///</summary>
    public struct UPDBMath
    {
        /// <summary>
        /// make a root other than square roots, let you make cubic root, 4 roots, or wathever X root you want
        /// </summary>
        /// <param name="number"> number to root </param>
        /// <param name="root"> wich root is asked </param>
        /// <returns></returns>
        public static float Root(float number, float root)
        {
            return Mathf.Pow(number, 1 / root);
        }

        /// <summary>
        /// make a square power
        /// </summary>
        /// <param name="number"> number to square </param>
        /// <returns></returns>
        public static float Sqr(float number)
        {
            return Mathf.Pow(number, 2);
        }

        /// <summary>
        /// make a cube power
        /// </summary>
        /// <param name="number"> number to cube </param>
        /// <returns></returns>
        public static float Cbe(float number)
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
            return Random.Range(0f, 1f) <= probability ? 1 : 0;
        }

        /// <summary>
        /// take in argument probability, between 0 and 1, and return it, false if failed, true if succeed
        /// </summary>
        /// <param name="probability"> probability of operation </param>
        /// <returns></returns>
        public static bool Probool(float probability)
        {
            return Random.Range(0f, 1f) <= probability ? true : false;
        }

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
        /// return the gravitationnal constant G = 6.6743 × 10-11 m3 kg-1 s-2
        /// </summary>
        public static float G
        {
            get { return 0.000000000066743f; }
        }


    }

}