using UnityEngine;
using UPDB.CoreHelper.Usable;

namespace UPDB.Data.CustomScriptExecutionOrder
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

        public int IterationLaterRUpdateCount
        {
            get => _iterationLaterUpdateCount;
            set => _iterationLaterUpdateCount = value;
        }

        public int IterationLateOnDrawGizmosCount
        {
            get => _iterationLateOnDrawGizmosCount;
            set => _iterationLateOnDrawGizmosCount = value;
        }

        #endregion

        #region MethodsEvents

        public delegate void CustomFunctionIterationCall(int i);
        public delegate void CustomFunctionUniqueCall();

        public event CustomFunctionUniqueCall OnLateFixedUpdate;
        public event CustomFunctionUniqueCall OnLaterUpdate;
        public event CustomFunctionUniqueCall OnLateOnDrawGizmo;

        public event CustomFunctionIterationCall OnLateFixedUpdateIteration;
        public event CustomFunctionIterationCall OnLaterUpdateIteration;
        public event CustomFunctionIterationCall OnLateOnDrawGizmosIteration;

        #endregion

        // Update is called once per frame
        private void FixedUpdate()
        {
            OnLateFixedUpdate?.Invoke();

            for (int i = 0; i < _iterationLateFixedUpdateCount; i++)
                OnLateFixedUpdateIteration?.Invoke(i);
        }

        private void LateUpdate()
        {
            OnLaterUpdate?.Invoke();

            for (int i = 0; i < _iterationLaterUpdateCount; i++)
                OnLaterUpdateIteration?.Invoke(i);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            OnLateOnDrawGizmo?.Invoke();

            for (int i = 0; i < _iterationLateOnDrawGizmosCount; i++)
                OnLateOnDrawGizmosIteration?.Invoke(i);
        }
    }
}
