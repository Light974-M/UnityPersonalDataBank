using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    public class Transition
    {
        private List<Condition> _conditionsList;

        private State _stateFrom;
        private State _stateTo;

        #region public API

        public List<Condition> ConditionsList
        {
            get
            {
                return _conditionsList;
            }

            set
            {
                _conditionsList = value;
            }
        }

        public State StateFrom => _stateFrom;
        public State StateTo => _stateTo;

        #endregion

        public Transition(State stateFrom, State stateTo)
        {
            _stateFrom = stateFrom;
            _stateTo = stateTo;
        }

        public bool TestTransition(Dictionary<string, object> blackBoard)
        {
            bool isTrue = true;

            foreach (Condition condition in _conditionsList)
                if (!condition.TestCondition(blackBoard))
                    isTrue = false;

            return isTrue;
        }
    }
}

