using UnityEditor;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// editor for mixer group
    /// </summary>
    [CustomEditor(typeof(RangeMixerGroup)), HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    public class RangeMixerGroupEditor : Editor
    {
        /// <summary>
        /// when inspector display a rangeMixerGroup
        /// </summary>
        public override void OnInspectorGUI()
        {
            //base of inspector, including config field
            base.OnInspectorGUI();

            //get targeted class
            RangeMixerGroup myTarget = (RangeMixerGroup)target;

            //make a small space
            GUILayout.Space(20);

            //begin a box for whole property
            GUILayout.BeginVertical("helpBox");
            {
                //draw main label
                EditorGUILayout.LabelField("CONFIG GROUP PROPERTIES", EditorStyles.boldLabel);

                //draw string of property tooltip
                GUIContent content = new GUIContent("Property Tooltip", "explain here what is this config about");
                myTarget.PropertyTooltip = EditorGUILayout.TextField(content, myTarget.PropertyTooltip);

                //draw color field of font color, and at the same line, a button to reset it
                GUILayout.BeginHorizontal("box");
                {
                    //create a content for property font color, and draw color field of it
                    content = new GUIContent("Property Font Color", "use to change font color of config field");
                    myTarget.PropertyFontColor = EditorGUILayout.ColorField(content, myTarget.PropertyFontColor);

                    //draw button that reset color to defaut value
                    if (GUILayout.Button("Reset Color"))
                        myTarget.PropertyFontColor = new Color(0, 0, 0, 0.3f);
                }
                GUILayout.EndHorizontal();

                //draw color field of text color, and at the same line, a button to reset it
                GUILayout.BeginHorizontal("box");
                {
                    //create a content for property text color, and draw color field of it
                    content = new GUIContent("Property Text Color", "use to change text color of config fields");
                    myTarget.PropertyTextColor = EditorGUILayout.ColorField(content, myTarget.PropertyTextColor);

                    //draw button that reset color to defaut value
                    if (GUILayout.Button("Reset Color"))
                        myTarget.PropertyTextColor = Color.white;
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();
        }
    } 
}
