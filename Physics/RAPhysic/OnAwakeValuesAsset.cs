using UnityEngine;

namespace UPDB.Physic.RAPhysic
{
    public class OnAwakeValuesAsset : ScriptableObject
    {
        [SerializeField]
        private bool _isOnFirstScriptInstanceLoading = true;

        public bool IsOnFirstScriptInstanceLoading
        {
            get => _isOnFirstScriptInstanceLoading;
            set => _isOnFirstScriptInstanceLoading = value;
        }
    }
}
