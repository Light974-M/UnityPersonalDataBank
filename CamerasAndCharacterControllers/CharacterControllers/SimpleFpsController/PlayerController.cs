using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.SimpleFpsController
{
    public class PlayerController : Singleton<PlayerController>
    {
        #region Serialized API

        [SerializeField]
        private CharacterController _charaController;


        /***************************INPUT PARAMETERS*****************************/
        [Space, Header("INPUT PARAMETERS"), Space]

        [SerializeField]
        private Vector2 _moveSpeed = Vector3.one;

        [SerializeField]
        private float _jumpStrength = 100;

        [SerializeField]
        private float _jumpFallofSpeed = 20;

        /********************************EVENTS**********************************/
        [Space, Header("EVENTS"), Space]

        [SerializeField, Tooltip("event triggered when user scheme changes")]
        private UnityEvent _onSchemeChange;

        /***************************PHYSIC INTERACTION PARAMETERS*****************************/
        [Space, Header("PHYSIC INTERACTION PARAMETERS"), Space]

        [SerializeField]
        private float _pushPhysicedObjectStrength = 1;

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
        protected override void Awake()
        {
            base.Awake();

            if (!_playerInput)
                if (!TryGetComponent(out _playerInput))
                    _playerInput = gameObject.AddComponent<PlayerInput>();

            if (!_charaController)
                if (!TryGetComponent(out _charaController))
                    _charaController = gameObject.AddComponent<CharacterController>();
        }

        private void Update()
        {
            if (GameManager.Instance.IsCharacterControllable)
                OnLandMove();

            JumpVelocityUpdate();

            Gravitymanager();

            if (_charaController.isGrounded == true)
                _velocity.y = 0;

            //if scheme(controller) change, call an event to reorganise controller
            if (_playerInput.currentControlScheme != _schemeMemo)
                _onSchemeChange.Invoke();

            //register last input scheme
            _schemeMemo = _playerInput.currentControlScheme;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject)
            {
                if (hit.gameObject.tag == "PushableObject" && hit.gameObject.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce((hit.gameObject.transform.position - transform.position).normalized * _pushPhysicedObjectStrength);
                }
            }
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
            _charaController.Move(_velocity * Time.deltaTime);
            _velocity.y = _velocity.y > 0 ? _velocity.y - Time.deltaTime * _jumpFallofSpeed : 0;
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
            if (callback.started)
            {
                if (_charaController.isGrounded && GameManager.Instance.IsCharacterControllable)
                    _velocity.y = _jumpStrength; 
            }
        }


        #endregion
    }
}
