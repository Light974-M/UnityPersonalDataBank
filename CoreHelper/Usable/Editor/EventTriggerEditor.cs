using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.Usable
{
    [CustomEditor(typeof(EventTrigger))]
    public class EventTriggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EventTrigger myTarget = (EventTrigger)target;

            GUIContent dropDownContent = new GUIContent("trigger events", "all events that are triggers at different states");
            myTarget.DropDownEnabled = EditorGUILayout.Foldout(myTarget.DropDownEnabled, dropDownContent);

            if (myTarget.DropDownEnabled)
            {
                SerializedProperty triggerEventProperty = serializedObject.FindProperty("_triggerEvent");
                SerializedProperty triggerEventStayProperty = serializedObject.FindProperty("_triggerEventStay");
                SerializedProperty triggerEventExitProperty = serializedObject.FindProperty("_triggerEventExit");

                serializedObject.Update();

                EditorGUILayout.PropertyField(triggerEventProperty);
                EditorGUILayout.PropertyField(triggerEventStayProperty);
                EditorGUILayout.PropertyField(triggerEventExitProperty);

                serializedObject.ApplyModifiedProperties(); 
            }

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("TRIGGER PARAMETERS");

                GUIContent triggerLayersContent = new GUIContent(nameof(myTarget.TriggerLayers), "layerMask of layers allowed to trigger event");
                myTarget.TriggerLayers = (LayerMask)EditorGUILayout.MaskField(triggerLayersContent, myTarget.TriggerLayers, UnityEditorInternal.InternalEditorUtility.layers);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.LabelField("COLLIDERS PARAMETERS");

                GUIContent freeCollidersPresetContent = new GUIContent(nameof(myTarget.FreeCollidersPreset), "if enabled, script will not interfer with colliders");
                myTarget.FreeCollidersPreset = EditorGUILayout.Toggle(freeCollidersPresetContent, myTarget.FreeCollidersPreset);

                GUIContent colliderTypeUsedContent = new GUIContent(nameof(myTarget.ColliderTypeUsed), "type of collider to use");
                myTarget.ColliderTypeUsed = (ColliderType)EditorGUILayout.EnumPopup(colliderTypeUsedContent, myTarget.ColliderTypeUsed);

                GUIContent colliderCountContent = new GUIContent(nameof(myTarget.ColliderCount), "number of colliders");
                myTarget.ColliderCount = EditorGUILayout.IntField(colliderCountContent, myTarget.ColliderCount);

                DrawScalePreset(myTarget);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawScalePreset(EventTrigger myTarget)
        {
            if (myTarget.ColliderTypeUsed == ColliderType.BoxCollider)
            {
                GUIContent scalePresetContent = new GUIContent(nameof(myTarget.ScalePreset), "size preset for box colliders");
                myTarget.ScalePreset = EditorGUILayout.Vector3Field(scalePresetContent, myTarget.ScalePreset);
                return;
            }
            if (myTarget.ColliderTypeUsed == ColliderType.CapsuleCollider)
            {
                GUIContent scalePresetXContent = new GUIContent(nameof(myTarget.ScalePreset) + " Radius", "radius preset for capsule colliders");
                myTarget.ScalePreset = new Vector3(EditorGUILayout.FloatField(scalePresetXContent, myTarget.ScalePreset.x), myTarget.ScalePreset.y, myTarget.ScalePreset.z);

                GUIContent scalePresetYContent = new GUIContent(nameof(myTarget.ScalePreset) + " Height", "height preset for capsule colliders");
                myTarget.ScalePreset = new Vector3(myTarget.ScalePreset.x, EditorGUILayout.FloatField(scalePresetYContent, myTarget.ScalePreset.y), myTarget.ScalePreset.z);
                return;
            }
            if (myTarget.ColliderTypeUsed == ColliderType.SphereCollider)
            {
                GUIContent scalePresetXContent = new GUIContent(nameof(myTarget.ScalePreset) + " Radius", "radius preset for sphere colliders");
                myTarget.ScalePreset = new Vector3(EditorGUILayout.FloatField(scalePresetXContent, myTarget.ScalePreset.x), myTarget.ScalePreset.y, myTarget.ScalePreset.z);
                return;
            }
            if (myTarget.ColliderTypeUsed == ColliderType.BoxCollider2D)
            {
                GUIContent scalePresetContent = new GUIContent(nameof(myTarget.ScalePreset), "size preset for box colliders 2D");
                myTarget.ScalePreset = EditorGUILayout.Vector2Field(scalePresetContent, myTarget.ScalePreset);
                return;
            }
            if (myTarget.ColliderTypeUsed == ColliderType.CapsuleCollider2D)
            {
                GUIContent scalePresetContent = new GUIContent(nameof(myTarget.ScalePreset), "size preset for capsule colliders 2D");
                myTarget.ScalePreset = EditorGUILayout.Vector2Field(scalePresetContent, myTarget.ScalePreset);
                return;
            }
            if (myTarget.ColliderTypeUsed == ColliderType.CircleCollider2D)
            {
                GUIContent scalePresetXContent = new GUIContent(nameof(myTarget.ScalePreset) + " Radius", "radius preset for circle colliders 2D");
                myTarget.ScalePreset = new Vector3(EditorGUILayout.FloatField(scalePresetXContent, myTarget.ScalePreset.x), myTarget.ScalePreset.y, myTarget.ScalePreset.z);
                return;
            }
        }
    }
}
