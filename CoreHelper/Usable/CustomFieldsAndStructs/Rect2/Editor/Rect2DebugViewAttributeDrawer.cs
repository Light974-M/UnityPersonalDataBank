using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    [CustomPropertyDrawer(typeof(Rect2DebugViewAttribute))]
    public class Rect2DebugViewAttributeDrawer : PropertyDrawer
    {
        private float _lineDistance = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);

            SerializedProperty xMinAttribute = property.FindPropertyRelative("_xMin");
            SerializedProperty yMinAttribute = property.FindPropertyRelative("_yMin");
            SerializedProperty widthAttribute = property.FindPropertyRelative("_width");
            SerializedProperty heightAttribute = property.FindPropertyRelative("_height");

            Vector2 min = new Vector2(xMinAttribute.floatValue, yMinAttribute.floatValue);
            Vector2 max = new Vector2(xMinAttribute.floatValue + widthAttribute.floatValue, yMinAttribute.floatValue + heightAttribute.floatValue);

            UPDBBehaviour.DebugDrawMinMaxCube(min, max, Vector3.right, Vector3.up, Vector3.forward, Color.yellow);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ((EditorGUIUtility.singleLineHeight + _lineDistance) * 1) + EditorGUIUtility.singleLineHeight;
        }
    }
}

