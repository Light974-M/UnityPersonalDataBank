using UnityEngine;

namespace UPDB.CoreHelper.ExtensionMethods
{
    /// <summary>
    /// method bank that can be used everywhere for string extensions
    /// </summary>
    public static class StringExtensionMethods
    {
        /// <summary>
        /// make a string value smoothly appear character by character in another variable
        /// </summary>
        /// <param name="text"> text to defil </param>
        /// <param name="defil"> where defil text will be stocked </param>
        public static void TextDefil(this string text, out string defil)
        {
            defil = null;
            //float invokeTimer = 0;

            //while(defil != text)
            //{
            //    MonoBehaviour.Invoke("UpdateDefil", invokeTimer);
            //}
        }

        /// <summary>
        /// update defilText
        /// </summary>
        /// <returns></returns>
        public static float UpdateDefil()
        {
            return 0.1f;
        }
    } 
}
