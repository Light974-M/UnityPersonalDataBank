using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.Usable;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.PlacementTools
{
    /// <summary>
    /// component that make the affected object automatically stick to a surface, helpful to place enemies and decorations easily
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/Physics/PlacementTools/README.md")]
    [AddComponentMenu(NamespaceID.PhysicPath + "/" + NamespaceID.PlacementTools + "/Stick To Surface")]
    public class StickToSurface : UPDBBehaviour
    {
        [SerializeField, Tooltip("search for a surface and stick to it")]
        private bool _stickToSurface = false;

        [SerializeField, Tooltip("stick to surface when object move")]
        private bool _stickOnMove = false;

        [SerializeField, Tooltip("distance that trigger the onMoveStick")]
        private float _moveStep = 1;

        [Space, Header("STICK PARAMETERS"), Space]
        [SerializeField, Tooltip("range of stick")]
        private float _stickRange = 5;

        [SerializeField, Tooltip("wich transform is used as the perpendicular transform")]
        private Direction _perpendicularDirection = Direction.Up;

        [SerializeField, Tooltip("offset of sticked position")]
        private Vector3 _offsetPos = Vector3.zero;

        [Space, Header("SEARCHING PARAMETERS"), Space]
        [SerializeField, Tooltip("surface searching precision, number of raycast thrown")]
        private int _surfaceSearchingPrecision = 10;

        [SerializeField, Tooltip("what layers are considered surface ?")]
        private LayerMask _includeLayers;

        [SerializeField, Tooltip("list of all objects ignored by component")]
        private Collider[] _excludeColliders;

        [Space, Header("DEBUG PARAMETERS"), Space]
        [SerializeField, Tooltip("display raycast used when is searching for surface")]
        private bool _displayRaycastDebug = false;

        [SerializeField, Tooltip("when does sphere of stick surface display ?")]
        private DisplayEditor _displayRangeOfStick;

        private Vector3 _lastStickedPos = Vector3.zero;

        protected override void OnScene()
        {
            if (_stickToSurface)
            {
                _stickToSurface = false;

                OnStickSurface();
            }

            if(_stickOnMove)
            {
                float distanceStep = Vector3.Distance(transform.position, _lastStickedPos);

                if (distanceStep > _moveStep)
                    OnStickSurface();
            }

            if (_displayRangeOfStick == DisplayEditor.OnScene)
                    DebugRangeSphere();

            if (_displayRaycastDebug)
                DebugRaycasts();
        }

        protected override void OnSceneSelected()
        {
            if (_displayRangeOfStick == DisplayEditor.OnSceneSelected)
                DebugRangeSphere();
        }

        private void OnStickSurface()
        {
            Collider[] collidersInRange = SearchForSurfaces();

            if (collidersInRange != null && collidersInRange.Length > 0)
                SetObjectTransform();
        }

        private Collider[] SearchForSurfaces()
        {
            Collider[] collidersArray = Physics.OverlapSphere(transform.position, _stickRange, _includeLayers);

            if (collidersArray.Length == 0)
                return null;

            List<Collider> collidersList = new List<Collider>();
            gameObject.TryGetComponent(out Collider selfCol);

            foreach (Collider collider in collidersArray)
            {
                if ((selfCol && collider == selfCol) || _excludeColliders.Contains(collider))
                    continue;

                collidersList.Add(collider);
            }

            return collidersList.ToArray();
        }

        private Vector3 SearchNearestPoint(out Collider collidedObj)
        {
            int length = Mathf.RoundToInt(Mathf.Sqrt(_surfaceSearchingPrecision));

            List<List<Vector3>> posLists = new List<List<Vector3>>();
            List<Vector3> posList = new List<Vector3>();
            List<Collider> colliderPosList = new List<Collider>();

            float angle = 0;

            for (int i = 0; i < length; i++)
            {
                Vector3 dir = Vector3.forward;
                posLists.Add(new List<Vector3>());

                for (int j = 0; j < length; j++)
                {
                    Vector3 rotatedDir = RotateVector(Vector3.up, dir, angle);
                    posLists[i].Add(transform.position + rotatedDir * _stickRange);

                    bool hitted = Physics.Raycast(transform.position, rotatedDir, out RaycastHit hit, _stickRange, _includeLayers);

                    if (hitted && !_excludeColliders.Contains(hit.collider))
                    {
                        posList.Add(hit.point);
                        colliderPosList.Add(hit.collider);
                    }

                    dir = RotateVector(dir, Vector3.up, 360 / (float)length);
                }

                angle += 180 / (float)length;
            }

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    bool hitted = Physics.Raycast(posLists[i][j], posLists[i][LoopClamp(j + 1, 0, length - 1)] - posLists[i][j], out RaycastHit hit, _stickRange, _includeLayers);

                    if (hitted && !_excludeColliders.Contains(hit.collider))
                    {
                        posList.Add(hit.point);
                        colliderPosList.Add(hit.collider);
                    }
                }
            }

            for (int i = 0; i < length - 1; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    bool hitted = Physics.Raycast(posLists[i][j], posLists[i + 1][j] - posLists[i][j], out RaycastHit hit, _stickRange, _includeLayers);

                    if (hitted && !_excludeColliders.Contains(hit.collider))
                    {
                        posList.Add(hit.point);
                        colliderPosList.Add(hit.collider);
                    }
                }
            }

            if (posList.Count == 0)
            {
                Debug.LogWarning("Warning : the precision of raycast wasn't good enough to detect the colliders, if you still want to detect the current collider(s) please higher the raycast precision,");
                collidedObj = null;
                return Vector3.zero;
            }

            int nearestPointIndex = 0;

            for (int i = 0; i < posList.Count; i++)
                if (Vector3.Distance(transform.position, posList[i]) < Vector3.Distance(transform.position, posList[nearestPointIndex]))
                    nearestPointIndex = i;

            collidedObj = colliderPosList[nearestPointIndex];
            return posList[nearestPointIndex];
        }

        private void SetObjectTransform()
        {
            Vector3 posToGo = SearchNearestPoint(out Collider collidedObj);
            _lastStickedPos = posToGo;

            if (_perpendicularDirection == Direction.Up)
            {
                transform.up = transform.position - posToGo;
                transform.position = posToGo;

                transform.position += transform.right * _offsetPos.x + transform.up * _offsetPos.y + transform.forward * _offsetPos.z;

                return;
            }

            if (_perpendicularDirection == Direction.Forward)
            {
                transform.forward = transform.position - posToGo;
                transform.position = posToGo;

                transform.position += transform.right * _offsetPos.x + transform.up * _offsetPos.y + transform.forward * _offsetPos.z;

                return;
            }

            if (_perpendicularDirection == Direction.Right)
            {
                transform.right = transform.position - posToGo;
                transform.position = posToGo;

                transform.position += transform.right * _offsetPos.x + transform.up * _offsetPos.y + transform.forward * _offsetPos.z;

                return;
            }

            if (_perpendicularDirection == Direction.Down)
            {
                transform.up = posToGo - transform.position;
                transform.position = posToGo;

                transform.position += transform.right * _offsetPos.x + transform.up * _offsetPos.y + transform.forward * _offsetPos.z;

                return;
            }

            if (_perpendicularDirection == Direction.Backward)
            {
                transform.forward = posToGo - transform.position;
                transform.position = posToGo;

                transform.position += transform.right * _offsetPos.x + transform.up * _offsetPos.y + transform.forward * _offsetPos.z;

                return;
            }

            if (_perpendicularDirection == Direction.Left)
            {
                transform.right = posToGo - transform.position;
                transform.position = posToGo;

                transform.position += transform.right * _offsetPos.x + transform.up * _offsetPos.y + transform.forward * _offsetPos.z;

                return;
            }
        }

        private void DebugRaycasts()
        {
            int length = Mathf.RoundToInt(Mathf.Sqrt(_surfaceSearchingPrecision));
            List<List<Vector3>> posLists = new List<List<Vector3>>();

            float angle = 0;
            for (int i = 0; i < length; i++)
            {
                Vector3 dir = Vector3.forward;
                posLists.Add(new List<Vector3>());

                for (int j = 0; j < length; j++)
                {
                    Vector3 rotatedDir = RotateVector(Vector3.up, dir, angle);
                    Debug.DrawRay(transform.position, rotatedDir * _stickRange, Color.red);

                    posLists[i].Add(transform.position + rotatedDir * _stickRange);

                    dir = RotateVector(dir, Vector3.up, 360 / (float)length);
                }

                angle += 180 / (float)length;
            }

            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    Debug.DrawRay(posLists[i][j], posLists[i][LoopClamp(j + 1, 0, length - 1)] - posLists[i][j], Color.red);

            for (int i = 0; i < length - 1; i++)
                for (int j = 0; j < length; j++)
                    Debug.DrawRay(posLists[i][j], posLists[i + 1][j] - posLists[i][j], Color.red);
        }

        private void DebugRangeSphere()
        {
            Color color = Gizmos.color;
            Gizmos.color = Color.black;

            Gizmos.DrawWireSphere(transform.position, _stickRange);

            Gizmos.color = color;
        }
    }

}