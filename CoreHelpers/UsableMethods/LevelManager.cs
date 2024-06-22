using System.CodeDom;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UPDB.CoreHelper.UsableMethods
{
    /// <summary>
    /// singleton manager of scenes global properties, unique scene properties are stored in children classes of levelManager
    /// </summary>
    public abstract class LevelManager<LevelOverrideStartInfo> : Singleton<LevelManager<LevelOverrideStartInfo>> where LevelOverrideStartInfo : LevelStartInfo
    {
        [SerializeField, Tooltip("name of level")]
        protected string _levelName = "";

        [SerializeField, Tooltip("unique index of level")]
        protected int _levelIndex = 0;

        [SerializeField, Tooltip("all parameters that will define start state of level")]
        protected LevelOverrideStartInfo _startInfo;

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
            _levelName = SceneManager.GetActiveScene().name;
            _levelIndex = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// called when level is loading on awake, init level values
        /// </summary>
        protected virtual void LoadLevel()
        {
            GameObject.FindWithTag("Player").transform.position = _startInfo.PlayerStartPos;
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
            GameObject.FindWithTag("Player").transform.position = _startInfo.PlayerStartPos;
        }
    }

}