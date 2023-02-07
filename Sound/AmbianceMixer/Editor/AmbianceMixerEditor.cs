using UnityEngine;
using UnityEditor;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// editor for AmbianceMixer, to draw button, and list of audioRandomizerConfig custom property
    /// </summary>
    [CustomEditor(typeof(AmbianceMixer)), HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    public class AmbianceMixerEditor : Editor
    {
        /*****************************************UNSERIALIZED PROPERTIES**********************************************/

        /// <summary>
        /// drag audioRandomizer to automatically add it into list
        /// </summary>
        private AudioRandomizer _audioRandomizerToAddElement;

        /// <summary>
        /// drag rangeMixerGroup to automatically unassign it from all elements
        /// </summary>
        private RangeMixerGroup _groupToDelete;

        /// <summary>
        /// make a simulation that what will render ambianceMixer at play
        /// </summary>
        private bool _isSimulating = false;


        /*****************************************CLASS METHODS**********************************************/

        /// <summary>
        /// called when ambianceMixer inspector is refreshing
        /// </summary>
        public override void OnInspectorGUI()
        {
            //get the target of inspector to draw
            AmbianceMixer myTarget = (AmbianceMixer)target;

            //if is in editor, initialize
            if (!Application.isPlaying)
                myTarget.Init();

            //draw list of audioRandomizerConfig
            base.OnInspectorGUI();

            //make a space of one field height
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);

            //draw simulate, stop all, and disarm all buttons(on one height)
            GUILayout.BeginHorizontal();
            {
                //button that make a simulation preRendered of sounds(like it would sound in play mode)
                #region SIMULATE

                //make content to display button, if is already simulating or not
                GUIContent simulateContent = new GUIContent("SIMULATE", "click to launch a simulation that show what play mode will render");
                GUIContent stopSimulContent = new GUIContent("STOP SIMUL", "click to stop simulation");

                //if is already simulating
                if (_isSimulating)
                {
                    //call simulate function
                    myTarget.Simulate();

                    //change color of field to a more transparency button
                    GUI.color = new Color(1, 1, 1, 0.5f);

                    //draw button with content of stop simulation
                    if (GUILayout.Button(stopSimulContent))
                    {
                        _isSimulating = false;
                        myTarget.StopAll();
                    }
                }
                //if not, draw button with content of start simulation
                else if (GUILayout.Button(simulateContent))
                {
                    //init values to avoid problems of editor mode
                    myTarget.Init();

                    //put simulating to true;
                    _isSimulating = true;
                }

                //reset color to white
                GUI.color = Color.white;

                #endregion

                //button that call function to stop every active song playing
                if (GUILayout.Button("STOP ALL"))
                    myTarget.StopAll();

                //button that remove every randomizer, to completely avoid a song to play in ambianceMixer, keeping in save elements
                #region DISARM ALL

                //create a content for disable all button
                GUIContent buttonContent = new GUIContent("DISARM ALL", "click to disable list and every song(keep in memory songs and parameters)");

                //if button is enabled, override content and color
                if (myTarget.DisableAll)
                {
                    //make a more transparency button color
                    GUI.color = new Color(1, 1, 1, 0.5f);

                    //make content of button while disableAll is true
                    buttonContent = new GUIContent("     ENABLE     ", "click to reload every disabled songs");
                }

                //if button, inverse disableAll boolean
                if (GUILayout.Button(buttonContent))
                    myTarget.DisableAll = !myTarget.DisableAll;

                #endregion

                //reset color to white
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();

            #region AMBIANCE MIXER SETTINGS

            //draw boolean allowCrossFade
            myTarget.AllowCrossfade = EditorGUILayout.Toggle(new GUIContent("Allow Crossfade", "is not allowed, a song list will always wait to be done before to call a new song"), myTarget.AllowCrossfade);

            //draw text field for default name of new Audio Randomizer
            myTarget.NewClipName = EditorGUILayout.TextField(new GUIContent("New Clip Name", "when you click on Add a new audioRandomizer, what should be its default name ?"), myTarget.NewClipName);

            #endregion

            #region DRAG RANDOMIZER

            //create rect for GUI Fields
            GUIContent dragContent = new GUIContent("v DRAG v", "drag audioRandomizer to add into list");
            GUIStyle style = EditorStyles.miniButton;
            Vector2 size = style.CalcSize(dragContent);
            Rect rect = new Rect(Screen.width - size.x * 1.3f, EditorGUILayout.GetControlRect(myTarget.AllowCrossfade).y - (EditorGUIUtility.singleLineHeight * 5 + 8), size.x, EditorGUIUtility.singleLineHeight + 2);

            //make an invisible audioRandomizer field
            GUI.color = Color.clear;
            _audioRandomizerToAddElement = (AudioRandomizer)EditorGUI.ObjectField(rect, _audioRandomizerToAddElement, typeof(AudioRandomizer), true);
            GUI.color = Color.white;

            //set color of drag button
            GUI.backgroundColor = Color.black;

            //make a label at the same position as audioRandomizer field
            EditorGUI.LabelField(rect, dragContent, EditorStyles.miniButton);

            //if user has dragged object, add it to the list
            if (_audioRandomizerToAddElement != null)
            {
                //add object to list
                myTarget.RandomClipConfig.Add(new AudioRandomizerConfig(_audioRandomizerToAddElement));

                //make empty field of drag
                _audioRandomizerToAddElement = null;
            }

            #endregion

            #region NEW RANDOMIZER

            //set color of new button
            GUI.backgroundColor = Color.green;

            //draw button to add a new object to list and to scene
            if (GUI.Button(new Rect(rect.x - rect.width - 5, rect.y, rect.width, rect.height - 2), new GUIContent("NEW", "click to add a new AudioRandomizer in the scene")))
            {
                //name of the new created object
                string objectName = myTarget.NewClipName;

                //add the index of new object
                if (myTarget.RandomClipConfig.Count > 0)
                    objectName = $"{objectName} {myTarget.RandomClipConfig.Count}";

                //create new GameObject, put it into ambianceMixer Object, then, add it an audioRandomizer, and finally, add it into the list
                GameObject newGameObject = new GameObject(objectName);
                newGameObject.transform.SetParent(myTarget.transform);
                AudioRandomizer randomizer = newGameObject.AddComponent<AudioRandomizer>();
                myTarget.RandomClipConfig.Add(new AudioRandomizerConfig(randomizer));
            }

            #endregion

            #region DRAG GROUP

            //make rect and content for delete group
            rect = new Rect(rect.x - rect.width * 3 - 10, rect.y, rect.width * 2, rect.height - 2);
            dragContent = new GUIContent("v DELETE GROUP v", "drag group to unassign it to all elements");

            //make an invisible audioRandomizer field
            GUI.color = Color.clear;
            _groupToDelete = (RangeMixerGroup)EditorGUI.ObjectField(rect, _groupToDelete, typeof(RangeMixerGroup), true);
            GUI.color = Color.white;

            //set color of drag button
            GUI.backgroundColor = Color.blue;

            //make a label at the same position as audioRandomizer field
            EditorGUI.LabelField(rect, dragContent, EditorStyles.miniButton);

            //make color to default white
            GUI.backgroundColor = Color.white;

            //if user drag a group, delete it from every elements
            if (_groupToDelete != null)
            {
                //for each element of RandomClipConfig, if group of config is the dragged one, delete reference
                foreach (AudioRandomizerConfig config in myTarget.RandomClipConfig)
                    if (config.Group == _groupToDelete)
                        config.Group = null;

                //make field of drag empty
                _groupToDelete = null;
            }

            #endregion
        }
    }
}
