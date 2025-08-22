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
        private bool _isDragingHandle2 = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2 && property.propertyType != SerializedPropertyType.Vector3)
            {
                EditorGUI.LabelField(position, "use attribute with Vector2 or Vector3", new GUIStyle(EditorStyles.label));
                return;
            }

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                #region Draw Vector2 Field

                Vector2DirectionSelectorAttribute myAttribute = (Vector2DirectionSelectorAttribute)attribute;

                //setup rects and positions
                Rect labelRect = new Rect(position.x, position.y, (position.width / 5f) * 2, EditorGUIUtility.singleLineHeight);

                Rect fieldRect = new Rect(labelRect.xMax, labelRect.y, EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1, EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1);
                Rect insidefieldRect = new Rect(fieldRect.x + 2, fieldRect.y + 2, fieldRect.width - 4, fieldRect.height - 4);
                Rect vector2FieldRect = new Rect(fieldRect.xMax + 5, labelRect.y, 150, labelRect.height);
                Rect fieldsAreaRect = new Rect(position.x, position.y, vector2FieldRect.xMax - position.x, position.height);

                Rect rectBoolToggleRect = new Rect(fieldRect.x - EditorGUIUtility.singleLineHeight - 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                Rect boolLabelRect = new Rect(labelRect.x, labelRect.y + EditorGUIUtility.singleLineHeight, labelRect.width - EditorGUIUtility.singleLineHeight - 5, labelRect.height);
                Rect drawDebugLineLabelRect = new Rect(fieldRect.xMax + 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                Rect drawDebugLineToggleRect = new Rect(drawDebugLineLabelRect.xMax + 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

                Vector2 fieldCenter = new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, 0.5f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, 0.5f));

                Vector2 boundedPos = FromMultipliedToBaseHeight(property.vector2Value, myAttribute.SelectorBounds);
                Vector2 handlePos = new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, (boundedPos.x + 1) / 2f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, (boundedPos.y + 1) / 2f));
                Rect handleRect = new Rect(handlePos.x - 3, handlePos.y - 3, 7, 7);

                handleRect.xMin = Mathf.Clamp(handleRect.xMin, insidefieldRect.xMin, insidefieldRect.xMax);
                handleRect.xMax = Mathf.Clamp(handleRect.xMax, insidefieldRect.xMin, insidefieldRect.xMax);
                handleRect.yMin = Mathf.Clamp(handleRect.yMin, insidefieldRect.yMin, insidefieldRect.yMax);
                handleRect.yMax = Mathf.Clamp(handleRect.yMax, insidefieldRect.yMin, insidefieldRect.yMax);

                float alpha = GUI.enabled ? 1 : 0.5f;

                //draw main property rect
                EditorGUI.DrawRect(fieldsAreaRect, new Color(0.18f, 0.18f, 0.18f));

                //draw main label
                EditorGUI.LabelField(labelRect, label, new GUIStyle(EditorStyles.boldLabel));

                //Draw Selector Borders Field
                EditorGUI.DrawRect(fieldRect, new Color(0.05f, 0.05f, 0.05f, alpha));
                EditorGUI.DrawRect(insidefieldRect, new Color(0.15f, 0.15f, 0.15f, alpha));

                //draw selector background lines
                EditorGUI.DrawRect(new Rect(insidefieldRect.x + ((insidefieldRect.width - 1) / 4f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.xMax - ((insidefieldRect.width - 1) / 4f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.y + ((insidefieldRect.height - 1) / 4f) - 1, insidefieldRect.width, 1), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.yMax - ((insidefieldRect.height - 1) / 4f) - 1, insidefieldRect.width, 1), new Color(0.3f, 0.3f, 0.3f, alpha));

                EditorGUI.DrawRect(new Rect(insidefieldRect.x + (insidefieldRect.width / 2f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.5f, 0.5f, 0.5f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.y + (insidefieldRect.height / 2f) - 1, insidefieldRect.width, 1), new Color(0.5f, 0.5f, 0.5f, alpha));

                if (myAttribute.DisplayClampMagAndNormalize && myAttribute.SelectorAreaSize >= 2)
                {
                    //draw normalize
                    EditorGUI.LabelField(boolLabelRect, new GUIContent(nameof(myAttribute.Normalize), "normalize vector Value"));
                    myAttribute.Normalize = EditorGUI.Toggle(rectBoolToggleRect, GUIContent.none, myAttribute.Normalize);

                    if (myAttribute.SelectorAreaSize >= 3)
                    {
                        //draw clamp magnitude
                        EditorGUI.LabelField(new Rect(boolLabelRect.x, boolLabelRect.y + EditorGUIUtility.singleLineHeight, boolLabelRect.width, boolLabelRect.height), new GUIContent(nameof(myAttribute.ClampMagnitude), "clamp vector magnitude under 1"));
                        myAttribute.ClampMagnitude = EditorGUI.Toggle(new Rect(rectBoolToggleRect.x, rectBoolToggleRect.y + EditorGUIUtility.singleLineHeight, rectBoolToggleRect.width, rectBoolToggleRect.height), GUIContent.none, myAttribute.ClampMagnitude);
                    }
                }

                if (myAttribute.DisplayDebugLine && myAttribute.SelectorAreaSize >= 2)
                {
                    //draw toggle of draw debug line
                    myAttribute.DrawDebugLine = EditorGUI.Toggle(drawDebugLineToggleRect, myAttribute.DrawDebugLine);

                    //draw debug Line Label
                    Handles.BeginGUI();
                    Handles.color = new Color(0, 1, 0, alpha);
                    Handles.DrawLine(new Vector2(drawDebugLineLabelRect.xMin + 2, drawDebugLineLabelRect.yMax - 2), new Vector2(drawDebugLineLabelRect.xMax - 2, drawDebugLineLabelRect.yMin + 2));
                    Handles.EndGUI();
                }

                //if normalize or clamp magnitude, draw debug circle
                if (myAttribute.Normalize || myAttribute.ClampMagnitude)
                {
                    Handles.BeginGUI();
                    Handles.color = new Color(0, 1, 0, alpha);
                    Handles.DrawWireDisc(fieldCenter, Vector3.forward, insidefieldRect.width / 2f);
                    Handles.EndGUI();
                }

                //considering parameters, either normalize, clamp magnitude, or just clamp between -1 and 1 vector
                if (myAttribute.Normalize)
                    property.vector2Value = property.vector2Value.normalized * Mathf.Max(Mathf.Abs(myAttribute.SelectorBounds.x), Mathf.Abs(myAttribute.SelectorBounds.y));
                else if (myAttribute.ClampMagnitude)
                    property.vector2Value = Vector2.ClampMagnitude(property.vector2Value, Mathf.Max(Mathf.Abs(myAttribute.SelectorBounds.x), Mathf.Abs(myAttribute.SelectorBounds.y)));
                else
                    property.vector2Value = new Vector2(Mathf.Clamp(property.vector2Value.x, myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y), Mathf.Clamp(property.vector2Value.y, myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y));

                if (myAttribute.DisplayVector2Field)
                {
                    //draw vector2 field
                    property.vector2Value = EditorGUI.Vector2Field(vector2FieldRect, GUIContent.none, property.vector2Value);
                }

                //draw debug line
                if (myAttribute.DrawDebugLine)
                {
                    Handles.BeginGUI();
                    Handles.color = new Color(0, 1, 0, alpha);
                    Handles.DrawLine(new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, 0.5f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, 0.5f)), handlePos);
                    Handles.EndGUI();
                }

                //Draw Handle
                EditorGUI.DrawRect(handleRect, new Color(_isDragingHandle ? 0.8f : 1, 0, 0, alpha));

                //if mouse is inside selector and click, start to drag
                if (Event.current.mousePosition.x >= insidefieldRect.xMin && Event.current.mousePosition.y >= insidefieldRect.yMin && Event.current.mousePosition.x <= insidefieldRect.xMax && Event.current.mousePosition.y <= insidefieldRect.yMax)
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        _isDragingHandle = true;
                        EditorWindow.focusedWindow?.Repaint();
                        property.vector2Value = new Vector2(Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.x, insidefieldRect.xMax, Event.current.mousePosition.x)), Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.yMax, insidefieldRect.y, Event.current.mousePosition.y)));
                    }
                }

                //if mouse release, stop drag
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _isDragingHandle = false;
                    EditorWindow.focusedWindow?.Repaint();
                }

                //while mouse drag, put selector to mouse pos value
                if (_isDragingHandle && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    property.vector2Value = new Vector2(Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.x, insidefieldRect.xMax, Event.current.mousePosition.x)), Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.yMax, insidefieldRect.y, Event.current.mousePosition.y)));

                    EditorWindow.focusedWindow?.Repaint();
                }

                #endregion 
            }
            if (property.propertyType == SerializedPropertyType.Vector3)
            {
                #region Draw Vector3 Field

                Vector2DirectionSelectorAttribute myAttribute = (Vector2DirectionSelectorAttribute)attribute;

                //setup rects and positions
                Rect labelRect = new Rect(position.x, position.y, (position.width / 5f) * 2, EditorGUIUtility.singleLineHeight);

                Rect fieldRect = new Rect(labelRect.xMax, labelRect.y, EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1, EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1);
                Rect insidefieldRect = new Rect(fieldRect.x + 2, fieldRect.y + 2, fieldRect.width - 4, fieldRect.height - 4);
                Rect fieldRect2 = new Rect(fieldRect.xMax + 5, labelRect.y, EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1, EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1);
                Rect insidefieldRect2 = new Rect(fieldRect2.x + 2, fieldRect2.y + 2, fieldRect2.width - 4, fieldRect2.height - 4);
                Rect vector3FieldRect = new Rect(fieldRect2.xMax + 5, labelRect.y, 150, labelRect.height);
                Rect fieldsAreaRect = new Rect(position.x, position.y, vector3FieldRect.xMax - position.x, position.height);

                Rect rectBoolToggleRect = new Rect(fieldRect.x - EditorGUIUtility.singleLineHeight - 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                Rect boolLabelRect = new Rect(labelRect.x, labelRect.y + EditorGUIUtility.singleLineHeight, labelRect.width - EditorGUIUtility.singleLineHeight - 5, labelRect.height);
                Rect drawDebugLineLabelRect = new Rect(fieldRect2.xMax + 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                Rect drawDebugLineToggleRect = new Rect(drawDebugLineLabelRect.xMax + 5, labelRect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

                Vector2 fieldCenter = new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, 0.5f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, 0.5f));
                Vector2 field2Center = new Vector2(Mathf.Lerp(insidefieldRect2.x, insidefieldRect2.xMax, 0.5f), Mathf.Lerp(insidefieldRect2.yMax, insidefieldRect2.y, 0.5f));

                Vector3 boundedPos = FromMultipliedToBaseHeight(property.vector3Value, myAttribute.SelectorBounds);

                Vector2 handlePos = new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, (boundedPos.x + 1) / 2f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, (boundedPos.y + 1) / 2f));
                Rect handleRect = new Rect(handlePos.x - 3, handlePos.y - 3, 7, 7);

                Vector2 handle2Pos = new Vector2(Mathf.Lerp(insidefieldRect2.x, insidefieldRect2.xMax, (boundedPos.z + 1) / 2f), Mathf.Lerp(insidefieldRect2.yMax, insidefieldRect2.y, (boundedPos.y + 1) / 2f));
                Rect handle2Rect = new Rect(handle2Pos.x - 3, handle2Pos.y - 3, 7, 7);

                handleRect.xMin = Mathf.Clamp(handleRect.xMin, insidefieldRect.xMin, insidefieldRect.xMax);
                handleRect.xMax = Mathf.Clamp(handleRect.xMax, insidefieldRect.xMin, insidefieldRect.xMax);
                handleRect.yMin = Mathf.Clamp(handleRect.yMin, insidefieldRect.yMin, insidefieldRect.yMax);
                handleRect.yMax = Mathf.Clamp(handleRect.yMax, insidefieldRect.yMin, insidefieldRect.yMax);

                handle2Rect.xMin = Mathf.Clamp(handle2Rect.xMin, insidefieldRect2.xMin, insidefieldRect2.xMax);
                handle2Rect.xMax = Mathf.Clamp(handle2Rect.xMax, insidefieldRect2.xMin, insidefieldRect2.xMax);
                handle2Rect.yMin = Mathf.Clamp(handle2Rect.yMin, insidefieldRect2.yMin, insidefieldRect2.yMax);
                handle2Rect.yMax = Mathf.Clamp(handle2Rect.yMax, insidefieldRect2.yMin, insidefieldRect2.yMax);

                float alpha = GUI.enabled ? 1 : 0.5f;

                //draw main property rect
                EditorGUI.DrawRect(fieldsAreaRect, new Color(0.18f, 0.18f, 0.18f));

                //draw main label
                EditorGUI.LabelField(labelRect, label, new GUIStyle(EditorStyles.boldLabel));

                //Draw Selector Borders Field
                EditorGUI.DrawRect(fieldRect, new Color(0.05f, 0.05f, 0.05f, alpha));
                EditorGUI.DrawRect(insidefieldRect, new Color(0.15f, 0.15f, 0.15f, alpha));
                EditorGUI.DrawRect(fieldRect2, new Color(0.05f, 0.05f, 0.05f, alpha));
                EditorGUI.DrawRect(insidefieldRect2, new Color(0.15f, 0.15f, 0.15f, alpha));

                //draw selector background lines
                EditorGUI.DrawRect(new Rect(insidefieldRect.x + ((insidefieldRect.width - 1) / 4f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.xMax - ((insidefieldRect.width - 1) / 4f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.y + ((insidefieldRect.height - 1) / 4f) - 1, insidefieldRect.width, 1), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.yMax - ((insidefieldRect.height - 1) / 4f) - 1, insidefieldRect.width, 1), new Color(0.3f, 0.3f, 0.3f, alpha));

                EditorGUI.DrawRect(new Rect(insidefieldRect.x + (insidefieldRect.width / 2f), insidefieldRect.y, 1, insidefieldRect.height), new Color(0.5f, 0.5f, 0.5f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect.x, insidefieldRect.y + (insidefieldRect.height / 2f) - 1, insidefieldRect.width, 1), new Color(0.5f, 0.5f, 0.5f, alpha));

                EditorGUI.DrawRect(new Rect(insidefieldRect2.x + ((insidefieldRect2.width - 1) / 4f), insidefieldRect2.y, 1, insidefieldRect2.height), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect2.xMax - ((insidefieldRect2.width - 1) / 4f), insidefieldRect2.y, 1, insidefieldRect2.height), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect2.x, insidefieldRect2.y + ((insidefieldRect2.height - 1) / 4f) - 1, insidefieldRect2.width, 1), new Color(0.3f, 0.3f, 0.3f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect2.x, insidefieldRect2.yMax - ((insidefieldRect2.height - 1) / 4f) - 1, insidefieldRect2.width, 1), new Color(0.3f, 0.3f, 0.3f, alpha));

                EditorGUI.DrawRect(new Rect(insidefieldRect2.x + (insidefieldRect2.width / 2f), insidefieldRect2.y, 1, insidefieldRect2.height), new Color(0.5f, 0.5f, 0.5f, alpha));
                EditorGUI.DrawRect(new Rect(insidefieldRect2.x, insidefieldRect2.y + (insidefieldRect2.height / 2f) - 1, insidefieldRect2.width, 1), new Color(0.5f, 0.5f, 0.5f, alpha));

                if (myAttribute.DisplayClampMagAndNormalize && myAttribute.SelectorAreaSize >= 2)
                {
                    //draw normalize
                    EditorGUI.LabelField(boolLabelRect, new GUIContent(nameof(myAttribute.Normalize), "normalize vector Value"));
                    myAttribute.Normalize = EditorGUI.Toggle(rectBoolToggleRect, GUIContent.none, myAttribute.Normalize);

                    if (myAttribute.SelectorAreaSize >= 3)
                    {
                        //draw clamp magnitude
                        EditorGUI.LabelField(new Rect(boolLabelRect.x, boolLabelRect.y + EditorGUIUtility.singleLineHeight, boolLabelRect.width, boolLabelRect.height), new GUIContent(nameof(myAttribute.ClampMagnitude), "clamp vector magnitude under 1"));
                        myAttribute.ClampMagnitude = EditorGUI.Toggle(new Rect(rectBoolToggleRect.x, rectBoolToggleRect.y + EditorGUIUtility.singleLineHeight, rectBoolToggleRect.width, rectBoolToggleRect.height), GUIContent.none, myAttribute.ClampMagnitude);
                    }
                }

                if (myAttribute.DisplayDebugLine && myAttribute.SelectorAreaSize >= 2)
                {
                    //draw toggle of draw debug line
                    myAttribute.DrawDebugLine = EditorGUI.Toggle(drawDebugLineToggleRect, myAttribute.DrawDebugLine);

                    //draw debug Line Label
                    Handles.BeginGUI();
                    Handles.color = new Color(0, 1, 0, alpha);
                    Handles.DrawLine(new Vector2(drawDebugLineLabelRect.xMin + 2, drawDebugLineLabelRect.yMax - 2), new Vector2(drawDebugLineLabelRect.xMax - 2, drawDebugLineLabelRect.yMin + 2));
                    Handles.EndGUI();
                }

                //if normalize or clamp magnitude, draw debug circle
                if (myAttribute.Normalize || myAttribute.ClampMagnitude)
                {
                    Handles.BeginGUI();
                    Handles.color = new Color(0, 1, 0, alpha);
                    Handles.DrawWireDisc(fieldCenter, Vector3.forward, insidefieldRect.width / 2f);
                    Handles.DrawWireDisc(field2Center, Vector3.forward, insidefieldRect2.width / 2f);
                    Handles.EndGUI();
                }

                //considering parameters, either normalize, clamp magnitude, or just clamp between -1 and 1 vector
                if (myAttribute.Normalize)
                    property.vector3Value = property.vector3Value.normalized * Mathf.Max(Mathf.Abs(myAttribute.SelectorBounds.x), Mathf.Abs(myAttribute.SelectorBounds.y));
                else if (myAttribute.ClampMagnitude)
                    property.vector3Value = Vector3.ClampMagnitude(property.vector3Value, Mathf.Max(Mathf.Abs(myAttribute.SelectorBounds.x), Mathf.Abs(myAttribute.SelectorBounds.y)));
                else
                    property.vector3Value = new Vector3(Mathf.Clamp(property.vector3Value.x, myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y), Mathf.Clamp(property.vector3Value.y, myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y), Mathf.Clamp(property.vector3Value.z, myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y));

                if (myAttribute.DisplayVector2Field)
                {
                    //draw vector2 field
                    property.vector3Value = EditorGUI.Vector3Field(vector3FieldRect, GUIContent.none, property.vector3Value);
                }

                //draw debug line
                if (myAttribute.DrawDebugLine)
                {
                    Handles.BeginGUI();
                    Handles.color = new Color(0, 1, 0, alpha);
                    Handles.DrawLine(new Vector2(Mathf.Lerp(insidefieldRect.x, insidefieldRect.xMax, 0.5f), Mathf.Lerp(insidefieldRect.yMax, insidefieldRect.y, 0.5f)), handlePos);
                    Handles.DrawLine(new Vector2(Mathf.Lerp(insidefieldRect2.x, insidefieldRect2.xMax, 0.5f), Mathf.Lerp(insidefieldRect2.yMax, insidefieldRect2.y, 0.5f)), handle2Pos);
                    Handles.EndGUI();
                }

                //Draw Handle
                EditorGUI.DrawRect(handleRect, new Color(_isDragingHandle ? 0.8f : 1, 0, 0, alpha));
                EditorGUI.DrawRect(handle2Rect, new Color(_isDragingHandle2 ? 0.8f : 1, 0, 0, alpha));

                //if mouse is inside selector and click, start to drag
                if (Event.current.mousePosition.x >= insidefieldRect.xMin && Event.current.mousePosition.y >= insidefieldRect.yMin && Event.current.mousePosition.x <= insidefieldRect.xMax && Event.current.mousePosition.y <= insidefieldRect.yMax && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isDragingHandle = true;
                    EditorWindow.focusedWindow?.Repaint();
                    property.vector3Value = new Vector3(Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.x, insidefieldRect.xMax, Event.current.mousePosition.x)), Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.yMax, insidefieldRect.y, Event.current.mousePosition.y)), property.vector3Value.z);
                }
                if (Event.current.mousePosition.x >= insidefieldRect2.xMin && Event.current.mousePosition.y >= insidefieldRect2.yMin && Event.current.mousePosition.x <= insidefieldRect2.xMax && Event.current.mousePosition.y <= insidefieldRect2.yMax && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isDragingHandle2 = true;
                    EditorWindow.focusedWindow?.Repaint();
                    property.vector3Value = new Vector3(property.vector3Value.x, Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect2.yMax, insidefieldRect2.y, Event.current.mousePosition.y)), Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect2.x, insidefieldRect2.xMax, Event.current.mousePosition.x)));
                }

                //if mouse release, stop drag
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _isDragingHandle = false;
                    _isDragingHandle2 = false;
                    EditorWindow.focusedWindow?.Repaint();
                }

                //while mouse drag, put selector to mouse pos value
                if (_isDragingHandle && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    property.vector3Value = new Vector3(Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.x, insidefieldRect.xMax, Event.current.mousePosition.x)), Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect.yMax, insidefieldRect.y, Event.current.mousePosition.y)), property.vector3Value.z);

                    EditorWindow.focusedWindow?.Repaint();
                }
                if (_isDragingHandle2 && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    property.vector3Value = new Vector3(property.vector3Value.x, Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect2.yMax, insidefieldRect2.y, Event.current.mousePosition.y)), Mathf.Lerp(myAttribute.SelectorBounds.x, myAttribute.SelectorBounds.y, Mathf.InverseLerp(insidefieldRect2.x, insidefieldRect2.xMax, Event.current.mousePosition.x)));

                    EditorWindow.focusedWindow?.Repaint();
                }

                #endregion 
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2 && property.propertyType != SerializedPropertyType.Vector3)
                return EditorGUIUtility.singleLineHeight;

            Vector2DirectionSelectorAttribute myAttribute = (Vector2DirectionSelectorAttribute)attribute;

            return EditorGUIUtility.singleLineHeight * (float)myAttribute.SelectorAreaSize + 1;
        }

        private Vector2 FromMultipliedToBaseHeight(Vector2 value, Vector2 factor)
        {
            float Remap(float v, float min, float max)
            {
                float t = Mathf.InverseLerp(min, max, v);
                return t * 2f - 1f;
            }

            return new Vector2(
                Remap(value.x, factor.x, factor.y),
                Remap(value.y, factor.x, factor.y)
            );
        }

        private Vector3 FromMultipliedToBaseHeight(Vector3 value, Vector2 factor)
        {
            float Remap(float v, float min, float max)
            {
                float t = Mathf.InverseLerp(min, max, v);
                return t * 2f - 1f;
            }

            return new Vector3(
                Remap(value.x, factor.x, factor.y),
                Remap(value.y, factor.x, factor.y),
                Remap(value.z, factor.x, factor.y)
            );
        }
    }
}