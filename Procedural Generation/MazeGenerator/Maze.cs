using System;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
    public class Maze
    {
        #region Private API

        private int _width;
        private int _height;
        private Cell[,] _cellsArray = null;
        private List<Cell> _entriesList = new List<Cell>();

        private Stack<Cell> _mazeGenerationStack = new Stack<Cell>();

        private MazeGenerationUpdater _generationUpdater = new MazeGenerationUpdater();

        private Cell _rootOfMaze = null;

        #endregion

        #region Public API

        public int Width => _width;
        public int Height => _height;
        public Cell[,] CellsArray => _cellsArray;
        public List<Cell> EntriesList
        {
            get { return _entriesList; }
            set { _entriesList = value; }
        }

        public Stack<Cell> MazeGenerationStack
        {
            get { return _mazeGenerationStack; }
            set { _mazeGenerationStack = value; }
        }

        public MazeGenerationUpdater GenerationUpdater
        {
            get { return _generationUpdater; }
            set { _generationUpdater = value; }
        }

        public delegate void EndOfMazeGenerationCallback();
        public event EndOfMazeGenerationCallback EndOfMazeGeneration;

        public delegate void MazeGenerationdUpdateCallback();
        public event MazeGenerationdUpdateCallback MazeGenerationdUpdate;

        #endregion

        /// <summary>
        /// Constructor for level map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Maze(int width, int height, List<Vector2Int> entries)
        {
            _width = width;
            _height = height;

            _cellsArray = new Cell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _cellsArray[x, y] = new Cell(x, y, this);
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x + 1 < width)
                    {
                        Path newPath = new Path(_cellsArray[x, y], _cellsArray[x + 1, y], false, false);
                        _cellsArray[x, y].OriginPaths.Add(newPath);
                        _cellsArray[x + 1, y].TargetPaths.Add(newPath);
                    }

                    if (y + 1 < height)
                    {
                        Path newPath = new Path(_cellsArray[x, y], _cellsArray[x, y + 1], false, false);
                        _cellsArray[x, y].OriginPaths.Add(newPath);
                        _cellsArray[x, y + 1].TargetPaths.Add(newPath);
                    }
                }
            }

            GenerateEntries(entries);
        }

        public void RegenerateEntries(List<Vector2Int> newEntries)
        {
            for (int i = 0; i < _entriesList.Count; i++)
            {
                for (int j = 0; j < _entriesList[i].OriginPaths.Count; j++)
                {
                    Path path = _entriesList[i].OriginPaths[j];
                    path.Target.TargetPaths.Remove(path);
                    path.Target = null;
                }
                for (int j = 0; j < _entriesList[i].TargetPaths.Count; j++)
                {
                    Path path = _entriesList[i].TargetPaths[j];
                    path.Origin.OriginPaths.Remove(path);
                    path.Origin = null;
                }
            }

            _entriesList.Clear();

            GenerateEntries(newEntries);
        }

        private void GenerateEntries(List<Vector2Int> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                int x = entries[i].x;
                int y = entries[i].y;
                bool isXInBounds = x >= 0 && x < _width;
                bool isYInBounds = y >= 0 && y < _height;

                if (!(isXInBounds) && !(isYInBounds))
                    continue;

                Cell newEntryCell = new Cell(x, y, false, this);
                _entriesList.Add(newEntryCell);

                bool isXPInBounds = x + 1 >= 0 && x + 1 < _width;
                bool isXMInBounds = x - 1 >= 0 && x - 1 < _width;
                bool isYPInBounds = y + 1 >= 0 && y + 1 < _height;
                bool isYMInBounds = y - 1 >= 0 && y - 1 < _height;

                if (isYInBounds && isXPInBounds)
                {
                    Path newPath = new Path(newEntryCell, _cellsArray[x + 1, y], true, true);
                    newEntryCell.OriginPaths.Add(newPath);
                    _cellsArray[x + 1, y].TargetPaths.Add(newPath);
                }

                if (isYInBounds && isXMInBounds)
                {
                    Path newPath = new Path(newEntryCell, _cellsArray[x - 1, y], true, true);
                    newEntryCell.OriginPaths.Add(newPath);
                    _cellsArray[x - 1, y].TargetPaths.Add(newPath);
                }

                if (isXInBounds && isYPInBounds)
                {
                    Path newPath = new Path(newEntryCell, _cellsArray[x, y + 1], true, true);
                    newEntryCell.OriginPaths.Add(newPath);
                    _cellsArray[x, y + 1].TargetPaths.Add(newPath);
                }

                if (isXInBounds && isYMInBounds)
                {
                    Path newPath = new Path(newEntryCell, _cellsArray[x, y - 1], true, true);
                    newEntryCell.OriginPaths.Add(newPath);
                    _cellsArray[x, y - 1].TargetPaths.Add(newPath);
                }
            }
        }

        public void GenerateMaze()
        {
            ReinitMaze();

            int originX = UnityEngine.Random.Range(0, _width);
            int originY = UnityEngine.Random.Range(0, _height);

            _rootOfMaze = _cellsArray[originX, originY];
            _generationUpdater.ToCallCell = _rootOfMaze;

            if (_generationUpdater.UpdateType == GenerationUpdateType.ManualUpdate)
                return;

            if (_generationUpdater.UpdateType == GenerationUpdateType.Instant)
            {
                do
                {
                    if (_generationUpdater.ToCallCell != null)
                        _generationUpdater.ToCallCell.Propagate();

                } while (_mazeGenerationStack.Count > 0);

                EndOfMazeGenerationBehaviour();

                return;
            }

            if (_generationUpdater.UpdateType == GenerationUpdateType.FrameRate)
            {
                MazeGenerationdUpdate?.Invoke();
                return;
            }

            if (_generationUpdater.UpdateType == GenerationUpdateType.FixedSecond)
            {
                MazeGenerationdUpdate?.Invoke();
                return;
            }
        }

        private void ReinitMaze()
        {
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    foreach (Path path in _cellsArray[x, y].OriginPaths)
                        path.Walkable = false;
        }

        public void EndOfMazeGenerationBehaviour()
        {
            _mazeGenerationStack.Clear();
            _generationUpdater.ToCallCell = null;
            CleanVisitedCell();
            EndOfMazeGeneration?.Invoke();
        }

        private void CleanVisitedCell()
        {
            foreach (Cell cell in _cellsArray)
                cell.Visited = false;

            _mazeGenerationStack.Clear();
        }

        public void MoveRootToRandomNeighbor()
        {
            List<Path> availablePaths = new List<Path>();

            foreach (Path path in _rootOfMaze.OriginPaths)
                if (!path.IsEntry && path.Target.IsInMaze)
                    availablePaths.Add(path);

            foreach (Path path in _rootOfMaze.TargetPaths)
                if (!path.IsEntry && path.Origin.IsInMaze)
                    availablePaths.Add(path);

            if (availablePaths.Count == 0)
                return;

            int randomPathIndex = UnityEngine.Random.Range(0, availablePaths.Count);
            Path selectedPath = availablePaths[randomPathIndex];

            selectedPath.Walkable = true;

            if (selectedPath.Origin != _rootOfMaze)
                selectedPath.SwapPathSide();

            _rootOfMaze = selectedPath.Target;

            _rootOfMaze.DestroyNonTargetPaths();
        }
    }
}
