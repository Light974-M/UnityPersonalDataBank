using UnityEngine;
using UnityEditor;
using UPDB.CoreHelper;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.TerrainTools;
using UPDB.Data.NativeTools.SimpleGridLevel;
using UnityEngine.Assertions.Must;
using static UnityEngine.GraphicsBuffer;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
    public class MazeEditorWindow : EditorWindow
    {
        private static MazeGenerator _target;
        private static MazeGeneratorEditor _targetEditor;
        private const string windowName = "Maze Editor";
        private const float _toggleScale = 14;
        private Color _selectedColor = new Color(0.4f, 0.4f, 1, 0.5f);
        private Color _mouseOverColor = new Color(0, 0, 0, 0.25f);

        private Cell _selectedCell;
        private Path _selectedPath;
        private Rect _selectedRect;

        /// <summary>
        /// expressed in toggle length
        /// </summary>
        private float _arraySpacing = 1;

        private bool IsLeftClick
        {
            get
            {
                return Event.current.type == EventType.MouseDown && Event.current.button == 0;
            }
        }

        private bool IsRightClick
        {
            get
            {
                return Event.current.type == EventType.MouseDown && Event.current.button == 1;
            }
        }

        #region Public API

        public enum ForeachPointsExecutionType
        {
            PathsBehaviour,
            CellsBehaviour,
        }

        #endregion

        public static void ShowMyEditor(MazeGenerator target, MazeGeneratorEditor targetEditor)
        {
            // This method is called when the user selects the menu item in the Editor.
            MazeEditorWindow window = GetWindow<MazeEditorWindow>(false, windowName, true);
            window.titleContent = new GUIContent(windowName, "level editor for maze");

            // Limit size of the window.
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 1080);

            _target = target;
            _targetEditor = targetEditor;
            targetEditor.CallEditorWindowRefresh += GetWindow<MazeEditorWindow>().OnRefreshRequestCallback;
        }

        private void OnGUI()
        {
            if (!_target)
                return;

            EditorGUI.BeginChangeCheck();

            //register initial color of gui
            Color guiColor = GUI.color;
            Color guiFontColor = GUI.backgroundColor;
            Color guiContentColor = GUI.contentColor;

            OnSelectedCellMethods();
            OnSelectedPathMethods();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUILayout.BeginHorizontal("helpBox");
            EditorGUILayout.LabelField("Click on path to switch state", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            DrawGrid(guiColor);

            //reattribute original GUI color
            GUI.color = guiColor;
            GUI.backgroundColor = guiFontColor;
            GUI.contentColor = guiContentColor;

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        private void DrawGrid(Color guiColor)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                _selectedPath = null;
                _selectedCell = null;
                Repaint();
            }

            Vector2 mousePos = Event.current.mousePosition;
            Rect arrayRect = new Rect(_toggleScale, GUILayoutUtility.GetLastRect().y + EditorGUIUtility.singleLineHeight * 2, (_toggleScale * _target.MazeData.Width) * 2 - _toggleScale, (_toggleScale * _target.MazeData.Height) * 2 - _toggleScale);

            Rect boxRect = new Rect(arrayRect.x - (_toggleScale / 2f), arrayRect.y - (_toggleScale / 2f), arrayRect.width + _toggleScale, arrayRect.height + _toggleScale);
            boxRect.xMin += 1;
            boxRect.yMin += 1;
            EditorGUI.DrawRect(boxRect, new Color(0f, 0f, 0f, 0.25f));

            ForeachPoints(arrayRect, ForeachPointsExecutionType.PathsBehaviour, mousePos, guiColor);
            ForeachPoints(arrayRect, ForeachPointsExecutionType.CellsBehaviour, mousePos, guiColor);

            if (_selectedCell != null || _selectedPath != null)
                EditorGUI.DrawRect(_selectedRect, _selectedColor);
        }

        private void ForeachPoints(Rect arrayRect, ForeachPointsExecutionType executionType, Vector2 mousePos, Color guiColor)
        {
            Rect toggleRect = new Rect(0, 0, _toggleScale, _toggleScale);
            Rect finalRect = new Rect(0, 0, _toggleScale, _toggleScale);

            for (int y = _target.MazeData.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < _target.MazeData.Width; x++)
                {
                    finalRect.x = arrayRect.x + toggleRect.x;
                    finalRect.y = arrayRect.y + toggleRect.y;

                    if (executionType == ForeachPointsExecutionType.PathsBehaviour)
                        OnDrawPathsBehaviour(x, y, arrayRect, finalRect, mousePos, guiColor);

                    if (executionType == ForeachPointsExecutionType.CellsBehaviour)
                        OnDrawCellsBehaviour(x, y, finalRect, mousePos, guiColor);

                    toggleRect.x += _toggleScale * (_arraySpacing + 1);
                }

                toggleRect.x = 0;
                toggleRect.y += _toggleScale * (_arraySpacing + 1);
            }
        }

        private void OnDrawPathsBehaviour(int x, int y, Rect arrayRect, Rect finalRect, Vector2 mousePos, Color guiColor)
        {
            Rect neighborRect = new Rect(0, 0, _toggleScale, _toggleScale);
            Rect pathRect = new Rect(0, 0, _toggleScale, _toggleScale);

            foreach (Path path in _target.MazeData.CellsArray[x, y].OriginPaths)
            {
                if (path.IsEntry)
                    continue;

                neighborRect.x = arrayRect.x + (path.Target.X * _toggleScale * (_arraySpacing + 1));
                neighborRect.y = arrayRect.y + (((_target.MazeData.Height - 1) - path.Target.Y) * _toggleScale * (_arraySpacing + 1));

                pathRect.x = Mathf.Lerp(finalRect.x, neighborRect.x, 0.5f);
                pathRect.y = Mathf.Lerp(finalRect.y, neighborRect.y, 0.5f);

                float linesLength = 2;

                Rect toggleDrawRect = pathRect;
                Rect lineRect = new Rect();

                if ((path.Walkable && path.Target.X == path.Origin.X) || (!path.Walkable && path.Target.X != path.Origin.X))
                {
                    lineRect = new Rect(pathRect.x + (pathRect.width / 2f), pathRect.y - (_toggleScale / linesLength) + 1, 1, pathRect.height * linesLength - 1);
                }
                else
                {
                    lineRect = new Rect(pathRect.x - (_toggleScale / linesLength) + 1, pathRect.y + (pathRect.height / 2f), pathRect.width * linesLength - 1, 1);
                }

                if (DrawMouseOverRect(toggleDrawRect, mousePos) && IsLeftClick)
                    SelectNewPath(path, toggleDrawRect);

                GUI.color = path.Walkable ? Color.green : Color.red;
                EditorGUI.DrawRect(lineRect, GUI.color);
                GUI.color = Color.white;

                GUI.color = Color.clear;
                path.Walkable = EditorGUI.Toggle(toggleDrawRect, path.Walkable);
                GUI.color = guiColor;
            }
        }

        private void OnDrawCellsBehaviour(int x, int y, Rect finalRect, Vector2 mousePos, Color guiColor)
        {
            Rect pointRect = new Rect(finalRect.x + (finalRect.width / 2f) - 1, finalRect.y + (finalRect.height / 2f) - 1, 3, 3);
            EditorGUI.DrawRect(pointRect, GUI.color);

            bool isLeftClick = IsLeftClick;

            if (DrawMouseOverRect(finalRect, mousePos) && isLeftClick)
                SelectNewCell(_target.MazeData.CellsArray[x, y], finalRect);

            Cell[] entriesLinked = ListEntryLinked(_target.MazeData.CellsArray[x, y]);

            if (x == 0)
            {
                Rect borderRect = new Rect(finalRect.x - (_toggleScale / 2f) - 1, finalRect.y - (_toggleScale / 2f), 2, _toggleScale * 2 + 1);
                Rect toggleBorderRect = new Rect(finalRect.x - _toggleScale, finalRect.y, _toggleScale, _toggleScale);

                borderRect.yMax += y == 0 ? 1 : 0;
                borderRect.yMin -= y == _target.MazeData.Height - 1 ? 1 : 0;

                DrawWalls(borderRect, toggleBorderRect, x - 1, y, entriesLinked, guiColor, isLeftClick, mousePos);
            }

            if (x == _target.MazeData.Width - 1)
            {
                Rect borderRect = new Rect(finalRect.x + (_toggleScale * 1.5f), finalRect.y - (_toggleScale / 2f), 2, _toggleScale * 2 + 1);
                Rect toggleBorderRect = new Rect(finalRect.x + _toggleScale, finalRect.y, _toggleScale, _toggleScale);

                borderRect.yMax += y == 0 ? 1 : 0;
                borderRect.yMin -= y == _target.MazeData.Height - 1 ? 1 : 0;

                DrawWalls(borderRect, toggleBorderRect, x + 1, y, entriesLinked, guiColor, isLeftClick, mousePos);
            }

            if (y == 0)
            {
                Rect borderRect = new Rect(finalRect.x - (_toggleScale / 2f), finalRect.y + (_toggleScale * 1.5f), _toggleScale * 2 + 1, 2);
                Rect toggleBorderRect = new Rect(finalRect.x, finalRect.y + _toggleScale, _toggleScale, _toggleScale);

                borderRect.xMax += y == 0 ? 1 : 0;
                borderRect.xMin -= y == _target.MazeData.Width - 1 ? 1 : 0;

                DrawWalls(borderRect, toggleBorderRect, x, y - 1, entriesLinked, guiColor, isLeftClick, mousePos);
            }

            if (y == _target.MazeData.Height - 1)
            {
                Rect borderRect = new Rect(finalRect.x - (_toggleScale / 2f), finalRect.y - (_toggleScale / 2f) - 1, _toggleScale * 2 + 1, 2);
                Rect toggleBorderRect = new Rect(finalRect.x, finalRect.y - _toggleScale, _toggleScale, _toggleScale);

                borderRect.xMax += y == 0 ? 1 : 0;
                borderRect.xMin -= y == _target.MazeData.Width - 1 ? 1 : 0;

                DrawWalls(borderRect, toggleBorderRect, x, y + 1, entriesLinked, guiColor, isLeftClick, mousePos);
            }
        }


        private void DrawWalls(Rect borderRect, Rect toggleBorderRect, int x, int y, Cell[] entriesLinked, Color guiColor, bool isLeftClick, Vector2 mousePos)
        {
            if (!IsCellCoords(entriesLinked, x, y))
                EditorGUI.DrawRect(borderRect, Color.black);

            Cell entryCell = FindCellWithCoords(entriesLinked, x, y);

            GUI.color = Color.clear;
            if (GUI.Button(toggleBorderRect, string.Empty))
                SwapWall(entryCell, x, y);
            GUI.color = guiColor;


            if (DrawMouseOverRect(toggleBorderRect, mousePos) && isLeftClick && entryCell != null)
                SelectNewCell(entryCell, toggleBorderRect);
        }

        private Cell[] ListEntryLinked(Cell target)
        {
            List<Cell> list = new List<Cell>();

            foreach (Path elem in target.OriginPaths)
                if (elem.IsEntry)
                    list.Add(elem.Target);

            foreach (Path elem in target.TargetPaths)
                if (elem.IsEntry)
                    list.Add(elem.Origin);

            return list.ToArray();
        }

        private bool IsCellCoords(Cell[] list, int x, int y)
        {
            foreach (Cell cell in list)
                if (cell.X == x && cell.Y == y)
                    return true;

            return false;
        }

        private void OnSelectedCellMethods()
        {
            if (_selectedCell == null)
                GUI.enabled = false;

            if (GUILayout.Button("make origin of paths"))
                _selectedCell.MakeAllPathsOrigin();

            if (GUILayout.Button("make target of paths"))
                _selectedCell.MakeAllPathsTarget();

            GUI.enabled = true;
        }

        private void OnSelectedPathMethods()
        {
            if (_selectedPath == null)
                GUI.enabled = false;

            if (GUILayout.Button("Swap Path Side"))
            {
                _selectedPath.SwapPathSide();
            }

            GUI.enabled = true;
        }

        private void OnRefreshRequestCallback()
        {
            Repaint();
        }

        private bool DrawMouseOverRect(Rect selectedRect, Vector2 mousePos)
        {
            if (selectedRect.Contains(mousePos))
            {
                EditorGUI.DrawRect(selectedRect, _mouseOverColor);
                return true;
            }

            return false;
        }

        private void SelectNewCell(Cell cell, Rect newSelectedRect)
        {
            _selectedCell = cell;
            _selectedPath = null;
            _selectedRect = newSelectedRect;
            Repaint();
        }

        private void SelectNewPath(Path path, Rect newSelectedRect)
        {
            _selectedCell = null;
            _selectedPath = path;
            _selectedRect = newSelectedRect;
            Repaint();
        }

        private Cell FindCellWithCoords(Cell[] list, int x, int y)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].X == x && list[i].Y == y)
                    return list[i];

            return null;
        }

        private void SwapWall(Cell entryCell, int x, int y)
        {
            if (entryCell == null)
            {
                _target.EntriesPos.Add(new Vector2Int(x, y));
            }
            else
            {
                int index = FindIndexOfVector2IntWithCoords(_target.EntriesPos, entryCell.X, entryCell.Y);
                _target.EntriesPos.RemoveAt(index);
            }

            _target.MazeData.RegenerateEntries(_target.EntriesPos);

            SceneView.lastActiveSceneView.Repaint();
            _targetEditor.Repaint();
            Repaint();
        }

        private int FindIndexOfVector2IntWithCoords(List<Vector2Int> list, int x, int y)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].x == x && list[i].y == y)
                    return i;

            return -1;
        }
    }
}
