using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
    [AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.ProceduralGeneration + "/" + NamespaceID.MazeGenerator)]
    public class MazeGenerator : UPDBBehaviour
    {
        [Header("Maze Render Parameters")]
        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        [SerializeField]
        private List<Vector2Int> _entriesPos = new List<Vector2Int>();

        [SerializeField]
        private Side _facedAxis;

        [Header("Maze Build Parameters")]
        [SerializeField]
        private bool _proceduralMeshGeneration = false;

        [SerializeField]
        private GameObject _wallPrefab;

        [SerializeField]
        private GameObject _wallBorderPrefab;

        [SerializeField]
        private Mesh _proceduralMesh;

        [SerializeField]
        private Material _proceduralMeshMaterial;


        [Header("Debug parameters")]
        [SerializeField]
        private float _wallWidth = 0;

        [SerializeField]
        private float _wallHeight = 1;

        [SerializeField]
        private bool _showCellPos = true;

        [SerializeField]
        private bool _showPath = true;

        [SerializeField]
        private bool _showPathDirection = false;

        [SerializeField]
        private bool _showWalls = true;

        [SerializeField]
        private bool _clampNumberOfRenderedCells = true;

        [SerializeField]
        private int _numberOfRootIterations = 1;

        [SerializeField]
        private bool _economiserOfPerformanceRenderer = false;

        [SerializeField]
        private bool _refreshOnY = false;

        #region Non serialized API

        private Maze _mazeData;

        private bool _entriesArrayDropDownValue = false;

        private bool _isGeneratingMaze = false;

        private float _generationTime = 0.01f;

        private int _generationTimeMultiplier = 1;

        private int _generationTimeMultiplierTimer = 0;

        private int _previewY = 0;

        private GameObject _mazeObject = null;
        private GameObject _wallsObjects = null;
        private GameObject _borderWallsObjects = null;

        #endregion

        #region Public API

        public Maze MazeData
        {
            get
            {
                if (_mazeData == null || _mazeData.Width != _width || _mazeData.Height != _height)
                {
                    _mazeData = new Maze(_width, _height, _entriesPos);

                    _mazeData.EndOfMazeGeneration += OnMazeGenerationEnd;
                    _mazeData.MazeGenerationdUpdate += OnMazeGenerationUpdateRequest;

                    _previewY = 0;
                }

                return _mazeData;
            }

            set
            {
                _mazeData = value;
            }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public Side FacedAxis
        {
            get { return _facedAxis; }
            set { _facedAxis = value; }
        }

        public float WallWidth
        {
            get { return _wallWidth; }
            set { _wallWidth = value; }
        }

        public float WallHeight
        {
            get { return _wallHeight; }
            set { _wallHeight = value; }
        }

        public bool ShowCellPos
        {
            get { return _showCellPos; }
            set { _showCellPos = value; }
        }

        public bool ShowPath
        {
            get { return _showPath; }
            set { _showPath = value; }
        }

        public bool ShowPathDirection
        {
            get { return _showPathDirection; }
            set { _showPathDirection = value; }
        }

        public bool ShowWalls
        {
            get { return _showWalls; }
            set { _showWalls = value; }
        }

        public bool ClampNumberOfRenderedCells
        {
            get { return _clampNumberOfRenderedCells; }
            set { _clampNumberOfRenderedCells = value; }
        }

        public bool EconomiserOfPerformanceRenderer
        {
            get { return _economiserOfPerformanceRenderer; }
            set { _economiserOfPerformanceRenderer = value; }
        }

        public List<Vector2Int> EntriesPos
        {
            get { return _entriesPos; }
            set { _entriesPos = value; }
        }

        public bool EntriesArrayDropDownValue
        {
            get { return _entriesArrayDropDownValue; }
            set { _entriesArrayDropDownValue = value; }
        }

        public bool IsGeneratingMaze
        {
            get { return _isGeneratingMaze; }
            set { _isGeneratingMaze = value; }
        }

        public float GenerationTime
        {
            get { return _generationTime; }
            set { _generationTime = value; }
        }

        public int GenerationTimeMultiplier
        {
            get { return _generationTimeMultiplier; }
            set { _generationTimeMultiplier = value; }
        }

        public int NumberOfRootIterations
        {
            get { return _numberOfRootIterations; }
            set { _numberOfRootIterations = value; }
        }

        public bool RefreshOnY
        {
            get { return _refreshOnY; }
            set { _refreshOnY = value; }
        }

        public bool ProceduralMeshGeneration
        {
            get { return _proceduralMeshGeneration; }
            set { _proceduralMeshGeneration = value; }
        }

        public GameObject WallPrefab
        {
            get { return _wallPrefab; }
            set { _wallPrefab = value; }
        }

        public GameObject MazeObject
        {
            get
            {
                if (_mazeObject == null)
                {
                    _mazeObject = new GameObject("Maze Object");
                    _mazeObject.transform.SetParent(transform);
                }

                return _mazeObject;
            }
        }

        public GameObject WallsObjects
        {
            get
            {
                if (_wallsObjects == null)
                {
                    _wallsObjects = new GameObject("Walls");
                    _wallsObjects.transform.SetParent(MazeObject.transform);
                }

                return _wallsObjects;
            }
        }

        public GameObject BorderWallsObjects
        {
            get
            {
                if (_borderWallsObjects == null)
                {
                    _borderWallsObjects = new GameObject("Border Walls");
                    _borderWallsObjects.transform.SetParent(MazeObject.transform);
                }

                return _borderWallsObjects;
            }
        }

        public GameObject WallBorderPrefab
        {
            get { return _wallBorderPrefab; }
            set { _wallBorderPrefab = value; }
        }

        public Mesh ProceduralMesh
        {
            get { return _proceduralMesh; }
            set { _proceduralMesh = value; }
        }

        public Material ProceduralMeshMaterial
        {
            get { return _proceduralMeshMaterial; }
            set { _proceduralMeshMaterial = value; }
        }

        #endregion

        protected override void OnScene()
        {
            if (_refreshOnY)
            {
                DrawWavePreview();
            }
            else
            {
                DrawPreview();
            }

            if (_isGeneratingMaze && MazeData.GenerationUpdater.UpdateType == GenerationUpdateType.FrameRate)
            {
                UpdateNextCell();

                if (MazeData.MazeGenerationStack.Count <= 0)
                    MazeData.EndOfMazeGenerationBehaviour();
            }
        }


        /********************************************TOOL PARAMETERS METHODS***********************************************/

        public void SwapAllPaths()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    foreach (Path path in MazeData.CellsArray[x, y].OriginPaths)
                    {
                        path.Walkable = !path.Walkable;
                    }
                }
            }
        }


        /********************************************PREVIEW DRAWING METHODS***********************************************/

        /// <summary>
        /// draw preview of maze with rays(wich means it's independant from gizmos)
        /// </summary>
        public void DrawPreview()
        {
            Color gizmoColor = Gizmos.color;

            bool isRendered = _showCellPos || _showPath || _showWalls;

            Vector3 right = Vector3.zero;
            Vector3 up = Vector3.zero;
            Vector3 forward = Vector3.zero;

            InitMazeScaleAndDir(ref right, ref up, ref forward, _wallWidth + 1);

            int incrementOfOptimizer = 10000;
            int optimizer = _clampNumberOfRenderedCells ? _width * _height / incrementOfOptimizer : 0;
            int optimizerBreakerY = 0;

            for (int y = 0; y < _height; y++)
            {
                bool isYEdgeOfMaze = y == 0 || y == _height - 1;

                int optimizerBreakerX = 0;

                if (optimizerBreakerY < optimizer)
                {
                    optimizerBreakerY++;
                    continue;
                }
                optimizerBreakerY = 0;

                for (int x = 0; x < _width; x++)
                {
                    bool isEdgeOfMaze = x == 0 || x == _width - 1 || isYEdgeOfMaze;

                    if (!isRendered && !isEdgeOfMaze)
                        continue;

                    if (optimizerBreakerX < optimizer)
                    {
                        optimizerBreakerX++;
                        continue;
                    }
                    optimizerBreakerX = 0;

                    if (_economiserOfPerformanceRenderer)
                    {
                        DrawEconomisedRender(x, y);
                        continue;
                    }

                    DrawCellPosPreview(x, y, right, up, forward);

                    DrawPathsAndWallsPreview(x, y, right, up, forward);
                }
            }

            for (int y = 0; y < _height; y++)
            {
                bool isYEdgeOfMaze = y == 0 || y == _height - 1;

                for (int x = 0; x < _width; x++)
                {
                    bool isEdgeOfMaze = x == 0 || x == _width - 1 || isYEdgeOfMaze;

                    if (!isEdgeOfMaze)
                        continue;

                    Cell[] entriesLinked = ListEntryLinked(MazeData.CellsArray[x, y]);

                    bool isLeftWall = !IsCellCoords(entriesLinked, x - 1, y);
                    bool isRightWall = !IsCellCoords(entriesLinked, x + 1, y);
                    bool isDownWall = !IsCellCoords(entriesLinked, x, y - 1);
                    bool isUpWall = !IsCellCoords(entriesLinked, x, y + 1);

                    if (!isLeftWall && !isRightWall && !isDownWall && !isUpWall)
                        continue;

                    if (_economiserOfPerformanceRenderer)
                    {
                        if (x == 0 && isLeftWall)
                            DrawEconomisedBorderWall(x, y, x - 1, y);

                        if (x >= _width - 1 && isRightWall)
                            DrawEconomisedBorderWall(x, y, x + 1, y);

                        if (y == 0 && isDownWall)
                            DrawEconomisedBorderWall(x, y, x, y - 1);

                        if (y >= _height - 1 && isUpWall)
                            DrawEconomisedBorderWall(x, y, x, y + 1);

                        continue;
                    }

                    if (x == 0 && isLeftWall)
                        DrawBorderWallPreview(x, y, false, x - 1, y, right, up, forward);

                    if (x >= _width - 1 && isRightWall)
                        DrawBorderWallPreview(x, y, false, x + 1, y, right, up, forward);

                    if (y == 0 && isDownWall)
                        DrawBorderWallPreview(x, y, true, x, y - 1, right, up, forward);

                    if (y >= _height - 1 && isUpWall)
                        DrawBorderWallPreview(x, y, true, x, y + 1, right, up, forward);
                }
            }

            Gizmos.color = gizmoColor;
        }

        private void DrawWavePreview()
        {
            for (int x = 0; x < MazeData.Width; x++)
            {
                DrawEconomisedRender(x, _previewY);
            }

            _previewY++;

            if (_previewY >= MazeData.Height)
            {
                _previewY = 0;
            }

            SceneView.currentDrawingSceneView.Repaint();
        }

        private void DrawEconomisedRender(int x, int y)
        {
            if (!_showWalls && !_showPath)
                return;

            foreach (Path elem in MazeData.CellsArray[x, y].OriginPaths)
            {
                if (elem.IsEntry)
                    continue;

                if ((elem.Walkable && !_showPath) || (!elem.Walkable && !_showWalls))
                    continue;

                if (elem.Walkable)
                {
                    Debug.DrawLine(new Vector3(x, y, 0), new Vector3(elem.Target.X, elem.Target.Y, 0), Color.green);
                    continue;
                }

                if (!elem.Walkable)
                {
                    float targetMinX = 0;
                    float targetMinY = 0;
                    float targetMaxX = 0;
                    float targetMaxY = 0;

                    if (x == elem.Target.X && y < elem.Target.Y)
                    {
                        targetMinY = y + 0.5f;
                        targetMaxY = y + 0.5f;
                        targetMinX = x - 0.5f;
                        targetMaxX = x + 0.5f;
                    }
                    if (x == elem.Target.X && y > elem.Target.Y)
                    {
                        targetMinY = y - 0.5f;
                        targetMaxY = y - 0.5f;
                        targetMinX = x - 0.5f;
                        targetMaxX = x + 0.5f;
                    }
                    if (y == elem.Target.Y && x < elem.Target.X)
                    {
                        targetMinY = y - 0.5f;
                        targetMaxY = y + 0.5f;
                        targetMinX = x + 0.5f;
                        targetMaxX = x + 0.5f;
                    }
                    if (y == elem.Target.Y && x > elem.Target.X)
                    {
                        targetMinY = y - 0.5f;
                        targetMaxY = y + 0.5f;
                        targetMinX = x - 0.5f;
                        targetMaxX = x - 0.5f;
                    }

                    Debug.DrawLine(new Vector3(targetMinX, targetMinY, 0), new Vector3(targetMaxX, targetMaxY, 0), Color.red);

                    continue;
                }
            }
        }

        private void DrawEconomisedBorderWall(int x, int y, int neighborX, int neighborY)
        {
            float targetMinX = 0;
            float targetMinY = 0;
            float targetMaxX = 0;
            float targetMaxY = 0;

            if (x == neighborX && y < neighborY)
            {
                targetMinY = y + 0.5f;
                targetMaxY = y + 0.5f;
                targetMinX = x - 0.5f;
                targetMaxX = x + 0.5f;
            }
            if (x == neighborX && y > neighborY)
            {
                targetMinY = y - 0.5f;
                targetMaxY = y - 0.5f;
                targetMinX = x - 0.5f;
                targetMaxX = x + 0.5f;
            }
            if (y == neighborY && x < neighborX)
            {
                targetMinY = y - 0.5f;
                targetMaxY = y + 0.5f;
                targetMinX = x + 0.5f;
                targetMaxX = x + 0.5f;
            }
            if (y == neighborY && x > neighborX)
            {
                targetMinY = y - 0.5f;
                targetMaxY = y + 0.5f;
                targetMinX = x - 0.5f;
                targetMaxX = x - 0.5f;
            }

            Debug.DrawLine(new Vector3(targetMinX, targetMinY, 0), new Vector3(targetMaxX, targetMaxY, 0), Color.black);
        }

        private void GenerateXandYBounds(out float xMin, out float xMax, out float yMin, out float yMax)
        {
            float maxDistOfCore = 1;
            Camera targetCam = Camera.main;
            Ray rayCenterOfCam = targetCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
            Ray rayLowerCamBound = targetCam.ViewportPointToRay(new Vector3(0, 0, 1));
            Ray rayUpperCamBound = targetCam.ViewportPointToRay(new Vector3(1, 1, 1));

            Vector3 lowerBoundOrigin = rayLowerCamBound.origin + (rayLowerCamBound.direction * maxDistOfCore);
            Vector3 upperBoundOrigin = rayUpperCamBound.origin + (rayUpperCamBound.direction * maxDistOfCore);

            if (FindIntersectionWithHeight(rayCenterOfCam.origin, rayCenterOfCam.direction, transform.position.y, out float t))
                DebugDrawPoint(GetIntersectionPointWithHeight(rayCenterOfCam.origin, rayCenterOfCam.direction, t));

            //if (FindIntersectionWithHeight(lowerBoundOrigin, rayCenterOfCam.direction, transform.position.y, out float t))
            //    DebugDrawPoint(GetIntersectionPointWithHeight(lowerBoundOrigin, rayCenterOfCam.direction, t));

            //if (FindIntersectionWithHeight(upperBoundOrigin, rayCenterOfCam.direction, transform.position.y, out t))
            //    DebugDrawPoint(GetIntersectionPointWithHeight(upperBoundOrigin, rayCenterOfCam.direction, t));

            Debug.DrawRay(rayLowerCamBound.origin + (rayLowerCamBound.direction * maxDistOfCore), rayCenterOfCam.direction * targetCam.farClipPlane, Color.blue);
            Debug.DrawRay(rayUpperCamBound.origin + (rayUpperCamBound.direction * maxDistOfCore), rayCenterOfCam.direction * targetCam.farClipPlane, Color.blue);
            Debug.DrawRay(rayLowerCamBound.origin, rayLowerCamBound.direction * targetCam.farClipPlane, Color.magenta);
            Debug.DrawRay(rayUpperCamBound.origin, rayUpperCamBound.direction * targetCam.farClipPlane, Color.magenta);

            xMin = 0;
            xMax = 0;
            yMin = 0;
            yMax = 0;
        }

        private void InitMazeScaleAndDir(ref Vector3 right, ref Vector3 up, ref Vector3 forward, float wallWidthMultiplier)
        {
            if (_facedAxis == Side.YSided)
            {
                right = transform.right * wallWidthMultiplier * transform.localScale.x;
                up = transform.up * transform.localScale.y;
                forward = transform.forward * wallWidthMultiplier * transform.localScale.z;

                return;
            }

            if (_facedAxis == Side.ZSided)
            {
                right = transform.right * transform.localScale.x;
                up = transform.up * transform.localScale.y;
                forward = transform.forward * transform.localScale.z;

                return;
            }

            if (_facedAxis == Side.XSided)
            {
                right = transform.right * transform.localScale.x;
                up = transform.up * transform.localScale.y;
                forward = transform.forward * transform.localScale.z;

                return;
            }
        }

        private Vector3 GetLocalCoordsOfPos(float x, float y)
        {
            if (_facedAxis == Side.ZSided)
                return new Vector3(x, y, 0);
            if (_facedAxis == Side.XSided)
                return new Vector3(0, y, x);
            if (_facedAxis == Side.YSided)
                return new Vector3(x, 0, y);

            return Vector3.zero;
        }

        private Vector3 GetGlobalCoordsOfPos(float x, float y, Vector3 position, Vector3 right, Vector3 up, Vector3 forward)
        {
            Vector3 localCoords = GetLocalCoordsOfPos(x, y);
            return Point3LocalToWorld(localCoords, position, right, up, forward);
        }

        private Vector3 GetWallScale(bool isHorizontal, Vector3 right, Vector3 up, Vector3 forward)
        {
            InitMazeScaleAndDir(ref right, ref up, ref forward, (_wallWidth * 2) + 1);

            if (_facedAxis == Side.ZSided)
                return isHorizontal ? new Vector3(right.magnitude, _wallWidth, _wallHeight * transform.localScale.z) : new Vector3(_wallWidth, up.magnitude, _wallHeight * transform.localScale.z);
            if (_facedAxis == Side.XSided)
                return isHorizontal ? new Vector3(_wallHeight * transform.localScale.x, _wallWidth, forward.magnitude) : new Vector3(_wallHeight * transform.localScale.x, up.magnitude, _wallWidth);
            if (_facedAxis == Side.YSided)
                return isHorizontal ? new Vector3(right.magnitude, _wallHeight * transform.localScale.y, _wallWidth) : new Vector3(_wallWidth, _wallHeight * transform.localScale.y, forward.magnitude);


            return Vector3.zero;
        }

        private void DrawCellPosPreview(int x, int y, Vector3 right, Vector3 up, Vector3 forward)
        {
            if (!_showCellPos)
                return;

            Vector3 globalCoords = GetGlobalCoordsOfPos(x, y, transform.position, right, up, forward);
            DebugDrawPoint(globalCoords, MazeData.CellsArray[x, y].Visited ? Color.green : Color.white);
        }

        private void DrawPathsAndWallsPreview(int x, int y, Vector3 right, Vector3 up, Vector3 forward)
        {
            if (!_showWalls && !_showPath)
                return;

            foreach (Path elem in MazeData.CellsArray[x, y].OriginPaths)
            {
                if (elem.IsEntry)
                    continue;

                if ((elem.Walkable && !_showPath) || (!elem.Walkable && !_showWalls))
                    continue;

                DrawPathElement(elem, x, y, right, up, forward);
            }
        }

        private void DrawPathElement(Path elem, int x, int y, Vector3 right, Vector3 up, Vector3 forward)
        {
            if (elem.Walkable)
            {
                Vector3 globalCoords = GetGlobalCoordsOfPos(x, y, transform.position, right, up, forward);
                Vector3 neighborGlobalCoords = GetGlobalCoordsOfPos(elem.Target.X, elem.Target.Y, transform.position, right, up, forward);

                if (_showPathDirection)
                    DebugDrawArrowLine(globalCoords, neighborGlobalCoords, Color.green, 0.2f);
                else
                    Debug.DrawLine(globalCoords, neighborGlobalCoords, Color.green);

                return;
            }

            if (!elem.Walkable)
            {
                if (_wallHeight == 0 && _wallWidth == 0)
                {
                    DrawOptimizedWall(x, y, elem.Target.X, elem.Target.Y, right, up, forward, Color.red);
                    return;
                }

                Vector3 globalCoords = GetGlobalCoordsOfPos(x, y, transform.position, right, up, forward);
                Vector3 neighborGlobalCoords = GetGlobalCoordsOfPos(elem.Target.X, elem.Target.Y, transform.position, right, up, forward);

                Vector3 wallCentrerPos = Vector3.Lerp(globalCoords, neighborGlobalCoords, 0.5f);
                Vector3 cubeScale = GetWallScale(x == elem.Target.X, right, up, forward);

                DebugDrawCube(wallCentrerPos, cubeScale, transform, Color.red);

                return;
            }
        }

        private void DrawBorderWallPreview(int x, int y, bool isHorizontal, int neighborX, int neighborY, Vector3 right, Vector3 up, Vector3 forward)
        {
            if (_wallHeight == 0 && _wallWidth == 0)
            {
                DrawOptimizedWall(x, y, neighborX, neighborY, right, up, forward, Color.black);
                return;
            }

            Vector3 globalCoords = GetGlobalCoordsOfPos(x, y, transform.position, right, up, forward);
            Vector3 neighborGlobalCoords = GetGlobalCoordsOfPos(neighborX, neighborY, transform.position, right, up, forward);

            Vector3 wallCentrerPos = Vector3.Lerp(globalCoords, neighborGlobalCoords, 0.5f);
            Vector3 cubeScale = GetWallScale(isHorizontal, right, up, forward);

            DebugDrawCube(wallCentrerPos, cubeScale, transform, Color.black);
        }

        private void DrawOptimizedWall(int x, int y, int neighborX, int neighborY, Vector3 right, Vector3 up, Vector3 forward, Color color)
        {
            float targetMinX = 0;
            float targetMinY = 0;
            float targetMaxX = 0;
            float targetMaxY = 0;

            if (x == neighborX && y < neighborY)
            {
                targetMinY = y + 0.5f;
                targetMaxY = y + 0.5f;
                targetMinX = x - 0.5f;
                targetMaxX = x + 0.5f;
            }
            if (x == neighborX && y > neighborY)
            {
                targetMinY = y - 0.5f;
                targetMaxY = y - 0.5f;
                targetMinX = x - 0.5f;
                targetMaxX = x + 0.5f;
            }
            if (y == neighborY && x < neighborX)
            {
                targetMinY = y - 0.5f;
                targetMaxY = y + 0.5f;
                targetMinX = x + 0.5f;
                targetMaxX = x + 0.5f;
            }
            if (y == neighborY && x > neighborX)
            {
                targetMinY = y - 0.5f;
                targetMaxY = y + 0.5f;
                targetMinX = x - 0.5f;
                targetMaxX = x - 0.5f;
            }

            Vector3 globalMinCoords = GetGlobalCoordsOfPos(targetMinX, targetMinY, transform.position, right, up, forward);
            Vector3 globalMaxCoords = GetGlobalCoordsOfPos(targetMaxX, targetMaxY, transform.position, right, up, forward);

            Debug.DrawLine(globalMinCoords, globalMaxCoords, color);
        }


        /********************************************MAZE GENERATION METHODS***********************************************/

        public void OnGenerateMaze()
        {
            _isGeneratingMaze = true;
            MazeData.GenerateMaze();
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

        private void OnMazeGenerationEnd()
        {
            _isGeneratingMaze = false;
            SceneView.RepaintAll();
        }

        private void OnMazeGenerationUpdateRequest()
        {

            if (MazeData.GenerationUpdater.UpdateType == GenerationUpdateType.FixedSecond)
            {
                _generationTimeMultiplierTimer++;

                if (_generationTimeMultiplierTimer >= _generationTimeMultiplier)
                {
                    _generationTimeMultiplierTimer = 0;
                    Invoke(nameof(UpdateNextCell), _generationTime);
                }
                else
                {
                    UpdateNextCell();
                }

                return;
            }
        }

        private void UpdateNextCell()
        {
            if (MazeData.GenerationUpdater.ToCallCell != null)
                MazeData.GenerationUpdater.ToCallCell.Propagate();
        }


        /********************************************MAZE BUILD METHODS***********************************************/

        public void OnBuildMaze()
        {
            if (_proceduralMeshGeneration)
                OnProceduralMeshBuild();
            else
                OnPrefabMeshBuild();
        }


        /********************************************MAZE PROCEDURAL MESH BUILD METHODS***********************************************/

        private void OnProceduralMeshBuild()
        {
            ClearGameObjects();

            MeshFilter filter = null;
            MeshRenderer renderer = null;

            if (!MazeObject.TryGetComponent(out filter))
                filter = MazeObject.AddComponent<MeshFilter>();

            if (!MazeObject.TryGetComponent(out renderer))
                renderer = MazeObject.AddComponent<MeshRenderer>();

            renderer.material = _proceduralMeshMaterial;

            _proceduralMesh = new Mesh();
            _proceduralMesh.name = "Procedural Mesh";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();

            Dictionary<Vector3, int> verticesLibrary = new Dictionary<Vector3, int>();

            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.up;
            Vector3 c = Vector3.right;
            Vector3 d = new Vector3(1, 1, 0);
            Vector3 e = new Vector3(0, 0, 1);
            Vector3 f = new Vector3(1, 0, 1);
            Vector3 g = new Vector3(0, 1, 1);
            Vector3 h = new Vector3(1, 1, 1);

            _proceduralMesh.vertices = vertices.ToArray();
            _proceduralMesh.triangles = triangles.ToArray();
            _proceduralMesh.colors = colors.ToArray();

            _proceduralMesh.CalculateNormals();
            _proceduralMesh.RecalculateBounds();
            _proceduralMesh.RecalculateTangents();

            filter.mesh = _proceduralMesh;
        }

        /********************************************MAZE PREFAB BUILD METHODS***********************************************/

        private void OnPrefabMeshBuild()
        {
            if (!_wallPrefab || !_wallBorderPrefab)
                return;

            if (MazeObject.TryGetComponent(out MeshFilter filter))
                IntelliDestroy(filter);

            if (MazeObject.TryGetComponent(out MeshRenderer renderer))
                IntelliDestroy(renderer);

            ClearGameObjects();

            Vector3 right = Vector3.zero;
            Vector3 up = Vector3.zero;
            Vector3 forward = Vector3.zero;

            InitMazeScaleAndDir(ref right, ref up, ref forward, _wallWidth + 1);

            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    BuildPrefabWalls(x, y, right, up, forward);

            for (int y = 0; y < _height; y++)
            {
                bool isYEdgeOfMaze = y == 0 || y == _height - 1;

                for (int x = 0; x < _width; x++)
                {
                    bool isEdgeOfMaze = x == 0 || x == _width - 1 || isYEdgeOfMaze;

                    if (!isEdgeOfMaze)
                        continue;

                    Cell[] entriesLinked = ListEntryLinked(MazeData.CellsArray[x, y]);

                    bool isLeftWall = !IsCellCoords(entriesLinked, x - 1, y);
                    bool isRightWall = !IsCellCoords(entriesLinked, x + 1, y);
                    bool isDownWall = !IsCellCoords(entriesLinked, x, y - 1);
                    bool isUpWall = !IsCellCoords(entriesLinked, x, y + 1);

                    if (!isLeftWall && !isRightWall && !isDownWall && !isUpWall)
                        continue;

                    if (x == 0 && isLeftWall)
                        BuildBorderWall(x, y, false, x - 1, y, right, up, forward);

                    if (x >= _width - 1 && isRightWall)
                        BuildBorderWall(x, y, false, x + 1, y, right, up, forward);

                    if (y == 0 && isDownWall)
                        BuildBorderWall(x, y, true, x, y - 1, right, up, forward);

                    if (y >= _height - 1 && isUpWall)
                        BuildBorderWall(x, y, true, x, y + 1, right, up, forward);
                }
            }
        }

        public void ClearGameObjects()
        {
            int childCount = MazeObject.transform.childCount;

            for (int i = 0; i < childCount; i++)
                IntelliDestroy(MazeObject.transform.GetChild(0).gameObject);
        }

        private void BuildPrefabWalls(int x, int y, Vector3 right, Vector3 up, Vector3 forward)
        {
            foreach (Path elem in MazeData.CellsArray[x, y].OriginPaths)
            {
                if (elem.IsEntry || elem.Walkable)
                    continue;

                BuildPrefabWallElement(elem, x, y, right, up, forward);
            }
        }

        private void BuildPrefabWallElement(Path elem, int x, int y, Vector3 right, Vector3 up, Vector3 forward)
        {
            Vector3 globalCoords = GetGlobalCoordsOfPos(x, y, transform.position, right, up, forward);
            Vector3 neighborGlobalCoords = GetGlobalCoordsOfPos(elem.Target.X, elem.Target.Y, transform.position, right, up, forward);

            Vector3 wallCentrerPos = Vector3.Lerp(globalCoords, neighborGlobalCoords, 0.5f);
            Vector3 cubeScale = GetWallScale(x == elem.Target.X, right, up, forward);

            string wallName = "Wall #";
            wallName += WallsObjects.transform.childCount + 1;
            wallName += $" : origin = ({elem.Origin.X}, {elem.Origin.Y}) ; target = ({elem.Target.X}, {elem.Target.Y})";

            GameObject wallObj = Instantiate(_wallPrefab);
            wallObj.name = wallName;
            wallObj.transform.SetParent(WallsObjects.transform);

            wallObj.transform.position = wallCentrerPos;

            wallObj.transform.right = right;
            wallObj.transform.up = up;
            wallObj.transform.forward = forward;

            wallObj.transform.localScale = cubeScale;
        }

        private void BuildBorderWall(int x, int y, bool isHorizontal, int neighborX, int neighborY, Vector3 right, Vector3 up, Vector3 forward)
        {
            Vector3 globalCoords = GetGlobalCoordsOfPos(x, y, transform.position, right, up, forward);
            Vector3 neighborGlobalCoords = GetGlobalCoordsOfPos(neighborX, neighborY, transform.position, right, up, forward);

            Vector3 wallCentrerPos = Vector3.Lerp(globalCoords, neighborGlobalCoords, 0.5f);
            Vector3 cubeScale = GetWallScale(isHorizontal, right, up, forward);

            string borderWallName = "Border Wall #";
            borderWallName += BorderWallsObjects.transform.childCount + 1;
            borderWallName += $" : origin = ({x}, {y}) ; target = ({neighborX}, {neighborY})";

            GameObject borderWallObj = Instantiate(_wallBorderPrefab);
            borderWallObj.name = borderWallName;
            borderWallObj.transform.SetParent(BorderWallsObjects.transform);

            borderWallObj.transform.position = wallCentrerPos;

            borderWallObj.transform.right = right;
            borderWallObj.transform.up = up;
            borderWallObj.transform.forward = forward;

            borderWallObj.transform.localScale = cubeScale;
        }
    }

    public enum Side
    {
        ZSided,
        XSided,
        YSided,
    }
}
