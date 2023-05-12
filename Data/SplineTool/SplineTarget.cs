using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Data.SplineTool
{
    [System.Serializable]
    public class SplineTarget
    {
        #region Serialized API

        /*****************************************************BASE REFERENCES**************************************************************/
        [Space, Header("BASE REFERENCES"), Space]

        [SerializeField, Tooltip("linked target")]
        private Transform _target;

        [SerializeField, Tooltip("manual reference to make a fade of material")]
        private Material _targetManualFadeReference;

        [SerializeField, Tooltip("if you want target to have an animation linked to this spline")]
        private Animator _targetLinkedAnimation;


        /*****************************************************POSITION PARAMETERS**************************************************************/
        [Space, Header("POSITION PARAMETERS"), Space]

        [SerializeField, Range(0, 1), Tooltip("represent overall position of target on spline")]
        private float _targetSplinePos;

        [SerializeField, Tooltip("represent position of target on active key")]
        private float _targetKeyPos = 0;

        [SerializeField, Tooltip("key representing point A of lerp")]
        private int _activeKey = 0;

        [Space]

        [SerializeField, Tooltip("choose fade of target at beginning of spline")]
        private float _startFadeTime = 0;

        [SerializeField, Tooltip("choose fade of target at end of spline")]
        private float _endFadeTime = 0;


        /*****************************************************MOVEMENTS PARAMETERS**************************************************************/
        [Space, Header("MOVEMENTS PARAMETERS"), Space]

        [SerializeField, Tooltip("choose a time when target will begin to move")]
        private float _startTime = 0;

        [SerializeField, Tooltip("default speed multiplier")]
        private float _speedMultiplier = 1;

        [SerializeField, Tooltip("speed multiplier for animation linked(if one)")]
        private float _animSpeedMultiplier = 1;

        [SerializeField, Tooltip("is movements looping when at the end ?")]
        private bool _loop = true;

        [SerializeField, Tooltip("is movements looping when at the end ?")]
        private float _loopTime = 0;

        [SerializeField, Range(0, 1), Tooltip("choose a position where to loop")]
        private float _loopPos = 0;


        /**************************************************PROPERTY DRAWER ELEMENTS***********************************************************/

        [SerializeField, HideInInspector, Tooltip("only used by custom property drawer, determine if main foldout is displayed or not")]
        private bool _mainFoldoutDisplay;

        #endregion

        #region Private API

        /**************************************************POSITION PARAMETERS***********************************************************/

        /// <summary>
        /// scaled targetSlinePos to be the size of key number(ex : 10 keys = 9 length scale)
        /// </summary>
        private float _targetSplinePosScaled = 0;

        /// <summary>
        /// keep in mind first rotation key
        /// </summary>
        private Quaternion _startKeyRotationOverride;

        /// <summary>
        /// used to get rotation with modifications
        /// </summary>
        private Quaternion[] _keysRotationOverrideList;


        /**************************************************MOVEMENTS PARAMETERS***********************************************************/

        /// <summary>
        /// timer launched when loop is enabled and target is at end of spline
        /// </summary>
        private float _loopTimer = 0;

        /// <summary>
        /// count when target will begin to move at start
        /// </summary>
        private float _startTimer = 0;

        /// <summary>
        /// keep last value of active key
        /// </summary>
        private int _activeKeyMemo = 0;

        /// <summary>
        /// timer used to time pauses
        /// </summary>
        private float _pauseTimer = 0;

        /// <summary>
        /// store value to get when in pause
        /// </summary>
        private float _pauseRandomValue = 0;

        #endregion

        #region Public API

        public Transform Target => _target;
        public Material TargetManualFadeReference => _targetManualFadeReference;
        public Animator TargetLinkedAnimation => _targetLinkedAnimation;
        public float TargetSplinePos
        {
            get => _targetSplinePos;
            set => _targetSplinePos = value;
        }
        public float TargetKeyPos
        {
            get => _targetKeyPos;
            set => _targetKeyPos = value;
        }
        public int ActiveKey
        {
            get => _activeKeyMemo;
            set => _activeKeyMemo = value;
        }
        public float StartFadeTime => _startFadeTime;
        public float EndFadeTime => _endFadeTime;
        public float StartTime => _startTime;
        public float SpeedMultiplier => _speedMultiplier;
        public float AnimSpeedMultiplier => _animSpeedMultiplier;
        public float LoopPos => _loopPos;
        public bool Loop => _loop;
        public float LoopTime => _loopTime;
        public float TargetSplinePosScaled
        {
            get => _targetSplinePosScaled;
            set => _targetSplinePosScaled = value;
        }
        public Quaternion StartKeyRotationOverride
        {
            get => _startKeyRotationOverride;
            set => _startKeyRotationOverride = value;
        }
        public Quaternion[] KeysRotationOverrideList
        {
            get => _keysRotationOverrideList;
            set => _keysRotationOverrideList = value;
        }
        public float LoopTimer
        {
            get => _loopTimer;
            set => _loopTimer = value;
        }
        public float StartTimer
        {
            get => _startTimer;
            set => _startTimer = value;
        }
        public int ActiveKeyMemo
        {
            get => _activeKeyMemo;
            set => _activeKeyMemo = value;
        }
        public float PauseTimer
        {
            get => _pauseTimer;
            set => _pauseTimer = value;
        }
        public float PauseRandomValue
        {
            get => _pauseRandomValue;
            set => _pauseRandomValue = value;
        }

        #endregion
    } 
}
