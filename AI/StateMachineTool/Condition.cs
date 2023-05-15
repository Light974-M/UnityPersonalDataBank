using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Ai.StateMachineTool
{
    ///<summary>
    /// 
    ///</summary>
    public abstract class Condition
    {
        public abstract bool TestCondition(Dictionary<string, object> blackBoard);
    }
}

