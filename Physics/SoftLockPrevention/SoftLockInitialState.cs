using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.SoftLockPrevention
{
    public class SoftLockInitialState : UPDBBehaviour
    {
        /// <summary>
        /// initial stored position of object
        /// </summary>
        private Vector3 _initialPos;

        /// <summary>
        /// initial stored rotation of object
        /// </summary>
        private Quaternion _initialRot;

        /// <summary>
        /// initial stored scale of object
        /// </summary>
        private Vector3 _initialScale;

        #region Public API

        ///<inheritdoc cref="_initialPos"/>
        public Vector3 InitialPos => _initialPos;

        ///<inheritdoc cref="_initialRot"/>
        public Quaternion InitialRot => _initialRot;

        ///<inheritdoc cref="_initialScale"/>
        public Vector3 InitialScale => _initialScale;

        #endregion

        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        private void Awake()
        {
            _initialPos = transform.position;
            _initialRot = transform.rotation;
            _initialScale = transform.localScale;
        }
    } 
}
