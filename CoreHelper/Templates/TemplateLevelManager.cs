using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.Usable;

namespace UPDB.CoreHelper.Templates
{
    /// <summary>
    /// premade levelManager for tests scene
    /// </summary>
    [AddComponentMenu(NamespaceID.TemplatesPath + "/Level Manager Template")]
    public class TemplateLevelManager : LevelManager<TemplateLevelManager>
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
