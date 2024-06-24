using UnityEngine;
using UnityEngine.SceneManagement;

namespace UPDB.CoreHelper.UsableMethods
{
    /// <summary>
    /// singleton manager of all projects global properties
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField, Tooltip("tell if the game is in pause or not")]
        private bool _isPaused = false;

        [SerializeField, Tooltip("tell if the game is in pause or not")]
        private bool _isGameOver = false;

        [SerializeField, Tooltip("tell if player can control character")]
        private bool _isCharacterControllable = true;

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

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Initialize();
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

}