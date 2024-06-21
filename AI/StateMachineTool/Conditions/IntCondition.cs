using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    public class IntCondition : Condition
    {
        private int _variable;
        private int _conditionValue;

        public enum ConditionType
        {
            Equal,
            NotEqual,
            More,
            MoreEqual,
            Less,
            LessEqual,
        }

        private ConditionType _conditionType;

        public IntCondition(int variable, int conditionValue, ConditionType conditionType)
        {
            _variable = variable;
            _conditionValue = conditionValue;
            _conditionType = conditionType;
        }

        public override bool TestCondition(Dictionary<string, object> blackBoard)
        {
            bool isTrue = false;

            switch (_conditionType)
            {
                case ConditionType.Equal:
                    isTrue = _variable == _conditionValue;
                    break;

                case ConditionType.NotEqual:
                    isTrue = _variable != _conditionValue;
                    break;

                case ConditionType.More:
                    isTrue = _variable > _conditionValue;
                    break;

                case ConditionType.MoreEqual:
                    isTrue = _variable >= _conditionValue;
                    break;

                case ConditionType.Less:
                    isTrue = _variable < _conditionValue;
                    break;

                case ConditionType.LessEqual:
                    isTrue = _variable <= _conditionValue;
                    break;
            }

            return isTrue;
        }
    }
}

