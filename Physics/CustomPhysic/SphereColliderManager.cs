using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.physic.CustomPhysic
{
    [AddComponentMenu("UPDB/Physics/CustomPhysic/Sphere Collider")]
    public class SphereColliderManager : MonoBehaviour
    {
        [SerializeField, Tooltip("position offset of collider")]
        private Vector3 _position = Vector3.zero;

        [SerializeField, Tooltip("scale of sphere")]
        private float _scale = 1;

        private void Awake()
        {

        }


        private void FixedUpdate()
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + _position, _scale);
            Gizmos.color = Color.white;
        }
    } 
}
