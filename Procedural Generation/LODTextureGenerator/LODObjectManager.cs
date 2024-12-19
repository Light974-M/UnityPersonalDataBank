using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.LODTextureGenerator
{
    public class LODObjectManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("list of gameObjects to render for LOD")]
        private List<LODConfig> _lODList = new List<LODConfig>();

        #region Public API

        [System.Serializable]
        public class LODConfig
        {
            [SerializeField, Tooltip("GameObject to enable")]
            private GameObject _LODobject;

            [SerializeField, Tooltip("distance range for element to be active")]
            private Vector2 _effectRange = Vector2.zero;
        }

        #endregion

    }
}
