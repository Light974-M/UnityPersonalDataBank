using UnityEngine;
using System;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.physic.GravityManager
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/Physics/GravityManager/gravityManager")]
    public class GravityManager : UPDBBehaviour
    {

        [SerializeField, Tooltip("")]
        private GravityType _gravityUsed;

        private Vector3 _gravityVector = Vector3.zero;

        private Vector3 _lastEulerAngles;
        private Vector3 _lastLocalScale;


        #region Public API

        public enum GravityType
        {
            Physics,
            GravityManager,
            Custom,
        }

        public GravityType GravityUsed
        {
            get
            {
                return _gravityUsed;
            }

            set
            {
                _gravityUsed = value;

                if ((int)_gravityUsed >= Enum.GetNames(typeof(GravityType)).Length)
                    _gravityUsed -= Enum.GetNames(typeof(GravityType)).Length;

                UpdateGravityManager();
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                return NormalizedScale();
            }
        }

        #endregion


        private void OnValidate()
        {
            UpdateGravityManager();
        }

        private void UpdateGravityManager()
        {
            if (_gravityUsed == GravityType.Physics)
            {
                Physics.gravity = _gravityVector;
            }
        }

        private Vector3 NormalizedScale()
        {
            if (!(transform.localScale.x == transform.localScale.y && transform.localScale.y == transform.localScale.z))
            {
                Vector3 localScale = Vector3.zero;

                if (transform.localScale.x > transform.localScale.y)
                {
                    localScale.x = transform.localScale.y;
                    localScale.y = transform.localScale.y;
                }
                else
                {
                    localScale.x = transform.localScale.x;
                    localScale.y = transform.localScale.x;
                }

                if (transform.localScale.z > transform.localScale.y)
                    localScale.z = transform.localScale.y;
                else
                {
                    localScale.x = transform.localScale.z;
                    localScale.y = transform.localScale.z;
                    localScale.z = transform.localScale.z;
                }

                transform.localScale = localScale;
            }
            return transform.localScale;
        }

        private void OnDrawGizmos()
        {
            if (_lastEulerAngles != transform.eulerAngles || _lastLocalScale != transform.localScale)
            {
                _gravityVector = transform.forward * transform.localScale.z/*LocalScale.x*/;
                UpdateGravityManager();
            }

            _lastEulerAngles = transform.eulerAngles;
            _lastLocalScale = transform.localScale;

            Debug.DrawLine(transform.position, transform.position + _gravityVector, Color.white);
        }
    }
}

