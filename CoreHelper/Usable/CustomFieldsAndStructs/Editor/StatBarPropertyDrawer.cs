using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
    [CustomPropertyDrawer(typeof(StatBar))]
    public class StatBarPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            SerializedProperty minProperty = property.FindPropertyRelative("_min");
            SerializedProperty maxProperty = property.FindPropertyRelative("_max");

            #region Get All Rects

            float labelWidth = 0.3f;
            float valueSplit1Width = 0.3f;
            float valueSplit2Width = 0.4f;
            float valueLabel1Width = 0.4f;
            float valueLabel2Width = 0.4f;
            float valueLabel3Width = 0.4f;
            float valuesSpacing = 4f;

            Rect labelRect = new Rect(position.x, position.y, position.width * labelWidth, position.height);
            Rect valueRect = new Rect(labelRect.xMax, position.y, position.width * (1 - labelWidth), position.height);

            Rect valueSplit1Rect = new Rect(valueRect.x, position.y, (valueRect.width - (valuesSpacing * 2)) * valueSplit1Width, position.height);
            Rect valueSplit2Rect = new Rect(valueSplit1Rect.xMax + valuesSpacing, position.y, (valueRect.width - (valuesSpacing * 2)) * valueSplit2Width, position.height);
            Rect valueSplit3Rect = new Rect(valueSplit2Rect.xMax + valuesSpacing, position.y, (valueRect.width - (valuesSpacing * 2)) * (1 - (valueSplit1Width + valueSplit2Width)), position.height);

            Rect valueSplitLabel1Rect = new Rect(valueSplit1Rect.x, position.y, valueSplit1Rect.width * valueLabel1Width, position.height);
            Rect valueSplitValue1Rect = new Rect(valueSplitLabel1Rect.xMax, position.y, valueSplit1Rect.width * (1 - valueLabel1Width), position.height);

            Rect valueSplitLabel2Rect = new Rect(valueSplit2Rect.x, position.y, valueSplit2Rect.width * valueLabel2Width, position.height);
            Rect valueSplitValue2Rect = new Rect(valueSplitLabel2Rect.xMax, position.y, valueSplit2Rect.width * (1 - valueLabel2Width), position.height);

            Rect valueSplitLabel3Rect = new Rect(valueSplit3Rect.x, position.y, valueSplit3Rect.width * valueLabel3Width, position.height);
            Rect valueSplitValue3Rect = new Rect(valueSplitLabel3Rect.xMax, position.y, valueSplit3Rect.width * (1 - valueLabel3Width), position.height);

            #endregion

            EditorGUI.LabelField(labelRect, property.displayName);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.LabelField(valueSplitLabel1Rect, minProperty.displayName);
                minProperty.floatValue = EditorGUI.FloatField(valueSplitValue1Rect, minProperty.floatValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                minProperty.floatValue = Mathf.Clamp(minProperty.floatValue, -Mathf.Infinity, maxProperty.floatValue);
                valueProperty.floatValue = Mathf.Clamp(valueProperty.floatValue, minProperty.floatValue, maxProperty.floatValue);
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.LabelField(valueSplitLabel2Rect, valueProperty.displayName);
                valueProperty.floatValue = EditorGUI.FloatField(valueSplitValue2Rect, valueProperty.floatValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                valueProperty.floatValue = Mathf.Clamp(valueProperty.floatValue, minProperty.floatValue, maxProperty.floatValue);
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.LabelField(valueSplitLabel3Rect, maxProperty.displayName);
                maxProperty.floatValue = EditorGUI.FloatField(valueSplitValue3Rect, maxProperty.floatValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                maxProperty.floatValue = Mathf.Clamp(maxProperty.floatValue, minProperty.floatValue, Mathf.Infinity);
                valueProperty.floatValue = Mathf.Clamp(valueProperty.floatValue, minProperty.floatValue, maxProperty.floatValue);
            }
        }
    }
}
