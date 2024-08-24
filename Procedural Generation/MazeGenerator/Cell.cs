using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
    public class Cell
    {
        private Maze _linkedMaze;

        private int _x;
        private int _y;

        private List<Path> _originPaths = new List<Path>();
        private List<Path> _targetPaths = new List<Path>();

        private bool _isInMaze;

        private bool _visited = false;
        
        #region Public API

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public bool IsInMaze
        {
            get { return _isInMaze; }
            set { _isInMaze = value; }
        }

        public List<Path> OriginPaths
        {
            get
            {
                if (_originPaths == null)
                    _originPaths = new List<Path>();

                return _originPaths;
            }

            set
            {
                _originPaths = value;
            }
        }

        public List<Path> TargetPaths
        {
            get
            {
                if (_targetPaths == null)
                    _targetPaths = new List<Path>();

                return _targetPaths;
            }

            set
            {
                _targetPaths = value;
            }
        }

        public bool Visited
        {
            get { return _visited; }
            set { _visited = value; }
        }

        #endregion

        public Cell(int x, int y, Maze linkedMaze)
        {
            _x = x;
            _y = y;
            _isInMaze = true;
            _linkedMaze = linkedMaze;
        }

        public Cell(int x, int y, bool isInmaze, Maze linkedMaze)
        {
            _x = x;
            _y = y;
            _isInMaze = false;
            _linkedMaze = linkedMaze;
        }

        public void MakeAllPathsOrigin()
        {
            int length = _targetPaths.Count;

            for (int i = 0; i < length; i++)
                _targetPaths[0].SwapPathSide();
        }

        public void MakeAllPathsTarget()
        {
            int length = _originPaths.Count;

            for (int i = 0; i < length; i++)
                _originPaths[0].SwapPathSide();
        }

        public void DestroyNonOriginPaths()
        {
            int length = _targetPaths.Count;

            for (int i = 0; i < length; i++)
            {
                _targetPaths[0].Walkable = false;
                _targetPaths[0].SwapPathSide();
            }
        }

        public void DestroyNonTargetPaths()
        {
            int length = _originPaths.Count;

            for (int i = 0; i < length; i++)
            {
                _originPaths[0].Walkable = false;
                _originPaths[0].SwapPathSide();
            }
        }

        public void Propagate()
        {
            _visited = true;

            List<Path> availablePaths = FindAvailablePaths();

            if (availablePaths.Count != 0)
                ContinuePropagate(ref availablePaths);
            else
                StackPileGoBack();

            //if (_linkedMaze.GenerationUpdater.ToCallCell != null)
            //    _linkedMaze.OnMazeGenerationUpdateRequest();
        }

        private List<Path> FindAvailablePaths()
        {
            List<Path> availablePaths = new List<Path>();

            foreach (Path path in _originPaths)
                if (!path.Target.Visited && !path.IsEntry && path.Target.IsInMaze)
                    availablePaths.Add(path);

            foreach (Path path in _targetPaths)
                if (!path.Origin.Visited && !path.IsEntry && path.Origin.IsInMaze)
                    availablePaths.Add(path);

            return availablePaths;
        }

        private void ContinuePropagate(ref List<Path> availablePaths)
        {
            int randomPathIndex = Random.Range(0, availablePaths.Count);
            Path selectedPath = availablePaths[randomPathIndex];

            if (selectedPath.Target != this)
                selectedPath.SwapPathSide();

            selectedPath.Walkable = true;

            Cell targetCellToCall = selectedPath.Origin;

            _linkedMaze.MazeGenerationStack.Push(this);

            _linkedMaze.GenerationUpdater.ToCallCell = targetCellToCall;
        }

        private void StackPileGoBack()
        {
            if (_linkedMaze.MazeGenerationStack.Count != 0)
                _linkedMaze.GenerationUpdater.ToCallCell = _linkedMaze.MazeGenerationStack.Pop();
        }
    }
}
