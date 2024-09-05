using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.physic.CustomPhysic
{
    [AddComponentMenu("UPDB/Physics/CustomPhysic/Box Collider")]
    public class BoxColliderManager : MonoBehaviour
    {
        [SerializeField, Tooltip("position offset of collider")]
        private Vector3 _position = Vector3.zero;

        [SerializeField, Tooltip("scale of box")]
        private Vector3 _scale = Vector3.one;

        #region Private Unserialized API

        private Vector3 _collisionMin;
        private Vector3 _collisionMax;

        #endregion

        #region Public API

        public Vector3 CollisionMin => _collisionMin;
        public Vector3 CollisionMax => _collisionMax;

        #endregion


        private void Awake()
        {

        }

        private void FixedUpdate()
        {
            _collisionMin = (transform.position - (_scale / 2)) + _position;
            _collisionMax = (transform.position + (_scale / 2)) + _position;

            foreach (BoxColliderManager collider in FindObjectsOfType<BoxColliderManager>())
            {
                if (collider != this)
                    Debug.Log(BoxCollisionTest(collider));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _position, _scale);
            Gizmos.color = Color.white;
        }

        private bool BoxCollisionTest(BoxColliderManager objToCollide)
        {
            bool xMinAxisCross = objToCollide.CollisionMin.x >= _collisionMin.x && objToCollide.CollisionMin.x <= _collisionMax.x;
            bool yMinAxisCross = objToCollide.CollisionMin.y >= _collisionMin.y && objToCollide.CollisionMin.y <= _collisionMax.y;
            bool zMinAxisCross = objToCollide.CollisionMin.z >= _collisionMin.z && objToCollide.CollisionMin.z <= _collisionMax.z;

            bool xMaxAxisCross = objToCollide.CollisionMax.x >= _collisionMin.x && objToCollide.CollisionMax.x <= _collisionMax.x;
            bool yMaxAxisCross = objToCollide.CollisionMax.y >= _collisionMin.y && objToCollide.CollisionMax.y <= _collisionMax.y;
            bool zMaxAxisCross = objToCollide.CollisionMax.z >= _collisionMin.z && objToCollide.CollisionMax.z <= _collisionMax.z;


            bool xMaxMinMinAxisCross = objToCollide.CollisionMax.x >= _collisionMin.x && objToCollide.CollisionMax.x <= _collisionMax.x;
            bool yMaxMinMinAxisCross = objToCollide.CollisionMin.y >= _collisionMin.y && objToCollide.CollisionMin.y <= _collisionMax.y;
            bool zMaxMinMinAxisCross = objToCollide.CollisionMin.z >= _collisionMin.z && objToCollide.CollisionMin.z <= _collisionMax.z;

            bool xMinMaxMinAxisCross = objToCollide.CollisionMin.x >= _collisionMin.x && objToCollide.CollisionMin.x <= _collisionMax.x;
            bool yMinMaxMinAxisCross = objToCollide.CollisionMax.y >= _collisionMin.y && objToCollide.CollisionMax.y <= _collisionMax.y;
            bool zMinMaxMinAxisCross = objToCollide.CollisionMin.z >= _collisionMin.z && objToCollide.CollisionMin.z <= _collisionMax.z;

            bool xMinMinMaxAxisCross = objToCollide.CollisionMin.x >= _collisionMin.x && objToCollide.CollisionMin.x <= _collisionMax.x;
            bool yMinMinMaxAxisCross = objToCollide.CollisionMin.y >= _collisionMin.y && objToCollide.CollisionMin.y <= _collisionMax.y;
            bool zMinMinMaxAxisCross = objToCollide.CollisionMax.z >= _collisionMin.z && objToCollide.CollisionMax.z <= _collisionMax.z;

            bool xMaxMaxMinAxisCross = objToCollide.CollisionMax.x >= _collisionMin.x && objToCollide.CollisionMax.x <= _collisionMax.x;
            bool yMaxMaxMinAxisCross = objToCollide.CollisionMax.y >= _collisionMin.y && objToCollide.CollisionMax.y <= _collisionMax.y;
            bool zMaxMaxMinAxisCross = objToCollide.CollisionMin.z >= _collisionMin.z && objToCollide.CollisionMin.z <= _collisionMax.z;

            bool xMinMaxMaxAxisCross = objToCollide.CollisionMin.x >= _collisionMin.x && objToCollide.CollisionMin.x <= _collisionMax.x;
            bool yMinMaxMaxAxisCross = objToCollide.CollisionMax.y >= _collisionMin.y && objToCollide.CollisionMax.y <= _collisionMax.y;
            bool zMinMaxMaxAxisCross = objToCollide.CollisionMax.z >= _collisionMin.z && objToCollide.CollisionMax.z <= _collisionMax.z;

            bool xMaxMinMaxAxisCross = objToCollide.CollisionMax.x >= _collisionMin.x && objToCollide.CollisionMax.x <= _collisionMax.x;
            bool yMaxMinMaxAxisCross = objToCollide.CollisionMin.y >= _collisionMin.y && objToCollide.CollisionMin.y <= _collisionMax.y;
            bool zMaxMinMaxAxisCross = objToCollide.CollisionMax.z >= _collisionMin.z && objToCollide.CollisionMax.z <= _collisionMax.z;


            bool minAxisCross = xMinAxisCross && yMinAxisCross && zMinAxisCross;
            bool maxAxisCross = xMaxAxisCross && yMaxAxisCross && zMaxAxisCross;

            bool maxMinMinAxisCross = xMaxMinMinAxisCross && yMaxMinMinAxisCross && zMaxMinMinAxisCross;
            bool minMaxMinAxisCross = xMinMaxMinAxisCross && yMinMaxMinAxisCross && zMinMaxMinAxisCross;
            bool minMinMaxAxisCross = xMinMinMaxAxisCross && yMinMinMaxAxisCross && zMinMinMaxAxisCross;
            bool maxMaxMinAxisCross = xMaxMaxMinAxisCross && yMaxMaxMinAxisCross && zMaxMaxMinAxisCross;
            bool minMaxMaxAxisCross = xMinMaxMaxAxisCross && yMinMaxMaxAxisCross && zMinMaxMaxAxisCross;
            bool maxMinMaxAxisCross = xMaxMinMaxAxisCross && yMaxMinMaxAxisCross && zMaxMinMaxAxisCross;

            return minAxisCross || maxAxisCross || maxMinMinAxisCross || minMaxMinAxisCross || minMinMaxAxisCross || maxMaxMinAxisCross || minMaxMaxAxisCross || maxMinMaxAxisCross;
        }
    }
}
