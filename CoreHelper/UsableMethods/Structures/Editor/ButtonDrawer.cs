using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods.Structures
{
    [CustomPropertyDrawer(typeof(Button))]
    public class ButtonDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (GUI.Button(position, property.name))
                property.FindPropertyRelative("_value").boolValue = true;
        }
    }
}
