using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    [CustomEditor(typeof(MaterialAutoTilling))]
    public class MaterialAutoTillingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MaterialAutoTilling myTarget = (MaterialAutoTilling)target;

            GUIContent scaleContent = new GUIContent(nameof(myTarget.Scale), "scale to maintain");
            myTarget.Scale = EditorGUILayout.Vector2Field(scaleContent, myTarget.Scale);

            if (GUILayout.Button("Regenerate Material"))
            {
                myTarget.ObjectMaterial = null;
                SceneView.lastActiveSceneView.Repaint();
            }
        }
    }
}
