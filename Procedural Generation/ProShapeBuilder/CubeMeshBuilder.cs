using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.CustomPropertyAttributes;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    public class CubeMeshBuilder : ShapeMeshBuilder
    {
        [SerializeField, Tooltip("number of quad for each axis of draw cube")]
        protected Vector3Int _quadNumber = Vector3Int.one;

        [SerializeField, Tooltip("tell wich of cube faces to draw")]
        private Face _faceToDraw = (Face)(-1);

        [SerializeField, Tooltip("tell to build an opposite triangle to every triangles, pretty heavy on performances")]
        private bool _doubleSidedMesh = false;

        [SerializeField, Tooltip("draw everything upside down")]
        private bool _flipNormals = false;

        [SerializeField, Tooltip("set position of pivot compared to mesh")]
        private Pivot _pivotPosition = Pivot.Default;

        [SerializeField, Tooltip("draw separate vertices at edges")]
        private bool _hardEdges = true;

        [SerializeField, Tooltip("if enabled, base number of quads with scale to preserve ratio of quad number")]
        private bool _preserveQuadsRatioWithScale = false;

        #region Public API

        public Vector3Int QuadNumber
        {
            get { return _quadNumber; }
            set { _quadNumber = value; }
        }

        public Face FaceToDraw
        {
            get { return _faceToDraw; }
            set { _faceToDraw = value; }
        }

        public bool DoubleSidedMesh
        {
            get { return _doubleSidedMesh; }
            set { _doubleSidedMesh = value; }
        }

        public bool FlipNormals
        {
            get { return _flipNormals; }
            set { _flipNormals = value; }
        }

        public Pivot PivotPosition
        {
            get { return _pivotPosition; }
            set { _pivotPosition = value; }
        }

        public bool HardEdges
        {
            get { return _hardEdges; }
            set { _hardEdges = value; }
        }

        public bool PreserveQuadsRatioWithScale
        {
            get { return _preserveQuadsRatioWithScale; }
            set { _preserveQuadsRatioWithScale = value; }
        }

        #endregion


        /******************************************CUSTOM METHODS******************************************/

        protected override void OnBuildTrianglesAndVertices(ref List<Vector3> vertices, ref List<int> triangles)
        {
            _relativeCenterPos = Vector3.one * (_scaleFactor / 2f);
            Vector3Int cubeVerticeNumber = _preserveQuadsRatioWithScale ? new Vector3Int( (_quadNumber.x + 1) * Mathf.RoundToInt(transform.localScale.x), (_quadNumber.y + 1) * Mathf.RoundToInt(transform.localScale.y), (_quadNumber.z + 1) * Mathf.RoundToInt(transform.localScale.z)) : new Vector3Int(_quadNumber.x + 1, _quadNumber.y + 1, _quadNumber.z + 1);

            if (_hardEdges)
                OnBuildHardEdgesMesh(ref vertices, ref triangles, cubeVerticeNumber);
            else
                OnBuildSoftEdgesMesh(ref vertices, ref triangles, cubeVerticeNumber);

            MovePivotTo(_pivotPosition, ref vertices);
        }

        private void OnBuildHardEdgesMesh(ref List<Vector3> vertices, ref List<int> triangles, Vector3Int cubeVerticeNumber)
        {
            if ((_faceToDraw & Face.Up) == Face.Up)
                DrawSingleFace(Face.Up, cubeVerticeNumber, ref vertices, ref triangles);
            if ((_faceToDraw & Face.Down) == Face.Down)
                DrawSingleFace(Face.Down, cubeVerticeNumber, ref vertices, ref triangles);
            if ((_faceToDraw & Face.Right) == Face.Right)
                DrawSingleFace(Face.Right, cubeVerticeNumber, ref vertices, ref triangles);
            if ((_faceToDraw & Face.Left) == Face.Left)
                DrawSingleFace(Face.Left, cubeVerticeNumber, ref vertices, ref triangles);
            if ((_faceToDraw & Face.Forward) == Face.Forward)
                DrawSingleFace(Face.Forward, cubeVerticeNumber, ref vertices, ref triangles);
            if ((_faceToDraw & Face.Backward) == Face.Backward)
                DrawSingleFace(Face.Backward, cubeVerticeNumber, ref vertices, ref triangles);
        }

        private void OnBuildSoftEdgesMesh(ref List<Vector3> vertices, ref List<int> triangles, Vector3Int cubeVerticeNumber)
        {
            Dictionary<Vector3, int> verticesLibrary = new Dictionary<Vector3, int>();
            Vector3[,,] verticesCoords = new Vector3[cubeVerticeNumber.x, cubeVerticeNumber.y, cubeVerticeNumber.z];

            DrawFullVertices(cubeVerticeNumber, ref vertices, ref verticesLibrary, ref verticesCoords);

            if ((_faceToDraw & Face.Up) == Face.Up)
                DrawFaceTriangles(Face.Up, cubeVerticeNumber, ref triangles, ref verticesLibrary, ref verticesCoords);
            if ((_faceToDraw & Face.Down) == Face.Down)
                DrawFaceTriangles(Face.Down, cubeVerticeNumber, ref triangles, ref verticesLibrary, ref verticesCoords);
            if ((_faceToDraw & Face.Right) == Face.Right)
                DrawFaceTriangles(Face.Right, cubeVerticeNumber, ref triangles, ref verticesLibrary, ref verticesCoords);
            if ((_faceToDraw & Face.Left) == Face.Left)
                DrawFaceTriangles(Face.Left, cubeVerticeNumber, ref triangles, ref verticesLibrary, ref verticesCoords);
            if ((_faceToDraw & Face.Forward) == Face.Forward)
                DrawFaceTriangles(Face.Forward, cubeVerticeNumber, ref triangles, ref verticesLibrary, ref verticesCoords);
            if ((_faceToDraw & Face.Backward) == Face.Backward)
                DrawFaceTriangles(Face.Backward, cubeVerticeNumber, ref triangles, ref verticesLibrary, ref verticesCoords);
        }


        private void DrawSingleFace(Face faceToDraw, Vector3Int cubeVerticeNumber, ref List<Vector3> vertices, ref List<int> triangles)
        {
            int verticesLength = vertices.Count;

            int cubeVerticeNumberX = 0;
            int cubeVerticeNumberY = 0;

            if (faceToDraw == Face.Up)
            {
                cubeVerticeNumberX = cubeVerticeNumber.x;
                cubeVerticeNumberY = cubeVerticeNumber.z;
            }
            if (faceToDraw == Face.Down)
            {
                cubeVerticeNumberX = cubeVerticeNumber.x;
                cubeVerticeNumberY = cubeVerticeNumber.z;
            }
            if (faceToDraw == Face.Right)
            {
                cubeVerticeNumberX = cubeVerticeNumber.y;
                cubeVerticeNumberY = cubeVerticeNumber.z;
            }
            if (faceToDraw == Face.Left)
            {
                cubeVerticeNumberX = cubeVerticeNumber.y;
                cubeVerticeNumberY = cubeVerticeNumber.z;
            }
            if (faceToDraw == Face.Forward)
            {
                cubeVerticeNumberX = cubeVerticeNumber.x;
                cubeVerticeNumberY = cubeVerticeNumber.y;
            }
            if (faceToDraw == Face.Backward)
            {
                cubeVerticeNumberX = cubeVerticeNumber.x;
                cubeVerticeNumberY = cubeVerticeNumber.y;
            }

            for (int y = 0; y < cubeVerticeNumberY; y++)
            {
                float initPosY = (y / (float)(cubeVerticeNumberY - 1)) * _scaleFactor;

                for (int x = 0; x < cubeVerticeNumberX; x++)
                {
                    float initPosX = (x / (float)(cubeVerticeNumberX - 1)) * _scaleFactor;

                    Vector3 vertexInitPos = Vector3.zero;

                    if (faceToDraw == Face.Up)
                    {
                        vertexInitPos = new Vector3(initPosX, _scaleFactor, initPosY);
                    }
                    if (faceToDraw == Face.Down)
                    {
                        vertexInitPos = new Vector3(initPosX, 0, initPosY);
                    }
                    if (faceToDraw == Face.Right)
                    {
                        vertexInitPos = new Vector3(_scaleFactor, initPosX, initPosY);
                    }
                    if (faceToDraw == Face.Left)
                    {
                        vertexInitPos = new Vector3(0, initPosX, initPosY);
                    }
                    if (faceToDraw == Face.Forward)
                    {
                        vertexInitPos = new Vector3(initPosX, initPosY, _scaleFactor);
                    }
                    if (faceToDraw == Face.Backward)
                    {
                        vertexInitPos = new Vector3(initPosX, initPosY, 0);
                    }

                    vertices.Add(vertexInitPos);
                }
            }

            bool faceOne = faceToDraw == Face.Backward || faceToDraw == Face.Left || faceToDraw == Face.Up;
            bool faceTwo = faceToDraw == Face.Forward || faceToDraw == Face.Right || faceToDraw == Face.Down;

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if ((faceOne && !_flipNormals) || (faceTwo && _flipNormals) || _doubleSidedMesh)
                drawFirstFace = true;

            if ((faceTwo && !_flipNormals) || (faceOne && _flipNormals) || _doubleSidedMesh)
                drawSecondFace = true;

            for (int y = 0; y < cubeVerticeNumberY - 1; y++)
            {
                for (int x = 0; x < cubeVerticeNumberX - 1; x++)
                {
                    int quadIndexA = GetIndexWithCoords2D(x, y, cubeVerticeNumberX) + verticesLength;
                    int quadIndexB = GetIndexWithCoords2D(x + 1, y, cubeVerticeNumberX) + verticesLength;
                    int quadIndexC = GetIndexWithCoords2D(x, y + 1, cubeVerticeNumberX) + verticesLength;
                    int quadIndexD = GetIndexWithCoords2D(x + 1, y + 1, cubeVerticeNumberX) + verticesLength;

                    if (drawFirstFace)
                        AddQuadFourVerticesLinked(ref triangles, quadIndexA, quadIndexC, quadIndexB, quadIndexD);

                    if (drawSecondFace)
                        AddQuadFourVerticesLinked(ref triangles, quadIndexA, quadIndexB, quadIndexC, quadIndexD);
                }
            }
        }

        private void DrawFullVertices(Vector3Int cubeVerticeNumber, ref List<Vector3> vertices, ref Dictionary<Vector3, int> verticesLibrary, ref Vector3[,,] verticesCoords)
        {
            for (int z = 0; z < cubeVerticeNumber.z; z++)
            {
                float vertexPosZ = (z / (float)(cubeVerticeNumber.z - 1)) * _scaleFactor;

                for (int y = 0; y < cubeVerticeNumber.y; y++)
                {
                    float vertexPosY = (y / (float)(cubeVerticeNumber.y - 1)) * _scaleFactor;

                    for (int x = 0; x < cubeVerticeNumber.x; x++)
                    {
                        float vertexPosX = (x / (float)(cubeVerticeNumber.x - 1)) * _scaleFactor;

                        if (x == 0 || x == cubeVerticeNumber.x - 1 || y == 0 || y == cubeVerticeNumber.y - 1 || z == 0 || z == cubeVerticeNumber.z - 1)
                        {
                            Vector3 vertexPosition = new Vector3(vertexPosX, vertexPosY, vertexPosZ);

                            verticesCoords[x, y, z] = vertexPosition;
                            vertices.Add(vertexPosition);
                            verticesLibrary.Add(vertexPosition, vertices.Count - 1);
                        }
                    }
                }
            }
        }

        private void DrawFaceTriangles(Face faceToDraw, Vector3Int cubeVerticeNumber, ref List<int> triangles, ref Dictionary<Vector3, int> verticesLibrary, ref Vector3[,,] verticesCoords)
        {
            int cubeVerticeNumberX = 0;
            int cubeVerticeNumberY = 0;

            if (faceToDraw == Face.Up || faceToDraw == Face.Down)
            {
                cubeVerticeNumberX = cubeVerticeNumber.x;
                cubeVerticeNumberY = cubeVerticeNumber.z;
            }
            if (faceToDraw == Face.Right || faceToDraw == Face.Left)
            {
                cubeVerticeNumberX = cubeVerticeNumber.y;
                cubeVerticeNumberY = cubeVerticeNumber.z;
            }
            if (faceToDraw == Face.Forward || faceToDraw == Face.Backward)
            {
                cubeVerticeNumberX = cubeVerticeNumber.x;
                cubeVerticeNumberY = cubeVerticeNumber.y;
            }

            bool faceOne = faceToDraw == Face.Backward || faceToDraw == Face.Left || faceToDraw == Face.Up;
            bool faceTwo = faceToDraw == Face.Forward || faceToDraw == Face.Right || faceToDraw == Face.Down;

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if ((faceOne && !_flipNormals) || (faceTwo && _flipNormals) || _doubleSidedMesh)
                drawFirstFace = true;

            if ((faceTwo && !_flipNormals) || (faceOne && _flipNormals) || _doubleSidedMesh)
                drawSecondFace = true;

            for (int y = 0; y < cubeVerticeNumberY - 1; y++)
            {
                for (int x = 0; x < cubeVerticeNumberX - 1; x++)
                {
                    int quadIndexA = 0;
                    int quadIndexB = 0;
                    int quadIndexC = 0;
                    int quadIndexD = 0;

                    if (faceToDraw == Face.Up)
                    {
                        verticesLibrary.TryGetValue(verticesCoords[x, cubeVerticeNumber.y - 1, y], out quadIndexA);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, cubeVerticeNumber.y - 1, y], out quadIndexB);
                        verticesLibrary.TryGetValue(verticesCoords[x, cubeVerticeNumber.y - 1, y + 1], out quadIndexC);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, cubeVerticeNumber.y - 1, y + 1], out quadIndexD);
                    }
                    if (faceToDraw == Face.Down)
                    {
                        verticesLibrary.TryGetValue(verticesCoords[x, 0, y], out quadIndexA);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, 0, y], out quadIndexB);
                        verticesLibrary.TryGetValue(verticesCoords[x, 0, y + 1], out quadIndexC);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, 0, y + 1], out quadIndexD);
                    }
                    if (faceToDraw == Face.Right)
                    {
                        verticesLibrary.TryGetValue(verticesCoords[cubeVerticeNumber.x - 1, x, y], out quadIndexA);
                        verticesLibrary.TryGetValue(verticesCoords[cubeVerticeNumber.x - 1, x + 1, y], out quadIndexB);
                        verticesLibrary.TryGetValue(verticesCoords[cubeVerticeNumber.x - 1, x, y + 1], out quadIndexC);
                        verticesLibrary.TryGetValue(verticesCoords[cubeVerticeNumber.x - 1, x + 1, y + 1], out quadIndexD);
                    }
                    if (faceToDraw == Face.Left)
                    {
                        verticesLibrary.TryGetValue(verticesCoords[0, x, y], out quadIndexA);
                        verticesLibrary.TryGetValue(verticesCoords[0, x + 1, y], out quadIndexB);
                        verticesLibrary.TryGetValue(verticesCoords[0, x, y + 1], out quadIndexC);
                        verticesLibrary.TryGetValue(verticesCoords[0, x + 1, y + 1], out quadIndexD);
                    }
                    if (faceToDraw == Face.Forward)
                    {
                        verticesLibrary.TryGetValue(verticesCoords[x, y, cubeVerticeNumber.z - 1], out quadIndexA);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, y, cubeVerticeNumber.z - 1], out quadIndexB);
                        verticesLibrary.TryGetValue(verticesCoords[x, y + 1, cubeVerticeNumber.z - 1], out quadIndexC);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, y + 1, cubeVerticeNumber.z - 1], out quadIndexD);
                    }
                    if (faceToDraw == Face.Backward)
                    {
                        verticesLibrary.TryGetValue(verticesCoords[x, y, 0], out quadIndexA);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, y, 0], out quadIndexB);
                        verticesLibrary.TryGetValue(verticesCoords[x, y + 1, 0], out quadIndexC);
                        verticesLibrary.TryGetValue(verticesCoords[x + 1, y + 1, 0], out quadIndexD);
                    }

                    if (drawFirstFace)
                        AddQuadFourVerticesLinked(ref triangles, quadIndexA, quadIndexC, quadIndexB, quadIndexD);

                    if (drawSecondFace)
                        AddQuadFourVerticesLinked(ref triangles, quadIndexA, quadIndexB, quadIndexC, quadIndexD);
                }
            }
        }

        private void MovePivotTo(Pivot posToMovePivot, ref List<Vector3> vertices)
        {
            if(posToMovePivot == Pivot.Centered)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= _relativeCenterPos;

                return;
            }
            if (posToMovePivot == Pivot.BottomLeftForward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(0, 0, _relativeCenterPos.z * 2);

                return;
            }
            if (posToMovePivot == Pivot.BottomRightBackward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(_relativeCenterPos.x * 2, 0, 0);

                return;
            }
            if (posToMovePivot == Pivot.BottomRightForward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(_relativeCenterPos.x * 2, 0, _relativeCenterPos.z * 2);

                return;
            }
            if (posToMovePivot == Pivot.TopLeftBackward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(0, _relativeCenterPos.y * 2, 0);

                return;
            }
            if (posToMovePivot == Pivot.TopLeftForward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(0, _relativeCenterPos.y * 2, _relativeCenterPos.z * 2);

                return;
            }
            if (posToMovePivot == Pivot.TopRightBackward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(_relativeCenterPos.x * 2, _relativeCenterPos.y * 2, 0);

                return;
            }
            if (posToMovePivot == Pivot.TopRightForward)
            {
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= new Vector3(_relativeCenterPos.x * 2, _relativeCenterPos.y * 2, _relativeCenterPos.z * 2);

                return;
            }
        }
    }

    [System.Flags]
    public enum Face
    {
        None = 0,
        Up = 1,
        Down = 2,
        Right = 4,
        Left = 8,
        Forward = 16,
        Backward = 32,
    }

    public enum Pivot
    {
        Default,
        Centered,

        BottomLeftBackward,
        BottomLeftForward,
        BottomRightBackward,
        BottomRightForward,
        TopLeftBackward,
        TopLeftForward,
        TopRightBackward,
        TopRightForward,
    }
}
