using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPDB.Data.PerformanceDebugger
{
    [CustomEditor(typeof(PerformanceDebugger))]
    public class PerformanceDebuggerEditor : Editor
    {
        private void OnEnable()
        {
            PerformanceDebugger myTarget = (PerformanceDebugger)target;

            myTarget.UpdateHandler += RepaintEditor;
        }

        public override void OnInspectorGUI()
        {
            PerformanceDebugger myTarget = (PerformanceDebugger)target;

            base.OnInspectorGUI();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("FPS DISPLAY");

            Color GUIBackground = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            GUIContent resetCountContent = new GUIContent("RESET COUNT", "reinit fps and average Fps counts");

            if (GUILayout.Button(resetCountContent))
                myTarget.ReinitAll();

            GUIContent makeCaptureContent = new GUIContent("MAKE A CAPTURE", "capture current state into scriptable manually");

            if (GUILayout.Button(makeCaptureContent))
                myTarget.Capture();

            GUI.enabled = false;

            GUIContent averageFpsDisplayContent = new GUIContent("Fps", "display fps following parameters input");
            EditorGUILayout.FloatField(averageFpsDisplayContent, myTarget.AverageFps);

            GUIContent averageFpsSincePlayDisplayContent = new GUIContent("Average Fps", "display average fps since play hits or reinit button has been clicked");
            EditorGUILayout.FloatField(averageFpsSincePlayDisplayContent, myTarget.AverageFpsSincePlay);

            GUI.enabled = true;
            GUI.backgroundColor = GUIBackground;

            GUIContent realtimeFpsGraphContent = new GUIContent("Fps Graph", "show realtime evolution of fps in a graph");
            EditorGUILayout.CurveField(realtimeFpsGraphContent, myTarget.RealtimeFpsGraph);
        }

        public void RepaintEditor()
        {
            Repaint();
        }
    } 
}
