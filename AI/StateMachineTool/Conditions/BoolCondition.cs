using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    public class BoolCondition : Condition
    {
        private string _variableRef;
        private bool _conditionValue;

        public BoolCondition(string variableRef, bool conditionValue)
        {
            _conditionValue = conditionValue;
            _variableRef = variableRef;
        }

        public BoolCondition()
        {
            _variableRef = "";
            _conditionValue = false;
        }

        public override bool TestCondition(Dictionary<string, object> blackBoard)
        {
            return (bool)blackBoard[_variableRef] == _conditionValue;
        }
    }
}

