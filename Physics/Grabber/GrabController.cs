using UnityEngine;

namespace Bug
{
    /// <summary>
    /// control grab system of every tagged objects
    /// </summary>
    public class GrabController : MonoBehaviour
    {
        /********************INPUTS********************/

        [Header("INPUTS")]
        [SerializeField, Tooltip("determine if player has to maintain grab input or just to swap state by pressing it one time")]
        private GrabMode _grabMode = GrabMode.Swap;

        [SerializeField, Tooltip("choose if inputs of flip act like a wheel, a snapped wheel, or a drag")]
        private FlipMode _flipMode = FlipMode.SnappedWheel;

        /*****************GRAB SETTINGS****************/

        [Header("GRAB SETTINGS")]
        [SerializeField, Tooltip("higher value means the grabbed object will be attracted with more strength")]
        private float _grabStrength = 100;

        [SerializeField, Tooltip("determine distance wich player can grab objects")]
        private float _grabRange = 1;

        [SerializeField, Tooltip("determine position of objects grab")]
        private Vector3 _grabOffset = Vector3.one;

        [SerializeField, Tooltip("determine strength of angular deceleration while grabbing")]
        private float _angularDeceleration = 5;

        [SerializeField, Tooltip("determine strength of deceleration at trowing of objects")]
        private float _trowVelocityDivider = 2;

        [SerializeField, Tooltip("if enabled, rotation will be constrainted while grabbing")]
        private bool _constraintRotationOnGrab = false;

        [SerializeField, Tooltip("if enabled, rotation will be constrainted while not grabbing")]
        private bool _constraintRotationOnNotGrab = false;

        [SerializeField, Tooltip("if disabled, object will reset to original rotation when ungrabing")]
        private bool _rotationSaveState = false;


        /*****************GRAB SETTINGS****************/

        [Header("FLIP SETTINGS")]
        [SerializeField, Tooltip("determine sensitivity of flip inputs")]
        private Vector2 _flipSensitivity = Vector2.one;


        /*********************TAG*********************/

        [Header("TAG")]
        [SerializeField, Tooltip("the name of the Tag you want to be used for grabable objects")]
        private string _tagName = "Grabable";


        /*******************OBJECTS*******************/

        [Header("OBJECT")]
        [SerializeField, Tooltip("give the current grabbed object, null means that grab is not active")]
        private Transform _grabbedObject;


        /*******************NON SERIALIZED VARIABLES*******************/

        /// <summary>
        /// save of last rotation of transform when grabbing
        /// </summary>
        private Vector3 _eulerAngleSaveState = Vector3.zero;

        /// <summary>
        /// save of last rotation of rigidbody when grabbing
        /// </summary>
        private Quaternion _rigidbodyAngleSaveState;

        /// <summary>
        /// keep in memory the last value of grabbedObject before updating
        /// </summary>
        private Transform _grabbedObjMemo;

        /// <summary>
        /// rigidbody used for calculs of grabbed object
        /// </summary>
        private Rigidbody _rb;

        /// <summary>
        /// ensure that code have been trough OnEnter function.
        /// </summary>
        private bool _hasEntered = false;

        /// <summary>
        /// ensure that code have been trough OnExit function.
        /// </summary>
        private bool _hasExited = false;

        #region Public API

        /// <summary>
        /// determine wich input mode is used for grab, between hold, swap...etc
        /// </summary>
        public enum GrabMode
        {
            Hold,
            Swap,
            Disabled,
        }

        /// <summary>
        /// determine wich input mode is used for flip, between drag, wheel...etc
        /// </summary>
        public enum FlipMode
        {
            Drag,
            Wheel,
            SnappedWheel,
        }

        #endregion

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if (_grabMode != GrabMode.Disabled)
            {
                //init enter and exit callback value
                _hasEntered = false;
                _hasExited = false;

                //detect input of player
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    GrabInputManager(true);
                else if (Input.GetKeyUp(KeyCode.Mouse0) && _grabMode != GrabMode.Swap)
                    GrabInputManager(false);

                //recup OnEnter and OnExit if user has not used inputs
                EnterExitRecup();

                //updating grabbedObject memo value
                _grabbedObjMemo = _grabbedObject;

                //raycast debug
                Debug.DrawRay(transform.position, transform.forward * _grabRange, Color.blue);
            }
        }

        private void FixedUpdate()
        {
            bool isEnabled = _grabMode != GrabMode.Disabled;
            bool isGrabActive = _grabbedObject != null;
            bool hasSkipedEnterAtFirst = _grabbedObject != null && _grabbedObjMemo == null && !_hasEntered;

            if (isEnabled && isGrabActive && !hasSkipedEnterAtFirst)
                OnGrabStay();
        }

        #region InputManagerFunctions

        /// <summary>
        /// controls order of grab function calls, depending on type of input, hold, or swap
        /// </summary>
        private void GrabInputManager(bool isEntering)
        {
            bool grabbedObjChange = _grabbedObject;
            bool isExitingSwap = false;

            //if player input is down
            if (isEntering)
            {
                //if there is no object currently grabbed, cast a ray, else, exiting grab
                if (_grabbedObject == null)
                    RayCastManager();
                else
                    isExitingSwap = true;
            }

            //launch grab test phase, hold or swap, depending on choosed mode
            if (_grabMode == GrabMode.Hold)
                HoldGrab(grabbedObjChange, isEntering);
            else if (_grabMode == GrabMode.Swap)
                SwapGrab(grabbedObjChange, isExitingSwap);
        }

