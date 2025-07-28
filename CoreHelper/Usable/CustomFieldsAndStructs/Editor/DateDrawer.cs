using System.Reflection;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
	[CustomPropertyDrawer(typeof(Date))]
	public class DateDrawer : PropertyDrawer
	{
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            valueProperty.intValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), valueProperty.intValue);
        }
    } 
}
