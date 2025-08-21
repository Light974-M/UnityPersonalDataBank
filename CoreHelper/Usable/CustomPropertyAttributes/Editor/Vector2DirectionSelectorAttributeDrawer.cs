using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace UPDB.CoreHelper.CustomPropertyAttributes
{
    [CustomPropertyDrawer(typeof(Vector2DirectionSelectorAttribute))]
    public class Vector2DirectionSelectorAttributeDrawer : PropertyDrawer
    {
        private bool _isDragingHandle = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2DirectionSelectorAttribute myAttribute = (Vector2DirectionSelectorAttribute)attribute;

            Rect labelRect = new Rect(position.x, position.y, (position.width / 5f) * 2, EditorGUIUtility.singleLineHeight);
            Rect fieldRect = new Rect(labelRect.xMax, labelRect.y, EditorGUIUtility.singleLineHeight * 3 + 1, EditorGUIUtility.singleLineHeight * 3 + 1);
            Rect insidefieldRect = new Rect(fieldRect.x + 2, fieldRect.y + 2, fieldRect.width - 4, fieldRect.height - 4);
            Rect vector2FieldRect = new Rect(fieldRect.xMax + 5, labelRect.y + (EditorGUIUtility.singleLineHeight) * 2, 150, labelRect.height);
            Rect fieldsAreaRect = new Rect(position.x, position.y, vector2FieldRect.xMax - position.x, position.height);
            Vector2 fieldCenter = new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, 0.5f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, 0.5f));

            Rect rectBoolToggleRect = new Rect(fieldRect.x - EditorGUIUtility.singleLineHeight - 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            Rect boolLabelRect = new Rect(labelRect.x, labelRect.y + EditorGUIUtility.singleLineHeight, labelRect.width - EditorGUIUtility.singleLineHeight - 5, labelRect.height);
            Rect drawDebugLineLabelRect = new Rect(fieldRect.xMax + 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            Rect drawDebugLineToggleRect = new Rect(drawDebugLineLabelRect.xMax + 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

            EditorGUI.DrawRect(fieldsAreaRect, new Color(0.18f, 0.18f, 0.18f));

            EditorGUI.LabelField(labelRect, label, new GUIStyle(EditorStyles.boldLabel));

            EditorGUI.DrawRect(fieldRect, new Color(0.05f, 0.05f, 0.05f));
            EditorGUI.DrawRect(insidefieldRect, new Color(0.15f, 0.15f, 0.15f));

            EditorGUI.DrawRect(new Rect(insidefieldRect.x + ((insidefieldRect.width - 1) / 4f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.3f, 0.3f, 0.3f));
            EditorGUI.DrawRect(new Rect(insidefieldRect.xMax - ((insidefieldRect.width - 1) / 4f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.3f, 0.3f, 0.3f));

            EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.y + ((insidefieldRect.height - 1) / 4f) - 1, insidefieldRect.width, 1), new Color(0.3f, 0.3f, 0.3f));
            EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.yMax - ((insidefieldRect.height - 1) / 4f) - 1, insidefieldRect.width, 1), new Color(0.3f, 0.3f, 0.3f));

            EditorGUI.DrawRect(new Rect(insidefieldRect.x + (insidefieldRect.width / 2f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.5f, 0.5f, 0.5f));
            EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.y + (insidefieldRect.height / 2f) - 1, insidefieldRect.width, 1), new Color(0.5f, 0.5f, 0.5f));

            EditorGUI.LabelField(boolLabelRect, new GUIContent(nameof(myAttribute.Normalize), "normalize vector Value"));
            EditorGUI.LabelField(new Rect(boolLabelRect.x, boolLabelRect.y + EditorGUIUtility.singleLineHeight, boolLabelRect.width, boolLabelRect.height), new GUIContent(nameof(myAttribute.ClampMagnitude), "clamp vector magnitude under 1"));

            myAttribute.Normalize = EditorGUI.Toggle(rectBoolToggleRect, GUIContent.none, myAttribute.Normalize);
            myAttribute.ClampMagnitude = EditorGUI.Toggle(new Rect(rectBoolToggleRect.x, rectBoolToggleRect.y + EditorGUIUtility.singleLineHeight, rectBoolToggleRect.width, rectBoolToggleRect.height), GUIContent.none, myAttribute.ClampMagnitude);
            myAttribute.DrawDebugLine = EditorGUI.Toggle(drawDebugLineToggleRect, myAttribute.DrawDebugLine);

            Handles.BeginGUI();
            Handles.color = Color.green;
            Handles.DrawLine(new Vector2(drawDebugLineLabelRect.xMin + 2, drawDebugLineLabelRect.yMax - 2), new Vector2(drawDebugLineLabelRect.xMax - 2, drawDebugLineLabelRect.yMin + 2));
            Handles.EndGUI();

            if(myAttribute.Normalize || myAttribute.ClampMagnitude)
            {
                
                Handles.BeginGUI();
                Handles.color = Color.green;
                Handles.DrawWireDisc(fieldCenter, Vector3.forward, insidefieldRect.width / 2f);
                Handles.EndGUI();
            }

            if (myAttribute.Normalize)
                property.vector2Value = property.vector2Value.normalized;
            else if (myAttribute.ClampMagnitude)
                property.vector2Value = Vector2.ClampMagnitude(property.vector2Value, 1);
            else
                property.vector2Value = new Vector2(Mathf.Clamp(property.vector2Value.x, -1f, 1f), Mathf.Clamp(property.vector2Value.y, -1f, 1f));

            Vector2 handlePos = new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, (property.vector2Value.x + 1) / 2f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, (property.vector2Value.y + 1) / 2f));
            Rect handleRect = new Rect(handlePos.x - 3, handlePos.y - 3, 7, 7);

            handleRect.xMin = Mathf.Clamp(handleRect.xMin, insidefieldRect.xMin, insidefieldRect.xMax);
            handleRect.xMax = Mathf.Clamp(handleRect.xMax, insidefieldRect.xMin, insidefieldRect.xMax);
            handleRect.yMin = Mathf.Clamp(handleRect.yMin, insidefieldRect.yMin, insidefieldRect.yMax);
            handleRect.yMax = Mathf.Clamp(handleRect.yMax, insidefieldRect.yMin, insidefieldRect.yMax);

            property.vector2Value = EditorGUI.Vector2Field(vector2FieldRect, GUIContent.none, property.vector2Value);

            if (myAttribute.DrawDebugLine)
            {
                Handles.BeginGUI();
                Handles.color = Color.green;
                Handles.DrawLine(new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, 0.5f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, 0.5f)), handlePos);
                Handles.EndGUI(); 
            }

            EditorGUI.DrawRect(handleRect, Color.red);

            if(Event.current.mousePosition.x >= insidefieldRect.xMin && Event.current.mousePosition.y >= insidefieldRect.yMin && Event.current.mousePosition.x <= insidefieldRect.xMax && Event.current.mousePosition.y <= insidefieldRect.yMax)
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    _isDragingHandle = true;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                _isDragingHandle = false;

            if (_isDragingHandle && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                property.vector2Value = new Vector2(Mathf.Lerp(-1, 1, Mathf.InverseLerp(insidefieldRect.x, insidefieldRect.xMax, Event.current.mousePosition.x)), Mathf.Lerp(-1, 1, Mathf.InverseLerp(insidefieldRect.yMax, insidefieldRect.y, Event.current.mousePosition.y)));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 1;
        }
    }
}