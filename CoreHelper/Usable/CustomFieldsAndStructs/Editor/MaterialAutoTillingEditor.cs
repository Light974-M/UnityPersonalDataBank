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

            EditorGUI.BeginChangeCheck();

            GUIContent referenceMaterialContent = new GUIContent(nameof(myTarget.ReferenceMaterial), "material used to create material instance of tilling");
            myTarget.ReferenceMaterial = (Material)EditorGUILayout.ObjectField(referenceMaterialContent, myTarget.ReferenceMaterial, typeof(Material), true);

            if(EditorGUI.EndChangeCheck())
            {
                myTarget.Renderer.sharedMaterial = myTarget.ReferenceMaterial;
                myTarget.ObjectMaterial = null;
                SceneView.lastActiveSceneView.Repaint();
            }

            GUIContent scaleContent = new GUIContent(nameof(myTarget.Scale), "scale to maintain");
            myTarget.Scale = EditorGUILayout.Vector2Field(scaleContent, myTarget.Scale);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("linkedAxis");
                myTarget.XTillingLinkedScale = (Axis)EditorGUILayout.EnumPopup(myTarget.XTillingLinkedScale);
                myTarget.YTillingLinkedScale = (Axis)EditorGUILayout.EnumPopup(myTarget.YTillingLinkedScale);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Regenerate Material"))
            {
                myTarget.ObjectMaterial = null;
                SceneView.lastActiveSceneView.Repaint();
            }
        }
    }
}
