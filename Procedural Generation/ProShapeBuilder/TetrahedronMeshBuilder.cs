using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.ProceduralGeneration.ProShapeBuilder;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    [HelpURL(URL.baseURL + "/tree/main/Proceduralgeneration/ProShapeBuilder/README.md")]
    [AddComponentMenu(NamespaceID.ProceduralGenerationPath + "/" + NamespaceID.ProShapeBuilder + "/Tetrahedron Mesh Builder")]
    public class TetrahedronMeshBuilder : ShapeMeshBuilder
    {
        [SerializeField, Tooltip("tell what base scale should look like")]
        private BaseScaleUnitOfSolid _baseScaleType;

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

        public BaseScaleUnitOfSolid BaseScaleType
        {
            get { return _baseScaleType; }
            set { _baseScaleType = value; }
        }

        #endregion

        protected override void OnBuildTrianglesAndVertices(ref List<Vector3> vertices, ref List<int> triangles)
        {
            float refUnit = 1;

            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesOneMeter)
                refUnit = Mathf.Sqrt(0.5f);
            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesSqrtTwo)
                refUnit = 1;
            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesSqrtThree)
                refUnit = Mathf.Sqrt(1.5f);
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusHalfMeter)
                refUnit = Mathf.Sqrt(1/3f);
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusHalfSqrtTwo)
                refUnit = Mathf.Sqrt(2 / 3f);
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusOneMeter)
                refUnit = Mathf.Sqrt(4 / 3f);

            if (_hardEdges)
                OnBuildHardEdgeMesh(ref vertices, ref triangles, refUnit);
            else
                OnBuildSoftEdgeMesh(ref vertices, ref triangles, refUnit);

            if (_pivotPosition == Pivot.Centered)
            {
                CalculateCenterOfMesh(vertices);

                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] -= _relativeCenterPos;
            }
        }

        private void OnBuildHardEdgeMesh(ref List<Vector3> vertices, ref List<int> triangles, float refUnit)
        {

            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(refUnit, refUnit, 0);
            Vector3 c = new Vector3(refUnit, 0, refUnit);
            Vector3 d = new Vector3(0, refUnit, refUnit);

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                CreateMeshTriangle(ref vertices, ref triangles, a, b, c);
                CreateMeshTriangle(ref vertices, ref triangles, a, d, b);
                CreateMeshTriangle(ref vertices, ref triangles, c, d, a);
                CreateMeshTriangle(ref vertices, ref triangles, c, b, d);
            }
            if (drawSecondFace)
            {
                CreateMeshTriangle(ref vertices, ref triangles, a, c, b);
                CreateMeshTriangle(ref vertices, ref triangles, a, b, d);
                CreateMeshTriangle(ref vertices, ref triangles, c, a, d);
                CreateMeshTriangle(ref vertices, ref triangles, c, d, b);
            }
        }

        private void OnBuildSoftEdgeMesh(ref List<Vector3> vertices, ref List<int> triangles, float refUnit)
        {
            vertices.Add(new Vector3(0, 0, 0));
            vertices.Add(new Vector3(refUnit, refUnit, 0));
            vertices.Add(new Vector3(refUnit, 0, refUnit));
            vertices.Add(new Vector3(0, refUnit, refUnit));

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                AddMeshTriangleThreeVertexLinked(ref triangles, 0, 1, 2);
                AddMeshTriangleThreeVertexLinked(ref triangles, 0, 3, 1);
                AddMeshTriangleThreeVertexLinked(ref triangles, 2, 3, 0);
                AddMeshTriangleThreeVertexLinked(ref triangles, 2, 1, 3);
            }
            if (drawSecondFace)
            {
                AddMeshTriangleThreeVertexLinked(ref triangles, 0, 2, 1);
                AddMeshTriangleThreeVertexLinked(ref triangles, 0, 1, 3);
                AddMeshTriangleThreeVertexLinked(ref triangles, 2, 0, 3);
                AddMeshTriangleThreeVertexLinked(ref triangles, 2, 3, 1);
            }
        }

    }
}
