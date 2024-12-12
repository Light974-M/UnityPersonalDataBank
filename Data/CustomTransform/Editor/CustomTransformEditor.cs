using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UPDB.Data.CustomTransform
{
    [CustomEditor(typeof(CustomTransformManager))]
    public class CustomTransformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CustomTransformManager myTarget = (CustomTransformManager)target;

            EditorGUI.BeginChangeCheck();

            GUIContent parentContent = new GUIContent(nameof(myTarget.Parent), "parent of simulated transform");
            myTarget.Parent = (Transform)EditorGUILayout.ObjectField(parentContent, myTarget.Parent, typeof(Transform), true);

            GUIContent localPositionContent = new GUIContent("Position", "position of transform");
            myTarget.LocalPosition = EditorGUILayout.Vector3Field(localPositionContent, myTarget.LocalPosition);

            GUIContent localEulerAnglesContent = new GUIContent("Rotation", "rotation of transform");
            myTarget.LocalEulerAngles = EditorGUILayout.Vector3Field(localEulerAnglesContent, myTarget.LocalEulerAngles);

            GUIContent localScaleContent = new GUIContent("Scale", "scale of transform");

            if (myTarget.ProportionsConstraint && (myTarget.LastRegisteredproportions.x == 0 || myTarget.LastRegisteredproportions.y == 0 || myTarget.LastRegisteredproportions.z == 0))
                myTarget.LocalScale = CustomVector3Field(localScaleContent, myTarget.LocalScale, myTarget.LastRegisteredproportions.x != 0, myTarget.LastRegisteredproportions.y != 0, myTarget.LastRegisteredproportions.z != 0);
            else
                myTarget.LocalScale = EditorGUILayout.Vector3Field(localScaleContent, myTarget.LocalScale);

            GUIContent proportionsContraintContent = new GUIContent(nameof(myTarget.ProportionsConstraint), "is constraints enabled for local scale ?");
            myTarget.ProportionsConstraint = EditorGUILayout.Toggle(proportionsContraintContent, myTarget.ProportionsConstraint);

            if(EditorGUI.EndChangeCheck())
            {
                myTarget.OnChangeEventInvoke();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(myTarget);
            }
        }

        public static Vector3 CustomVector3Field(GUIContent label, Vector3 value, bool enableX = true, bool enableY = true, bool enableZ = true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);

            // Sauvegarder l'état initial de GUI.enabled
            bool previousGuiState = GUI.enabled;

            // Champ X
            GUI.enabled = enableX;
            value.x = EditorGUILayout.FloatField(value.x);

            // Champ Y
            GUI.enabled = enableY;
            value.y = EditorGUILayout.FloatField(value.y);

            // Champ Z
            GUI.enabled = enableZ;
            value.z = EditorGUILayout.FloatField(value.z);

            // Restaurer l'état initial de GUI.enabled
            GUI.enabled = previousGuiState;

            EditorGUILayout.EndHorizontal();

            return value;
        }
    }
}
