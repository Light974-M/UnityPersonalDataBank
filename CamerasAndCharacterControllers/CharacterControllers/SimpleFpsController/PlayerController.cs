using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.Animations;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.SimpleFpsController
{
    public class PlayerController : UPDBBehaviour
    {
        #region Serialized API

        [SerializeField]
        private CharacterController _charaController;


        /***************************INPUT PARAMETERS*****************************/

        [SerializeField]
        private Vector2 _moveSpeed = Vector3.one;

        [SerializeField]
        private float _jumpStrength = 100;

        /********************************EVENTS**********************************/
        [Space, Header("EVENTS"), Space]

        [SerializeField, Tooltip("event triggered when user scheme changes")]
        private UnityEvent _onSchemeChange;

        #endregion

        #region Private API


        /***************************INPUT PARAMETERS*****************************/
        private PlayerInput _playerInput;
        private Vector3 _moveInputValue = Vector3.zero;
        private Vector3 _velocity = Vector3.zero;

        /********************************EVENTS**********************************/
        /// <summary>
        /// keep in mind last active scheme, used to launch onSchemeChange event
        /// </summary>
        private string _schemeMemo = "";

        #endregion

        #region Public API



        #endregion

        // Start is called before the first frame update
        private void Awake()
        {
            if (!_playerInput)
                if (!TryGetComponent(out _playerInput))
                    _playerInput = gameObject.AddComponent<PlayerInput>();

            if (!_charaController)
                if (!TryGetComponent(out _charaController))
                    _charaController = gameObject.AddComponent<CharacterController>();
        }

        private void Update()
        {
            OnLandMove();

            JumpVelocityUpdate();

            Gravitymanager();

            //if scheme(controller) change, call an event to reorganise controller
            if (_playerInput.currentControlScheme != _schemeMemo)
                _onSchemeChange.Invoke();

            //register last input scheme
            _schemeMemo = _playerInput.currentControlScheme;
        }


        /***************************************CUSTOM FUNCTIONS*****************************************/

        private void OnLandMove()
        {
            Vector2 normalizedInput = _moveInputValue.normalized;
            Vector2 multipliedInput = new Vector2(normalizedInput.x * _moveSpeed.x, normalizedInput.y * _moveSpeed.y);

            Vector3 motion = transform.right * multipliedInput.x + transform.forward * multipliedInput.y;
            _charaController.Move(motion * Time.deltaTime);
        }

        private void JumpVelocityUpdate()
        {
            _charaController.Move(_velocity);
            _velocity.y = _velocity.y > 0 ? _velocity.y - Time.deltaTime : 0;
        }

        private void Gravitymanager()
        {
            _charaController.SimpleMove(Vector3.zero);
        }

        #region EventsCallback

        public void GetMove(InputAction.CallbackContext callback)
        {
            _moveInputValue = callback.ReadValue<Vector2>();
        }

        public void OnJumpAction(InputAction.CallbackContext callback)
        {
            if (_charaController.isGrounded)
                _velocity.y = _jumpStrength * Time.deltaTime;
        }


        #endregion
    }
}
