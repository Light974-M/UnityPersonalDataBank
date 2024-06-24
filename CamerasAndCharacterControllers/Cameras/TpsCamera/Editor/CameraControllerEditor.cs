using System;
using UnityEditor;
using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.Cameras.TpsCamera
{
    [CustomEditor(typeof(CameraController)), CanEditMultipleObjects]
    public class CameraControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CameraController myTarget = (CameraController)target;

            DrawCustomInspector(myTarget);

            if (!Application.isPlaying)
                myTarget.InitVariables();
        }

        private void DrawCustomInspector(CameraController myTarget)
        {
            myTarget.CameraPivot = (Transform)EditorGUILayout.ObjectField("CameraPivot", myTarget.CameraPivot, typeof(Transform), true);
            myTarget.LookSpeed = EditorGUILayout.Vector2Field(new GUIContent("Look Speed"), myTarget.LookSpeed);
        }
    } 
}
