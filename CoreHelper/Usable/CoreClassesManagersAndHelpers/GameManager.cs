using UnityEngine;
using UnityEngine.SceneManagement;

namespace UPDB.CoreHelper.Usable
{
    /// <summary>
    /// singleton manager of all projects global properties
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField, Tooltip("tell if the game is in pause or not")]
        private bool _isPaused = false;

        [SerializeField, Tooltip("tell if the game is in pause or not")]
        private bool _isGameOver = false;

        [SerializeField, Tooltip("tell if player can control character")]
        private bool _isCharacterControllable = true;

        private bool _isPausedMemo = false;
        private float _timeScaleDuringPauseSave = 0;

        #region Public API

        public bool IsPaused
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        public bool IsGameOver
        {
            get { return _isGameOver; }
            set { _isGameOver = value; }
        }

        public bool IsCharacterControllable
        {
            get { return _isCharacterControllable; }
            set { _isCharacterControllable = value; }
        }

        #endregion


        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _isPausedMemo = _isPaused;
        }

        protected virtual void Update()
        {
            if (_isPaused != _isPausedMemo)
                OnSwitchPauseAction();

            _isPausedMemo = _isPaused;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (hideInInspector)
                hideFlags = HideFlags.HideInInspector;
            else
                hideFlags = HideFlags.None;
        }

        protected virtual void OnSwitchPauseAction()
        {
            if(_isPaused)
            {
                _timeScaleDuringPauseSave = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = _timeScaleDuringPauseSave;
            }
        }
    }


    //base class to implement as child script of GameManager in projects :
    //public class ThisGameManager : GameManager
    //{
    //    public static ThisGameManager ThisInstance
    //    {
    //        get
    //        {
    //            return (ThisGameManager)Instance;
    //        }
    //    }
    //}
}