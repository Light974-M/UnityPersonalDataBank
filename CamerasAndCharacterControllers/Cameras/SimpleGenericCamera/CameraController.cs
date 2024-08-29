using UnityEngine;
#if INPUT_SYSTEM_PRESENT
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endif
using UPDB.CoreHelper;
using UPDB.CoreHelper.Usable;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleGenericCamera
{
    /// <summary>
    /// usefull generic camera controller, can be used with fps or tps controller or alone(or wathever controller you want, simply drag script into object that will be rotated)
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/CamerasAndCharacterControllers/Cameras/SimpleGenericCamera/README.md")]
    [AddComponentMenu(NamespaceID.CamerasPath + "/" + NamespaceID.SimpleGenericCamera + "/Generic Camera Controller")]
    public class CameraController : UPDBBehaviour
    {
        #region Serialized API

        /*****************************************DEFAULT*******************************************/
        [Space, Header("DEFAULT"), Space]

        [SerializeField, Tooltip("do camera use input system or native input ?")]
        private bool _inputSystem = true;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.one * 0.2f;

        [SerializeField, Tooltip("degrees of angles to clamp camera vertically")]
        private Vector2 _verticalBorders = new Vector2(-89, 89);

        [SerializeField, Tooltip("transform to rotate on y axis, null means this transform")]
        private Transform _horizontalPivot;

        [SerializeField, Tooltip("transform to rotate on x axis, null means this transform")]
        private Transform _verticalPivot;

        [SerializeField, Tooltip("if enabled, will hide the cursor while focused")]
        private bool _hideCursor = true;

        [SerializeField, Tooltip("type of constraint for mouse")]
        private CursorLockMode _cursorLockState = CursorLockMode.Locked;


        /*************************************CAMERA EFFECTS***************************************/
        [Space, Header("CAMERA EFFECTS"), Space]

        [SerializeField, Tooltip("reference of camera for every camera effects, null means that there is no camera effects")]
        private Camera _cameraFXTarget;


        /*************************************CAMERA FOV FX***************************************/
        [Space, Header("CAMERA FOV FX"), Space]

        [SerializeField, Tooltip("does camera has FOV variations ?")]
        private bool _fOVSystem = true;

        [SerializeField, Tooltip("default value of FOV, when there is no movements")]
        private float _defaultFOV = 60;

        [SerializeField, Tooltip("minimum and maximum value reachable by FOV")]
        private Vector2 _fOVMinMax = new Vector2(60, 75);

        [SerializeField, Tooltip("multiplier, the higher the value, the higher fov will increase with speed")]
        private float _fOVIntensity = 1.3f;

        [SerializeField, Tooltip("curve that give precise override values of FOV Between it's min and max value")]
        private AnimationCurve _fOVEvolutionShape;

        [SerializeField, Tooltip("clamp FOV increasing or decreasing speed")]
        private float _fOVVelocityClamp = 20;

        [SerializeField, Tooltip("make a smooth on every movements of FOV, to avoid brutal speed changing")]
        [Range(0, 1)]
        private float _fOVAccelerationClampIncrement = 0.05f;


        /*************************************CAMERA SHAKE FX***************************************/
        [Space, Header("CAMERA SHAKE FX"), Space]

        [SerializeField, Tooltip("does camera shake when velocity break down hardely and for some other effects")]
        private bool _cameraShakeSystem = false;

        [SerializeField, Tooltip("duration of one camera shake in sec")]
        private float _shakeTime = 0.1f;

        [SerializeField, Tooltip("intensity of shake, means min and max angle camera can have")]
        private float _shakeIntensity = 0.5f;

        #endregion

        #region Private API

        /*****************************************DEFAULT*******************************************/

        /// <summary>
        /// main variable to setup input value each update
        /// </summary>
        private Vector2 _inputValue = Vector2.zero;

        /// <summary>
        /// last value to make a scroll manually
        /// </summary>
        private Vector2 _inputValueMemo = Vector2.zero;

        /// <summary>
        /// used to store rotation that will be offset by input each update
        /// </summary>
        private Vector2 _rotation = Vector2.zero;

        /// <summary>
        /// current speed used by camera
        /// </summary>
        private Vector2 _currentLookSpeed = Vector2.zero;

        /// <summary>
        /// last value of _hideCursor
        /// </summary>
        private bool _hideCursorMemo = false;

        /// <summary>
        /// last value of _cursorLockState
        /// </summary>
        private CursorLockMode _cursorLockStateMemo = CursorLockMode.Locked;


        /*************************************CAMERA EFFECTS***************************************/

        private Vector3 _cameraVelocity = Vector3.zero;

        private Vector3 _cameraAcceleration = Vector3.zero;

        private float _cameraAccelerationMagnitude = 0;

        private Vector3 _cameraPosMemo = Vector3.zero;

        private Vector3 _cameraVelocityMemo = Vector3.zero;


        /*************************************CAMERA FOV FX***************************************/

        private bool _fOVSystemMemo = false;

        private float _basicFOVSave = 60;


        private float _velocityLimitedValueMemo = 60;

        private float _smoothedValue = 60;


        /*************************************CAMERA SHAKE FX***************************************/

        private bool _shakePhase = false;

        private float _shakeTimer = 0;

        private int _shakeMultiFrameTest = 0;

        private bool _isFirstConditionValidated = false;


        /******************************************DEBUG********************************************/

        private bool _isTesting = false;

        #endregion

        #region Public API

        public Vector2 LookSpeed
        {
            get { return _lookSpeed; }
            set { _lookSpeed = value; }
        }

        public Vector2 VerticalBorders
        {
            get => _verticalBorders;
            set => _verticalBorders = value;
        }
        public bool InputSystem
        {
            get { return _inputSystem; }
            set { _inputSystem = value; }
        }
        public Camera CameraFXTarget
        {
            get { return _cameraFXTarget; }
            set { _cameraFXTarget = value; }
        }
        public bool FOVSystem
        {
            get => _fOVSystem;
            set { _fOVSystem = value; }
        }
        public bool CameraShakeSystem
        {
            get => _cameraShakeSystem;
            set { _cameraShakeSystem = value; }
        }
        public float DefaultFOV
        {
            get { return _defaultFOV; }
            set { _defaultFOV = value; }
        }
        public Vector2 FOVMinMax
        {
            get => _fOVMinMax;
            set { _fOVMinMax = value; }
        }
        public float FOVIntensity
        {
            get { return _fOVIntensity; }
            set { _fOVIntensity = value; }
        }
        public AnimationCurve FOVEvolutionShape
        {
            get => _fOVEvolutionShape;
            set => _fOVEvolutionShape = value;
        }
        public float FOVVelocityClamp
        {
            get => _fOVVelocityClamp;
            set { _fOVVelocityClamp = value; }
        }
        public float FOVAccelerationClampIncrement
        {
            get => _fOVAccelerationClampIncrement;
            set { _fOVAccelerationClampIncrement = value; }
        }
        public float ShakeTime
        {
            get => _shakeTime;
            set { _shakeTime = value; }
        }
        public float ShakeIntensity
        {
            get => _shakeIntensity;
            set { _shakeIntensity = value; }
        }
        public bool IsTesting
        {
            get => _isTesting;
            set { _isTesting = value; }
        }
        public Transform HorizontalPivot
        {
            get { return _horizontalPivot; }
            set { _horizontalPivot = value; }
        }
        public Transform VerticalPivot
        {
            get { return _verticalPivot; }
            set { _verticalPivot = value; }
        }
        public bool HideCursor
        {
            get { return _hideCursor; }
            set { _hideCursor = value; }
        }
        public CursorLockMode CursorLockState
        {
            get { return _cursorLockState; }
            set { _cursorLockState = value; }
        }

        #endregion

        /// <summary>
        /// awake is called when script instace is being loaded
        /// </summary>
        private void Awake()
        {
            _currentLookSpeed = _lookSpeed;
            _smoothedValue = _defaultFOV;
            _velocityLimitedValueMemo = _defaultFOV;
            _cameraVelocityMemo = Vector3.zero;
            _cameraPosMemo = transform.position;

            Transform horizontalPivot = _horizontalPivot ? _horizontalPivot : transform;
            Transform verticalPivot = _verticalPivot ? _verticalPivot : transform;

            _rotation.x = verticalPivot.eulerAngles.x;
            _rotation.y = horizontalPivot.eulerAngles.y;

            Cursor.lockState = _cursorLockState;
            Cursor.visible = !_hideCursor;
        }

        /// <summary>
        /// update is called each frame
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance.IsPaused)
                return;

            CursorModesUpdate();

            if (GameManager.Instance.IsCharacterControllable)
                Look();

            if (_cameraFXTarget)
                CameraEffects();
        }

        private void FixedUpdate()
        {
            if (_cameraFXTarget && _fOVSystem)
                UpdateCameraVelocity();
        }

        private void CursorModesUpdate()
        {
            if (_cursorLockState != _cursorLockStateMemo)
                Cursor.lockState = _cursorLockState;

            if (_hideCursor != _hideCursorMemo)
                Cursor.visible = !_hideCursor;

            _cursorLockStateMemo = _cursorLockState;
            _hideCursorMemo = _hideCursor;
        }

        /// <summary>
        /// manage rotation of transform, following input value
        /// </summary>
        public void Look()
        {
            if (!_inputSystem)
            {
                _inputValue = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _inputValueMemo;
                _inputValueMemo = Input.mousePosition;
            }

            Transform horizontalPivot = _horizontalPivot ? _horizontalPivot : transform;
            Transform verticalPivot = _verticalPivot ? _verticalPivot : transform;

            Vector2 mouse = new Vector2(_inputValue.x * _currentLookSpeed.x, _inputValue.y * _currentLookSpeed.y);
            _rotation += new Vector2(-mouse.y, mouse.x);

            _rotation.x = Mathf.Clamp(_rotation.x, _verticalBorders.x, _verticalBorders.y);

            horizontalPivot.eulerAngles = new Vector3(horizontalPivot.eulerAngles.x, _rotation.y, 0.0f);
            verticalPivot.eulerAngles = new Vector3(_rotation.x, verticalPivot.eulerAngles.y, 0.0f);
        }

        /// <summary>
        /// called in update, manage all camera effects enabled
        /// </summary>
        private void CameraEffects()
        {
            if (_fOVSystem != _fOVSystemMemo)
            {
                if (_fOVSystem)
                    _basicFOVSave = _cameraFXTarget.fieldOfView;
                else
                    _cameraFXTarget.fieldOfView = _basicFOVSave;
            }

            _fOVSystemMemo = _fOVSystem;

            if (_fOVSystem)
                CameraFOVUpdate();

            if (_cameraShakeSystem)
                CameraShakeUpdate();
        }

        /// <summary>
        /// call by CameraEffects, manage fov of camera depending on player speed and values increment
        /// </summary>
        private void CameraFOVUpdate()
        {
            float additiveValue = (new Vector3(_cameraVelocity.x, 0, _cameraVelocity.z).magnitude * _fOVIntensity);
            float nonTransformedValue = _defaultFOV + additiveValue;
            float clampedValue = Mathf.Clamp(nonTransformedValue, _fOVMinMax.x, _fOVMinMax.y);
            float zeroToOneValue = ZeroToOneValue(clampedValue, _fOVMinMax.x, _fOVMinMax.y);
            float evaluedValue = _fOVEvolutionShape.Evaluate(zeroToOneValue);
            float minToMaxValue = FromZeroToOneValue(evaluedValue, _fOVMinMax.x, _fOVMinMax.y);
            float velocityLimitedValue = _velocityLimitedValueMemo + Mathf.Clamp(minToMaxValue - _velocityLimitedValueMemo, -_fOVVelocityClamp * Time.deltaTime, _fOVVelocityClamp * Time.deltaTime);
            _velocityLimitedValueMemo = velocityLimitedValue;
            _smoothedValue = Mathf.Lerp(_smoothedValue, velocityLimitedValue, _fOVAccelerationClampIncrement);

            float finalValue = _smoothedValue;

            //Debug.Log($"additiveValue : {additiveValue} nonTransformedValue : {nonTransformedValue} clampedValue : {clampedValue} zeroToOneValue : {zeroToOneValue} evaluedValue : {evaluedValue} minToMaxValue : {minToMaxValue}");
            _cameraFXTarget.fieldOfView = finalValue;


            float ZeroToOneValue(float value, float min, float max)
            {
                return (value - min) / (max - min);
            }

            float FromZeroToOneValue(float value, float min, float max)
            {
                return value * (max - min) + min;
            }
        }

        /// <summary>
        /// call by CameraEffects, make camera shake depending on velocity breaks and values increment
        /// </summary>
        private void CameraShakeUpdate()
        {
            if (_cameraAcceleration.y > 2)
            {
                _shakePhase = true;
                //_isFirstConditionValidated = true;
                //_shakeMultiFrameTest = 1;
            }

            //if (_cameraAccelerationMagnitude < 0)
            //{
            //    _isFirstConditionValidated = false;
            //    _shakeMultiFrameTest = 1;
            //}

            if (_shakeMultiFrameTest > 0)
            {
                if ((_isFirstConditionValidated && _cameraAccelerationMagnitude < 0) || (!_isFirstConditionValidated && _cameraAcceleration.y > 2))
                    _shakePhase = true;

                _shakeMultiFrameTest++;

                if (_shakeMultiFrameTest >= 3)
                    _shakeMultiFrameTest = 0;
            }


            if (_shakePhase)
            {
                _cameraFXTarget.transform.localEulerAngles = new Vector3(Random.Range(-_shakeIntensity, _shakeIntensity), Random.Range(-_shakeIntensity, _shakeIntensity), 0);

                if (_shakeTimer >= _shakeTime)
                {
                    _shakePhase = false;
                    _cameraFXTarget.transform.localEulerAngles = Vector3.zero;
                }

                _shakeTimer += Time.deltaTime;
            }
            else
                _shakeTimer = 0;
        }

        /// <summary>
        /// update native velocity and acceleration calculation value
        /// </summary>
        private void UpdateCameraVelocity()
        {
            _cameraVelocity = (transform.position - _cameraPosMemo) / Time.fixedDeltaTime;
            _cameraPosMemo = transform.position;
            _cameraAcceleration = _cameraVelocity - _cameraVelocityMemo;
            _cameraAccelerationMagnitude = _cameraVelocity.magnitude - _cameraVelocityMemo.magnitude;
            _cameraVelocityMemo = _cameraVelocity;
        }

#if INPUT_SYSTEM_PRESENT
        #region Event Functions

        /// <summary>
        /// public function to put input value with input system callback system
        /// </summary>
        /// <param name="callback"></param>
        public void GetLook(InputAction.CallbackContext callback)
        {
            if (_inputSystem)
                _inputValue = callback.ReadValue<Vector2>();
        }

        public void SetMouseSensitivityValue(Slider slider)
        {
            _lookSpeed = Vector2.one * slider.value;
        }

        /// <summary>
        /// called by On Scheme Change event in Player Controller, make adjustements of speed depending on controller used(avoiding one controller value to act way faster than another)
        /// </summary>
        /// <param name="playerInput"></param>
        public void MultiplySpeedValue(PlayerInput playerInput)
        {
            if (playerInput.currentControlScheme == "KeyboardAndMouse" || playerInput.currentControlScheme == null)
                _currentLookSpeed = _lookSpeed;
            else if (playerInput.currentControlScheme == "GamepadController")
                _currentLookSpeed = _lookSpeed * 25;
        }

        #endregion  
#endif
    }
}
