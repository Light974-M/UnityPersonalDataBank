using UnityEditor;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    [CustomEditor(typeof(AmbianceMixer)), CanEditMultipleObjects]
    public class AmbianceMixerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AmbianceMixer myTarget = (AmbianceMixer)target;

            if (myTarget.DrawDefaultInspector)
                DrawDefaultInspector();
            else
                DrawCustomInspector(myTarget);
        }

        private void DrawCustomInspector(AmbianceMixer myTarget)
        {
            /************MIX TABLE************/
            //GUILayout.Label("MIX TABLE", EditorStyles.boldLabel);
            //GUILayout.BeginVertical("HelpBox");
            //{
                
            //}
            //GUILayout.EndVertical();
            //GUILayout.Space(20);


            /************CUSTOM INSPECTOR************/
            GUILayout.Label("CUSTOM INSPECTOR", EditorStyles.boldLabel);
            GUILayout.BeginVertical("HelpBox");
            {
                myTarget.DrawDefaultInspector = EditorGUILayout.Toggle(new GUIContent("Draw Default Inspector"), myTarget.DrawDefaultInspector);
            }
            GUILayout.EndVertical();
        }
    } 
}
