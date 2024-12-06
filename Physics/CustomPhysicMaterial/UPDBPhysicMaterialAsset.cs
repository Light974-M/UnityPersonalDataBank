using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Physic.CustomPhysicMaterial
{
    [CreateAssetMenu(fileName = "new UPDBPhysicMaterial", menuName = NamespaceID.PhysicPath + "/" + NamespaceID.CustomPhysicMaterial + "/UPDB Physic Material")]
    public class UPDBPhysicMaterialAsset : ScriptableObject
    {
        [SerializeField, Tooltip("How Much Friction the Collider's surface has when moving, while in contact with another Collider. [-Infinity, Infinity]")]
        private float _dynamicFriction = 0.6f;

        [SerializeField, Tooltip("How Much Friction the Collider's surface has when stationary, while in contact with another Collider. [-Infinity, Infinity]")]
        private float _staticFriction = 0.6f;

        [SerializeField, Tooltip("How bouncy the Collider's surface is, defined by how much speed the other Collider retains after collision, values over 1 may not be realistic(not possible to have more energy than previously) [0, Infinity]")]
        private float _bounciness = 0;

        [SerializeField, Tooltip("how bounciness strength is calculated")]
        private BounceCombineMode _bounceCombine;

        #region Public API

        public float DynamicFriction
        {
            get { return _dynamicFriction; }
            set { _dynamicFriction = value; }
        }

        public float StaticFriction
        {
            get { return _staticFriction; }
            set { _staticFriction = value; }
        }

        public float Bounciness
        {
            get { return _bounciness; }
            set { _bounciness = value; }
        }

        public BounceCombineMode BounceCombine
        {
            get { return _bounceCombine; }
            set { _bounceCombine = value; }
        }

        #endregion
    } 

    public enum BounceCombineMode
    {
        BasedOnCollidedObjectVelocity,
        BasedOnBounceObjectVelocity,
        BasedOnBothObjectVelocity,
        NormalizedCollidedObjectVelocity,
        NormalizedBounceObjectVelocity,
        NormalizedBothObjectVelocity,
    }
}
