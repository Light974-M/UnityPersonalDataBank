using UnityEngine;
using UnityEditor;

namespace UPDB.Data.SaveStateManager
{
    ///<summary>
    /// 
    ///</summary>
    [CustomEditor(typeof(PlayerData))]
    public class PlayerDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PlayerData playerData = target as PlayerData;
            EditorGUILayout.Space();

            if(GUILayout.Button("Save"))
                playerData.Save();

            if (GUILayout.Button("Load"))
                playerData.Load();
        }
    }
}

