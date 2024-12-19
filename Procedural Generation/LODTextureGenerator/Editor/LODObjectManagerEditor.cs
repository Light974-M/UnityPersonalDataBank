using System.IO;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.LODTextureGenerator
{
    [CustomEditor(typeof(LODObjectManager))]
    public class LODObjectManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LODObjectManager myTarget = (LODObjectManager)target;

            SerializedProperty lODListProperty = serializedObject.FindProperty("_lODList");

            EditorGUILayout.PropertyField(lODListProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
