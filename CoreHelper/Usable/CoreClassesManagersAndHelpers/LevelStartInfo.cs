using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.Usable
{
    public abstract class LevelStartInfo
    {
        [SerializeField, Tooltip("where does player start ?")]
        private Vector3 _playerStartPos = Vector3.zero;

        #region Public API

        public Vector3 PlayerStartPos
        {
            get => _playerStartPos;
            set => _playerStartPos = value;
        }

        #endregion
    } 
}
