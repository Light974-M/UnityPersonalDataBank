using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    [CustomEditor(typeof(TriggerSingleton))]
	public class TriggerSingletonEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            TriggerSingleton myTarget = (TriggerSingleton)target;

            GUI.enabled = !myTarget.Value;

            if(GUILayout.Button("Trigger"))
                myTarget.Activate();

            GUI.enabled = myTarget.Value;

            if (GUILayout.Button("Reinitialize"))
                myTarget.Value = false;

            GUI.enabled = true;
        }
    } 
}
