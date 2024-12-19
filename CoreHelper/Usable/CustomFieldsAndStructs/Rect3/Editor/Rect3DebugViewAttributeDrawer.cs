using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    [CustomPropertyDrawer(typeof(Rect3DebugViewAttribute))]
    public class Rect3DebugViewAttributeDrawer : PropertyDrawer
    {
        private float _lineDistance = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);

            SerializedProperty xMinAttribute = property.FindPropertyRelative("_xMin");
            SerializedProperty yMinAttribute = property.FindPropertyRelative("_yMin");
            SerializedProperty zMinAttribute = property.FindPropertyRelative("_zMin");
            SerializedProperty widthAttribute = property.FindPropertyRelative("_width");
            SerializedProperty heightAttribute = property.FindPropertyRelative("_height");
            SerializedProperty lengthAttribute = property.FindPropertyRelative("_length");

            Vector3 min = new Vector3(xMinAttribute.floatValue, yMinAttribute.floatValue, zMinAttribute.floatValue);
            Vector3 max = new Vector3(xMinAttribute.floatValue + widthAttribute.floatValue, yMinAttribute.floatValue + heightAttribute.floatValue, zMinAttribute.floatValue + lengthAttribute.floatValue);

            UPDBBehaviour.DebugDrawMinMaxCube(min, max, Vector3.right, Vector3.up, Vector3.forward, Color.yellow);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ((EditorGUIUtility.singleLineHeight + _lineDistance) * 1) + EditorGUIUtility.singleLineHeight;
        }
    }
}
