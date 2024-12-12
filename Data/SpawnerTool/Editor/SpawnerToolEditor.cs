using UnityEditor;
using UnityEngine;
using UPDB.Data.CustomTransform;

namespace UPDB.Data.UPDBSpawner
{
    [CustomEditor(typeof(SpawnerTool))]
    public class SpawnerToolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SpawnerTool myTarget = (SpawnerTool)target;

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("BASE SPAWN PARAMETERS");

                SerializedProperty objectToSpawnArrayProperty = serializedObject.FindProperty("_objectsToSpawnArray");
                serializedObject.Update();
                EditorGUILayout.PropertyField(objectToSpawnArrayProperty);
                serializedObject.ApplyModifiedProperties();

                GUIContent spawnOnAwakeContent = new GUIContent(nameof(myTarget.SpawnOnAwake), "does spawner spawn a first object on awake ?");
                myTarget.SpawnOnAwake = EditorGUILayout.Toggle(spawnOnAwakeContent, myTarget.SpawnOnAwake);

                GUIContent spawnNumberRangeContent = new GUIContent(nameof(myTarget.SpawnNumberRange), "one generation include how many objects ? random Range");
                myTarget.SpawnNumberRange = EditorGUILayout.Vector2IntField(spawnNumberRangeContent, myTarget.SpawnNumberRange);

                GUIContent spawnRateContent = new GUIContent(nameof(myTarget.SpawnRate), "range of time where to instantiate it");
                myTarget.SpawnRate = EditorGUILayout.Vector2Field(spawnRateContent, myTarget.SpawnRate);

                EditorGUILayout.BeginHorizontal();
                {
                    GUIContent rotationDirectionContent = new GUIContent("Object And Spawner Direction", "wich transform direction of objects is aligned with wich transform direction of spawner ? (with applied transform offset of spawner)");
                    EditorGUILayout.LabelField(rotationDirectionContent);
                    myTarget.ObjectsRotationDirection = (RotationDirection)EditorGUILayout.EnumPopup(myTarget.ObjectsRotationDirection);
                    myTarget.SpawnerRotationDirection = (RotationDirection)EditorGUILayout.EnumPopup(myTarget.SpawnerRotationDirection);
                }
                EditorGUILayout.EndHorizontal();

                GUIContent minRotationOffsetContent = new GUIContent(nameof(myTarget.MinRotationOffset), "min rotation range for offset rotation randomization");
                myTarget.MinRotationOffset = EditorGUILayout.Vector3Field(minRotationOffsetContent, myTarget.MinRotationOffset);

                GUIContent maxRotationOffsetContent = new GUIContent(nameof(myTarget.MaxRotationOffset), "max rotation range for offset rotation randomization");
                myTarget.MaxRotationOffset = EditorGUILayout.Vector3Field(maxRotationOffsetContent, myTarget.MaxRotationOffset);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("SPAWNER SHAPE PARAMETERS");

                GUIContent spawnerShapeContent = new GUIContent(nameof(myTarget.SpawnerShape), "shape of spawner");
                myTarget.SpawnerShape = (Shape)EditorGUILayout.EnumPopup(spawnerShapeContent, myTarget.SpawnerShape);

                GUIContent spawnerOffsetTransformContent = new GUIContent(nameof(myTarget.SpawnerOffsetTransform), "parent of simulated transform");
                myTarget.SpawnerOffsetTransform = (CustomTransformManager)EditorGUILayout.ObjectField(spawnerOffsetTransformContent, myTarget.SpawnerOffsetTransform, typeof(CustomTransformManager), true);
            }
            EditorGUILayout.EndVertical();

            serializedObject.Update();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
}
