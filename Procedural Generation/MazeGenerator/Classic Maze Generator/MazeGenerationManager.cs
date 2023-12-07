using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Data.MazeGenerator.ClassicMazeGenerator
{
    /// <summary>
    /// manage generation of maze, to drag into empty Game Object to handle generation and store object tiles
    /// </summary>
    [ExecuteAlways]
    public class MazeGenerationManager : MonoBehaviour
    {
        #region Serialized API

        [SerializeField, Tooltip("define width of maze")]
        private int _width = 1;

        [SerializeField, Tooltip("define length of maze")]
        private int _length = 1;

        [SerializeField, Tooltip("define height of maze")]
        private int _height = 1;

        [SerializeField, Tooltip("if true, entry number will be procedural")]
        private bool _proceduralEntry = false;

        [SerializeField]
        private int _entryNumber = 1;

        [SerializeField, Tooltip("")]
        private GameObject _wallTilePrefab;

        [SerializeField, Tooltip("")]
        private bool _cutCircleShape = true;

        [SerializeField, Tooltip("")]
        private int _circleCenterSize = 8;

        [SerializeField, Tooltip("")]
        private bool _roundCircularEdges = false;

        #endregion

        #region Private API

        private bool[,] _mazeArray;

        #endregion

        #region Public API

        public bool[,] MazeArray
        {
            get
            {
                if (_mazeArray == null)
                    _mazeArray = new bool[_width, _length];

                return _mazeArray;
            }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public bool ProceduralEntry
        {
            get { return _proceduralEntry; }
            set { _proceduralEntry = value;}
        }
        public int EntryNumber
        {
            get { return _entryNumber; }
            set { _entryNumber = value; }
        }
        public GameObject WallTilePrefab
        {
            get { return _wallTilePrefab; }
            set { _wallTilePrefab = value; }
        }
        public bool CutCircleShape
        {
            get { return _cutCircleShape; }
            set { _cutCircleShape = value; }
        }
        public int CircleCenterSize
        {
            get { return _circleCenterSize; }
            set { _circleCenterSize = value; }
        }
        public bool RoundCircularEdges
        {
            get { return _roundCircularEdges; }
            set { _roundCircularEdges = value; }
        }

        #endregion

        // Update is called once per frame
        void Update()
        {
            while (!(IsImpair(_width) && IsImpair(_width / 2)))
                _width++;

            while (!(IsImpair(_length) && IsImpair(_length / 2)))
                _length++;
        }

        private bool IsPair(int number)
        {
            return number % 2 == 0;
        }

        private bool IsImpair(int number)
        {
            return number % 2 != 0;
        }

        public void GenerateMaze()
        {
            while (transform.childCount != 0)
                DestroyImmediate(transform.GetChild(0).gameObject);

            _mazeArray = new bool[_width, _length];

            for (int y = 0; y < _length; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == _width - 1 || y == _length - 1;

                    if (isBorder)
                    {
                        _mazeArray[x, y] = true;

                        bool isMiddleOfWidth = x == (_width / 2);
                        bool isMiddleOfLength = y == (_length / 2);
                        bool isFirstGate = (y == 0 && _entryNumber >= 1);
                        bool isSecondGate = (y == _length - 1 && _entryNumber >= 2);
                        bool isThirdGate = (x == 0 && _entryNumber >= 3);
                        bool isFourthGate = (x == _width - 1 && _entryNumber >= 4);

                        if (isMiddleOfWidth || isMiddleOfLength)
                        {
                            if (isFirstGate || isSecondGate || isThirdGate || isFourthGate)
                                _mazeArray[x, y] = false;

                        }
                    }
                }
            }

            for (int y = 0; y < _length; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == _width - 1 || y == _length - 1;

                    if (!isBorder)
                    {
                        if (IsPair(x) && IsPair(y))
                        {
                            _mazeArray[x, y] = true;

                            int random = Random.Range(0, 4);

                            if (random == 0)
                                _mazeArray[x - 1, y] = true;

                            if (random == 1)
                                _mazeArray[x + 1, y] = true;

                            if (random == 2)
                                _mazeArray[x, y - 1] = true;

                            if (random == 3)
                                _mazeArray[x, y + 1] = true;
                        }
                        //else if (IsPair(x) || IsPair(y))
                        //{
                        //    int random = Random.Range(0, 2);

                        //    _mazeArray[x, y] = random == 1;
                        //}
                    }
                }
            }

            for (int y = 0; y < _length; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == _width - 1 || y == _length - 1;

                    if (!isBorder)
                    {
                        if (IsImpair(x) && IsImpair(y))
                        {
                            bool isTileSurrounded = _mazeArray[x - 1, y] && _mazeArray[x + 1, y] && _mazeArray[x, y - 1] && _mazeArray[x, y + 1];

                            if (isTileSurrounded)
                            {
                                int random = Random.Range(0, 4);

                                if (random == 0)
                                    _mazeArray[x - 1, y] = false;

                                if (random == 1)
                                    _mazeArray[x + 1, y] = false;

                                if (random == 2)
                                    _mazeArray[x, y - 1] = false;

                                if (random == 3)
                                    _mazeArray[x, y + 1] = false;
                            }
                        }
                    }
                }
            }

            Vector2Int center = new Vector2Int((_width / 2) + 1, (_length / 2) + 1);

            if (_cutCircleShape)
            {
                for (int y = 0; y < _length; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        int distanceToKeep = _width >= _length ? (_length / 2) - 1 : (_width / 2) - 1;
                        bool isDistanceOutsideBounds = Vector2.Distance(center, new Vector2Int(x, y)) > distanceToKeep;
                        bool isDistanceEqual = Mathf.RoundToInt(Vector2.Distance(center, new Vector2Int(x, y))) == distanceToKeep;

                        if (isDistanceOutsideBounds)
                            MazeArray[x, y] = false;
                        if (isDistanceEqual)
                            MazeArray[x, y] = true;
                    }
                }

                for (int y = 0; y < _length; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        int distanceToKeep = _circleCenterSize;
                        bool isDistanceInside = Vector2.Distance(center, new Vector2Int(x, y)) < distanceToKeep;
                        bool isDistanceEqual = Mathf.RoundToInt(Vector2.Distance(center, new Vector2Int(x, y))) == distanceToKeep;

                        if (isDistanceInside)
                            MazeArray[x, y] = false;
                        if (isDistanceEqual)
                            MazeArray[x, y] = true;
                    }
                }
            }

            for (int z = 0; z < _height; z++)
            {
                for (int y = 0; y < _length; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        if (_mazeArray[x, y])
                        {
                            GameObject tile = Instantiate(_wallTilePrefab, transform.position + new Vector3(x, z, y), Quaternion.identity);
                            tile.transform.SetParent(transform);

                            int distanceToKeepOutside = _width >= _length ? (_length / 2) - 1 : (_width / 2) - 1;
                            int distanceToKeepInside = _circleCenterSize;
                            int Distance = Mathf.RoundToInt(Vector2.Distance(center, new Vector2Int(x, y)));
                            bool isDistanceOutsideEqual = Distance == distanceToKeepOutside;
                            bool isDistanceInsideEqual = Distance == distanceToKeepInside;
                            Vector3 mazeCenter = transform.position + new Vector3((_width / 2) + 1, z, (_length / 2) + 1);

                            if (_roundCircularEdges && (isDistanceOutsideEqual || isDistanceInsideEqual))
                            {
                                tile.transform.LookAt(mazeCenter);
                                tile.transform.localScale = new Vector3(1.5f, 1, 1);

                                if (isDistanceOutsideEqual)
                                    tile.transform.position = mazeCenter + tile.transform.forward * distanceToKeepOutside;
                                else if (isDistanceInsideEqual)
                                    tile.transform.position = mazeCenter + tile.transform.forward * distanceToKeepInside;
                            }
                        }
                    }
                }
            }
        }
    }
}
