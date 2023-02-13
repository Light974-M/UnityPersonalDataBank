using UnityEngine;
using UnityEngine.InputSystem;

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleGenericCamera
{
    /// <summary>
    /// usefull generic camera controller, can be used with fps or tps controller or alone(or wathever controller you want, simply drag script into object that will be rotated)
    /// </summary>
    [AddComponentMenu("UPDB/CamerasAndCharacterControllers/Cameras/SimpleGenericCamera/Generic Camera Controller")]
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Tooltip("choose the input mode")]
        private InputMode _input;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.one;

        #region Private API

        /// <summary>
        /// main variable to setup input value each update
        /// </summary>
        private Vector2 _inputValue = Vector2.zero;

        /// <summary>
        /// used to store rotation that will be offset by input each update
        /// </summary>
        private Vector2 _rotation = Vector2.zero;

        #endregion

        #region Public API

        public Vector2 InputValue
        {
            get { return _inputValue; }
            set { _inputValue = value; }
        }

        public enum InputMode
        {
            Standalone,
            InputSystem,
        }

        #endregion

        /// <summary>
        /// awake is called when script instance is loaded
        /// </summary>
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// update is called each frame
        /// </summary>
        private void Update()
        {
            if (_input == InputMode.Standalone)
                _inputValue = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            Look();
        }

        /// <summary>
        /// manage rotation of transform, following input value
        /// </summary>
        private void Look()
        {
            Vector2 mouse = new Vector2(_inputValue.x * _lookSpeed.x, _inputValue.y * _lookSpeed.y);
            _rotation += new Vector2(-mouse.y, mouse.x);

            _rotation.x = Mathf.Clamp(_rotation.x, -89, 89);

            transform.eulerAngles = new Vector3(_rotation.x, _rotation.y, 0.0f);

            _inputValue = Vector2.zero;
        }

        /// <summary>
        /// public function to put input value with input system callback system
        /// </summary>
        /// <param name="callback">value of input system callback, represent a Vector2(for mouse x and y)</param>
        public void GetLook(InputAction.CallbackContext callback)
        {
            if (_input == InputMode.InputSystem)
                _inputValue = callback.ReadValue<Vector2>();
        }
    }
}
