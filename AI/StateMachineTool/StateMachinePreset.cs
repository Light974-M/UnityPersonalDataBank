using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    [CreateAssetMenu(fileName = "NewStateMachinePreset", menuName = "UPDB/AI/StateMachineTool/StateMachinePreset")]
    public class StateMachinePreset : ScriptableObject
    {
        [SerializeField]
        private State[] _stateList;

        [SerializeField]
        private bool _init = false;

        #region Public API

        public State[] StateList => _stateList;

        #endregion


        /// <summary>
        /// 
        /// </summary>
        private void OnValidate()
        {
            _stateList = new State[3];

            _stateList[0] = new State("idle");
            _stateList[1] = new State("eat");
            _stateList[2] = new State("reproduce");

            _stateList[0].TransitionList.Add(new Transition(_stateList[0], _stateList[1]));
            _stateList[0].TransitionList.Add(new Transition(_stateList[0], _stateList[2]));
            _stateList[1].TransitionList.Add(new Transition(_stateList[1], _stateList[0]));
            _stateList[2].TransitionList.Add(new Transition(_stateList[2], _stateList[0]));

            _stateList[0].TransitionList[0].ConditionsList.Add(new BoolCondition("isHungry", true));
            _stateList[0].TransitionList[1].ConditionsList.Add(new BoolCondition("isHeat", true));
            _stateList[1].TransitionList[0].ConditionsList.Add(new BoolCondition("isHungry", false));
            _stateList[2].TransitionList[0].ConditionsList.Add(new BoolCondition("isHeat", false));

            _init = false;
        }
    } 
}