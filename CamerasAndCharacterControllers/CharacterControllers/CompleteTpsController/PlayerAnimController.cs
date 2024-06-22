using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.CompleteTpsController
{
    /// <summary>
    /// manage every animations of player, to link in input events, created to be used with CompleteTpsController components
    /// </summary>
    [AddComponentMenu("UPDB/CamerasAndCharacterControllers/CharacterControllers/CompleteTpsController/Complete Tps Anim Controller")]
    public class PlayerAnimController : UPDBBehaviour
    {
        #region Serialized API

        /*****************************BASE REFERENCES****************************/
        [Header("BASE REFERENCES"), Space]

        [SerializeField, Tooltip("used animator to setup player")]
        private Animator _animator;

        [SerializeField, Tooltip("used playerController")]
        private PlayerController _player;


        /***********************************WALK**********************************/
        [Space, Header("WALK"), Space]

        [SerializeField, Tooltip("determine wich curve blend tree is gonna use to make walk anim transitions")]
        private AnimationCurve _walkBlendCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField, Tooltip("determine wich curve blend tree is gonna use, affect only blend between idle and simple walk")]
        private AnimationCurve _walkBlendWalkCurve = AnimationCurve.Linear(0, 0, 1, 1);


        /***********************************IDLE**********************************/
        [Space, Header("IDLE"), Space]

        [SerializeField, Tooltip("times min and max of random value that will be the time when random idles launches")]
        private Vector2 _randomIdleMinMax = new Vector2(10, 20);


        /***********************************JUMP**********************************/
        [Space, Header("JUMP"), Space]

        [SerializeField, Tooltip("does jump anim skip the crouch phase to jump instantly ?")]
        private bool _instantJump = true;

        #endregion

        #region Private API

        /*****************************IDLE****************************/

        /// <summary>
        /// time wich random idles launches
        /// </summary>
        private float _randomIdleTime = 0;

        /// <summary>
        /// timer for random idle launch
        /// </summary>
        private float _randomIdleTimer = 0;


        /*****************************CROUCH****************************/

        /// <summary>
        /// timer used to time phase of keeling while walking
        /// </summary>
        private float _kneelingWalkingTimer = 0;

        /// <summary>
        /// is character on a kneeling while walking
        /// </summary>
        private bool _kneelingWalkingPhase = false;

        #endregion


        /**********************************************************BASE FUNCTIONS*************************************************************/

        /// <summary>
        /// awake is called when script instance is loaded
        /// </summary>
        private void Awake()
        {
            Init();

            //setup random idle time
            _randomIdleTime = Random.Range(_randomIdleMinMax.x, _randomIdleMinMax.y);
        }

        /// <summary>
        /// update is called every frame
        /// </summary>
        private void Update()
        {
            MoveAnim();
            RandomIdleManager();
            FallingManager();
            JumpManager();
            CrouchManager();
        }

        protected override void OnSceneSelected()
        {
            Init();
        }


        /**********************************************************CUSTOM FUNCTIONS*************************************************************/

        /// <summary>
        /// init is called in editor and at awake, to setup basic values and states
        /// </summary>
        private void Init()
        {
            if (!_animator)
                if (!TryGetComponent(out _animator))
                    _animator = gameObject.AddComponent<Animator>();

            if (!_player)
                if (!TryGetComponent(out _player))
                    if (FindObjectOfType<PlayerController>())
                        _player = FindObjectOfType<PlayerController>();

            if (_walkBlendCurve.keys.Length < 2)
                _walkBlendCurve = AnimationCurve.Linear(0, 0, 1, 1);


            if (_walkBlendWalkCurve.keys.Length < 2)
                _walkBlendWalkCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        /// <summary>
        /// manage animation of basic movements
        /// </summary>
        private void MoveAnim()
        {
            //set a value of normalized input magnitude, but it's equal to 0 if under 0.001
            float value = _player.NormalizedInputMagnitude < 0.001f ? 0 : _player.NormalizedInputMagnitude;

            //set anmator float move magnitude to the value
            _animator.SetFloat("MoveInputMagnitude", value);

            //set an anim blend value equal to the magnitude of a vector3 of player velocity divided by sprint speed(value always between the same a and b range
            float animBlendValue = new Vector3(_player.playerVelocity.x / _player.SprintSpeed.x, _player.playerVelocity.y, _player.playerVelocity.z / _player.SprintSpeed.y).magnitude;

            //set animator float of anim blend value and anim blend walking value, applyied to their anim curve
            _animator.SetFloat("WalkBlendLerp", _walkBlendCurve.Evaluate(animBlendValue));
            _animator.SetFloat("WalkBlendWalkLerp", _walkBlendWalkCurve.Evaluate(animBlendValue));
        }

        /// <summary>
        /// manage launching of random idle when not in movement
        /// </summary>
        private void RandomIdleManager()
        {
            //if animator is in an anim transition, reset timer of random idle
            if (_animator.IsInTransition(0))
            {
                _randomIdleTimer = 0;
                _animator.ResetTrigger("RandomIdle");
            }

            //if random idle timer has reach random time, launch random idle
            if (_randomIdleTimer >= _randomIdleTime)
            {
                //set trigger of random idle to true
                _animator.SetTrigger("RandomIdle");

                //set a random value for wich random idle will be launched
                int idleLaunch = Random.Range(0, 2);

                //set animator integer value to random value
                if (idleLaunch == 0)
                    _animator.SetInteger("RandomIdleValue", 0);
                else if (idleLaunch == 1)
                    _animator.SetInteger("RandomIdleValue", 1);

                //reset timer and create a new random time
                _randomIdleTimer = 0;
                _randomIdleTime = Random.Range(_randomIdleMinMax.x, _randomIdleMinMax.y);
            }

            //update timer
            _randomIdleTimer += Time.deltaTime;
        }

        /// <summary>
        /// manage if player is grounded or not
        /// </summary>
        private void FallingManager()
        {
            //set animator bool to is grounded value
            _animator.SetBool("IsGrounded", _player.IsGrounded);
        }

        /// <summary>
        /// manage launching of jump phases
        /// </summary>
        private void JumpManager()
        {
            //if jump input is true, manage trigger to setup jump, else, reset all triggers
            if (_player.JumpInput)
            {
                //if jump is instant, set trigger to ckip kneeling, if not, set trigger to crouch
                if (_instantJump)
                    _animator.SetTrigger("JumpSkipCrouch");
                else
                    _animator.SetTrigger("JumpKneeling");

                //enable kneeling phase
                _kneelingWalkingPhase = true;
            }
            else
            {
                _animator.ResetTrigger("JumpSkipCrouch");
                _animator.ResetTrigger("JumpKneeling");
            }

            //if jump phase is true, set trigger of jump, if not, reset trigger of jump
            if (_player.JumpPhase)
            {
                _animator.SetTrigger("Jump");

                //reset kneeling timer and set phase to false
                _kneelingWalkingTimer = 0;
                _kneelingWalkingPhase = false;
            }
            else
                _animator.ResetTrigger("Jump");

            //if kneeling while walking phase is true, manage anim of kneeling walking
            if (_kneelingWalkingPhase)
            {
                //kneeling walking timer update multiplied by 5
                _kneelingWalkingTimer += Time.deltaTime * 5;

                //set animator float to kneeling walking timer
                _animator.SetFloat("KneelingWalkingBlendLerp", _kneelingWalkingTimer);

                //if kneeling walking timer is over 1, reset phase and timer
                if (_kneelingWalkingTimer >= 1)
                {
                    _kneelingWalkingPhase = false;
                    _kneelingWalkingTimer = 0;
                }
            }
        }

        /// <summary>
        /// manage launching of crouch anim
        /// </summary>
        private void CrouchManager()
        {
            //set animator float crouch time to 1 divided crouch time of player
            _animator.SetFloat("CrouchTime", 1 / _player.CrouchTime);

            //set animator bool to is crouched value
            _animator.SetBool("IsCrouched", _player.IsCrouched);
        }
    }

}