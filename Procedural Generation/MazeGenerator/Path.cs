using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
    public class Path
    {
        private bool _walkable = false;

        private Cell _origin;

        private Cell _target;

        private bool _isEntry = false;

        #region Public API

        public bool Walkable
        {
            get { return _walkable; }
            set { _walkable = value; }
        }


        public Cell Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public Cell Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public bool IsEntry
        {
            get { return _isEntry; }
            set { _isEntry = value; }
        }

        #endregion

        public Path(Cell origin, Cell target, bool walkable, bool isEntry)
        {
            _origin = origin;
            _target = target;
            _walkable = walkable;
            _isEntry = isEntry;
        }

        public void SwapPathSide()
        {
            _origin.OriginPaths.Remove(this);
            _origin.TargetPaths.Add(this);

            _target.TargetPaths.Remove(this);
            _target.OriginPaths.Add(this);

            Cell origin = _origin;
            _origin = _target;
            _target = origin;
        }
    }
}
