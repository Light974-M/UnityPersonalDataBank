using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// property drawer that display AudioRandomizerConfig element, on RandomClipConfig, and on RangeMixerGroup
    /// </summary>
    [CustomPropertyDrawer(typeof(AudioRandomizerConfig)), HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    public class AudioRandomizerConfigEditor : PropertyDrawer
    {
        /// <summary>
        /// when inspector display property
        /// </summary>
        /// <param name="position"> global position of displayed property</param>
        /// <param name="property">property displayed</param>
        /// <param name="label">label of displayed property(if one)</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //if displayed property is in ambianceMixer
            if (property.serializedObject.targetObject.GetType() == typeof(AmbianceMixer))
            {
                #region AMBIANCE MIXER ELEMENT

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

                //set up group property
                SerializedProperty mixerGroupProperty = property.FindPropertyRelative("_group");
                RangeMixerGroup group = (RangeMixerGroup)mixerGroupProperty.objectReferenceValue;

                #region MAIN LABEL

                //create subProperty and give it the first field to draw
                SerializedProperty subProperty = property.FindPropertyRelative("_randomizer");

                //create GUIContent for label of the class
                GUIContent randomizerContent = new GUIContent("null", "null reference, please drag AudioRandomizer");

                //get the actual randomizer of element
                AudioRandomizer randomizer = (AudioRandomizer)subProperty.objectReferenceValue;

                //if there is no rangeMixerGroup in element
                if (group == null)
                {
                    //if there is a randomizer in element
                    if (subProperty.objectReferenceValue != null)
                    {
                        //make content of label to name and tooltip of randomizer
                        randomizerContent = new GUIContent(randomizer.name, randomizer.PropertyTooltip);

                        //draw box of class field
                        EditorGUI.DrawRect(visibleRect, randomizer.PropertyFontColor);

                        //make background color of GUI's opacity to randomizer color(black text means 0 opacity, white text means 1 opacity) and text color to randomizer text color
                        GUI.backgroundColor = new Color(1, 1, 1, (randomizer.PropertyTextColor.r + randomizer.PropertyTextColor.g + randomizer.PropertyTextColor.b) / 3);
                        GUI.contentColor = randomizer.PropertyTextColor;

                        //set up randomizer property
                        randomizerContent.text = string.Empty;

                        //get exact rect of randomizer name
                        GUIStyle style = EditorStyles.boldLabel;
                        Vector2 size = style.CalcSize(new GUIContent(randomizer.gameObject.name));

                        //draw string field of main label
                        randomizer.gameObject.name = EditorGUI.TextField(new Rect(rect.x + 4, rect.y, size.x, rect.height), randomizer.gameObject.name, EditorStyles.boldLabel);
                    }
                    else
                    {
                        //draw box of class field
                        EditorGUI.DrawRect(visibleRect, new Color(0, 0, 0, 0.3f));

                        //make color of GUI to white
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.white;

                        //get length of content to display
                        GUIStyle style = EditorStyles.boldLabel;
                        Vector2 size = style.CalcSize(randomizerContent);

                        //draw main label
                        EditorGUI.LabelField(new Rect(rect.x + 4, rect.y, size.x, rect.height), randomizerContent.text, EditorStyles.boldLabel);
                    } 
                }
                else
                {
                    //if there is a randomizer in element
                    if (subProperty.objectReferenceValue != null)
                    {
                        //make content of label to name and tooltip of group
                        randomizerContent = new GUIContent(randomizer.name, group.PropertyTooltip);

                        //draw box of class field
                        EditorGUI.DrawRect(visibleRect, group.PropertyFontColor);

                        //make background color of GUI's opacity to randomizer color(black text means 0 opacity, white text means 1 opacity) and text color to group text color
                        GUI.backgroundColor = new Color(1, 1, 1, (group.PropertyTextColor.r + group.PropertyTextColor.g + group.PropertyTextColor.b) / 3);
                        GUI.contentColor = group.PropertyTextColor;

                        //set up randomizer property
                        randomizerContent.text = string.Empty;

                        //get exact rect of randomizer name
                        GUIStyle style = EditorStyles.boldLabel;
                        Vector2 size = style.CalcSize(new GUIContent(randomizer.gameObject.name));

                        //draw string field of main label
                        randomizer.gameObject.name = EditorGUI.TextField(new Rect(rect.x + 4, rect.y, size.x, rect.height), randomizer.gameObject.name, EditorStyles.boldLabel);
                    }
                    else
                    {
                        //draw box of class field
                        EditorGUI.DrawRect(visibleRect, group.PropertyFontColor);

                        //make background color of GUI's opacity to randomizer color(black text means 0 opacity, white text means 1 opacity) and text color to group text color
                        GUI.backgroundColor = new Color(1, 1, 1, (group.PropertyTextColor.r + group.PropertyTextColor.g + group.PropertyTextColor.b) / 3);
                        GUI.contentColor = group.PropertyTextColor;

                        //get exact rect of label name
                        GUIStyle style = EditorStyles.boldLabel;
                        Vector2 size = style.CalcSize(randomizerContent);

                        //draw label field
                        EditorGUI.LabelField(new Rect(rect.x + 4, rect.y, size.x, rect.height), randomizerContent.text, EditorStyles.boldLabel);
                    }
                }

                //make foldout rect to label rect value
                Rect foldoutRect = rect;

                #endregion

                #region RANDOMIZER

                //set up randomizer property
                subProperty = property.FindPropertyRelative("_randomizer");

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
                subProperty.objectReferenceValue = (AudioRandomizer)EditorGUI.ObjectField(rect, (AudioRandomizer)subProperty.objectReferenceValue, typeof(AudioRandomizer), true);
                EditorGUI.LabelField(rect, new GUIContent(string.Empty, "AudioRandomizer used while calling randomPreset"));

                //reapply saved colors of GUI
                GUI.backgroundColor = memoFontColor;
                GUI.contentColor = memoTextColor;

                //if foldout is true and there is no randomizer, draw a warning box
                if (attributesProperty.boolValue && subProperty.objectReferenceValue == null)
                    EditorGUI.HelpBox(new Rect(rect.x - rect.width, rect.y, rect.width, rect.height * 2), "please choose AudioRandomizer", MessageType.Warning);

                #endregion

                #region RANGE MIXER GROUP

                //set rect
                Rect rangeRect = new Rect(rect.x - rect.width * 1.1f, rect.y, rect.width, rect.height);
                minWidthSwitch = 400;
                width = 100;
                rangeRect.xMin = position.width > minWidthSwitch ? rangeRect.xMax - width : rangeRect.xMax - (width * (position.width / minWidthSwitch));

                //save color of GUI
                memoFontColor = GUI.backgroundColor;
                memoTextColor = GUI.contentColor;

                //change color if foldout is false, if its true, make a blue font color
                if (!attributesProperty.boolValue)
                {
                    GUI.backgroundColor = new Color(0, 0, 1, GUI.backgroundColor.a - 0.7f);
                    GUI.contentColor = new Color(1, 1, 1, GUI.contentColor.a - 0.5f);
                }
                else
                    GUI.backgroundColor = new Color(0, 0, 1, GUI.backgroundColor.a);

                //if there is a randomizer
                if (randomizer != null)
                {
                    //draw group field and its tooltip wih label field
                    EditorGUI.LabelField(rangeRect, new GUIContent(string.Empty, "group that will override config parameters, if there is one"));
                    group = (RangeMixerGroup)EditorGUI.ObjectField(rangeRect, group, typeof(RangeMixerGroup), true);
                }

                //reapply color of GUI
                GUI.backgroundColor = memoFontColor;
                GUI.contentColor = memoTextColor;

                #endregion

                //draw main foldout
                attributesProperty.boolValue = EditorGUI.BeginFoldoutHeaderGroup(new Rect(foldoutRect.x + 4, foldoutRect.y, foldoutRect.width, foldoutRect.height), attributesProperty.boolValue, randomizerContent, EditorStyles.foldout);

                //display main label of class and begin foldout
                if (attributesProperty.boolValue)
                {
                    #region NEW RANDOMIZER

                    //set up randomizer property
                    subProperty = property.FindPropertyRelative("_randomizer");

                    if (subProperty.objectReferenceValue == null)
                    {
                        //set rect for button field
                        rect.y += EditorGUIUtility.singleLineHeight;

                        //save color of GUI
                        Color memoColor = GUI.backgroundColor;

                        //set color of GUI
                        GUI.backgroundColor = new Color(0, 1, 0, GUI.backgroundColor.a);

                        //if click on new button, create a new audioRandomizer
                        if (GUI.Button(rect, new GUIContent("NEW", "click to add a new AudioRandomizer in the scene")))
                        {
                            //create a reference of ambianceMixer property that will need to be reapplied
                            AmbianceMixer mixer = (AmbianceMixer)property.serializedObject.targetObject;

                            //name of the new created object
                            string objectName = mixer.NewClipName;

                            //add the index of new object
                            if (mixer.RandomClipConfig.Count > 0)
                                objectName = $"{objectName} {mixer.RandomClipConfig.Count}";

                            //create new GameObject, put it into ambianceMixer Object, then, add it an audioRandomizer, and finally, add it into the list
                            GameObject newGameObject = new GameObject(objectName);
                            newGameObject.transform.SetParent(mixer.transform);
                            AudioRandomizer newRandomizer = newGameObject.AddComponent<AudioRandomizer>();
                            property.FindPropertyRelative("_randomizer").objectReferenceValue = newRandomizer;
                            randomizer = newRandomizer;
                        }

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoColor;
                    }

                    #endregion

                    //is there is not group, use randomizer values, if there is, use group values
                    if (group == null)
                    {
                        #region TIME RANGE

                        #region CURVE FIX VALUES

                        //get curve and range values for property
                        AnimationCurve timeCurveProperty = property.FindPropertyRelative("_timeProbabilityCurve").animationCurveValue;
                        Vector2 timeRangeProperty = property.FindPropertyRelative("_timeRange").vector2Value;

                        //create a list that will handle keys of time curve
                        Keyframe[] timeKeys = timeCurveProperty.keys;

                        //if first and last keys are not in place one one axe, or there is not at least 2 keys
                        if (timeKeys.Length != 0 && (timeKeys[0].time != 0 || timeKeys[0].value != timeRangeProperty.x || timeKeys[timeKeys.Length - 1].time != 1 || timeKeys[timeKeys.Length - 1].value != timeRangeProperty.y))
                        {
                            //if there is less than 2 keys, make a new array with two keys
                            if (timeKeys.Length < 2)
                                timeKeys = new Keyframe[2];

                            //put first key to first range value, and last key to second range value
                            timeKeys[0] = new Keyframe(0, timeRangeProperty.x);
                            timeKeys[timeKeys.Length - 1] = new Keyframe(1, timeRangeProperty.y);

                            //apply new array to curve array
                            timeCurveProperty.keys = timeKeys;
                        }

                        //apply new properties to curve and range properties
                        property.FindPropertyRelative("_timeProbabilityCurve").animationCurveValue = timeCurveProperty;
                        property.FindPropertyRelative("_timeRange").vector2Value = timeRangeProperty;

                        #endregion

                        //set rect of box field
                        visibleRect.width -= 3;
                        visibleRect.height = EditorGUIUtility.singleLineHeight * 2;
                        visibleRect.xMin = position.xMin + 20;
                        visibleRect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                        //draw box field
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

                        //set opacity to 1 to avoid invisible sliders and curves
                        memoFontColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                        //draw double slider
                        float sliderX = subProperty.vector2Value.x;
                        float sliderY = subProperty.vector2Value.y;
                        EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                        subProperty.vector2Value = new Vector2(sliderX, sliderY);

                        //set up curve field
                        subProperty = property.FindPropertyRelative("_timeProbabilityCurve");

                        //draw curve
                        subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoFontColor;

                        #endregion

                        #region VOLUME RANGE

                        #region CURVE FIX VALUES

                        //get curve and range values for property
                        AnimationCurve volumeCurveProperty = property.FindPropertyRelative("_volumeProbabilityCurve").animationCurveValue;
                        Vector2 volumeRangeProperty = property.FindPropertyRelative("_volumeRange").vector2Value;

                        //create a list that will handle keys of time curve
                        Keyframe[] volumeKeys = volumeCurveProperty.keys;

                        //if first and last keys are not in place one one axe, or there is not at least 2 keys
                        if (volumeKeys.Length != 0 && (volumeKeys[0].time != 0 || volumeKeys[0].value != volumeRangeProperty.x || volumeKeys[volumeKeys.Length - 1].time != 1 || volumeKeys[volumeKeys.Length - 1].value != volumeRangeProperty.y))
                        {
                            //if there is less than 2 keys, make a new array with two keys
                            if (volumeKeys.Length < 2)
                                volumeKeys = new Keyframe[2];

                            //put first key to first range value, and last key to second range value
                            volumeKeys[0] = new Keyframe(0, volumeRangeProperty.x);
                            volumeKeys[volumeKeys.Length - 1] = new Keyframe(1, volumeRangeProperty.y);

                            //apply new array to curve array
                            volumeCurveProperty.keys = volumeKeys;
                        }

                        //apply new properties to curve and range properties
                        property.FindPropertyRelative("_volumeProbabilityCurve").animationCurveValue = volumeCurveProperty;
                        property.FindPropertyRelative("_volumeRange").vector2Value = volumeRangeProperty;

                        #endregion

                        //move y axis of box rect
                        visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                        //draw box field
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
                        EditorGUI.LabelField(rect, new GUIContent("Volume Range", "fields to control volume of a played song, between min and max value, and with probability control(curve)"));
                        subProperty.vector2Value = new Vector2(EditorGUI.FloatField(rectMin, subProperty.vector2Value.x), EditorGUI.FloatField(rectMax, subProperty.vector2Value.y));

                        //set rect for doubleSlider
                        rect.xMax = position.xMax - 40;
                        rect.xMin += 40;
                        rect.y += EditorGUIUtility.singleLineHeight;

                        //set slider min and max rect
                        rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                        rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                        //set opacity to 1 to avoid invisible sliders and curves
                        memoFontColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

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

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoFontColor;

                        #endregion

                        #region PITCH RANGE

                        #region CURVE FIX VALUES

                        //get curve and range values for property
                        AnimationCurve pitchCurveProperty = property.FindPropertyRelative("_pitchProbabilityCurve").animationCurveValue;
                        Vector2 pitchRangeProperty = property.FindPropertyRelative("_pitchRange").vector2Value;

                        //create a list that will handle keys of time curve
                        Keyframe[] pitchKeys = pitchCurveProperty.keys;

                        //if first and last keys are not in place one one axe, or there is not at least 2 keys
                        if (pitchKeys.Length != 0 && (pitchKeys[0].time != 0 || pitchKeys[0].value != pitchRangeProperty.x || pitchKeys[pitchKeys.Length - 1].time != 1 || pitchKeys[pitchKeys.Length - 1].value != pitchRangeProperty.y))
                        {
                            //if there is less than 2 keys, make a new array with two keys
                            if (pitchKeys.Length < 2)
                                pitchKeys = new Keyframe[2];

                            //put first key to first range value, and last key to second range value
                            pitchKeys[0] = new Keyframe(0, pitchRangeProperty.x);
                            pitchKeys[pitchKeys.Length - 1] = new Keyframe(1, pitchRangeProperty.y);

                            //apply new array to curve array
                            pitchCurveProperty.keys = pitchKeys;
                        }

                        //apply new properties to curve and range properties
                        property.FindPropertyRelative("_pitchProbabilityCurve").animationCurveValue = pitchCurveProperty;
                        property.FindPropertyRelative("_pitchRange").vector2Value = pitchRangeProperty;

                        #endregion

                        //move y axis of box rect
                        visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                        //draw box field
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
                        EditorGUI.LabelField(rect, new GUIContent("Pitch Range", "fields to control pitch of a played song, between min and max value, and with probability control(curve)"));
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

                        //set opacity to 1 to avoid invisible sliders and curves
                        memoFontColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                        //draw double slider
                        sliderX = subProperty.vector2Value.x;
                        sliderY = subProperty.vector2Value.y;
                        EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                        subProperty.vector2Value = new Vector2(sliderX, sliderY);

                        //set up curve field
                        subProperty = property.FindPropertyRelative("_pitchProbabilityCurve");

                        //draw curve
                        subProperty.animationCurveValue = EditorGUI.CurveField(rectCurve, subProperty.animationCurveValue);

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoFontColor;

                        #endregion 
                    }
                    else
                    {
                        #region TIME RANGE

                        #region CURVE FIX VALUES

                        //get curve and range values for property
                        AnimationCurve timeCurveProperty = group.Config.TimeProbabilityCurve;
                        Vector2 timeRangeProperty = group.Config.TimeRange;

                        //create a list that will handle keys of time curve
                        Keyframe[] timeKeys = timeCurveProperty.keys;

                        //if first and last keys are not in place one one axe, or there is not at least 2 keys
                        if (timeKeys.Length != 0 && (timeKeys[0].time != 0 || timeKeys[0].value != timeRangeProperty.x || timeKeys[timeKeys.Length - 1].time != 1 || timeKeys[timeKeys.Length - 1].value != timeRangeProperty.y))
                        {
                            //if there is less than 2 keys, make a new array with two keys
                            if (timeKeys.Length < 2)
                                timeKeys = new Keyframe[2];

                            //put first key to first range value, and last key to second range value
                            timeKeys[0] = new Keyframe(0, timeRangeProperty.x);
                            timeKeys[timeKeys.Length - 1] = new Keyframe(1, timeRangeProperty.y);

                            //apply new array to curve array
                            timeCurveProperty.keys = timeKeys;
                        }

                        //apply new properties to curve and range properties
                        group.Config.TimeProbabilityCurve = timeCurveProperty;
                        group.Config.TimeRange = timeRangeProperty;

                        #endregion

                        //set rect of box field
                        visibleRect.width -= 3;
                        visibleRect.height = EditorGUIUtility.singleLineHeight * 2;
                        visibleRect.xMin = position.xMin + 20;
                        visibleRect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                        //draw box field
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
                        group.Config.TimeRange = new Vector2(EditorGUI.FloatField(rectMin, group.Config.TimeRange.x), EditorGUI.FloatField(rectMax, group.Config.TimeRange.y));

                        //set rect for doubleSlider
                        rect.xMax = position.xMax - 40;
                        rect.xMin += 40;
                        rect.y += EditorGUIUtility.singleLineHeight;

                        //set slider min and max rect
                        Rect rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                        Rect rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                        //draw min and max slider
                        group.Config.MinMaxTimeSlider = new Vector2(EditorGUI.FloatField(rectSliderMin, group.Config.MinMaxTimeSlider.x), EditorGUI.FloatField(rectSliderMax, group.Config.MinMaxTimeSlider.y));
                        attributesProperty.vector2Value = group.Config.MinMaxTimeSlider;

                        //set opacity to 1 to avoid invisible sliders and curves
                        memoFontColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                        //draw double slider
                        float sliderX = group.Config.TimeRange.x;
                        float sliderY = group.Config.TimeRange.y;
                        EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                        group.Config.TimeRange = new Vector2(sliderX, sliderY);
                        subProperty.vector2Value = group.Config.TimeRange;

                        //set up curve field
                        subProperty = property.FindPropertyRelative("_timeProbabilityCurve");

                        //draw curve
                        group.Config.TimeProbabilityCurve = EditorGUI.CurveField(rectCurve, group.Config.TimeProbabilityCurve);
                        subProperty.animationCurveValue = group.Config.TimeProbabilityCurve;

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoFontColor;

                        #endregion

                        #region VOLUME RANGE

                        #region CURVE FIX VALUES

                        //get curve and range values for property
                        AnimationCurve volumeCurveProperty = group.Config.VolumeProbabilityCurve;
                        Vector2 volumeRangeProperty = group.Config.VolumeRange;

                        //create a list that will handle keys of time curve
                        Keyframe[] volumeKeys = volumeCurveProperty.keys;

                        //if first and last keys are not in place one one axe, or there is not at least 2 keys
                        if (volumeKeys.Length != 0 && (volumeKeys[0].time != 0 || volumeKeys[0].value != volumeRangeProperty.x || volumeKeys[volumeKeys.Length - 1].time != 1 || volumeKeys[volumeKeys.Length - 1].value != volumeRangeProperty.y))
                        {
                            //if there is less than 2 keys, make a new array with two keys
                            if (volumeKeys.Length < 2)
                                volumeKeys = new Keyframe[2];

                            //put first key to first range value, and last key to second range value
                            volumeKeys[0] = new Keyframe(0, volumeRangeProperty.x);
                            volumeKeys[volumeKeys.Length - 1] = new Keyframe(1, volumeRangeProperty.y);

                            //apply new array to curve array
                            volumeCurveProperty.keys = volumeKeys;
                        }

                        //apply new properties to curve and range properties
                        group.Config.VolumeProbabilityCurve = volumeCurveProperty;
                        group.Config.VolumeRange = volumeRangeProperty;

                        #endregion

                        //add y axis of box rect
                        visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                        //draw box field
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
                        EditorGUI.LabelField(rect, new GUIContent("Volume Range", "fields to control volume of a played song, between min and max value, and with probability control(curve)"));
                        group.Config.VolumeRange = new Vector2(EditorGUI.FloatField(rectMin, group.Config.VolumeRange.x), EditorGUI.FloatField(rectMax, group.Config.VolumeRange.y));

                        //set rect for doubleSlider
                        rect.xMax = position.xMax - 40;
                        rect.xMin += 40;
                        rect.y += EditorGUIUtility.singleLineHeight;

                        //set slider min and max rect
                        rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                        rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                        //draw min and max slider
                        group.Config.MinMaxVolumeSlider = new Vector2(EditorGUI.FloatField(rectSliderMin, group.Config.MinMaxVolumeSlider.x), EditorGUI.FloatField(rectSliderMax, group.Config.MinMaxVolumeSlider.y));
                        attributesProperty.vector2Value = group.Config.MinMaxVolumeSlider;

                        //set opacity to 1 to avoid invisible sliders and curves
                        memoFontColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                        //draw double slider
                        sliderX = group.Config.VolumeRange.x;
                        sliderY = group.Config.VolumeRange.y;
                        EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                        group.Config.VolumeRange = new Vector2(sliderX, sliderY);

                        //set up curve field
                        subProperty = property.FindPropertyRelative("_volumeProbabilityCurve");

                        //draw curve
                        group.Config.VolumeProbabilityCurve = EditorGUI.CurveField(rectCurve, group.Config.VolumeProbabilityCurve);

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoFontColor;

                        #endregion

                        #region PITCH RANGE

                        #region CURVE FIX VALUES

                        //get curve and range values for property
                        AnimationCurve pitchCurveProperty = group.Config.PitchProbabilityCurve;
                        Vector2 pitchRangeProperty = group.Config.PitchRange;

                        //create a list that will handle keys of time curve
                        Keyframe[] pitchKeys = pitchCurveProperty.keys;

                        //if first and last keys are not in place one one axe, or there is not at least 2 keys
                        if (pitchKeys.Length != 0 && (pitchKeys[0].time != 0 || pitchKeys[0].value != pitchRangeProperty.x || pitchKeys[pitchKeys.Length - 1].time != 1 || pitchKeys[pitchKeys.Length - 1].value != pitchRangeProperty.y))
                        {
                            //if there is less than 2 keys, make a new array with two keys
                            if (pitchKeys.Length < 2)
                                pitchKeys = new Keyframe[2];

                            //put first key to first range value, and last key to second range value
                            pitchKeys[0] = new Keyframe(0, pitchRangeProperty.x);
                            pitchKeys[pitchKeys.Length - 1] = new Keyframe(1, pitchRangeProperty.y);

                            //apply new array to curve array
                            pitchCurveProperty.keys = pitchKeys;
                        }

                        //apply new properties to curve and range properties
                        group.Config.PitchProbabilityCurve = pitchCurveProperty;
                        group.Config.PitchRange = pitchRangeProperty;

                        #endregion

                        //add y axis of box rect
                        visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                        //draw box field
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
                        EditorGUI.LabelField(rect, new GUIContent("Pitch Range", "fields to control pitch of a played song, between min and max value, and with probability control(curve)"));
                        group.Config.PitchRange = new Vector2(EditorGUI.FloatField(rectMin, group.Config.PitchRange.x), EditorGUI.FloatField(rectMax, group.Config.PitchRange.y));

                        //set rect for doubleSlider
                        rect.xMax = position.xMax - 40;
                        rect.xMin += 40;
                        rect.y += EditorGUIUtility.singleLineHeight;

                        //set slider min and max rect
                        rectSliderMin = new Rect(position.xMin + 20, rect.y, rect.x - (position.xMin + 20) - 5, rect.height);
                        rectSliderMax = new Rect(rect.xMax + 5, rect.y, (position.xMax - 5) - rect.xMax, rect.height);

                        //draw min and max slider
                        group.Config.MinMaxPitchSlider = new Vector2(EditorGUI.FloatField(rectSliderMin, group.Config.MinMaxPitchSlider.x), EditorGUI.FloatField(rectSliderMax, group.Config.MinMaxPitchSlider.y));
                        attributesProperty.vector2Value = group.Config.MinMaxPitchSlider;

                        //set opacity to 1 to avoid invisible sliders and curves
                        memoFontColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1);

                        //draw double slider
                        sliderX = group.Config.PitchRange.x;
                        sliderY = group.Config.PitchRange.y;
                        EditorGUI.MinMaxSlider(rect, ref sliderX, ref sliderY, attributesProperty.vector2Value.x, attributesProperty.vector2Value.y);
                        group.Config.PitchRange = new Vector2(sliderX, sliderY);

                        //set up curve field
                        subProperty = property.FindPropertyRelative("_pitchProbabilityCurve");

                        //draw curve
                        group.Config.PitchProbabilityCurve = EditorGUI.CurveField(rectCurve, group.Config.PitchProbabilityCurve);

                        //reapply saved color of GUI
                        GUI.backgroundColor = memoFontColor;

                        #endregion 

                    }

                    //apply new group object to group property
                    property.FindPropertyRelative("_group").objectReferenceValue = group;

                    #region DELETE

                    //init property, content, and enable state of button
                    subProperty = property.FindPropertyRelative("_randomizer");
                    GUIContent deleteContent = new GUIContent("DELETE", "click to destroy element and GameObject associated");
                    GUI.enabled = subProperty.objectReferenceValue == null ? false : true;
                    randomizer = (AudioRandomizer)subProperty.objectReferenceValue;

                    //set rect for delete button field
                    rect.xMax = position.xMax;
                    minWidthSwitch = 250;
                    width = 60;
                    rect.xMin = position.width > minWidthSwitch ? position.xMax - width : position.xMax - (width * (position.width / minWidthSwitch));
                    rect.y += EditorGUIUtility.singleLineHeight * 2 - 6;

                    //set color of GUI
                    GUI.backgroundColor = new Color(1, 0, 0, GUI.backgroundColor.a);

                    //draw button and dialog of delete field
                    if (GUI.Button(rect, deleteContent) && EditorUtility.DisplayDialog("Are you sure you want to delete ?", " randomizer will not be saved !", "delete", "Do Not delete"))
                    {
                        //destroy randomizer and its gameObject
                        GameObject.DestroyImmediate(randomizer.gameObject);
                        subProperty.objectReferenceValue = null;
                    }

                    //make color of GUI to group, randomizer, or white color depending on implementations
                    GUI.backgroundColor = group != null ? group.PropertyFontColor : subProperty.objectReferenceValue != null ? randomizer.PropertyFontColor : Color.white;
                    GUI.enabled = true;

                    #endregion

                    #region DELETE GROUP

                    //init property, content, and enable state of button
                    subProperty = property.FindPropertyRelative("_group");
                    deleteContent = new GUIContent("DELETE GROUP", "click to unassign group of element");
                    group = (RangeMixerGroup)subProperty.objectReferenceValue;
                    GUI.enabled = group == null ? false : true;

                    //set rect for delete button field
                    rect.xMax = rect.x - 5;
                    minWidthSwitch = 250;
                    width = 110;
                    rect.xMin = position.width > minWidthSwitch ? rect.xMax - width : rect.xMax - (width * (position.width / minWidthSwitch));


                    //set color of GUI
                    GUI.backgroundColor = new Color(0f, 0, 1f, GUI.backgroundColor.a);

                    //draw button of delete group
                    if (GUI.Button(rect, deleteContent))
                    {
                        group = null;
                        subProperty.objectReferenceValue = group;
                    }

                    //make color of GUI to group, randomizer, or white color depending on implementations
                    GUI.backgroundColor = group != null ? group.PropertyFontColor : subProperty.objectReferenceValue != null ? randomizer.PropertyFontColor : Color.white;
                    GUI.enabled = true;

                    #endregion
                }

                //apply new group object to group property
                property.FindPropertyRelative("_group").objectReferenceValue = group;

                //end foldout
                EditorGUI.EndFoldoutHeaderGroup();

                #endregion
            }
            //if displayed property is in rangeMixerGroup
            else if (property.serializedObject.targetObject.GetType() == typeof(RangeMixerGroup))
            {
                #region RANGE MIXER GROUP ELEMENT

                /*******************INITIALIZE*********************/

                //create SerializedProperty for propertyAttributes
                SerializedProperty attributesProperty;

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

                //find randomizer property
                SerializedProperty subProperty = property.FindPropertyRelative("_randomizer");

                #region TIME RANGE

                #region CURVE FIX VALUES

                //get curve and range values for property
                AnimationCurve timeCurveProperty = property.FindPropertyRelative("_timeProbabilityCurve").animationCurveValue;
                Vector2 timeRangeProperty = property.FindPropertyRelative("_timeRange").vector2Value;

                //create a list that will handle keys of time curve
                Keyframe[] timeKeys = timeCurveProperty.keys;

                //if first and last keys are not in place one one axe, or there is not at least 2 keys
                if (timeKeys.Length != 0 && (timeKeys[0].time != 0 || timeKeys[0].value != timeRangeProperty.x || timeKeys[timeKeys.Length - 1].time != 1 || timeKeys[timeKeys.Length - 1].value != timeRangeProperty.y))
                {
                    //if there is less than 2 keys, make a new array with two keys
                    if (timeKeys.Length < 2)
                        timeKeys = new Keyframe[2];

                    //put first key to first range value, and last key to second range value
                    timeKeys[0] = new Keyframe(0, timeRangeProperty.x);
                    timeKeys[timeKeys.Length - 1] = new Keyframe(1, timeRangeProperty.y);
                    //apply new array to curve array
                    timeCurveProperty.keys = timeKeys;
                }

                //apply new properties to curve and range properties
                property.FindPropertyRelative("_timeProbabilityCurve").animationCurveValue = timeCurveProperty;
                property.FindPropertyRelative("_timeRange").vector2Value = timeRangeProperty;

                #endregion

                //set rect of box field
                visibleRect.width -= 3;
                visibleRect.height = EditorGUIUtility.singleLineHeight * 2;
                visibleRect.xMin = position.xMin + 20;
                visibleRect.y = position.yMin + EditorGUIUtility.singleLineHeight * 2;

                //draw box field
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

                #region CURVE FIX VALUES

                //get curve and range values for property
                AnimationCurve volumeCurveProperty = property.FindPropertyRelative("_volumeProbabilityCurve").animationCurveValue;
                Vector2 volumeRangeProperty = property.FindPropertyRelative("_volumeRange").vector2Value;

                //create a list that will handle keys of time curve
                Keyframe[] volumeKeys = volumeCurveProperty.keys;

                //if first and last keys are not in place one one axe, or there is not at least 2 keys
                if (volumeKeys.Length != 0 && (volumeKeys[0].time != 0 || volumeKeys[0].value != volumeRangeProperty.x || volumeKeys[volumeKeys.Length - 1].time != 1 || volumeKeys[volumeKeys.Length - 1].value != volumeRangeProperty.y))
                {
                    //if there is less than 2 keys, make a new array with two keys
                    if (volumeKeys.Length < 2)
                        volumeKeys = new Keyframe[2];

                    //put first key to first range value, and last key to second range value
                    volumeKeys[0] = new Keyframe(0, volumeRangeProperty.x);
                    volumeKeys[volumeKeys.Length - 1] = new Keyframe(1, volumeRangeProperty.y);

                    //apply new array to curve array
                    volumeCurveProperty.keys = volumeKeys;
                }

                //apply new properties to curve and range properties
                property.FindPropertyRelative("_volumeProbabilityCurve").animationCurveValue = volumeCurveProperty;
                property.FindPropertyRelative("_volumeRange").vector2Value = volumeRangeProperty;

                #endregion

                //add y axis of box rect
                visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                //draw box field
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
                EditorGUI.LabelField(rect, new GUIContent("Volume Range", "fields to control volume of a played song, between min and max value, and with probability control(curve)"));
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

                #region CURVE FIX VALUES

                //get curve and range values for property
                AnimationCurve pitchCurveProperty = property.FindPropertyRelative("_pitchProbabilityCurve").animationCurveValue;
                Vector2 pitchRangeProperty = property.FindPropertyRelative("_pitchRange").vector2Value;

                //create a list that will handle keys of time curve
                Keyframe[] pitchKeys = pitchCurveProperty.keys;

                //if first and last keys are not in place one one axe, or there is not at least 2 keys
                if (pitchKeys.Length != 0 && (pitchKeys[0].time != 0 || pitchKeys[0].value != pitchRangeProperty.x || pitchKeys[pitchKeys.Length - 1].time != 1 || pitchKeys[pitchKeys.Length - 1].value != pitchRangeProperty.y))
                {
                    //if there is less than 2 keys, make a new array with two keys
                    if (pitchKeys.Length < 2)
                        pitchKeys = new Keyframe[2];

                    //put first key to first range value, and last key to second range value
                    pitchKeys[0] = new Keyframe(0, pitchRangeProperty.x);
                    pitchKeys[pitchKeys.Length - 1] = new Keyframe(1, pitchRangeProperty.y);

                    //apply new array to curve array
                    pitchCurveProperty.keys = pitchKeys;
                }

                //apply new properties to curve and range properties
                property.FindPropertyRelative("_pitchProbabilityCurve").animationCurveValue = pitchCurveProperty;
                property.FindPropertyRelative("_pitchRange").vector2Value = pitchRangeProperty;

                #endregion

                //add y axis of box rect
                visibleRect.y += EditorGUIUtility.singleLineHeight * 2 + 10;

                //draw box field
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
                EditorGUI.LabelField(rect, new GUIContent("Pitch Range", "fields to control pitch of a played song, between min and max value, and with probability control(curve)"));
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

                #endregion
            }

            //reset color of GUI to white
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            GUI.color = Color.white;
        }

        /// <summary>
        /// get and set height of displayed property
        /// </summary>
        /// <param name="property">displayed property</param>
        /// <param name="label">label of displayed property(if one)</param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if displayed property is in ambianceMixer, make a variable height depending on foldout
            if (property.serializedObject.targetObject.GetType() == typeof(AmbianceMixer))
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
            //if displayed property is in rangeMixerGroup, make an open foldout height
            else if (property.serializedObject.targetObject.GetType() == typeof(RangeMixerGroup))
                return EditorGUIUtility.singleLineHeight * 11;
            //else, display height like a rangeMixerGroupe
            else
                return EditorGUIUtility.singleLineHeight * 11;
        }
    }
}
