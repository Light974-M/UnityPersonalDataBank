using System.Collections.Generic;
using UnityEngine;
using UPDB.CamerasAndCharacterControllers.Cameras.SimpleFpsCamera;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.ProceduralTpsController
{
    ///<summary>
    /// 
    ///</summary>
    [HelpURL(URL.baseURL + "/tree/main/CamerasAndCharacterControllers/CharacterControllers/ProceduralTpsController/README.md"), AddComponentMenu("UPDB/CamerasAndCharacterControllers/CharacterControllers/Procedural Tps Controller")]
    public class PlayerController : MonoBehaviour
    {
        #region Serialized And Public Variables

        [SerializeField, Tooltip("rigidBody used to calculate player movements")]
        private Rigidbody _rb;

        [SerializeField, Tooltip("collider Rigidbody will use to calculate player movements")]
        private Collider _collider;

        [SerializeField, Tooltip("speed of character movements")]
        private float _speed = 1;

        [SerializeField, Tooltip("speed of character movements")]
        private float _smoothness = 1;

        [SerializeField, Tooltip("camera pivot(at center of player)")]
        private Transform _cameraPivot;

        [SerializeField, Tooltip("camera")]
        private Camera _camera;

        [SerializeField, Tooltip("list of all points that can be moved for animation")]
        private List<Transform> _meshList;


        #endregion

        #region Private Variables



        #endregion

        #region Accessors

        /// <inheritdoc cref="_rb"/>
        public Rigidbody Rb
        {
            get { return _rb; }
            set { _rb = value; }
        }

        public Collider Collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Transform CameraPivot
        {
            get { return _cameraPivot; }
            set { _cameraPivot = value; }
        }

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        public List<Transform> MeshList
        {
            get { return _meshList; }
            set { _meshList = value; }
        }

        #endregion


        private void Awake()
        {
            InitVariables();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_rb != null)
                Move();
            else
                Debug.LogWarning("warning : there is no rigidbody attached to this Component");

            AnimationMove();
        }

        public void InitVariables()
        {
            if (_rb == null)
            {
                if (!TryGetComponent(out _rb))
                {
                    _rb = gameObject.AddComponent<Rigidbody>();
                    _rb.constraints = RigidbodyConstraints.FreezeRotation;
                    _rb.angularDrag = 0;
                    _rb.drag = 10;
                }

            }
            GameObject camObject;

            if (!transform.Find("Camera"))
            {
                camObject = new GameObject("Camera");
                camObject.transform.SetParent(transform);
                camObject.transform.localPosition = new Vector3(0, 0.5f, 0.48f);
                camObject.transform.localEulerAngles = Vector3.zero;
                camObject.AddComponent<CameraController>();
                camObject.AddComponent<AudioListener>();
            }

            if (_cameraPivot == null)
                if (!TryGetComponent(out _cameraPivot))
                    _cameraPivot = Instantiate(new GameObject("_cameraPivot")).transform;

            if (_camera == null)
                if (!TryGetComponent(out _camera))
                    _camera = gameObject.AddComponent<Camera>();
        }

        private void Move()
        {
            _cameraPivot.eulerAngles = new Vector3(0, _cameraPivot.eulerAngles.y, 0);
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 move = (_cameraPivot.right * input.x + _cameraPivot.forward * input.y).normalized;
            float speedMultiplier = (_speed * 5000);

            _rb.AddForce(move * speedMultiplier * Time.deltaTime);
            _rb.AddForce(-_rb.velocity.x * _smoothness, 0, -_rb.velocity.z * _smoothness);

            //_rb.drag = Mathf.Clamp(_rb.drag, 0, 50);
        }

        private void AnimationMove()
        {
            _meshList[1].position = _meshList[0].position + _rb.velocity;
            _meshList[0].LookAt(_meshList[1]);
            _meshList[0].eulerAngles = new Vector3( 0, _meshList[0].eulerAngles.y, 0);
        }
    } 
}
