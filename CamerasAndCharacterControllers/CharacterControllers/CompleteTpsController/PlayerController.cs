using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.CompleteTpsController
{
    /// <summary>
    /// Tps Controller of player movements and actions
    /// </summary>
    [AddComponentMenu("UPDB/CamerasAndCharacterControllers/CharacterControllers/CompleteTpsController/Complete Tps Controller")]
    public class PlayerController : MonoBehaviour
    {
        #region Serialized API

        /*****************************BASE REFERENCES****************************/
        [Header("BASE REFERENCES"), Space]

        [SerializeField, Tooltip("used player input, used to link input system")]
        private PlayerInput _playerInput;

        [SerializeField, Tooltip("character controller component used to move player")]
        private CharacterController _controller;

        [SerializeField, Tooltip("GameObject that represent player components in one parent object")]
        private Transform _playerTargetPivot;

        /*********************************ROTATION********************************/
        [Space, Header("ROTATION"), Space]

        [SerializeField, Tooltip("set player rotation mode while walking")]
        private PlayerRotationMode _rotationMode = PlayerRotationMode.Free;

        [SerializeField, Tooltip("speed of player to turn arround")]
        private float _rotationSpeed = 0.1f;

        [SerializeField, Tooltip("wich shape will take rotation, used to smooth rotations")]
        private AnimationCurve _rotationShape = AnimationCurve.Linear(0, 0, 1, 1);

        /*******************************VELOCITY*********************************/
        [Space, Header("VELOCITY"), Space]

        [SerializeField, Tooltip("speed of player")]
        private Vector2 _speed = new Vector2(5, 5);

        [SerializeField, Tooltip("speed of player while running")]
        private Vector2 _sprintSpeed = new Vector2(10, 10);

        [SerializeField, Tooltip("speed of player while walking")]
        private Vector2 _walkSpeed = new Vector2(2, 2);

        [SerializeField, Tooltip("time to make a transition between normal, sprint, or walk speed")]
        private float _speedLerpTime = 1;

        [SerializeField, Tooltip("avoid player to instant break velocity, 0 is disabled, 1 is permanent speed")]
        [Range(0f, 1f)]
        private float _smoothVelocityFallDown = 0.9f;

        [SerializeField, Tooltip("avoid player to instant get max velocity, 0 is disabled, 1 is no speed")]
        [Range(0f, 1f)]
        private float _smoothVelocityGetHigh = 0.9f;

        /****************************JUMP AND PHYSIC*****************************/
        [Space, Header("JUMP AND PHYSIC"), Space]

        [SerializeField, Tooltip("what layer should be considered as player ?")]
        private LayerMask _playerLayer;

        [SerializeField, Tooltip("determine how jump will be done, different modes are available")]
        private JumpModeAction _jumpMode = JumpModeAction.PressToJumpAndHoldToControl;

        [SerializeField, Tooltip("determine time for player to reach max fall speed")]
        private float _gravityHardeness = 1;

        [SerializeField, Tooltip("strength of player jump")]
        private float _jumpStrength = 10;

        [SerializeField, Tooltip("number of physic frame that will be applied to move player")]
        private int _jumpSubdivision = 500;

        [SerializeField, Tooltip("evolution of jump strength over time")]
        private AnimationCurve _jumpCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField, Tooltip("determine control of player in air, 0 means no air controls, 1 means full controls like on ground")]
        [Range(0, 1)]
        private float _airControl = 0.25f;

        [SerializeField, Tooltip("determine control of player rotation in air, 0 means no rotationd, 1 means full rotations like on ground")]
        [Range(0, 1)]
        private float _rotationAirControl = 0;

        [SerializeField, Tooltip("give position of isGrounded check")]
        private Vector3 _isGroundedPosition = new Vector3(0, -0.9f, 0);

        [SerializeField, Tooltip("give diameter of isGrounded check")]
        private float _isGroundedScale = 0.25f;

        /*********************************CROUCH*********************************/
        [Space, Header("CROUCH"), Space]

        [SerializeField, Tooltip("normal height of player")]
        private float _normalHeight = 1.777f;

        [SerializeField, Tooltip("crouched height of player")]
        private float _crouchHeight = 1f;

        [SerializeField, Tooltip("time in second for player to crouch and uncrounch")]
        private float _crouchTime = 0.5f;

        /*****************************INPUT OFFSET*******************************/
        [Space, Header("INPUT OFFSET"), Space]

        [SerializeField, Tooltip("allow you to set a time between user input and action lauching, to set animations on the right rythm, or to make smooth and realistic systems")]
        private InvokeTime _actionLaunchTime = new InvokeTime(0);

        /********************************EVENTS**********************************/
        [Space, Header("EVENTS"), Space]

        [SerializeField, Tooltip("event triggered when user scheme changes")]
        private UnityEvent _onSchemeChange;

        #endregion

        #region Private API

        /*************************************MOVEMENTS************************************/

        /// <summary>
        /// Value of Input that will be set in input system functions
        /// </summary>
        private Vector2 _inputValue = Vector2.zero;

        /// <summary>
        /// final ibput value, with added constraints of slow down and speed up
        /// </summary>
        private float _normalizedInputMagnitude = 0;

        /// <summary>
        /// used to keep last update of movements input magnitude normalized, and compare it to actual normalized input magnitude
        /// </summary>
        private float _normalizedInputMagnitudeMemo = 0;

        /// <summary>
        /// value used to move player, make a version of input value that will not be updated if input is null, making character stand the same position when user stop moving
        /// </summary>
        private Vector2 _inputValueNotNullable = Vector2.zero;

        /**************************************VELOCITY*************************************/

        /// <summary>
        /// speed at wich character has to be every frame(overrided by a smooth lerp)
        /// </summary>
        private Vector2 _currentSpeed = Vector2.one;

        /// <summary>
        /// last update of current speed, to compare and see if speed goal of player has changed
        /// </summary>
        private Vector2 _currentSpeedMemo = Vector2.one;

        /// <summary>
        /// start point of lerp for smooth speed transitions
        /// </summary>
        private Vector2 _sprintTransitionStartPoint = Vector2.one;

        /// <summary>
        /// timer used to make lerp of smooth speed transitions
        /// </summary>
        private float _sprintTransitionTimer = 0;

        /// <summary>
        /// float used to manage combined input of walking and sprinting
        /// </summary>
        private bool _walkFlipFlop = true;

        /// <summary>
        /// true when speed is on sprint mode
        /// </summary>
        private bool _isSprinting = false;

        /**************************************ROTATION*************************************/

        /// <summary>
        /// current speed used to make lerp with rotation
        /// </summary>
        private float _currentRotationSpeed = 0;

        /// <summary>
        /// timer used to make a smooth version of input value, to apply it to rotations, and make a smooth rotation effect
        /// </summary>
        private float _rotationLerpTimer = 0;

        /// <summary>
        /// start point of lerp for smooth rotation, update itself every time the goal is changing
        /// </summary>
        private Vector2 _rotationLerpStart = Vector2.zero;

        /// <summary>
        /// smooth version of input value, used in rotations, to make a smooth turning effect
        /// </summary>
        private Vector2 _smoothedInputValue = Vector2.zero;

        /**********************************JUMP AND PHYSIC**********************************/

        /// <summary>
        /// act like a timer, determine a value between 0 and 1, 0 means player is just started to fall, 1 means player has reach his max speed
        /// </summary>
        private float _gravityVelocity = 0;

        /// <summary>
        /// is player touching ground with feets
        /// </summary>
        private bool _isGrounded = false;

        /// <summary>
        /// used to animate kneeling phase of jump
        /// </summary>
        private float _jumpTimer = 0;

        /// <summary>
        /// give input information on jump when kneeling
        /// </summary>
        private bool _jumpInput = false;

        /// <summary>
        /// is currently jumping(kneeling excluded)
        /// </summary>
        private bool _jumpPhase = false;

        /// <summary>
        /// give strength of jump depending on how high jump is
        /// </summary>
        private float _jumpFactor = 0;

        /// <summary>
        /// timer used to cut jump forces on multiple frames
        /// </summary>
        private int _jumpSubdivisionTimer = 0;

        /// <summary>
        /// used to take frame where player is on ground but will be in air next frames, working only one time for a jump
        /// </summary>
        private bool _jumpGroundOut = false;

        /*********************************CROUCH*********************************/

        /// <summary>
        /// timer used for lerp transition of crouch height
        /// </summary>
        private float _crouchLerpTimer = 0;

        /// <summary>
        /// end point used for lerp transition of crouch height
        /// </summary>
        private float _targetHeight = 1.777f;

        /// <summary>
        /// start point used for lerp transition of crouch height
        /// </summary>
        private float _startHeight = 1.777f;

        /// <summary>
        /// is player currently crouched or in a crouch transition
        /// </summary>
        private bool _isCrouched = false;

        /// <summary>
        /// give real time crouch input
        /// </summary>
        private bool _lastCrouchInputInAir = false;

        /// <summary>
        /// triggered when user press crouch input in air
        /// </summary>
        private bool _crouchInAirTrigger = false;

        /***************************************OTHER***************************************/

        /// <summary>
        /// keep in mind last active scheme, used to launch onSchemeChange event
        /// </summary>
        private string _schemeMemo = "";

        private bool _workingWithoutGameManager = false;

        #endregion

        #region Public API

        ///<inheritdoc cref="_isGrounded"/>
        public bool IsGrounded => _isGrounded;

        ///<inheritdoc cref="_jumpPhase"/>
        public bool JumpPhase => _jumpPhase;

        ///<inheritdoc cref="_jumpInput"/>
        public bool JumpInput => _jumpInput;

        ///<inheritdoc cref="_isCrouched"/>
        public bool IsCrouched => _isCrouched;

        ///<inheritdoc cref="_crouchTime"/>
        public float CrouchTime => _crouchTime;

        ///<inheritdoc cref="_normalizedInputMagnitude"/>
        public float NormalizedInputMagnitude => _normalizedInputMagnitude;

        public Vector2 SprintSpeed
        {
            get => _sprintSpeed;
            set => _sprintSpeed = value;
        }

        [HideInInspector]
        public Vector3 playerVelocity;

        /// <summary>
        /// anum that give information about how character is supposed to rotate when user change direction input(has nothing to do with camera angle)
        /// </summary>
        public enum PlayerRotationMode
        {
            Free,
            Clamped,
            SmoothlyClamped,
        }

        /// <summary>
        /// allow player to set an offset between input and action, reducing accuracy of gameplay, but allowing to precisely setup actions for animations, or to get cinematic effects(if you plan a really accurate gameplay, just let everything down to 0)
        /// </summary>
        [System.Serializable]
        public struct InvokeTime
        {
            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _sprintBegin;

            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _sprintEnd;

            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _walkBegin;

            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _walkEnd;

            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _crouchBegin;

            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _crouchEnd;

            [SerializeField, Tooltip("time for this action to set when user call action")]
            private float _jump;

            #region Public API

            ///<inheritdoc cref="_sprintBegin"/>
            public float SprintBegin => _sprintBegin;

            ///<inheritdoc cref="_sprintEnd"/>
            public float SprintEnd => _sprintEnd;

            ///<inheritdoc cref="_walkBegin"/>
            public float WalkBegin => _walkBegin;

            ///<inheritdoc cref="_walkEnd"/>
            public float WalkEnd => _walkEnd;

            ///<inheritdoc cref="_crouchBegin"/>
            public float CrouchBegin => _crouchBegin;

            ///<inheritdoc cref="_crouchEnd"/>
            public float CrouchEnd => _crouchEnd;

            ///<inheritdoc cref="_jump"/>
            public float Jump => _jump;

            #endregion

            public InvokeTime(float sprint)
            {
                _sprintBegin = 0;
                _sprintEnd = 0;
                _walkBegin = 0;
                _walkEnd = 0;
                _crouchBegin = 0;
                _crouchEnd = 0;
                _jump = 0;
            }
        }

        /// <summary>
        /// wich input mode is used to make jump phases
        /// </summary>
        public enum JumpModeAction
        {
            PressToJumpAndHoldToControl,
            PressToJumpAndReleaseToControl,
        }

        #endregion


        /**********************************************************BASE FUNCTIONS*************************************************************/

        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        private void Awake()
        {
            Init();

            //initialize player move speed and rotation speed at correct values and mouse cursor to right state
            _currentRotationSpeed = _rotationSpeed;
            _currentSpeed = _speed;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _workingWithoutGameManager = GameManager.Instance == null;
        }

        /// <summary>
        /// Update is called each frame
        /// </summary>
        private void Update()
        {
            //if scheme(controller) change, call an event to reorganise controller
            if (_playerInput.currentControlScheme != _schemeMemo)
                _onSchemeChange.Invoke();

            //register last input scheme
            _schemeMemo = _playerInput.currentControlScheme;

            //test jump input
            if (_workingWithoutGameManager || GameManager.Instance.IsCharacterControllable)
                JumpInputCalculation();
        }

        /// <summary>
        /// FixedUpdate is called once per Physics Update
        /// </summary>
        private void FixedUpdate()
        {
            //if game is not paused, make game run
            if (_workingWithoutGameManager || !GameManager.Instance.IsPaused)
            {
                //is character controllable
                if (_workingWithoutGameManager || GameManager.Instance.IsCharacterControllable)
                {
                    LookDir();
                    Move();
                    CrouchUpdate();
                }

                IsGroundedUpdate();
                GravityApply();

                if (_jumpPhase)
                    JumpApply();
                else
                {
                    _jumpSubdivisionTimer = 0;
                    _jumpGroundOut = false;
                } 
            }
        }

        /// <summary>
        /// OnDrawGizmos is called each time scene refresh
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                Init();

            //draw isGrounded collider preview
            Gizmos.DrawWireSphere(transform.position + _isGroundedPosition, _isGroundedScale);
        }

        /*********************************************************CUSTOM FUNCTIONS************************************************************/

        /// <summary>
        /// init is called in editor mode and at awake, to ensure every variables and states are initialized and set
        /// </summary>
        private void Init()
        {
            //if controller reference is null, get one in gameObject, else, create one
            if (_controller == null)
                if (!TryGetComponent(out _controller))
                    _controller = gameObject.AddComponent<CharacterController>();

            //if player input component reference is null, try to get component in gameObject, or create one
            if (_playerInput == null)
                if (!TryGetComponent(out _playerInput))
                    _playerInput = gameObject.AddComponent<PlayerInput>();

            //if player rotation pivot reference is null
            if (_playerTargetPivot == null)
            {
                //find a GameObject named PlayerTargetPivot in children, if there is one, make it the pivot
                for (int i = 0; i < transform.childCount; i++)
                    if (transform.GetChild(i).name == "PlayerTargetPivot")
                        _playerTargetPivot = transform.GetChild(i);

                //if there is no pivot GameObject found, create one
                if (_playerTargetPivot == null)
                    _playerTargetPivot = new GameObject("PlayerTargetPivot").transform.parent = transform;
            }
        }

        /// <summary>
        /// manage where player turn arround(affect movements direction !)
        /// </summary>
        private void LookDir()
        {
            //if player is moving, then rotate player
            if (_inputValue.magnitude != 0)
            {
                //make a smoothed value of input direction
                _smoothedInputValue = AutoLerp(_rotationLerpStart, _inputValue, _currentRotationSpeed, ref _rotationLerpTimer, _rotationShape);

                //if player is on keyboard, use a function to prevent rotation from clipping by adding a square function to input dir
                if (_playerInput.currentControlScheme == "KeyboardAndMouse")
                    PreventRotationFromClipping(ref _smoothedInputValue);

                //make the parent object look forward the camera, to make all basic calculation
                transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
                transform.eulerAngles += new Vector3(0, 180, 0);

                //make different calculs depending on rotation mode
                if (_rotationMode == PlayerRotationMode.Free)
                {
                    //face toward input value(smoothed)
                    Vector3 dir = transform.forward * _smoothedInputValue.y + transform.right * _smoothedInputValue.x;

                    //if character is in air, make a lerp between fixed last rotation and player's wanted position, depending on air control
                    if (!_isGrounded)
                        dir = Vector3.Lerp(_playerTargetPivot.forward, dir, Mathf.Pow(_rotationAirControl, 3));

                    //get end point of new direction vector
                    Vector3 position = transform.position + dir;

                    //make player loo at the end point(so that it look at the new direction)
                    _playerTargetPivot.LookAt(position);
                }
                else if (_rotationMode == PlayerRotationMode.Clamped)
                {
                    //make player rotation static
                    _playerTargetPivot.localEulerAngles = Vector3.zero;
                }
                else if (_rotationMode == PlayerRotationMode.SmoothlyClamped)
                {
                    //make a lerp between player default rotation and the same rotation with an adding of the input value 
                    _playerTargetPivot.localRotation = Quaternion.Lerp(_playerTargetPivot.localRotation, new Quaternion(_playerTargetPivot.localRotation.x, _inputValue.x * 0.5f, _playerTargetPivot.localRotation.z, _playerTargetPivot.localRotation.w), 0.2f);
                }
            }
        }

        /// <summary>
        /// manage movements of player
        /// </summary>
        private void Move()
        {
            //if player touch ground with hif feets, make normal player movements, if not, apply air control
            if (_isGrounded)
                playerVelocity = GroundMove();
            else
                playerVelocity = AirControlMove();

            //apply calculs
            _controller.Move(playerVelocity * Time.fixedDeltaTime);
        }

        /// <summary>
        /// normal player movements
        /// </summary>
        private Vector3 GroundMove()
        {
            //get a clamped magnitude of current input value(give no direction, but give strength of the movement homogeneously)
            _normalizedInputMagnitude = Mathf.Clamp(_inputValue.magnitude, -1, 1);

            //if last input was greater(character is decelerating) clamp slow down speed with fall down value, if last input was smaller(character is accelerating) clamp speed up with get high value
            if (_normalizedInputMagnitudeMemo > _normalizedInputMagnitude)
                _normalizedInputMagnitude = Mathf.Lerp(_normalizedInputMagnitude, _normalizedInputMagnitudeMemo, _smoothVelocityFallDown);
            else if (_normalizedInputMagnitudeMemo < _normalizedInputMagnitude)
                _normalizedInputMagnitude = Mathf.Lerp(_normalizedInputMagnitude, _normalizedInputMagnitudeMemo, _smoothVelocityGetHigh);

            //put normalized input last value to normalized input value
            _normalizedInputMagnitudeMemo = _normalizedInputMagnitude;

            //make a smooth transition between character speeds
            Vector2 smoothedSpeed = AutoLerp(_sprintTransitionStartPoint, _currentSpeedMemo, _speedLerpTime, ref _sprintTransitionTimer);

            //if speed goal has changed, start a new lerp
            if (_currentSpeed != _currentSpeedMemo)
            {
                _sprintTransitionStartPoint = smoothedSpeed;
                _sprintTransitionTimer = 0;
            }

            //make last speed to current speed
            _currentSpeedMemo = _currentSpeed;

            //if player is in movements, set not nullable input to the input value(when player will not move, not nullable value will keep last moving input)
            if (_inputValue != Vector2.zero)
                _inputValueNotNullable = _inputValue;

            //make transform forward and right multiplied by input to have a direction(normalized)
            Vector3 dir = ((transform.forward * _inputValueNotNullable.y) + (transform.right * _inputValueNotNullable.x)).normalized;

            //return final input, the direction multipied by speed, the all vector multiplied by input magnitude
            return new Vector3(dir.x * smoothedSpeed.x, dir.y, dir.z * smoothedSpeed.y) * _normalizedInputMagnitude;
        }

        /// <summary>
        /// air player movements
        /// </summary>
        /// <returns></returns>
        private Vector3 AirControlMove()
        {
            //return a lerp between the ground move and the current player velocity, depending on air control value
            return Vector3.Lerp(playerVelocity, GroundMove(), Mathf.Pow(_airControl, 3));
        }

        /// <summary>
        /// apply gravity to character controller
        /// </summary>
        private void GravityApply()
        {
            //if player is in air, and is, not in a jump, or, is in a jump since more than 20 frames, else set gravity velocity to 0
            if (!_controller.isGrounded && (!_jumpPhase || _jumpSubdivisionTimer >= 20))
            {
                //move character in the direction of gravity, multiplied by physics delta time, and by gravity real time velocity
                _controller.Move(Physics.gravity * Time.fixedDeltaTime * _gravityVelocity);

                //if player is falling under the 1 limit, keep multiplying from 0 to 1, if 1, keep 1
                if (_gravityVelocity < 1)
                    _gravityVelocity += Time.fixedDeltaTime / _gravityHardeness;
                else
                    _gravityVelocity = 1;
            }
            else
            {
                _gravityVelocity = 0;
            }

        }

        /// <summary>
        /// update boolean value to get if player is grounded or in air
        /// </summary>
        private void IsGroundedUpdate()
        {
            //make a check sphere to ave isGrouned value
            _isGrounded = Physics.CheckSphere(transform.position + _isGroundedPosition, _isGroundedScale, ~_playerLayer);
        }

        /// <summary>
        /// use second degrees polynomes functions to make a curve when player is changing direction in a strait way, avoiding player rotation from changing to quickly
        /// </summary>
        /// <param name="input"></param>
        private void PreventRotationFromClipping(ref Vector2 input)
        {
            //if value x or y is close to 0, apply changes
            if (Mathf.Abs(input.x) <= 0.05f || Mathf.Abs(input.y) <= 0.05f)
            {
                //if value x is closer to 0 than y, input to change is x, if it's the opposite, input to change is y
                if (Mathf.Abs(input.x) < Mathf.Abs(input.y))
                    input.x = AvoidClipping(input.x, input.y);
                else
                    input.y = AvoidClipping(input.y, input.x);

                //calcul to avoid clipping
                float AvoidClipping(float inputToChange, float otherInput)
                {
                    //if input to change is over 0, make other input over 0, if not, make other input under 0
                    if (inputToChange >= 0)
                        otherInput = Mathf.Abs(otherInput);
                    else
                        otherInput = -Mathf.Abs(otherInput);

                    //make square function to have new input(- other input squared + 1)
                    inputToChange = -Mathf.Pow(otherInput, 2) + 1;

                    //return new input
                    return inputToChange;
                }
            }


        }

        /// <summary>
        /// apply forces and input for jump
        /// </summary>
        private void JumpApply()
        {
            //make different jump codes depending on jump mode
            if (_jumpMode == JumpModeAction.PressToJumpAndReleaseToControl)
            {
                //if player is in air, set ground out to true
                if (!_isGrounded)
                    _jumpGroundOut = true;

                //if jump subdivision is processing, and player is else in air, or in ground but waiting to be in air(ground out)
                if (_jumpSubdivisionTimer < _jumpSubdivision && (!_isGrounded || !_jumpGroundOut))
                {
                    //if there is no delay to jump, make a direct jump at max force, if there is a delay, make a factor of jump factor divided by delay
                    float factor = _actionLaunchTime.Jump == 0 ? 1f : _jumpFactor / _actionLaunchTime.Jump;

                    //make a jump force vector of jump strength multiplied by factor and fixed delta time, the all vector splitted by subdivision timer
                    Vector3 force = new Vector3(0, _jumpStrength * factor * Time.fixedDeltaTime, 0) / (_jumpSubdivisionTimer + 1);

                    //apply force to curve function, to make small speed variation in detail
                    Vector3 curveForce = new Vector3(force.x, _jumpCurve.Evaluate(force.y / _jumpStrength) * _jumpStrength, force.z);

                    //apply force to move character
                    _controller.Move(curveForce);

                    //update subdivision state
                    _jumpSubdivisionTimer++;
                }
                else
                {
                    //if subdivision timer is ended, jump phase is false
                    _jumpPhase = false;
                }
            }
            else if (_jumpMode == JumpModeAction.PressToJumpAndHoldToControl)
            {
                //if player is in air, set ground out to true
                if (!_isGrounded)
                    _jumpGroundOut = true;

                //if jump subdivision is processing, and player is else in air, or in ground but waiting to be in air(ground out)
                if (_jumpSubdivisionTimer < _jumpSubdivision && (!_isGrounded || !_jumpGroundOut))
                {
                    //make a force vector, with jump strength directely at maximum value, splitted by subdivision timer 
                    Vector3 force = new Vector3(0, _jumpStrength, 0) / (_jumpSubdivisionTimer + 1);

                    //apply force to curve function, to make small speed variation in detail
                    Vector3 curveForce = new Vector3(force.x, _jumpCurve.Evaluate(force.y / _jumpStrength) * _jumpStrength, force.z);

                    //apply force to move character
                    _controller.Move(curveForce * Time.fixedDeltaTime);

                    //update subdivision state
                    _jumpSubdivisionTimer++;
                }
                else
                {
                    //if subdivision timer is ended, jump phase is false
                    _jumpPhase = false;
                }
            }
        }

        /// <summary>
        /// calculate input value and call jump when it's time
        /// </summary>
        private void JumpInputCalculation()
        {
            //if jump has a delay, make a timer system to launch when delay is ended, if not, launch directely jump
            if (_actionLaunchTime.Jump > 0)
            {
                //if there is currently jump input enabled, make timer grow, if not, set timer to 0
                if (_jumpInput)
                    _jumpTimer += Time.deltaTime;
                else
                    _jumpTimer = 0;

                //if timer has reach delay, launch jump
                if (_jumpTimer >= _actionLaunchTime.Jump)
                {
                    _jumpFactor = _actionLaunchTime.Jump;
                    _jumpTimer = 0;
                    _jumpInput = false;
                    _jumpPhase = true;
                }
            }
            else if (_jumpInput)
            {
                _jumpFactor = _actionLaunchTime.Jump;
                _jumpTimer = 0;
                _jumpInput = false;
                _jumpPhase = true;
            }
        }

        /// <summary>
        /// update height of player for crouch system
        /// </summary>
        private void CrouchUpdate()
        {
            //if player is in air, uncrouch automatically, then manage last input player is pressing to reapply crouch or not when caracter will go down to ground
            if (!IsGrounded)
            {
                //if character is crouched, uncrounch him 
                if (IsCrouched)
                {
                    Invoke(nameof(CallNormalHeight), _actionLaunchTime.CrouchEnd);
                    _isCrouched = false;
                }

                //if player is pressing crouch while in air, enable crouch air trigger
                if (_lastCrouchInputInAir)
                    _crouchInAirTrigger = true;
            }

            //if character is grounded, and it has trigger enabled, and player is holding crouch input, crouch character
            if (IsGrounded && _crouchInAirTrigger && _lastCrouchInputInAir)
            {
                Invoke(nameof(CallCrouchHeight), _actionLaunchTime.CrouchBegin);
                _isCrouched = true;

                //reinitialize trigger
                _crouchInAirTrigger = false;
            }

            //update character height to goal height, then adapt center to make player pivot not change position
            _controller.height = AutoLerp(_startHeight, _targetHeight, _crouchTime, ref _crouchLerpTimer);
            _controller.center = new Vector3(_controller.center.x, -0.9985f + (_controller.height / 2), _controller.center.z);
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer)
        {
            //create a null vector 2
            Vector2 value = Vector2.zero;

            //if timer has not reach lerp time, update lerp state, if it has, put timer to lerp time, and value to end state
            if (timer < lerpTime)
            {
                //update value to a lerp between a and b, with timer / lerp timer for T (value between 0 and 1)
                value = Vector2.Lerp(a, b, timer / lerpTime);

                //update timer value
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            //return value updated
            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth</param>
        /// <returns></returns>
        private Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            //create a null vector 2
            Vector2 value = Vector2.zero;

            //if timer has not reach lerp time, update lerp state, if it has, put timer to lerp time, and value to end state
            if (timer < lerpTime)
            {
                //update value to a lerp between a and b, with timer / lerp timer for T (value between 0 and 1) applyied to the animation curve value
                value = Vector2.Lerp(a, b, smoothTimer.Evaluate(timer / lerpTime));

                //update timer value
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            //return value updated
            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        private float AutoLerp(float a, float b, float lerpTime, ref float timer)
        {
            //create a null value
            float value = 0;

            //if timer has not reach lerp time, update lerp state, if it has, put timer to lerp time, and value to end state
            if (timer < lerpTime)
            {
                //update value to a lerp between a and b, with timer / lerp timer for T (value between 0 and 1)
                value = Mathf.Lerp(a, b, timer / lerpTime);

                //update timer value
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            //return value updated
            return value;
        }


        #region Input System Functions

        /// <summary>
        /// Called by Input System, manage basic movements of player
        /// </summary>
        /// <param name="callback">current value and state of input</param>
        public void GetMove(InputAction.CallbackContext callback)
        {
            //if active device is keyboard and mouse, setup rotation speed, if device is a gamepad controller, set rotation speed to 0
            if (_playerInput.currentControlScheme == "KeyboardAndMouse")
                _currentRotationSpeed = _rotationSpeed;
            else if (_playerInput.currentControlScheme == "GamepadController")
                _currentRotationSpeed = 0;

            //if last input value normalized is not equal to current input value normalized, restart rotation lerp values
            if (_inputValue.normalized != callback.ReadValue<Vector2>().normalized)
            {
                _rotationLerpTimer = 0;
                _rotationLerpStart = _smoothedInputValue;
            }

            //set input value to current input read
            _inputValue = callback.ReadValue<Vector2>();
        }

        /// <summary>
        /// called by input system, detect when user press and release sprint input, and call the right function associated
        /// </summary>
        /// <param name="callback">current value and state of input</param>
        public void GetSprint(InputAction.CallbackContext callback)
        {
            //if player is not crouched, perform
            if (!_isCrouched)
            {
                //if player is pressing input, call sprint speed, if player is releasing, call normal speed
                if (callback.phase == InputActionPhase.Started)
                {
                    Invoke(nameof(CallSprintSpeed), _actionLaunchTime.SprintBegin);

                    //update flip flop value, so that it not broke when user use both sprint and walk input
                    _walkFlipFlop = true;
                    _isSprinting = true;
                }
                else if (callback.phase == InputActionPhase.Canceled && _walkFlipFlop)
                {
                    Invoke(nameof(CallNormalSpeed), _actionLaunchTime.SprintEnd);
                    _isSprinting = false;
                }
            }
        }

        /// <summary>
        /// called by input system, detect when user press and release walk input, and call the right function associated
        /// </summary>
        /// <param name="callback">current value and state of input</param>
        public void GetWalk(InputAction.CallbackContext callback)
        {
            //if player is pressing walk input
            if (callback.phase == InputActionPhase.Started)
            {
                //if flip flop is on true, invoke walk speed, if not, invoke normal speed
                if (_walkFlipFlop)
                    Invoke(nameof(CallWalkSpeed), _actionLaunchTime.WalkBegin);
                else
                    Invoke(nameof(CallNormalSpeed), _actionLaunchTime.SprintEnd);

                //invert flip flop value
                _walkFlipFlop = !_walkFlipFlop;
            }
        }

        /// <summary>
        /// called by input system, detect when user press and release jump input, and call the right function associated
        /// </summary>
        /// <param name="callback"></param>
        public void GetJump(InputAction.CallbackContext callback)
        {
            //if game is not paused and character is controllable
            if (_workingWithoutGameManager || (!GameManager.Instance.IsPaused && GameManager.Instance.IsCharacterControllable))
            {
                //apply different code depending on jump mode
                if (_jumpMode == JumpModeAction.PressToJumpAndReleaseToControl)
                {
                    //if player is pressing input, read input for jump to true, if input is releasing, read input for jump to false
                    if (callback.phase == InputActionPhase.Started)
                    {
                        //if character is grounded, set jump input to true
                        if (_isGrounded)
                            _jumpInput = true;
                    }
                    else if (callback.phase == InputActionPhase.Canceled)
                    {
                        //if jump input is true
                        if (_jumpInput)
                        {
                            //put jump phase to true
                            _jumpPhase = true;

                            //if timer for preparing jump has reached jump time, set it to it
                            if (_jumpTimer >= _actionLaunchTime.Jump)
                                _jumpTimer = _actionLaunchTime.Jump;

                            //set jump factor to jump timer value, the more advanced the timer is, the higher it will jump
                            _jumpFactor = _jumpTimer;
                        }

                        //set jump input to false
                        _jumpInput = false;
                    }
                }
                else if (_jumpMode == JumpModeAction.PressToJumpAndHoldToControl)
                {
                    //if player is pressing input, manage input to set true, if player is releasing, manage input to set false
                    if (callback.phase == InputActionPhase.Started)
                    {
                        //if character is grounded, set jump input to true
                        if (_isGrounded)
                            _jumpInput = true;
                    }
                    else if (callback.phase == InputActionPhase.Canceled)
                    {
                        //set jump input and jump phase to false, cancelling all performing jump
                        _jumpPhase = false;
                        _jumpInput = false;
                    }
                } 
            }
        }

        /// <summary>
        /// called by input system, detect when user press and release crouch input, and call the right function associated
        /// </summary>
        /// <param name="callback">current value and state of input</param>
        public void GetCrouch(InputAction.CallbackContext callback)
        {
            //if game is not paused and character is controllable, and player is not sprinting
            if (_workingWithoutGameManager || (!GameManager.Instance.IsPaused && !_isSprinting && GameManager.Instance.IsCharacterControllable))
            {
                //if character is on ground, perform normal input reading, if character is in air, perform trigger updates
                if (IsGrounded)
                {
                    //if player is pressing input, manage crouch, if input is releasing, manage uncrouch
                    if (callback.phase == InputActionPhase.Started)
                    {
                        //set crouch height
                        Invoke(nameof(CallCrouchHeight), _actionLaunchTime.CrouchBegin);

                        //set crouch and air crouch to true
                        _isCrouched = true;
                        _lastCrouchInputInAir = true;
                    }
                    else if (callback.phase == InputActionPhase.Canceled)
                    {
                        //set normal height
                        Invoke(nameof(CallNormalHeight), _actionLaunchTime.CrouchEnd);

                        //set crouch and air crouch to false
                        _isCrouched = false;
                        _lastCrouchInputInAir = false;
                    }
                }
                else
                {
                    //if player is pressing input, set trigger to true, if input is releasing, set trigger to false
                    if (callback.phase == InputActionPhase.Started)
                    {
                        //set crouch and air trigger to true
                        _lastCrouchInputInAir = true;
                        _crouchInAirTrigger = true;
                    }

                    if (callback.phase == InputActionPhase.Canceled)
                    {
                        //set crouch and air trigger to false
                        _lastCrouchInputInAir = false;
                        _crouchInAirTrigger = false;
                    }
                }
            }
        }

        #endregion


        #region Invoked Functions

        /// <summary>
        /// called when user press sprint input
        /// </summary>
        private void CallSprintSpeed()
        {
            //set speed goal to sprint speed
            _currentSpeed = _sprintSpeed;
        }

        /// <summary>
        /// called when user release sprint input
        /// </summary>
        private void CallNormalSpeed()
        {
            //set speed goal to normal speed
            _currentSpeed = _speed;
        }

        /// <summary>
        /// called when user enable walk mode
        /// </summary>
        private void CallWalkSpeed()
        {
            //set speed goal to walk speed
            _currentSpeed = _walkSpeed;
        }

        /// <summary>
        /// called when user release crouch input
        /// </summary>
        private void CallNormalHeight()
        {
            //set height goal to normal height
            _targetHeight = _normalHeight;

            //set star height to current height
            _startHeight = _controller.height;

            //restart lerp
            _crouchLerpTimer = 0;
        }

        /// <summary>
        /// called when user press crouch input
        /// </summary>
        private void CallCrouchHeight()
        {
            //set height goal to crouch height
            _targetHeight = _crouchHeight;

            //set star height to current height
            _startHeight = _controller.height;

            //restart lerp
            _crouchLerpTimer = 0;
        }



        #endregion
    }
}
