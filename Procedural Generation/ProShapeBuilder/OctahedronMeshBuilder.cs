using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    [HelpURL(URL.baseURL + "/tree/main/ProceduralGeneration/ProShapeBuilder/README.md")]
    [AddComponentMenu(NamespaceID.ProceduralGenerationPath + "/" + NamespaceID.ProShapeBuilder + "/Octahedron Mesh Builder")]
    public class OctahedronMeshBuilder : ShapeMeshBuilder
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
                refUnit = Mathf.Sqrt(0.5f) * 2;
            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesSqrtTwo || _baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusOneMeter)
                refUnit = 2;
            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesSqrtThree)
                refUnit = Mathf.Sqrt(1.5f) * 2;
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusHalfSqrtTwo)
                refUnit = Mathf.Sqrt(2);
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusHalfSqrtThree)
                refUnit = Mathf.Sqrt(3);


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
            float center = refUnit / 2f;

            Vector3 a = new Vector3(refUnit, center, center);
            Vector3 b = new Vector3(0, center, center);
            Vector3 c = new Vector3(center, refUnit, center);
            Vector3 d = new Vector3(center, 0, center);
            Vector3 e = new Vector3(center, center, refUnit);
            Vector3 f = new Vector3(center, center, 0);

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                CreateMeshTriangle(ref vertices, ref triangles, a, c, e);
                CreateMeshTriangle(ref vertices, ref triangles, e, c, b);
                CreateMeshTriangle(ref vertices, ref triangles, b, c, f);
                CreateMeshTriangle(ref vertices, ref triangles, f, c, a);

                CreateMeshTriangle(ref vertices, ref triangles, e, d, a);
                CreateMeshTriangle(ref vertices, ref triangles, b, d, e);
                CreateMeshTriangle(ref vertices, ref triangles, f, d, b);
                CreateMeshTriangle(ref vertices, ref triangles, a, d, f);
            }
            if (drawSecondFace)
            {
                CreateMeshTriangle(ref vertices, ref triangles, a, e, c);
                CreateMeshTriangle(ref vertices, ref triangles, e, b, c);
                CreateMeshTriangle(ref vertices, ref triangles, b, f, c);
                CreateMeshTriangle(ref vertices, ref triangles, f, a, c);

                CreateMeshTriangle(ref vertices, ref triangles, e, a, d);
                CreateMeshTriangle(ref vertices, ref triangles, b, e, d);
                CreateMeshTriangle(ref vertices, ref triangles, f, b, d);
                CreateMeshTriangle(ref vertices, ref triangles, a, f, d);
            }
        }

        private void OnBuildSoftEdgeMesh(ref List<Vector3> vertices, ref List<int> triangles, float refUnit)
        {
            int a = 0;
            int b = 1;
            int c = 2;
            int d = 3;
            int e = 4;
            int f = 5;

            float center = refUnit / 2f;

            vertices.Add(new Vector3(refUnit, center, center));
            vertices.Add(new Vector3(0, center, center));
            vertices.Add(new Vector3(center, refUnit, center));
            vertices.Add(new Vector3(center, 0, center));
            vertices.Add(new Vector3(center, center, refUnit));
            vertices.Add(new Vector3(center, center, 0));

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                AddMeshTriangleThreeVertexLinked(ref triangles, a, c, e);
                AddMeshTriangleThreeVertexLinked(ref triangles, e, c, b);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, c, f);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, c, a);

                AddMeshTriangleThreeVertexLinked(ref triangles, e, d, a);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, d, e);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, d, b);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, d, f);
            }
            if (drawSecondFace)
            {
                AddMeshTriangleThreeVertexLinked(ref triangles, a, e, c);
                AddMeshTriangleThreeVertexLinked(ref triangles, e, b, c);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, f, c);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, a, c);

                AddMeshTriangleThreeVertexLinked(ref triangles, e, a, d);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, e, d);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, b, d);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, f, d);
            }
        }
    }
}
