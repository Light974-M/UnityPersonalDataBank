using UnityEngine;
using UPDB.CoreHelper.UsableMethods;
using UPDB.CoreHelper;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleGenericCamera
{
    [ExecuteAlways, AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.CamerasAndCharacterControllers + "/" + NamespaceID.Cameras + "/" + NamespaceID.SimpleGenericCamera + "/Spring Arm")]
    public class SpringArm : UPDBBehaviour
    {
        #region Serialized API

        [SerializeField, Tooltip("")]
        private bool _armEnabled = true;

        [Space]
        [Header("Follow Settings \n--------------------")]
        [Space]

        [SerializeField, Tooltip("Tagret camera to move, by default, child of this transform")]
        private Camera _targetCamera;

        [SerializeField, Tooltip("setup length of arm, and so, distance of camera")]
        private float _targetArmLength = 3f;

        [SerializeField, Tooltip("offset for camera target, also offset colliders and raycast")]
        private Vector2 _cameraOffset = new Vector2(0, 0.6f);

        [Space]
        [Header("Collision Settings \n-----------------------")]
        [Space]


        [SerializeField, Tooltip("number of raycast iterations")]
        [Range(2, 20)]
        private int _collisionTestResolution = 4;

        [SerializeField, Tooltip("size of sphere collider arround camera")]
        private float _collisionProbeSize = 0.3f;

        [SerializeField, Tooltip("time for camera to go to a point to another(higher values = more cinematic but less reactive)")]
        private float _smoothness = 0.05f;

        [SerializeField, Tooltip("collision layers calculated by camera arm")]
        private LayerMask _collisionLayerMask = ~0;

        [Space]
        [Header("Debugging \n--------------")]
        [Space]

        [SerializeField, Tooltip("is editor render debugging tools")]
        private bool _visualDebugging = true;

        [SerializeField, Tooltip("color of spring arm and raycasts in scene view")]
        private Color _springArmColor = new Color(0.75f, 0.2f, 0.2f, 0.75f);

        [SerializeField, Tooltip("width of spring arm and raycasts in scene view")]
        [Range(1f, 25f)]
        private float _springArmLineWidth = 6f;

        [SerializeField, Tooltip("is scene view render every detailled raycast of arm instead of just a line ?")]
        private bool _showRaycasts = false;

        #endregion

        #region Private API

        /// <summary>
        /// end position of string arm
        /// </summary>
        private Vector3 _endPoint;

        /// <summary>
        /// position of main sphere collision of camera
        /// </summary>
        private Vector3 _socketPosition;

        /// <summary>
        /// list of hits for raycast of spring arm
        /// </summary>
        private RaycastHit[] _hits;

        /// <summary>
        /// container for raycast positions
        /// </summary>
        private Vector3[] _raycastPositions;

        /// <summary>
        /// refs for SmoothDamping speed
        /// </summary>
        private Vector3 _moveVelocity;

        /// <summary>
        /// refs for SmoothDamping collision test
        /// </summary>
        private Vector3 _collisionTestVelocity;

        #endregion

        #region Public API

        public bool ArmEnabled
        {
            get { return _armEnabled; }
            set { _armEnabled = value; }
        }

        #endregion

        private void Awake()
        {
            if (!_targetCamera)
                if (transform.childCount == 0 || !transform.GetChild(0).TryGetComponent(out _targetCamera))
                    _targetCamera = new GameObject("Camera").AddComponent<Camera>();

            _raycastPositions = new Vector3[_collisionTestResolution];
            _hits = new RaycastHit[_collisionTestResolution];
        }

        private void Update()
        {
            // Collision check
            if (_armEnabled)
                CheckCollisions();

            // set the socketPosition
            SetSocketTransform();
        }

        private void OnValidate()
        {
            _raycastPositions = new Vector3[_collisionTestResolution];
            _hits = new RaycastHit[_collisionTestResolution];
        }

        protected override void OnSceneSelected()
        {
            if (!_targetCamera)
                if (transform.childCount == 0 || !transform.GetChild(0).TryGetComponent(out _targetCamera))
                    _targetCamera = new GameObject("Camera").AddComponent<Camera>();


            if (!_visualDebugging)
                return;

#if UNITY_EDITOR
            //Draw main LineTrace or LineTraces of RaycastPositions, useful for debugging

            Handles.color = _springArmColor;

            if (_showRaycasts)
                foreach (Vector3 raycastPosition in _raycastPositions)
                    Handles.DrawAAPolyLine(_springArmLineWidth, 2, transform.position, raycastPosition);
            else
                Handles.DrawAAPolyLine(_springArmLineWidth, 2, transform.position, _endPoint);

            //Draw collisionProbe, useful for debugging

            Handles.color = new Color(0.2f, 0.75f, 0.2f, 0.15f);

            Handles.SphereHandleCap(0, _socketPosition, Quaternion.identity, 2 * _collisionProbeSize, EventType.Repaint);
#endif
        }

        /// <summary>
        /// Checks for collisions and fill the raycastPositions and hits array
        /// </summary>
        private void CheckCollisions()
        {
            // iterate through raycastPositions and hits and set the corresponding data
            for (int i = 0, angle = 0; i < _collisionTestResolution; i++, angle += 360 / _collisionTestResolution)
            {
                // Calculate the local position of a point w.r.t angle
                Vector3 raycastLocalEndPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * _collisionProbeSize;

                // Convert it to world space by offsetting it by origin: endPoint, and push in the array
                _raycastPositions[i] = _endPoint + (transform.rotation * raycastLocalEndPoint);

                // Sets the hit struct if collision is detected between this gameobject's position and calculated raycastPosition
                Physics.Linecast(transform.position, _raycastPositions[i], out _hits[i], _collisionLayerMask);
            }
        }

        /// <summary>
        /// Sets the translation of children according to filled raycastPositions and hits array data
        /// </summary>
        private void SetSocketTransform()
        {
            // offset a point in z direction of targetArmLength by socket offset and translating it into world space.
            Vector3 targetArmOffset = new Vector3(_cameraOffset.x, _cameraOffset.y, -_targetArmLength);
            _endPoint = transform.position + (transform.rotation * targetArmOffset);

            // if collisionTest is enabled
            if (_armEnabled)
            {
                // finds the minDistance
                float minDistance = _targetArmLength;
                foreach (RaycastHit hit in _hits)
                {
                    if (!hit.collider)
                        continue;

                    float distance = Vector3.Distance(hit.point, transform.position);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                    }
                }

                // calculate the direction of children movement
                Vector3 dir = (_endPoint - transform.position).normalized;
                // get vector for movement
                Vector3 armOffset = dir * (_targetArmLength - minDistance);
                // offset it by endPoint and set the socketPositionValue
                _socketPosition = _endPoint - armOffset;
            }
            // if collision is disabled
            else
            {
                // set socketPosition value as endPoint
                _socketPosition = _endPoint;
            }

            _targetCamera.transform.position = Vector3.SmoothDamp(_targetCamera.transform.position, _socketPosition, ref _collisionTestVelocity, _smoothness);
        }
    }
}
