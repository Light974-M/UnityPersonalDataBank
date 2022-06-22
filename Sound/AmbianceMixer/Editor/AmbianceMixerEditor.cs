using UnityEngine;
using UnityEditor;

namespace UPDB.Sound.AmbianceMixer
{
    [CustomEditor(typeof(AmbianceMixer))]
    public class AmbianceMixerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AmbianceMixer myTarget = (AmbianceMixer)target;

            if (!Application.isPlaying)
                myTarget.Init();

            base.OnInspectorGUI();

            myTarget.AllowCrossFade = EditorGUILayout.Toggle("Allow Crossfade", myTarget.AllowCrossFade);

            if(GUILayout.Button("STOP ALL"))
                myTarget.StopAll();
        }
    }
}
