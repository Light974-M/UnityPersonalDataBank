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
                EditorGUI.DrawRect(visibleRect, new Color(0.15f, 0.15f, 0.15f, 1));

                //make background color of GUI's opacity to randomizer color(black text means 0 opacity, white text means 1 opacity) and text color to randomizer text color
                GUI.backgroundColor = new Color(0,0,0, 1);
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
            EditorGUI.LabelField(rect, new GUIContent(string.Empty, "target used to calculate spine for"));

            //reapply saved colors of GUI
            GUI.backgroundColor = memoFontColor;
            GUI.contentColor = memoTextColor;

            //if foldout is true and there is no randomizer, draw a warning box
            if (attributesProperty.boolValue && subProperty.objectReferenceValue == null)
                EditorGUI.HelpBox(new Rect(rect.x - rect.width, rect.y, rect.width, rect.height * 2), "please choose AudioRandomizer", MessageType.Warning);

            #endregion

            //draw main foldout
            attributesProperty.boolValue = EditorGUI.BeginFoldoutHeaderGroup(new Rect(foldoutRect.x + 4, foldoutRect.y, foldoutRect.width, foldoutRect.height), attributesProperty.boolValue, targetTransformContent, EditorStyles.foldout);

            //display main label of class and begin foldout
            if (attributesProperty.boolValue)
            {
                #region BASE REFERENCES

                //set rect of box field
                visibleRect.width -= 3;
                visibleRect.height = EditorGUIUtility.singleLineHeight * 2;
                visibleRect.xMin = position.xMin + 20;
                visibleRect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                //draw box field
                EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                //set up timeRange value
                subProperty = property.FindPropertyRelative("_targetManualFadeReference");

                //set rects for first line
                rect.width = EditorGUIUtility.labelWidth / 1.6f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x = position.xMin + 20;
                rect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                //extend rect for 4 fields
                Rect rectMax = new Rect(position.xMax - rect.width / 1.5f, rect.y, rect.width / 1.5f, rect.height);
                Rect rectCurve = new Rect(rectMax.x - rect.width - 5, rect.y, rect.width, rect.height);
                Rect rectMin = new Rect(rectCurve.x - rectMax.width - 5, rect.y, rectMax.width, rect.height);

                rect.xMax = rectMin.x - 5;

                //draw label of material object reference
                EditorGUI.LabelField(rect, new GUIContent("Target Manual Fade Reference", "manual reference to make a fade of material"), EditorStyles.boldLabel);
                subProperty.objectReferenceValue = (Material)EditorGUI.ObjectField(rect, (Material)subProperty.objectReferenceValue, typeof(Material), true);

                //reapply saved color of GUI
                GUI.backgroundColor = memoFontColor;

                #endregion

                //#region VOLUME RANGE

                ////move y axis of box rect
                //visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                ////draw box field
                //EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                ////set up timeRange value
                //subProperty = property.FindPropertyRelative("_volumeRange");

                ////set up property attribute value
                //attributesProperty = property.FindPropertyRelative("_minMaxVolumeSlider");

                ////set rects for first line
                //rect.width = EditorGUIUtility.labelWidth / 1.6f;
                //rect.height = EditorGUIUtility.singleLineHeight;
                //rect.x = position.xMin + 20;
                //rect.y = visibleRect.y;

                ////extend rect for 4 fields
                //rectMax = new Rect(position.xMax - rect.width / 1.5f, rect.y, rect.width / 1.5f, rect.height);
                //rectCurve = new Rect(rectMax.x - rect.width - 5, rect.y, rect.width, rect.height);
                //rectMin = new Rect(rectCurve.x - rectMax.width - 5, rect.y, rectMax.width, rect.height);

                //rect.xMax = rectMin.x - 5;

                ////draw label and two float fields
                //EditorGUI.LabelField(rect, new GUIContent("Volume Range", "fields to control volume of a played song, between min and max value, and with probability control(curve)"));
                //subProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectMin, subProperty.vector2Value.x), EditorGUI.FloatField(rectMax, subProperty.vector2Value.y));

                ////set rect for doubleSlider
                //rect.xMax = position.xMax - 40;
                //rect.xMin += 40;
                //rect.y += EditorGUIUtility.singleLineHeight;

                ////set slider min and max rect
                //rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                //rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                ////set opacity to 1 to avoid invisible sliders and curves
                //memoFontColor = GUI.backgroundColor;
                //GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                ////draw min and max slider
                //attributesProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectSliderMin, attributesProperty.vector2Value.x), EditorGUI.FloatField(rectSliderMax, attributesProperty.vector2Value.y));

                ////draw double slider
                //sliderX = subProperty.vector2Value.x;
                //sliderY = subProperty.vector2Value.y;
                //EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                //subProperty.vector2Value = new Vector2(sliderX, sliderY);

                ////set up curve field
                //subProperty = property.FindPropertyRelative("_volumeProbabilityCurve");

                ////draw curve
                //subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                ////reapply saved color of GUI
                //GUI.backgroundColor = memoFontColor;

                //#endregion

                //#region PITCH RANGE

                ////move y axis of box rect
                //visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                ////draw box field
                //EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                ////set up timeRange value
                //subProperty = property.FindPropertyRelative("_pitchRange");

                ////set up property attribute value
                //attributesProperty = property.FindPropertyRelative("_minMaxPitchSlider");

                ////set rects for first line
                //rect.width = EditorGUIUtility.labelWidth / 1.6f;
                //rect.height = EditorGUIUtility.singleLineHeight;
                //rect.x = position.xMin + 20;
                //rect.y = visibleRect.y;

                ////extend rect for 4 fields
                //rectMax = new Rect(position.xMax - rect.width / 1.5f, rect.y, rect.width / 1.5f, rect.height);
                //rectCurve = new Rect(rectMax.x - rect.width - 5, rect.y, rect.width, rect.height);
                //rectMin = new Rect(rectCurve.x - rectMax.width - 5, rect.y, rectMax.width, rect.height);

                //rect.xMax = rectMin.x - 5;

                ////draw label and two float fields
                //EditorGUI.LabelField(rect, new GUIContent("Pitch Range", "fields to control pitch of a played song, between min and max value, and with probability control(curve)"));
                //subProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectMin, subProperty.vector2Value.x), EditorGUI.FloatField(rectMax, subProperty.vector2Value.y));

                ////set rect for doubleSlider
                //rect.xMax = position.xMax - 40;
                //rect.xMin += 40;
                //rect.y += EditorGUIUtility.singleLineHeight;

                ////set slider min and max rect
                //rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                //rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                ////draw min and max slider
                //attributesProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectSliderMin, attributesProperty.vector2Value.x), EditorGUI.FloatField(rectSliderMax, attributesProperty.vector2Value.y));

                ////set opacity to 1 to avoid invisible sliders and curves
                //memoFontColor = GUI.backgroundColor;
                //GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                ////draw double slider
                //sliderX = subProperty.vector2Value.x;
                //sliderY = subProperty.vector2Value.y;
                //EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                //subProperty.vector2Value = new Vector2(sliderX, sliderY);

                ////set up curve field
                //subProperty = property.FindPropertyRelative("_pitchProbabilityCurve");

                ////draw curve
                //subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                ////reapply saved color of GUI
                //GUI.backgroundColor = memoFontColor;

                //#endregion
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
                size += EditorGUIUtility.singleLineHeight * 10;

            //return height
            return size;
        }
    }

}