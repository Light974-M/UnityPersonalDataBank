using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.Ai + "/" + NamespaceID.StateMachineTool + "/StateMachine")]
    public class StateMachineRenderer : UPDBBehaviour
    {
        private StateMachine _stateMachine;

        [SerializeField, Tooltip("")]
        private StateMachinePreset _stateMachinePreset;

        [System.Serializable]
        public struct SerializedBlackBoard
        {
            public string _key;

            public int _intValue;
            public float _floatValue;
            public bool _boolValue;

            public enum ValueType
            {
                Bool,
                Int,
                Float,
            }

            public ValueType _valueType;

            public SerializedBlackBoard(string key, int value)
            {
                _key = key;
                _intValue = value;
                _floatValue = 0;
                _boolValue = false;
                _valueType = ValueType.Int;
            }

            public SerializedBlackBoard(string key, float value)
            {
                _key = key;
                _intValue = 0;
                _floatValue = value;
                _boolValue = false;
                _valueType = ValueType.Float;
            }

            public SerializedBlackBoard(string key, bool value)
            {
                _key = key;
                _intValue = 0;
                _floatValue = 0;
                _boolValue = value;
                _valueType = ValueType.Bool;
            }
        }

        [SerializeField, Tooltip("")]
        private SerializedBlackBoard[] _blackBoard;

        #region Public API

        public SerializedBlackBoard[] BlackBoard
        {
            set
            {
                _stateMachine.BlackBoard = new Dictionary<string, object>();

                foreach (SerializedBlackBoard item in _blackBoard)
                {
                    if (item._valueType == SerializedBlackBoard.ValueType.Bool)
                        _stateMachine.BlackBoard.Add(item._key, item._boolValue);
                    else if (item._valueType == SerializedBlackBoard.ValueType.Int)
                        _stateMachine.BlackBoard.Add(item._key, item._intValue);
                    else if (item._valueType == SerializedBlackBoard.ValueType.Float)
                        _stateMachine.BlackBoard.Add(item._key, item._floatValue);
                }
            }
        }

        public StateMachine StateMachine => _stateMachine;

        public StateMachinePreset StateMachinePreset => _stateMachinePreset;

        #endregion

        private void Update()
        {
            _stateMachine.UpdateMachine();
        }

        protected override void OnScene()
        {
            if (_stateMachine == null)
                _stateMachine = new StateMachine(_stateMachinePreset.StateList[0]);
        }
    }
}

