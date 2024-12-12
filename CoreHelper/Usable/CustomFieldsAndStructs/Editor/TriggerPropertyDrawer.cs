using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    [CustomPropertyDrawer(typeof(Trigger))]
    public class TriggerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");

            // Dessinez le label de la propriété
            Rect labelRect = new Rect(position.x, position.y, position.width * 0.7f, position.height);
            EditorGUI.LabelField(labelRect, label);

            // Zone pour le bouton rond
            Rect buttonRect = new Rect(position.x + position.width * 0.355f, position.y + (position.height - 16) / 2, 16, 16);

            // Couleur de base pour le bouton
            bool isMouseOver = buttonRect.Contains(Event.current.mousePosition);
            Color buttonColor = valueProperty.boolValue ? isMouseOver ? Color.green * 0.75f : Color.green : isMouseOver ? new Color(0.14f, 0.14f, 0.14f) : new Color(0.17f, 0.17f, 0.17f);

            HandleUtility.Repaint();

            //Dessinez un cercle avec Handles
            Handles.BeginGUI();
            Color originalColor = GUI.color;
            GUI.color = buttonColor;
            Handles.color = buttonColor;
            Handles.DrawSolidDisc(buttonRect.center, Vector3.forward, buttonRect.width / 2);
            Handles.color = Color.black;
            Handles.DrawWireDisc(buttonRect.center, Vector3.forward, buttonRect.width / 2);
            Handles.EndGUI();
            GUI.color = Color.clear;
            Rect toggleRect = new Rect(buttonRect.x - 15, buttonRect.y, 32, 16);
            valueProperty.boolValue = EditorGUI.Toggle(toggleRect, valueProperty.boolValue);
            GUI.color = originalColor;
        }
    }
}
