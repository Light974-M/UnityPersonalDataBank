using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/AI/StateMachineTool/StateMachine")]
    public class StateMachineRenderer : UPDBBehaviour
    {
        private StateMachine _stateMachine;

        private void Update()
        {
            _stateMachine.UpdateMachine(_stateMachine.BlackBoard);
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying)
            {
                if (_stateMachine == null)
                {
                    State idle = new State("idle");
                    State eat = new State("eat");
                    State reproduce = new State("reproduce");

                    _stateMachine = new StateMachine(idle);

                    idle.TransitionList.Add(new Transition(idle, eat));
                    idle.TransitionList.Add(new Transition(idle, reproduce));
                    eat.TransitionList.Add(new Transition(eat, idle));
                    reproduce.TransitionList.Add(new Transition(reproduce, idle));

                    _stateMachine.BlackBoard.Add("isHungry", false);
                    _stateMachine.BlackBoard.Add("isHeat", false);

                    idle.TransitionList[0].ConditionsList.Add(new BoolCondition("isHungry", true));
                    idle.TransitionList[1].ConditionsList.Add(new BoolCondition("isHeat", true));
                    eat.TransitionList[0].ConditionsList.Add(new BoolCondition("isHungry", false));
                    reproduce.TransitionList[0].ConditionsList.Add(new BoolCondition("isHeat", false));
                }
            }
        }
    }
}

