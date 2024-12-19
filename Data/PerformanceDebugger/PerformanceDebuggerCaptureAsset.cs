using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Data.PerformanceDebugger
{
    [CreateAssetMenu(fileName = "new Capture Asset", menuName = NamespaceID.DataPath + "/" + NamespaceID.PerformanceDebugger + "/PerformanceDebugger Capture Asset")]
    public class PerformanceDebuggerCaptureAsset : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, Tooltip("capture fps and average fps at a certain time to make precise tests")]
        private List<CapturedExperience> _captureExperiences;

        [SerializeField, Tooltip("is enabled, each start will automatically create a new experience")]
        private bool _createExperience = true;

        #region Public API

        public bool CreateExperience => _createExperience;

        public List<CapturedExperience> CapturedExperiences
        {
            get { return _captureExperiences; }
            set { _captureExperiences = value; }
        }

        #endregion

        public void RegisterCaptureInfo(CaptureInfo info)
        {
            if (_captureExperiences != null && _captureExperiences.Count != 0)
                _captureExperiences[_captureExperiences.Count - 1].CaptureSaves.Add(info);
        }

        public void CreateNewExperience()
        {
            _captureExperiences.Add(new CapturedExperience("experience #" + _captureExperiences.Count.ToString()));
        } 
#endif
    }
}
