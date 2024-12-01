using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UPDB.CoreHelper.Usable
{
    /// <summary>
    /// singleton manager of scenes global properties, unique scene properties are stored in children classes of levelManager
    /// </summary>
    public abstract class LevelManager<T> : Singleton<T> where T : Component
    {
        [SerializeField, Tooltip("name of level")]
        protected string _levelName = "";

        [SerializeField, Tooltip("unique index of level")]
        protected int _levelIndex = 0;

        [SerializeField, Tooltip("instance of player in this level, null means there is no player")]
        protected GameObject _player;

        [SerializeField, Tooltip("base parameters of level start")]
        protected LevelStartInfo _baseStartInfo;

        [SerializeField]
        private ClippingPass[] _clippingPreventionPasses;


        #region Public API

        [Serializable]
        public class LevelStartInfo
        {
            [SerializeField, Tooltip("where does player start ?")]
            private Vector3 _playerPosition = Vector3.zero;

            [SerializeField, Tooltip("set time scale at start of level")]
            private float _timeScale = 1;

            [SerializeField, Tooltip("set fixed time step at start of level")]
            private float _fixedTimeStep = 0.02f;

            [SerializeField, Tooltip("set lock mode of cursor, by default cursor is locked")]
            private CursorLockMode _lockMode = CursorLockMode.Locked;

            [SerializeField, Tooltip("set visibility of cursor")]
            private bool _cursorVisible = false;

            [SerializeField, Tooltip("set pause state at start")]
            private bool _pauseState = false;

            [SerializeField, Tooltip("set character controllable state at start")]
            private bool _characterControllable = true;

            [SerializeField, Tooltip("set here behaviour that happens at the start of level, if let to empty, nothing will happen at start")]
            private List<LevelStartBehaviour> _startBehaviourList;

            #region Public API

            public Vector3 PlayerPosition
            {
                get => _playerPosition;
                set => _playerPosition = value;
            }

            public float TimeScale
            {
                get => _timeScale;
                set => _timeScale = value;
            }

            public float FixedTimeStep
            {
                get => _fixedTimeStep;
                set => _fixedTimeStep = value;
            }

            public CursorLockMode LockMode
            {
                get => _lockMode;
                set => _lockMode = value;
            }

            public bool CursorVisible
            {
                get => _cursorVisible;
                set => _cursorVisible = value;
            }

            public List<LevelStartBehaviour> StartBehaviourList
            {
                get => _startBehaviourList;
                set => _startBehaviourList = value;
            }

            public bool PauseState
            {
                get => _pauseState;
                set => _pauseState = value;
            }

            public bool CharacterControllable
            {
                get => _characterControllable;
                set => _characterControllable = value;
            }


            #endregion
        }

        public enum LevelStartBehaviour
        {
            PlayerSetPos,
            TimeScaleSetValue,
            FixedTimeStepSetValue,
            LockModeSetValue,
            CursorVisibleSetValue,
            PauseSetValue,
            CharacterControllableSetValue,
        }

        public enum ClippingMode
        {
            LowerBound,
            UpperBound,
            LeftBound,
            RightBound,
            ForwardBound,
            BackwardBound,
        }

        [Serializable]
        public struct ClippingPass
        {
            [SerializeField]
            private ClippingMode _clippingAxis;

            [SerializeField]
            private float _boundCoordinate;

            [SerializeField]
            private float _tpCoordinate;

            public ClippingMode ClippingAxis => _clippingAxis;
            public float BoundCoordinate => _boundCoordinate;
            public float TpCoordinate => _tpCoordinate;
        }

        public LevelStartInfo BaseStartInfo
        {
            get { return _baseStartInfo; }
            set { _baseStartInfo = value; }
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();

            Init();

            LoadLevel();
        }

        protected virtual void Start()
        {
            StartLevel();
        }

        protected virtual void Update()
        {
            if (_player)
                PlayerBasedFunctionalities();
        }

        /*********************************************CUSTOM METHODS**********************************************/

        protected override void OnScene()
        {
            base.OnScene();

            Init();
        }

        /// <summary>
        /// when class values arebeing initialize in awake and scene update
        /// </summary>
        private void Init()
        {
            if (!_player)
                _player = GameObject.FindWithTag("Player");

            _levelName = SceneManager.GetActiveScene().name;
            _levelIndex = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// called when level is loading on awake, init level values
        /// </summary>
        protected virtual void LoadLevel()
        {
            LoadStartBehaviours();
        }

        /// <summary>
        /// called at first frame, initialize level values
        /// </summary>
        protected virtual void StartLevel()
        {

        }

        /// <summary>
        /// hen called, re initialize level values
        /// </summary>
        protected virtual void RestartLevel()
        {
            LoadStartBehaviours();
        }

        #region Start Level Behaviours

        private void LoadStartBehaviours()
        {
            foreach (LevelStartBehaviour behaviour in _baseStartInfo.StartBehaviourList)
            {
                if (behaviour == LevelStartBehaviour.PlayerSetPos)
                {
                    PlayerSetPos();
                    continue;
                }
                if (behaviour == LevelStartBehaviour.TimeScaleSetValue)
                {
                    TimeScaleSetPos();
                    continue;
                }
                if (behaviour == LevelStartBehaviour.FixedTimeStepSetValue)
                {
                    FixedTimeStepSetPos();
                    continue;
                }
                if (behaviour == LevelStartBehaviour.LockModeSetValue)
                {
                    LockModeSetValue();
                    continue;
                }
                if (behaviour == LevelStartBehaviour.CursorVisibleSetValue)
                {
                    CursorVisibleSetValue();
                    continue;
                }
                if (behaviour == LevelStartBehaviour.PauseSetValue)
                {
                    PauseSetValue();
                    continue;
                }
                if (behaviour == LevelStartBehaviour.CharacterControllableSetValue)
                {
                    CharacterControllableSetValue();
                    continue;
                }
            }
        }

        private void PlayerSetPos()
        {
            if (!_player)
                return;

            _player.transform.position = _baseStartInfo.PlayerPosition;
        }

        private void TimeScaleSetPos()
        {
            Time.timeScale = _baseStartInfo.TimeScale;
        }

        private void FixedTimeStepSetPos()
        {
            Time.fixedDeltaTime = _baseStartInfo.FixedTimeStep;
        }

        private void LockModeSetValue()
        {
            Cursor.lockState = _baseStartInfo.LockMode;
        }

        private void CursorVisibleSetValue()
        {
            Cursor.visible = _baseStartInfo.CursorVisible;
        }

        private void PauseSetValue()
        {
            GameManager.Instance.IsPaused = _baseStartInfo.PauseState;
        }

        private void CharacterControllableSetValue()
        {
            GameManager.Instance.IsCharacterControllable = _baseStartInfo.CharacterControllable;
        }

        #endregion

        private void PlayerBasedFunctionalities()
        {
            foreach (ClippingPass clippingPass in _clippingPreventionPasses)
            {
                if (clippingPass.ClippingAxis == ClippingMode.LowerBound)
                {
                    if (_player.transform.position.y < clippingPass.BoundCoordinate)
                        _player.transform.position = new Vector3(_player.transform.position.x, clippingPass.TpCoordinate, _player.transform.position.z);

                    continue;
                }
                if (clippingPass.ClippingAxis == ClippingMode.UpperBound)
                {
                    if (_player.transform.position.y > clippingPass.BoundCoordinate)
                        _player.transform.position = new Vector3(_player.transform.position.x, clippingPass.TpCoordinate, _player.transform.position.z);

                    continue;
                }
                if (clippingPass.ClippingAxis == ClippingMode.LeftBound)
                {
                    if (_player.transform.position.x < clippingPass.BoundCoordinate)
                        _player.transform.position = new Vector3(clippingPass.TpCoordinate, _player.transform.position.y, _player.transform.position.z);

                    continue;
                }
                if (clippingPass.ClippingAxis == ClippingMode.RightBound)
                {
                    if (_player.transform.position.x > clippingPass.BoundCoordinate)
                        _player.transform.position = new Vector3(clippingPass.TpCoordinate, _player.transform.position.y, _player.transform.position.z);

                    continue;
                }
                if (clippingPass.ClippingAxis == ClippingMode.ForwardBound)
                {
                    if (_player.transform.position.z > clippingPass.BoundCoordinate)
                        _player.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, clippingPass.TpCoordinate);

                    continue;
                }
                if (clippingPass.ClippingAxis == ClippingMode.BackwardBound)
                {
                    if (_player.transform.position.z < clippingPass.BoundCoordinate)
                        _player.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, clippingPass.TpCoordinate);

                    continue;
                }
            }
        }

        #region Input Callback

        public void GetPauseInput(InputAction.CallbackContext callback)
        {
            if (callback.started)
            {
                GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
            }
        }

        #endregion
    }

}