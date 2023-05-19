using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UPDB.Sound.AmbianceMixer;

namespace UPDB.Data.SplineTool
{
    [CustomPropertyDrawer(typeof(SplineTarget))]
    public class SplineTargetEditor : PropertyDrawer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /*******************INITIALIZE*********************/

            //create SerializedProperty for propertyAttributes
            SerializedProperty attributesProperty = property.FindPropertyRelative("_mainFoldoutDisplay");

            //set rect of main box field
            Rect visibleRect = new Rect(position);
            visibleRect.xMin -= 25;
            visibleRect.xMax += 3;
            visibleRect.yMax -= 3;


            //init rect for label
            Rect rect = new Rect(position);
            rect.width = 150;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.xMax = position.xMax - 158;

            #region MAIN LABEL

            //create subProperty and give it the first field to draw
            SerializedProperty subProperty = property.FindPropertyRelative("_target");

            //create GUIContent for label of the class
            GUIContent targetTransformContent = new GUIContent("null", "null reference, please drag Target transform");

            //get the actual randomizer of element
            Transform targetTransform = (Transform)subProperty.objectReferenceValue;

            //if there is a randomizer in element
            if (subProperty.objectReferenceValue != null)
            {
                //make content of label to name and tooltip of randomizer
                targetTransformContent = new GUIContent(targetTransform.name, "linked target");

                //draw box of class field
                EditorGUI.DrawRect(visibleRect, new Color(0, 0, 0, 0.3f));

                //make background color of GUI's opacity to randomizer color(black text means 0 opacity, white text means 1 opacity) and text color to randomizer text color
                GUI.backgroundColor = new Color(0, 0, 0, 1);
                GUI.contentColor = Color.white;

                //set up randomizer property
                targetTransformContent.text = string.Empty;

                //get exact rect of randomizer name
                GUIStyle style = EditorStyles.boldLabel;
                Vector2 size = style.CalcSize(new GUIContent(targetTransform.gameObject.name));

                //draw string field of main label
                targetTransform.gameObject.name = EditorGUI.TextField(new Rect(rect.x + 4, rect.y, size.x, rect.height), targetTransform.gameObject.name, EditorStyles.boldLabel);
            }
            else
            {
                //draw box of class field
                EditorGUI.DrawRect(visibleRect, new Color(0, 0, 0, 0.3f));

                //put color of GUI
                GUI.backgroundColor = new Color(0, 0, 0, 1);
                GUI.contentColor = Color.white;

                //get length of content to display
                GUIStyle style = EditorStyles.boldLabel;
                Vector2 size = style.CalcSize(targetTransformContent);

                //draw main label
                EditorGUI.LabelField(new Rect(rect.x + 4, rect.y, size.x, rect.height), targetTransformContent.text, EditorStyles.boldLabel);
            }

            //make foldout rect to label rect value
            Rect foldoutRect = rect;

            #endregion

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;

            #region TARGETTRANSFORM

            //set up randomizer property
            subProperty = property.FindPropertyRelative("_target");

            //set rect for randomizer field
            rect.xMax = position.xMax;
            float minWidthSwitch = 350;
            float width = 150;
            rect.xMin = position.width > minWidthSwitch ? position.xMax - width : position.xMax - (width * (position.width / minWidthSwitch));
            rect.height -= 1;

            //get color of GUI and keep it
            Color memoFontColor = GUI.backgroundColor;
            Color memoTextColor = GUI.contentColor;

            //change color if foldout is false
            if (!attributesProperty.boolValue)
            {
                GUI.backgroundColor = new Color(1, 1, 1, GUI.backgroundColor.a - 0.7f);
                GUI.contentColor = new Color(1, 1, 1, GUI.contentColor.a - 0.5f);
            }

            //draw randomizer and label for tooltip
            subProperty.objectReferenceValue = (Transform)EditorGUI.ObjectField(rect, (Transform)subProperty.objectReferenceValue, typeof(Transform), true);
            EditorGUI.LabelField(rect, new GUIContent(string.Empty, "target used to calculate spline for"));

            //reapply saved colors of GUI
            GUI.backgroundColor = memoFontColor;
            GUI.contentColor = memoTextColor;

            //if foldout is true and there is no randomizer, draw a warning box
            if (attributesProperty.boolValue && subProperty.objectReferenceValue == null)
                EditorGUI.HelpBox(new Rect(rect.x - rect.width, rect.y, rect.width, rect.height * 2), "please choose Target", MessageType.Warning);

            #endregion

            //draw main foldout
            attributesProperty.boolValue = EditorGUI.BeginFoldoutHeaderGroup(new Rect(foldoutRect.x + 4, foldoutRect.y, foldoutRect.width, foldoutRect.height), attributesProperty.boolValue, targetTransformContent, EditorStyles.foldout);