        /// <summary>
        /// Control order of grab function calls for Hold mode
        /// </summary>
        private void HoldGrab(bool grabbedObjChange, bool isEntering)
        {
            //if grabbed object value has changed, or object is currently grabbed
            if (grabbedObjChange != _grabbedObject || _grabbedObject != null)
            {
                //if input is Down, or else, is up
                if (isEntering)
                {
                    OnGrabEnter();
                    _hasEntered = true;
                }
                else
                {
                    OnGrabExit();
                    _grabbedObject = null;
                    _hasExited = true;
                }
            }
        }

        /// <summary>
        /// Control order of grab function calls for Swap mode
        /// </summary>
        private void SwapGrab(bool grabbedObjChange, bool isExitingSwap)
        {
            //if grabbedObject value has changed or the swap mode is exiting
            if (grabbedObjChange != _grabbedObject || isExitingSwap)
            {
                //if its exiting, else, is entering
                if (isExitingSwap)
                {
                    OnGrabExit();
                    _grabbedObject = null;
                    _hasExited = true;
                }
                else
                {
                    OnGrabEnter();
                    _hasEntered = true;
                }
            }
        }

        /// <summary>
        /// recup for OnEnter and OnExit on Debug
        /// </summary>
        private void EnterExitRecup()
        {
            //if grabbedObject is at first update after activating, but has not entered
            if (_grabbedObject != null && _grabbedObjMemo == null && !_hasEntered)
            {
                //use OnGrabEnter
                OnGrabEnter();

                //set callback to true
                _hasEntered = true;
            }

            //if grabbedObject is at last update after disabling, but has not exited
            if (_grabbedObject == null && _grabbedObjMemo != null && !_hasExited)
            {
                //set grabObject to its previous value to use it onExit, then, reset it to null
                _grabbedObject = _grabbedObjMemo;
                OnGrabExit();
                _grabbedObject = null;

                //set callback to true
                _hasExited = true;
            }
        }

        /// <summary>
        /// manage raycast when player grab object
        /// </summary>
        private void RayCastManager()
        {
            //trow raycast to camera target direction
            RaycastHit hit;
            bool grabRay = Physics.Raycast(transform.position, transform.forward, out hit);

            //if raycast hit a collider
            if (grabRay)
            {
                //if hited object is in range and has the grabable tag, put object into grabbedObject
                bool hasTag = hit.transform.gameObject.CompareTag(_tagName);
                bool isInRange = hit.distance <= _grabRange;

                if (isInRange && hasTag)
                    _grabbedObject = hit.transform;
            }
        }

        #endregion

        #region OnGrabFunctions

        /// <summary>
        /// called when an object is grabbed
        /// </summary>
        private void OnGrabEnter()
        {
            if (!_grabbedObject.gameObject.TryGetComponent(out _rb))
                _rb = _grabbedObject.gameObject.AddComponent<Rigidbody>();

            _rb.useGravity = false;

            if (_constraintRotationOnGrab)
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
            else
                _rb.constraints = RigidbodyConstraints.None;

            if (!_rotationSaveState)
            {
                _eulerAngleSaveState = transform.eulerAngles;
                _rigidbodyAngleSaveState = _rb.rotation;
            }
        }

        /// <summary>
        /// called when an object is no longer grabbed
        /// </summary>
        private void OnGrabExit()
        {
            _rb.useGravity = true;
            _rb.velocity /= _trowVelocityDivider;

            if (_constraintRotationOnNotGrab)
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
            else
                _rb.constraints = RigidbodyConstraints.None;

            if (!_rotationSaveState)
            {
                transform.eulerAngles = _eulerAngleSaveState;
                _rb.rotation = _rigidbodyAngleSaveState;
            }
        }

        /// <summary>
        /// called every frame when object is grabbed
        /// </summary>
        private void OnGrabStay()
        {
            Vector3 targetPosition = transform.position + ((transform.forward * _grabOffset.x) + (transform.up * _grabOffset.y) + (transform.right * _grabOffset.z));

            _rb.velocity = _rb.velocity / (_grabStrength / 50);
            _rb.angularVelocity = _rb.angularVelocity / _angularDeceleration;
            _rb.AddForce((targetPosition - _grabbedObject.position) * _grabStrength);

            FlipInputManager();
        }

        #endregion

        #region FlipFunctions

        private void FlipInputManager()
        {
            if (_flipMode == FlipMode.Drag && Input.GetKey(KeyCode.Mouse1))
            {

            }
            else if (_flipMode == FlipMode.Wheel)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    OnFlipWheel(false);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    OnFlipWheel(true);
                }
            }
            else if (_flipMode == FlipMode.SnappedWheel)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    OnFlipWheel(false);
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    OnFlipWheel(true);
                }
            }
        }

        private void OnFlipWheel(bool isRight)
        {
            if (isRight)
            {
                _grabbedObject.Rotate(new Vector3(_flipSensitivity.x, 0, 0), Space.World);
            }
            else
            {
                _grabbedObject.Rotate(new Vector3(-_flipSensitivity.x, 0, 0), Space.World);
            }
        }

        #endregion
    }
}
