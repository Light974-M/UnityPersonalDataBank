using UnityEngine;

namespace UPDB.CoreHelper.ExtensionMethods
{
    /// <summary>
    /// method bank that can be used everywhere for Component extensions
    /// </summary>
    public static class ComponentExtensionMethods
    {
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
    } 
}
