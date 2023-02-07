using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.CompleteTpsController
{
    /// <summary>
    /// Controller of player movements and actions
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Tooltip("character controller component used to move player")]
        private CharacterController _controller;

        [SerializeField, Tooltip("GameObject that represent player components in one parent object")]
        private Transform _playerComponentsPivot;

        [SerializeField, Tooltip("speed of player")]
        private Vector2 _speed = Vector2.zero;

        [SerializeField, Tooltip("avoid player to instant break velocity, 0 is disabled, 1 is permanent speed")]
        [Range(0f, 1f)]
        private float _smoothVelocityFallDown = 0;

        [SerializeField, Tooltip("set player rotation mode while walking")]
        private PlayerRotationMode _rotationMode;

        private Vector2 _inputValue = Vector2.zero;

        private float _normalizedInputMagnitudeMemo = 0;
        private Vector2 _inputValueNotNullable = Vector2.zero;

        #region Public API

        public enum PlayerRotationMode
        {
            Free,
            Clamped,
            SmoothlyClamped,
        }

        #endregion

        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        private void Awake()
        {
            Init();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// FixedUpdate is called once per Physics Update
        /// </summary>
        private void FixedUpdate()
        {
            LookDir();
            Move();
        }

        /// <summary>
        /// OnDrawGizmos is called each time scene refresh
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                Init();
        }

        /// <summary>
        /// init is called in editor mode and at awake, to ensure every variables and states are initialized and set
        /// </summary>
        private void Init()
        {
            if (_controller == null)
                if (!TryGetComponent(out _controller))
                    _controller = gameObject.AddComponent<CharacterController>();

            if (_playerComponentsPivot == null)
            {
                for (int i = 0; i < transform.childCount; i++)
                    if (transform.GetChild(i).name == "PlayerComponentsPivot")
                        _playerComponentsPivot = transform.GetChild(i);

                if (_playerComponentsPivot == null)
                    _playerComponentsPivot = new GameObject("PlayerComponentsPivot").transform.parent = transform;
            }
        }

        /// <summary>
        /// manage where player turn arround(affect movements direction !)
        /// </summary>
        private void LookDir()
        {
            if (_inputValue.magnitude != 0)
            {
                transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
                transform.eulerAngles += new Vector3(0, 180, 0);

                if (_rotationMode == PlayerRotationMode.Free)
                    _playerComponentsPivot.LookAt(new Vector3(transform.position.x + _controller.velocity.x, transform.position.y, transform.position.z + _controller.velocity.z));
                else if (_rotationMode == PlayerRotationMode.Clamped)
                    _playerComponentsPivot.localEulerAngles = Vector3.zero;
                else if (_rotationMode == PlayerRotationMode.SmoothlyClamped)
                    _playerComponentsPivot.localRotation = Quaternion.Lerp(_playerComponentsPivot.localRotation, new Quaternion(_playerComponentsPivot.localRotation.x, _inputValue.x * 0.5f, _playerComponentsPivot.localRotation.z, _playerComponentsPivot.localRotation.w), 0.2f);
            }
        }

        /// <summary>
        /// manage movements of player
        /// </summary>
        private void Move()
        {
            float normalizedInputMagnitude = Mathf.Clamp(_inputValue.magnitude, -1, 1);

            if (_normalizedInputMagnitudeMemo > normalizedInputMagnitude)
                normalizedInputMagnitude = Mathf.Lerp(normalizedInputMagnitude, _normalizedInputMagnitudeMemo, _smoothVelocityFallDown);

            _normalizedInputMagnitudeMemo = normalizedInputMagnitude;
            if (_inputValue != Vector2.zero)
                _inputValueNotNullable = _inputValue;

            Vector3 dir = ((transform.forward * _inputValueNotNullable.y) + (transform.right * _inputValueNotNullable.x)).normalized;
            _controller.Move(new Vector3(dir.x * _speed.x, dir.y, dir.z * _speed.y) * normalizedInputMagnitude);
        }

        #region Input System Functions

        /// <summary>
        /// Call by Input System, manage basic movements of player
        /// </summary>
        public void GetMove(InputAction.CallbackContext callback)
        {
            _inputValue = callback.ReadValue<Vector2>();
        }

        #endregion
    }

}