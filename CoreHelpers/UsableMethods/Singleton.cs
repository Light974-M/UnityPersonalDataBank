using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    /// <summary>
    /// create a singleton with unique and accessible instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : UPDBBehaviour where T : Component
    {
        #region Singleton Initialize

        /// <summary>
        /// unique instance of class
        /// </summary>
        protected static T _instance = null;

        public static T Instance
        {
            get
            {
                //if instance is null and no instance was found in scene, create a new obj to contain singleton
                if (_instance == null && !TryFindObjectOfType(out _instance))
                {
                    GameObject obj = new GameObject($"{typeof(T).Name}");
                    _instance = obj.AddComponent<T>();
                    Debug.LogWarning("[Singleton] Created new Singleton Object " + typeof(T).Name);
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        protected virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// called when script instance is being destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /*********************************CUSTOM METHODS**********************************/

        protected override void OnScene()
        {
            Initialize();
        }

        /// <summary>
        /// called at awake and on draw gizmos, make sure there is only one instance of class loaded in scene
        /// </summary>
        protected void Initialize()
        {
            if (_instance == null)
                _instance = this as T;
            else if (_instance != this)
                IntelliDestroy(this);
        }
    }
}
