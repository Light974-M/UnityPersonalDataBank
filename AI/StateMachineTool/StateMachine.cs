using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    public class StateMachine
    {
        private State _activeState;

        private State _initState;

        private Dictionary<string, object> _blackBoard;


        #region Public API

        public State InitState
        {
            get
            {
                return _initState;
            }

            set
            {
                _initState = value;
            }
        }

        public Dictionary<string, object> BlackBoard
        {
            get
            {
                return _blackBoard;
            }

            set
            {
                _blackBoard = value;
            }
        }

        #endregion


        public StateMachine()
        {

        }

        public StateMachine(State initState)
        {
            _initState = initState;
            _activeState = _initState;
        }

        public void UpdateMachine(Dictionary<string, object> blackBoard)
        {
            State newStateToPut = _activeState.TestTransiting(blackBoard).StateTo;

            if (newStateToPut != null)
                _activeState = newStateToPut;
        }
    }
}
