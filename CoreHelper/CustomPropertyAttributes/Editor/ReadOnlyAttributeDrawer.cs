using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.CustomPropertyAttributes
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    } 
}
