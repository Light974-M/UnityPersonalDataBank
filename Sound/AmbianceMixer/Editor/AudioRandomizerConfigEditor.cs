using UnityEditor;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    [CustomPropertyDrawer(typeof(AudioRandomizerConfig))]
    public class AudioRandomizerConfigEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /*******************INITIALIZE*********************/

            //create SerializedProperty for propertyAttributes
            SerializedProperty attributesProperty = property.FindPropertyRelative("_mainFoldoutDisplay");

            //draw box of class field
            Rect visibleRect = new Rect(position);
            visibleRect.xMin -= 25;
            visibleRect.xMax += 3;
            visibleRect.yMax -= 3;

            EditorGUI.DrawRect(visibleRect, new Color(0, 0, 0, 0.3f));

            //init rect for label
            Rect rect = new Rect(position);
            rect.width = 150;
            rect.height = EditorGUIUtility.singleLineHeight;

            #region MAIN LABEL

            //create subProperty and give it the first field to draw
            SerializedProperty subProperty = property.FindPropertyRelative("_randomizer");

            //create GUIContent for label of the class
            GUIContent randomizerContent = new GUIContent("null", "null reference, please drag AudioRandomizer");

            if (subProperty.objectReferenceValue != null)
            {
                AudioRandomizer randomizer = (AudioRandomizer)subProperty.objectReferenceValue;
                randomizerContent = new GUIContent(randomizer.name, randomizer.PropertyTooltip);
            }

            //set up randomizer property
            //subProperty = property.FindPropertyRelative("_randomizer");
            attributesProperty.boolValue = EditorGUI.BeginFoldoutHeaderGroup(new Rect(rect.x + 4, rect.y, rect.width, rect.height), attributesProperty.boolValue, randomizerContent, EditorStyles.foldout);

            #endregion

            #region RANDOMIZER

            //set up randomizer property
            subProperty = property.FindPropertyRelative("_randomizer");

            //set rect for randomizer field
            rect.xMax = position.xMax;

            if (position.width > 250)
                rect.xMin = position.xMax - 150;
            else
                rect.xMin = position.xMin * 3.15f;

            rect.height -= 1;

            //change color if foldout is false
            if (!attributesProperty.boolValue)
            {
                GUI.backgroundColor = new Color(1, 1, 1, 0.3f);
                GUI.contentColor = new Color(1, 1, 1, 0.5f);
            }

            //draw randomizer
            subProperty.objectReferenceValue = (AudioRandomizer)EditorGUI.ObjectField(rect, (AudioRandomizer)subProperty.objectReferenceValue, typeof(AudioRandomizer), true);
            EditorGUI.LabelField(rect, new GUIContent(string.Empty, "AudioRandomizer used while calling randomPreset"));

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;

            if (subProperty.objectReferenceValue == null)
                EditorGUI.HelpBox(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, rect.height * 2), "please choose AudioRandomizer", MessageType.Warning);

            #endregion

            //display main label of class and begin foldout
            if (attributesProperty.boolValue)
            {
                #region TIME RANGE

                visibleRect.width -= 3;
                visibleRect.height = EditorGUIUtility.singleLineHeight * 2;
                visibleRect.xMin = position.xMin + 20;
                visibleRect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                //set up timeRange value
                subProperty = property.FindPropertyRelative("_timeRange");

                //set up property attribute value
                attributesProperty = property.FindPropertyRelative("_minMaxTimeSlider");

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

                //draw label and two float fields
                EditorGUI.LabelField(rect, new GUIContent("Time Range", "fields to control frequency of audioRandomizer calls, between min and max value, and with probability control(curve)"), EditorStyles.boldLabel);
                subProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectMin, subProperty.vector2Value.x), EditorGUI.FloatField(rectMax, subProperty.vector2Value.y));

                //set rect for doubleSlider
                rect.xMax = position.xMax - 40;
                rect.xMin += 40;
                rect.y += EditorGUIUtility.singleLineHeight;

                //set slider min and max rect
                Rect rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                Rect rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                //draw min and max slider
                attributesProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectSliderMin, attributesProperty.vector2Value.x), EditorGUI.FloatField(rectSliderMax, attributesProperty.vector2Value.y));

                //draw double slider
                float sliderX = subProperty.vector2Value.x;
                float sliderY = subProperty.vector2Value.y;
                EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                subProperty.vector2Value = new Vector2(sliderX, sliderY);

                //set up curve field
                subProperty = property.FindPropertyRelative("_timeProbabilityCurve");

                //draw curve
                subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                #endregion

                #region VOLUME RANGE

                visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                //set up timeRange value
                subProperty = property.FindPropertyRelative("_volumeRange");

                //set up property attribute value
                attributesProperty = property.FindPropertyRelative("_minMaxVolumeSlider");

                //set rects for first line
                rect.width = EditorGUIUtility.labelWidth / 1.6f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x = position.xMin + 20;
                rect.y = visibleRect.y;

                //extend rect for 4 fields
                rectMax = new Rect(position.xMax - rect.width / 1.5f, rect.y, rect.width / 1.5f, rect.height);
                rectCurve = new Rect(rectMax.x - rect.width - 5, rect.y, rect.width, rect.height);
                rectMin = new Rect(rectCurve.x - rectMax.width - 5, rect.y, rectMax.width, rect.height);

                rect.xMax = rectMin.x - 5;

                //draw label and two float fields
                EditorGUI.LabelField(rect, new GUIContent("Volume Range", "fields to control frequency of audioRandomizer calls, between min and max value, and with probability control(curve)"));
                subProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectMin, subProperty.vector2Value.x), EditorGUI.FloatField(rectMax, subProperty.vector2Value.y));

                //set rect for doubleSlider
                rect.xMax = position.xMax - 40;
                rect.xMin += 40;
                rect.y += EditorGUIUtility.singleLineHeight;

                //set slider min and max rect
                rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                //draw min and max slider
                attributesProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectSliderMin, attributesProperty.vector2Value.x), EditorGUI.FloatField(rectSliderMax, attributesProperty.vector2Value.y));

                //draw double slider
                sliderX = subProperty.vector2Value.x;
                sliderY = subProperty.vector2Value.y;
                EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                subProperty.vector2Value = new Vector2(sliderX, sliderY);

                //set up curve field
                subProperty = property.FindPropertyRelative("_volumeProbabilityCurve");

                //draw curve
                subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                #endregion

                #region PITCH RANGE

                visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                EditorGUI.DrawRect(visibleRect, new Color(1, 1, 1, 0.03f));

                //set up timeRange value
                subProperty = property.FindPropertyRelative("_pitchRange");

                //set up property attribute value
                attributesProperty = property.FindPropertyRelative("_minMaxPitchSlider");

                //set rects for first line
                rect.width = EditorGUIUtility.labelWidth / 1.6f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x = position.xMin + 20;
                rect.y = visibleRect.y;

                //extend rect for 4 fields
                rectMax = new Rect(position.xMax - rect.width / 1.5f, rect.y, rect.width / 1.5f, rect.height);
                rectCurve = new Rect(rectMax.x - rect.width - 5, rect.y, rect.width, rect.height);
                rectMin = new Rect(rectCurve.x - rectMax.width - 5, rect.y, rectMax.width, rect.height);

                rect.xMax = rectMin.x - 5;

                //draw label and two float fields
                EditorGUI.LabelField(rect, new GUIContent("Pitch Range", "fields to control frequency of audioRandomizer calls, between min and max value, and with probability control(curve)"));
                subProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectMin, subProperty.vector2Value.x), EditorGUI.FloatField(rectMax, subProperty.vector2Value.y));

                //set rect for doubleSlider
                rect.xMax = position.xMax - 40;
                rect.xMin += 40;
                rect.y += EditorGUIUtility.singleLineHeight;

                //set slider min and max rect
                rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                //draw min and max slider
                attributesProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectSliderMin, attributesProperty.vector2Value.x), EditorGUI.FloatField(rectSliderMax, attributesProperty.vector2Value.y));

                //draw double slider
                sliderX = subProperty.vector2Value.x;
                sliderY = subProperty.vector2Value.y;
                EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                subProperty.vector2Value = new Vector2(sliderX, sliderY);

                //set up curve field
                subProperty = property.FindPropertyRelative("_pitchProbabilityCurve");

                //draw curve
                subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                #endregion 
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty subProperty = property.FindPropertyRelative("_mainFoldoutDisplay");
            float size = EditorGUIUtility.singleLineHeight;

            if (subProperty.boolValue)
                size += EditorGUIUtility.singleLineHeight * 9;

            return size;
        }
    }
}
