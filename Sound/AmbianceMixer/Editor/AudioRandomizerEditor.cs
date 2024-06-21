using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// editor for audioRandomizer, to draw button, and attributes for AudioRandomizerConfig custom property
    /// </summary>
    [CustomEditor(typeof(AudioRandomizer)), CanEditMultipleObjects, HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    public class AudioRandomizerEditor : Editor
    {
        /*****************************************CLASS METHODS**********************************************/

        /// <summary>
        /// called when inspector of target class is refreshing
        /// </summary>
        public override void OnInspectorGUI()
        {
            //draw inspector by default
            base.OnInspectorGUI();

            //set up target
            AudioRandomizer myTarget = (AudioRandomizer)target;

            //if in editor, call init function of target
            if (!Application.isPlaying)
                myTarget.Init();

            //button to pick a clip and call function
            if (GUILayout.Button(new GUIContent("PICK RANDOM CLIP", "play a song from the randomizer(act like a button)")))
                myTarget.OnRandomize();

            //try to get an ambianceMixer in the scene, if one, search in every element of RandomClipConfig, if target is founded, draw attributes proper to ambianceMixer
            if (UPDBBehaviour.TryFindObjectOfType(out AmbianceMixer ambianceMixer))
                for (int i = 0; i < ambianceMixer.RandomClipConfig.Count; i++)
                    if (ambianceMixer.RandomClipConfig[i].Randomizer == myTarget)
                        DrawConfigAttributes(myTarget);
        }


        /// <summary>
        /// draw variables and attributes that will be used by AudioRandomizerConfig custom property
        /// </summary>
        /// <param name="myTarget">target to draw variables</param>
        private void DrawConfigAttributes(AudioRandomizer myTarget)
        {
            //make a small space
            GUILayout.Space(20);

            //begin a box for whole property
            GUILayout.BeginVertical("helpBox");
            {
                //draw main label
                EditorGUILayout.LabelField("CONFIG PROPERTIES", EditorStyles.boldLabel);

                //draw string of property tooltip
                GUIContent content = new GUIContent("Property Tooltip", "explain here what is this config about");
                myTarget.PropertyTooltip = EditorGUILayout.TextField(content, myTarget.PropertyTooltip);

                //draw color field of font color, and at the same line, a button to reset it
                GUILayout.BeginHorizontal("box");
                {
                    //create content for colorField, and colorField itself, that will set color of font property in RandomClipConfig
                    content = new GUIContent("Property Font Color", "use to change font color of config field");
                    myTarget.PropertyFontColor = EditorGUILayout.ColorField(content, myTarget.PropertyFontColor);

                    //draw button that, if clicked, will reset colorField to it's default value
                    if (GUILayout.Button("Reset Color"))
                        myTarget.PropertyFontColor = new Color(0, 0, 0, 0.3f);
                }
                GUILayout.EndHorizontal();

                //draw color field of text color, and at the same line, a button to reset it
                GUILayout.BeginHorizontal("box");
                {
                    //create content for colorField, and colorField itself, that will set color of text property in RandomClipConfig
                    content = new GUIContent("Property Text Color", "use to change text color of config fields");
                    myTarget.PropertyTextColor = EditorGUILayout.ColorField(content, myTarget.PropertyTextColor);

                    //draw button that, if clicked, will reset colorField to it's default value
                    if (GUILayout.Button("Reset Color"))
                        myTarget.PropertyTextColor = Color.white;
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();
        }
    }
}
