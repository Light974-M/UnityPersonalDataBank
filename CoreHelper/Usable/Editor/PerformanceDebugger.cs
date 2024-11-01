using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.Usable
{
    [AddComponentMenu(NamespaceID.UsablePath + "/Performance Debugger")]
    public class PerformanceDebugger : Singleton<PerformanceDebugger>
    {
        #region Serialized API

        /***************************************CAPTURE PARAMETERS***************************************/
        [Space, Header("CAPTURE PARAMETERS")]
        [SerializeField, Tooltip("asset that contains all experiences and captures")]
        private PerformanceDebuggerCaptureAsset _experiencesSaves;

        [SerializeField, Tooltip("time since play was hit")]
        private float _playTime = 0;

        [SerializeField, Tooltip("time since last init or play")]
        private float _fromInitTime = 0;

        [SerializeField, Tooltip("choose a frequence of capture, 0 means no capture")]
        private float _captureFrequence = 0;

        [SerializeField, Tooltip("if enable, will capture each frame fps for fpsGraph, otherwise, it register average fps(can take ressources if captures are taking long times")]
        private bool _captureGraphByFrame = false;

        [SerializeField, Tooltip("follow a pattern of time wich system capture state")]
        private List<float> _capturePlan = new List<float>();

        /*****************************************FPS PARAMETERS*****************************************/
        [Space, Header("FPS PARAMETERS")]
        [SerializeField, Tooltip("time between two fps registration")]
        private float _averageFpsUpdateTime = 1;

        [SerializeField, Tooltip("make a debug in console for fps")]
        private bool _debugLog = false;

        [SerializeField, Tooltip("set size of realtime fps graph, 0 disable graph")]
        private int _fpsGraphSize = 60;

        [SerializeField, Tooltip("if enabled, graph will never delete fps, can use a lot of ressources if played for a long time")]
        private bool _infiniteGrowingGraph = false;

        #endregion

        #region Non Serialized API

        /***************************************CAPTURE PARAMETERS***************************************/
        private float _captureFrequenceTimer = 0;
        private float _beginTime = 0;
        

        /*****************************************FPS PARAMETERS*****************************************/
        private float _averageFps = 0;
        private float _averageFpsSincePlay = 0;
        private float _fpsUpdateTimer = 0;
        private List<float> _fpsList = new List<float>();
        private List<float> _averageFpsList = new List<float>();
        private List<float> _fpsListDuringCurrentCapture = new List<float>();
        private AnimationCurve _realtimeFpsGraph = new AnimationCurve();

        #endregion

        #region Public API

        /***************************************CAPTURE PARAMETERS***************************************/



        /*****************************************FPS PARAMETERS*****************************************/
        public float AverageFps => _averageFps;
        public float AverageFpsSincePlay => _averageFpsSincePlay;

        public delegate void UpdateHandlerCallback();
        public event UpdateHandlerCallback UpdateHandler;
        public AnimationCurve RealtimeFpsGraph => _realtimeFpsGraph;

        #endregion

        private void Start()
        {
            if (_experiencesSaves.CreateExperience)
                _experiencesSaves.CreateNewExperience();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_fpsUpdateTimer >= _averageFpsUpdateTime)
                UpdateFps();

            RegisterCurrentFrameFps();

            if (_experiencesSaves && _captureFrequence != 0 && _captureFrequenceTimer >= _captureFrequence)
            {
                Capture();
                _captureFrequenceTimer = 0;
            }
            else
            {
                TestCapturePlan();
            }

            _fpsUpdateTimer += Time.deltaTime;
            _playTime += Time.deltaTime;
            _captureFrequenceTimer += Time.deltaTime;
            _fromInitTime += Time.deltaTime;
        }

        public void ReinitAll()
        {
            _beginTime = _playTime;

            _fpsUpdateTimer = 0;
            _fromInitTime = 0;

            _fpsList.Clear();
            _averageFpsList.Clear();
        }

        private void RegisterCurrentFrameFps()
        {
            if (Time.deltaTime != 0)
            {
                float currentFrameFps = 1 / Time.deltaTime;

                _fpsList.Add(currentFrameFps);

                if (_captureGraphByFrame)
                    _fpsListDuringCurrentCapture.Add(currentFrameFps);

                if (_fpsGraphSize > 0)
                    UpdateRealtimeFpsGraph(currentFrameFps);
            }
        }

        private void UpdateFps()
        {
            _averageFps = CalculateAverageFps(_fpsList);
            _averageFpsSincePlay = CalculateAverageFps(_averageFpsList);

            if (_debugLog)
                Debug.Log(_averageFps);

            _averageFpsList.Add(_averageFps);

            if (!_captureGraphByFrame)
                _fpsListDuringCurrentCapture.Add(_averageFps);

            _fpsList.Clear();
            _fpsUpdateTimer = 0;
            UpdateHandler?.Invoke();
        }

        private float CalculateAverageFps(List<float> fpsList)
        {
            float averageFps = 0;

            foreach (float fps in fpsList)
                averageFps += fps;

            return averageFps / fpsList.Count;
        }

        private void TestCapturePlan()
        {
            for (int i = 0; i < _capturePlan.Count; i++)
            {
                if (_fromInitTime >= _capturePlan[i])
                {
                    _capturePlan.RemoveAt(i);
                    Capture();
                }
            }
        }

        public void Capture()
        {
            float time = _playTime;
            float fromInitTime = _fromInitTime;
            float fps = _averageFps;
            float averageFps = _averageFpsSincePlay;
            float updateRate = _averageFpsUpdateTime;
            float beginTime = _beginTime;
            AnimationCurve fpsGraph = AnimationCurve.Constant(0, 1, 0);

            float xDivider = _fpsListDuringCurrentCapture.Count - 1;
            float yDivider = FindHighestFps();
            fpsGraph.ClearKeys();

            for (int i = 0; i < _fpsListDuringCurrentCapture.Count; i++)
            {
                Keyframe key = new Keyframe(i / xDivider, _fpsListDuringCurrentCapture[i] / yDivider);
                fpsGraph.AddKey(key);
            }

            for (int i = 0; i < fpsGraph.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(fpsGraph, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(fpsGraph, i, AnimationUtility.TangentMode.Linear);
            }

            _fpsListDuringCurrentCapture.Clear();

            CapturedExperience currentExperience = _experiencesSaves.CapturedExperiences[_experiencesSaves.CapturedExperiences.Count - 1];
            float previousTimeDelta = 0;

            if (currentExperience.CaptureSaves != null && currentExperience.CaptureSaves.Count != 0)
            {
                CaptureInfo lastRegisteredCaptureInfo = currentExperience.CaptureSaves[currentExperience.CaptureSaves.Count - 1];
                previousTimeDelta = _playTime - lastRegisteredCaptureInfo.Time;
            }

            CaptureInfo captureSender = new CaptureInfo(time, fromInitTime, fps, averageFps, updateRate, beginTime, fpsGraph, previousTimeDelta);

            _experiencesSaves.RegisterCaptureInfo(captureSender);
        }

        private float FindHighestFps()
        {
            float highestFps = 0;

            foreach (float fps in _fpsListDuringCurrentCapture)
                if (fps > highestFps)
                    highestFps = fps;

            return highestFps;
        }

        private void UpdateRealtimeFpsGraph(float currentFrameFps)
        {
            Keyframe keyframe = new Keyframe(_playTime, currentFrameFps);
            _realtimeFpsGraph.AddKey(keyframe);

            AnimationUtility.SetKeyLeftTangentMode(_realtimeFpsGraph, _realtimeFpsGraph.keys.Length - 1, AnimationUtility.TangentMode.Linear);

            if (_realtimeFpsGraph.length <= 1)
                AnimationUtility.SetKeyRightTangentMode(_realtimeFpsGraph, _realtimeFpsGraph.keys.Length - 1, AnimationUtility.TangentMode.Linear);
            else
                AnimationUtility.SetKeyRightTangentMode(_realtimeFpsGraph, _realtimeFpsGraph.keys.Length - 2, AnimationUtility.TangentMode.Linear);

            if (!_infiniteGrowingGraph && _realtimeFpsGraph.keys.Length >= _fpsGraphSize)
                _realtimeFpsGraph.RemoveKey(0);
        }
    }

    [System.Serializable]
    public struct CaptureInfo
    {
        [Space, Header("BASIC INFOS")]
        [SerializeField, Tooltip("time during capture")]
        private float _time;

        [SerializeField, Tooltip("time since last init")]
        private float _fromInitTime;

        [SerializeField, Tooltip("value of last registered fps")]
        private float _fps;

        [SerializeField, Tooltip("value of last registered average fps")]
        private float _averageFps;


        [Space, Header("PARAMETERS DURING CAPTURE")]
        [SerializeField, Tooltip("fps register rate during test")]
        private float _updateTime;

        [SerializeField, Tooltip("time during last reset(0 if no reset since play mode)")]
        private float _beginTime;

        [Space, Header("ADVANCED INFOS")]
        [SerializeField, Tooltip("show evolution of fps during time after play or after the last reset")]
        private AnimationCurve _fpsGraph;

        [SerializeField, Tooltip("time between this capture and the previous")]
        private float _previousTimeDelta;

        #region Public API

        public float Time => _time;

        #endregion

        public CaptureInfo(float time, float fromInitTime, float fps, float averageFps, float updateTime, float beginTime, AnimationCurve fpsGraph, float previousTimeDelta)
        {
            _time = time;
            _fromInitTime = fromInitTime;
            _fps = fps;
            _averageFps = averageFps;
            _updateTime = updateTime;
            _beginTime = beginTime;
            _fpsGraph = fpsGraph;
            _previousTimeDelta = previousTimeDelta;
        }
    }

    [System.Serializable]
    public struct CapturedExperience
    {
        [SerializeField, Tooltip("name of experience")]
        private string _experienceName;

        [SerializeField, Tooltip("capture fps and average fps at a certain time to make precise tests")]
        private List<CaptureInfo> _captureSaves;

        #region Public API

        public List<CaptureInfo> CaptureSaves
        {
            get { return _captureSaves; }
            set { _captureSaves = value; }
        }

        #endregion

        public CapturedExperience(string experienceName)
        {
            _experienceName = experienceName;
            _captureSaves = new List<CaptureInfo>();
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PerformanceDebugger))]
    public class PerformanceDebuggerEditor : Editor
    {
        private void OnEnable()
        {
            PerformanceDebugger myTarget = (PerformanceDebugger)target;

            myTarget.UpdateHandler += RepaintEditor;
        }

        public override void OnInspectorGUI()
        {
            PerformanceDebugger myTarget = (PerformanceDebugger)target;

            base.OnInspectorGUI();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("FPS DISPLAY");

            Color GUIBackground = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            GUIContent resetCountContent = new GUIContent("RESET COUNT", "reinit fps and average Fps counts");

            if (GUILayout.Button(resetCountContent))
                myTarget.ReinitAll();

            GUIContent makeCaptureContent = new GUIContent("MAKE A CAPTURE", "capture current state into scriptable manually");

            if (GUILayout.Button(makeCaptureContent))
                myTarget.Capture();

            GUI.enabled = false;

            GUIContent averageFpsDisplayContent = new GUIContent("Fps", "display fps following parameters input");
            EditorGUILayout.FloatField(averageFpsDisplayContent, myTarget.AverageFps);

            GUIContent averageFpsSincePlayDisplayContent = new GUIContent("Average Fps", "display average fps since play hits or reinit button has been clicked");
            EditorGUILayout.FloatField(averageFpsSincePlayDisplayContent, myTarget.AverageFpsSincePlay);

            GUI.enabled = true;
            GUI.backgroundColor = GUIBackground;

            GUIContent realtimeFpsGraphContent = new GUIContent("Fps Graph", "show realtime evolution of fps in a graph");
            EditorGUILayout.CurveField(realtimeFpsGraphContent, myTarget.RealtimeFpsGraph);
        }

        public void RepaintEditor()
        {
            Repaint();
        }
    }

#endif
}
