using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    /// <summary>
    /// premade levelManager for tests scene
    /// </summary>
    public class LevelTestManager : LevelManager
    {
        [SerializeField]
        private LevelTestStartInfo _testStartInfo;

        #region Public API

        [Serializable]
        public class LevelTestStartInfo
        {
            
        }

        #endregion
    }
}
