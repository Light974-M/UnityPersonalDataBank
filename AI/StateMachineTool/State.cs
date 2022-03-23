using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    public class State
    {
        private List<Transition> _transitionList;

        private string _stateName;

        #region public API

        public List<Transition> TransitionList
        {
            get
            {
                return _transitionList;
            }
            
            set
            {
                _transitionList = value;
            }
        }

        #endregion

        public State(string stateName)
        {
            _stateName = stateName;
        }

        public Transition TestTransiting(Dictionary<string, object> blackBoard)
        {
            Transition isTransiting = null;

            foreach (Transition transition in _transitionList)
                if (transition.TestTransition(blackBoard))
                    isTransiting = transition;

            return isTransiting;
        }
    }
}

