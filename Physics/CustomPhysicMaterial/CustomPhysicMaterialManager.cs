using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.CustomPhysicMaterial
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("UPDB/Physic/CustomPhysicMaterial/Custom Physic Material Manager")]
    public class CustomPhysicMaterialManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("target custom physic material to use")]
        private UPDBPhysicMaterialAsset _physicMaterial;

        [SerializeField, Tooltip("collider used by Custom Physic Material")]
        private Collider _usedCollider;

        [SerializeField, Tooltip("Layers to exclude when managing physic of collisions")]
        private LayerMask _excludeLayers;

        #region Public API

        public UPDBPhysicMaterialAsset PhysicMaterial
        {
            get { return _physicMaterial; }
            set { _physicMaterial = value; }
        }
        public Collider UsedCollider
        {
            get { return _usedCollider; }
            set { _usedCollider = value; }
        }
        public LayerMask ExcludeLayers
        {
            get { return _excludeLayers; }
            set { _excludeLayers = value; }
        }

        #endregion

        private void Awake()
        {
            Init();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_physicMaterial || collision.gameObject.layer.IsInLayerMask(_excludeLayers))
                return;

            bool hasRigidbody = collision.gameObject.TryGetComponent(out Rigidbody rb);

            if (!hasRigidbody)
                return;

            ApplyDynamicFriction(collision.gameObject, collision.collider, rb);
            ApplyStaticFriction(collision.gameObject, collision.collider, rb);
            ApplyBounciness(collision.gameObject, collision.collider, rb);
        }

        public void Init()
        {
            MakeNonNullable<Collider, SphereCollider>(ref _usedCollider, gameObject);
        }

        private void ApplyDynamicFriction(GameObject collidedObj, Collider collidedCollider, Rigidbody collidedRb)
        {

        }

        private void ApplyStaticFriction(GameObject collidedObj, Collider collidedCollider, Rigidbody collidedRb)
        {

        }

        private void ApplyBounciness(GameObject collidedObj, Collider collidedCollider, Rigidbody collidedRb)
        {
            if (_physicMaterial.BounceCombine == BounceCombineMode.NormalizedCollidedObjectVelocity)
            {
                collidedRb.AddForce(collidedRb.velocity.normalized * _physicMaterial.Bounciness);
                return;
            }
        }
    }
}
