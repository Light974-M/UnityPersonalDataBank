using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UPDB.ProceduralGeneration.MazeGenerator;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
    [CustomEditor(typeof(MazeGenerator))]
    public class MazeGeneratorEditor : Editor
    {
        public delegate void CallEditorWindowRefreshEventCallback();
        public event CallEditorWindowRefreshEventCallback CallEditorWindowRefresh;

        public override void OnInspectorGUI()
        {
            MazeGenerator myTarget = (MazeGenerator)target;

            EditorGUI.BeginChangeCheck();

            //register initial color of gui
            Color guiColor = GUI.color;
            Color guiFontColor = GUI.backgroundColor;
            Color guiContentColor = GUI.contentColor;

            GUI.enabled = !myTarget.IsGeneratingMaze;
            GUI.backgroundColor = new Color(1, 0.25f, 0.25f);

            if (GUILayout.Button("\n Generate Maze \n"))
                myTarget.OnGenerateMaze();

            GUI.enabled = myTarget.IsGeneratingMaze;

            if (GUILayout.Button("Kill"))
                myTarget.MazeData.EndOfMazeGenerationBehaviour();

            GUI.backgroundColor = guiFontColor;
            GUI.enabled = true;

            GUIContent updateTypeContent = new GUIContent("Update Type", "how system update generation type ? useful for debug");
            myTarget.MazeData.GenerationUpdater.UpdateType = (GenerationUpdateType)EditorGUILayout.EnumPopup(updateTypeContent, myTarget.MazeData.GenerationUpdater.UpdateType);

            if (myTarget.MazeData.GenerationUpdater.UpdateType == GenerationUpdateType.FixedSecond)
            {
                GUIContent generationTimeContent = new GUIContent("Time", "what is the interval between two updates");
                myTarget.GenerationTime = EditorGUILayout.FloatField(generationTimeContent, myTarget.GenerationTime);

                GUIContent generationTimeMultiplierContent = new GUIContent("Time multiplier", "multiply speed intervals");
                myTarget.GenerationTimeMultiplier = EditorGUILayout.IntField(generationTimeMultiplierContent, myTarget.GenerationTimeMultiplier);
            }

            EditorGUI.BeginChangeCheck();

            if (myTarget.MazeData.GenerationUpdater.UpdateType == GenerationUpdateType.ManualUpdate)
            {
                GUI.enabled = !myTarget.MazeData.GenerationUpdater.Value;
                if (GUILayout.Button("Update Generation"))
                    myTarget.MazeData.GenerationUpdater.Activate();
                GUI.enabled = true;
            }

            if (GUILayout.Button("Move Root"))
            {
                for (int i = 0; i < myTarget.NumberOfRootIterations; i++)
                    myTarget.MazeData.MoveRootToRandomNeighbor();

                SceneView.lastActiveSceneView.Repaint();
            }

            GUIContent numberOfRootIterationsContent = new GUIContent("Number Of Root Iterations", "by pressing the button, how many times root will move ?");
            myTarget.NumberOfRootIterations = EditorGUILayout.IntField(numberOfRootIterationsContent, myTarget.NumberOfRootIterations);

            if (EditorGUI.EndChangeCheck() && myTarget.MazeData.GenerationUpdater.ToCallCell != null && myTarget.MazeData.GenerationUpdater.Read)
            {
                myTarget.MazeData.GenerationUpdater.ToCallCell.Propagate();
                SceneView.lastActiveSceneView.Repaint();
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            /**************************************MAZE RENDERER PARAMETERS****************************************/
            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.Label("Maze Render Parameters", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                GUIContent widthContent = new GUIContent("Width", "width of x coordinates of maze");
                myTarget.Width = EditorGUILayout.IntField(widthContent, myTarget.Width);

                GUIContent heightContent = new GUIContent("Height", "height of x coordinates of maze");
                myTarget.Height = EditorGUILayout.IntField(heightContent, myTarget.Height);

                if (EditorGUI.EndChangeCheck())
                {
                    myTarget.MazeData.RegenerateEntries(myTarget.EntriesPos);
                }

                DrawEntryArray(myTarget);

                GUIContent facedAxisContent = new GUIContent("Faced Axis", "wich axis represent the depth of maze ?");
                myTarget.FacedAxis = (Side)EditorGUILayout.EnumPopup(facedAxisContent, myTarget.FacedAxis);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            /**************************************MAZE DATA PARAMETERS****************************************/
            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.Label("Maze Datas Parameters", EditorStyles.boldLabel);

                GUI.backgroundColor = new Color(0.75f, 0.75f, 1f);
                if (GUILayout.Button("Open Maze Editor"))
                    MazeEditorWindow.ShowMyEditor(myTarget, this);
                GUI.backgroundColor = guiFontColor;
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            /**************************************MAZE BUILD PARAMETERS****************************************/
            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.Label("Maze Build Parameters", EditorStyles.boldLabel);

                GUI.backgroundColor = new Color(0.75f, 1f, 0.75f);
                if (GUILayout.Button("Build Maze"))
                    myTarget.OnBuildMaze();
                GUI.backgroundColor = guiFontColor;

                if (GUILayout.Button("Clear Maze Build"))
                    myTarget.ClearGameObjects();

                GUIContent proceduralMeshGenerationContent = new GUIContent("Procedural Mesh Generation", "if enable, use a generative system for mesh instead of using prefab objects");
                myTarget.ProceduralMeshGeneration = EditorGUILayout.Toggle(proceduralMeshGenerationContent, myTarget.ProceduralMeshGeneration);

                GUIContent wallPrefabContent = new GUIContent("Wall Prefab", "use a prefab to build wall(if procedural mesh is disabled)");
                myTarget.WallPrefab = (GameObject)EditorGUILayout.ObjectField(wallPrefabContent, myTarget.WallPrefab, typeof(GameObject), true);

                GUIContent wallBorderPrefabContent = new GUIContent("Wall Border Prefab", "use a prefab to build wall borders(if procedural mesh is disabled)");
                myTarget.WallBorderPrefab = (GameObject)EditorGUILayout.ObjectField(wallBorderPrefabContent, myTarget.WallBorderPrefab, typeof(GameObject), true);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            /**************************************DEBUG PARAMETERS****************************************/
            EditorGUILayout.BeginVertical("helpBox");
            {
                GUILayout.Label("Debug Parameters", EditorStyles.boldLabel);
                if (GUILayout.Button("clean and rebuild maze datas"))
                {
                    myTarget.MazeData = null;
                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("Swap All Paths"))
                {
                    myTarget.SwapAllPaths();
                    SceneView.RepaintAll();
                }

                GUIContent wallWidthContent = new GUIContent("Wall Width", "width of rendered walls of maze");
                myTarget.WallWidth = EditorGUILayout.FloatField(wallWidthContent, myTarget.WallWidth);

                GUIContent wallHeightContent = new GUIContent("Wall Height", "height of rendered walls of maze");
                myTarget.WallHeight = EditorGUILayout.FloatField(wallHeightContent, myTarget.WallHeight);

                GUIContent showCellPosContent = new GUIContent("Show Cell Pos", "display preview of all maze walkable positions");
                myTarget.ShowCellPos = EditorGUILayout.Toggle(showCellPosContent, myTarget.ShowCellPos);

                GUIContent showPathContent = new GUIContent("Show Path", "display preview of maze path");
                myTarget.ShowPath = EditorGUILayout.Toggle(showPathContent, myTarget.ShowPath);

                GUIContent showPathDirectionContent = new GUIContent("Show Path Direction", "display preview of maze path direction");
                myTarget.ShowPathDirection = EditorGUILayout.Toggle(showPathDirectionContent, myTarget.ShowPathDirection);

                GUIContent showWallsContent = new GUIContent("Show Walls", "display preview of maze walls");
                myTarget.ShowWalls = EditorGUILayout.Toggle(showWallsContent, myTarget.ShowWalls);

                GUIContent clampNumberOfRenderedCellsContent = new GUIContent("Clamp Number Of Rendered Cells", "auto breaker to avoid lagging when big values");
                myTarget.ClampNumberOfRenderedCells = EditorGUILayout.Toggle(clampNumberOfRenderedCellsContent, myTarget.ClampNumberOfRenderedCells);

                GUIContent economiserOfPerformanceRendererContent = new GUIContent("Economiser Of Performance Renderer", "when enabled, lots of options will be disabled to show only necessary content");
                myTarget.EconomiserOfPerformanceRenderer = EditorGUILayout.Toggle(economiserOfPerformanceRendererContent, myTarget.EconomiserOfPerformanceRenderer);

                GUIContent refreshOnYContent = new GUIContent("Refresh On Y", "when enabled, refresh screen, this is a lot less laggy than normal mode and allow really big numbers to be rendered");
                myTarget.RefreshOnY = EditorGUILayout.Toggle(refreshOnYContent, myTarget.RefreshOnY);
            }
            EditorGUILayout.EndVertical();

            //reattribute original GUI color
            GUI.color = guiColor;
            GUI.backgroundColor = guiFontColor;
            GUI.contentColor = guiContentColor;

            if (EditorGUI.EndChangeCheck())
            {
                CallEditorWindowRefresh?.Invoke();
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        private void DrawEntryArray(MazeGenerator myTarget)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

                GUIContent entriesPosContent = new GUIContent("Entries Pos", "array of positions for entries drawing");
                myTarget.EntriesArrayDropDownValue = EditorGUILayout.BeginFoldoutHeaderGroup(myTarget.EntriesArrayDropDownValue, entriesPosContent, EditorStyles.foldout);
            }
            EditorGUILayout.EndHorizontal();

            if (myTarget.EntriesArrayDropDownValue)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical("helpBox");
                {
                    for (int i = 0; i < myTarget.EntriesPos.Count; i++)
                    {
                        myTarget.EntriesPos[i] = EditorGUILayout.Vector2IntField($"Entry {i + 1}", myTarget.EntriesPos[i]);
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(EditorGUIUtility.singleLineHeight * 25);

                        if (GUILayout.Button(" + "))
                            myTarget.EntriesPos.Add(Vector2Int.zero);
                        if (GUILayout.Button(" - "))
                            myTarget.EntriesPos.Remove(myTarget.EntriesPos[myTarget.EntriesPos.Count - 1]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    myTarget.MazeData.RegenerateEntries(myTarget.EntriesPos);
                }
            }

        }
    }
}
