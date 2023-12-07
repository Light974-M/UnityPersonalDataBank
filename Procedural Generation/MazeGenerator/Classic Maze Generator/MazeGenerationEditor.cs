using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.Data.MazeGenerator.ClassicMazeGenerator
{
    [CustomEditor(typeof(MazeGenerationManager))]
    public class MazeGenerationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MazeGenerationManager myTarget = (MazeGenerationManager)target;

            if (GUILayout.Button("Generate"))
                myTarget.GenerateMaze();

            myTarget.WallTilePrefab = (GameObject)EditorGUILayout.ObjectField("Wall Tile Prefab", myTarget.WallTilePrefab, typeof(GameObject), true);

            EditorGUILayout.BeginVertical("box");
            {
                myTarget.Width = EditorGUILayout.IntField(new GUIContent("Width", "define width of maze"), myTarget.Width);
                myTarget.Length = EditorGUILayout.IntField(new GUIContent("Length", "define length of maze"), myTarget.Length);
                myTarget.Height = EditorGUILayout.IntField(new GUIContent("Height", "define height of maze"), myTarget.Height);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            {
                myTarget.ProceduralEntry = EditorGUILayout.Toggle(new GUIContent("Procedural Entry", "if true, entry number and placement will be procedural"), myTarget.ProceduralEntry);

                if (!myTarget.ProceduralEntry)
                    myTarget.EntryNumber = EditorGUILayout.IntField(new GUIContent("Entry Number", "define number of entry point to the maze"), myTarget.EntryNumber);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            {
                myTarget.CutCircleShape = EditorGUILayout.Toggle(new GUIContent("Cut Circle Shape", "if true, cut maze to make a circular shaped maze"), myTarget.CutCircleShape);

                if (myTarget.CutCircleShape)
                {
                    myTarget.CircleCenterSize = EditorGUILayout.IntField(new GUIContent("Circle Center Size", "define size of inner circle inside the maze"), myTarget.CircleCenterSize);
                    myTarget.RoundCircularEdges = EditorGUILayout.Toggle(new GUIContent("Round Circular Edges", "if true, round borders of maze to look circular"), myTarget.RoundCircularEdges);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