            //display main label of class and begin foldout
            if (attributesProperty.boolValue)
            {
                #region BASE REFERENCES

                //set rect of box field
                visibleRect.width -= 3;
                visibleRect.height = EditorGUIUtility.singleLineHeight * 4;
                visibleRect.xMin = position.xMin - 10;
                visibleRect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                //draw box field
                EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                //set rects for first line
                rect.width = Mathf.Clamp(EditorGUIUtility.labelWidth * 1, 190, Mathf.Infinity);
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x = position.xMin;
                rect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                //draw label of Reference Base
                EditorGUI.LabelField(rect, new GUIContent("BASE REFERENCES", "all references needed for core tool or other"), EditorStyles.boldLabel);

                rect.y += EditorGUIUtility.singleLineHeight * 2;

                //set up fade material property
                subProperty = property.FindPropertyRelative("_targetManualFadeReference");

                //draw label of material object reference
                EditorGUI.LabelField(rect, new GUIContent("Target Manual Fade Reference", "manual reference to make a fade of material"));

                rect.xMin = rect.xMax;
                rect.xMax = position.xMax - 5;

                subProperty.objectReferenceValue = (Material)EditorGUI.ObjectField(rect, (Material)subProperty.objectReferenceValue, typeof(Material), true);

                rect.y += EditorGUIUtility.singleLineHeight;

                //set up fade material property
                subProperty = property.FindPropertyRelative("_targetLinkedAnimation");

                //set rects for first line
                rect.width = Mathf.Clamp(EditorGUIUtility.labelWidth * 1, 190, Mathf.Infinity);
                rect.x = position.xMin;

                //draw label of material object reference
                EditorGUI.LabelField(rect, new GUIContent("Target Linked Animation", "animation reference to play while moving on spline"));

                rect.xMin = rect.xMax;
                rect.xMax = position.xMax - 5;

                subProperty.objectReferenceValue = (Animator)EditorGUI.ObjectField(rect, (Animator)subProperty.objectReferenceValue, typeof(Animator), true);

                #endregion

                #region POSITION PAREMETERS

                //set rect of box field
                visibleRect.width -= 3;
                visibleRect.height = EditorGUIUtility.singleLineHeight * 7;
                visibleRect.xMin = position.xMin - 10;
                visibleRect.y = rect.y + EditorGUIUtility.singleLineHeight * 2;

                //draw box field
                EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                //set rects for first line
                rect.width = EditorGUIUtility.labelWidth * 3;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x = position.xMin;
                rect.y = visibleRect.y;

                //draw label of Reference Base
                EditorGUI.LabelField(rect, new GUIContent("POSITION PARAMETERS", "parameters of position calculations"), EditorStyles.boldLabel);

                rect.y += EditorGUIUtility.singleLineHeight * 2;
                rect.width = Mathf.Clamp(EditorGUIUtility.labelWidth * 0.8f, 0, 95);
                rect.x = EditorGUIUtility.labelWidth * 0.8f;

                GUI.enabled = false;

                subProperty = property.FindPropertyRelative("_targetKeyPos");

                EditorGUI.LabelField(rect, new GUIContent("Target Key Pos", "represent position of target on active key"));

                rect.xMin = rect.xMax;
                rect.xMax = rect.xMin + 50;

                subProperty.floatValue = EditorGUI.FloatField(rect, subProperty.floatValue);

                rect.x = rect.xMax + 30;
                rect.width = Mathf.Clamp(EditorGUIUtility.labelWidth * 0.5f, 0, 68);

                subProperty = property.FindPropertyRelative("_activeKey");

                EditorGUI.LabelField(rect, new GUIContent("Active Key", "key representing point A of lerp"));

                rect.xMin = rect.xMax;
                rect.xMax = rect.xMin + 30;

                subProperty.intValue = EditorGUI.IntField(rect, subProperty.intValue);

                GUI.enabled = true;

                rect.width = 110;
                rect.x = position.xMin;
                rect.y += EditorGUIUtility.singleLineHeight;

                subProperty = property.FindPropertyRelative("_targetSplinePos");

                EditorGUI.LabelField(rect, new GUIContent("Target Spline Pos", "represent overall position of target on spline"));

                rect.xMin = rect.xMax;
                rect.xMax = position.xMax - 5;

                subProperty.floatValue = EditorGUI.Slider(rect, subProperty.floatValue, 0, 1);

                rect.width = 110;
                rect.x = position.xMin;
                rect.y += EditorGUIUtility.singleLineHeight * 2;

                EditorGUI.LabelField(rect, new GUIContent("Start Fade Time", "choose fade of target at beginning of spline"));

                rect.y += EditorGUIUtility.singleLineHeight;

                EditorGUI.LabelField(rect, new GUIContent("End Fade Time", "choose fade of target at end of spline"));

                rect.y -= EditorGUIUtility.singleLineHeight;

                subProperty = property.FindPropertyRelative("_startFadeTime");

                rect.xMin = rect.xMax;
                rect.xMax = position.xMax - 5;

                subProperty.floatValue = EditorGUI.Slider(rect, subProperty.floatValue, 0, 1);

                rect.y += EditorGUIUtility.singleLineHeight;

                subProperty = property.FindPropertyRelative("_endFadeTime");
                subProperty.floatValue = EditorGUI.Slider(rect, subProperty.floatValue, 0, 1);

                //reapply saved color of GUI
                GUI.backgroundColor = memoFontColor;

                #endregion
            }

            //end foldout
            EditorGUI.EndFoldoutHeaderGroup();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //get foldout boolean propety
            SerializedProperty subProperty = property.FindPropertyRelative("_mainFoldoutDisplay");

            //one height default height(if foldout is false)
            float size = EditorGUIUtility.singleLineHeight;

            //if foldout is true, add 10 height to default height
            if (subProperty.boolValue)
                size += EditorGUIUtility.singleLineHeight * 18;

            //return height
            return size;
        }
    }

}