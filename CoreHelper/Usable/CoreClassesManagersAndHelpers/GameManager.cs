using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace UPDB.CoreHelper.Usable
{
    /// <summary>
    /// singleton manager of all projects global properties
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField, Tooltip("tell if the game is in pause or not")]
        private bool _isGameOver = false;

        [SerializeField, Tooltip("tell if player can control character")]
        private bool _isCharacterControllable = true;

        [SerializeField, Tooltip("main volume mixer")]
        private AudioMixer _volumeMainMixer;

        [SerializeField, Tooltip("if enabled, will use values under to set cursor modes at start")]
        private bool _setCursorModes = false;

        [SerializeField, Tooltip("if disabled, will hide the cursor while focused at start")]
        private bool _startCursorVisible = true;

        [SerializeField, Tooltip("type of constraint for mouse at start")]
        private CursorLockMode _startCursorLockState = CursorLockMode.Confined;

        #region Public API

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

        public AudioMixer VolumeMainMixer
        {
            get => _volumeMainMixer;
            set { _volumeMainMixer = value; }
        }

        public bool SetCursorModes
        {
            get => _setCursorModes;
            set => _setCursorModes = value;
        }

        public bool StartCursorVisible
        {
            get => _startCursorVisible;
            set => _startCursorVisible = value;
        }

        public CursorLockMode StartCursorLockState
        {
            get => _startCursorLockState;
            set => _startCursorLockState = value;
        }

        #endregion


        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if(_setCursorModes)
            {
                Cursor.visible = _startCursorVisible;
                Cursor.lockState = _startCursorLockState;
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (hideInInspector)
                hideFlags = HideFlags.HideInInspector;
            else
                hideFlags = HideFlags.None;
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