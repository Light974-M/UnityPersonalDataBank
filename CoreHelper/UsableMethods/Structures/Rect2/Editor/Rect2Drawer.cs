using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    [CustomPropertyDrawer(typeof(Rect2))]
    public class Rect2Drawer : PropertyDrawer
    {
        private float _lineDistance = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            SerializedProperty xMinAttribute = property.FindPropertyRelative("_xMin");
            SerializedProperty yMinAttribute = property.FindPropertyRelative("_yMin");
            SerializedProperty widthAttribute = property.FindPropertyRelative("_width");
            SerializedProperty heightAttribute = property.FindPropertyRelative("_height");

            float minBoundLerp = Rect.NormalizedToPoint(position, new Vector2(0.45f, 0)).x - 30;
            float maxBoundLerp = Rect.NormalizedToPoint(position, new Vector2(0.815f, 0)).x - 10f;
            Rect fieldsRect = Rect.MinMaxRect(minBoundLerp, 0, maxBoundLerp, EditorGUIUtility.singleLineHeight * 2);

            float xMinOffset = 3;
            Rect labelrect = Rect.MinMaxRect(position.x, position.y, fieldsRect.x, EditorGUIUtility.singleLineHeight);
            Rect xMinRect = Rect.MinMaxRect(fieldsRect.x + xMinOffset, fieldsRect.y, fieldsRect.center.x, EditorGUIUtility.singleLineHeight);
            Rect yMinRect = Rect.MinMaxRect(fieldsRect.center.x + xMinOffset, fieldsRect.y, fieldsRect.xMax, EditorGUIUtility.singleLineHeight);
            Rect widthRect = Rect.MinMaxRect(fieldsRect.x + xMinOffset, EditorGUIUtility.singleLineHeight, fieldsRect.center.x, fieldsRect.yMax);
            Rect heightRect = Rect.MinMaxRect(fieldsRect.center.x + xMinOffset, EditorGUIUtility.singleLineHeight, fieldsRect.xMax, fieldsRect.yMax);

            widthRect.y += _lineDistance;
            heightRect.y += _lineDistance;


            EditorGUI.LabelField(labelrect, ObjectNames.NicifyVariableName(property.name));

            if (position.width > 335)
            {
                //EditorGUI.DrawRect(fieldsRect, new Color(1, 0, 0, 0.5f));
                //EditorGUI.DrawRect(xMinRect, new Color(0, 0, 1, 0.5f));
                //EditorGUI.DrawRect(yMinRect, new Color(0, 1, 1, 0.5f));
                //EditorGUI.DrawRect(widthRect, new Color(1, 0, 1, 0.5f));
                //EditorGUI.DrawRect(heightRect, new Color(1, 1, 1, 0.5f));
            }

            float labelSize = 15;

            GUIContent xMinContent = new GUIContent("X", "x pos");
            EditorGUI.LabelField(Rect.MinMaxRect(xMinRect.x, xMinRect.y, xMinRect.x + labelSize, xMinRect.yMax), xMinContent);
            xMinAttribute.floatValue = EditorGUI.FloatField(Rect.MinMaxRect(xMinRect.x + labelSize, xMinRect.y, xMinRect.xMax, xMinRect.yMax), xMinAttribute.floatValue);

            GUIContent yMinContent = new GUIContent("Y", "y pos");
            EditorGUI.LabelField(Rect.MinMaxRect(yMinRect.x, yMinRect.y, yMinRect.x + labelSize, yMinRect.yMax), yMinContent);
            yMinAttribute.floatValue = EditorGUI.FloatField(Rect.MinMaxRect(yMinRect.x + labelSize, yMinRect.y, yMinRect.xMax, yMinRect.yMax), yMinAttribute.floatValue);

            GUIContent widthContent = new GUIContent("W", "width");
            EditorGUI.LabelField(Rect.MinMaxRect(widthRect.x, widthRect.y, widthRect.x + labelSize, widthRect.yMax), widthContent);
            widthAttribute.floatValue = EditorGUI.FloatField(Rect.MinMaxRect(widthRect.x + labelSize, widthRect.y, widthRect.xMax, widthRect.yMax), widthAttribute.floatValue < 0 ? 0 : widthAttribute.floatValue);

            GUIContent heightContent = new GUIContent("H", "height");
            EditorGUI.LabelField(Rect.MinMaxRect(heightRect.x, heightRect.y, heightRect.x + labelSize, heightRect.yMax), heightContent);
            heightAttribute.floatValue = EditorGUI.FloatField(Rect.MinMaxRect(heightRect.x + labelSize, heightRect.y, heightRect.xMax, heightRect.yMax), heightAttribute.floatValue < 0 ? 0 : heightAttribute.floatValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ((EditorGUIUtility.singleLineHeight + _lineDistance) * 1) + EditorGUIUtility.singleLineHeight;
        }
    }
}
