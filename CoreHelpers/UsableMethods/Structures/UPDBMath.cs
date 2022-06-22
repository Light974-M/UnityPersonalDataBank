using UnityEngine;

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
}
