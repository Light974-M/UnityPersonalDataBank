using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.CustomScriptOrderMethods
{
    /// <summary>
    /// To put in script execution order, position 1
    /// </summary>
    public class LateCustomMethodsManager : Singleton<LateCustomMethodsManager>
    {
        [SerializeField, Tooltip("number of time the late fixed update method is called in one update, 0 to disable method call")]
        protected int _iterationLateFixedUpdateCount = 1;

        [SerializeField, Tooltip("number of time the later update method is called in one update, 0 to disable method call")]
        protected int _iterationLaterUpdateCount = 1;

        [SerializeField, Tooltip("number of time the late OnDrawGizmos update method is called in one update, 0 to disable method call")]
        protected int _iterationLateOnDrawGizmosCount = 1;

        #region Public API

        public int IterationLateFixedUpdateCount
        {
            get => _iterationLateFixedUpdateCount;
            set => _iterationLateFixedUpdateCount = value;
        }

        #endregion

        #region MethodsEvents

        public delegate void CustomFunctionCall(int i);
        public event CustomFunctionCall OnLateFixedUpdate;
        public event CustomFunctionCall OnLaterUpdate;
        public event CustomFunctionCall OnLateOnDrawGizmos;

        #endregion

        // Update is called once per frame
        private void FixedUpdate()
        {
            for (int i = 0; i < _iterationLateFixedUpdateCount; i++)
                OnLateFixedUpdate?.Invoke(i);
            Debug.Log("LateFixedUpdate");
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _iterationLaterUpdateCount; i++)
                OnLaterUpdate?.Invoke(i);
            Debug.Log("LaterUpdate");
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            for (int i = 0; i < _iterationLateOnDrawGizmosCount; i++)
                OnLateOnDrawGizmos?.Invoke(i);
        }
    }
}
