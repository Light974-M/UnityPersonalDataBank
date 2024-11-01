using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    [HelpURL(URL.baseURL + "/tree/main/ProceduralGeneration/ProShapeBuilder/README.md")]
    [AddComponentMenu(NamespaceID.ProceduralGenerationPath + "/" + NamespaceID.ProShapeBuilder + "/Icosahedron Mesh Builder")]
    public class IcosahedronMeshBuilder : ShapeMeshBuilder
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

            refUnit *= _scaleFactor;

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
            float upperValue = 0.809f * refUnit;
            float lowerValue = 0.191f * refUnit;
            float center = refUnit * 0.5f;

            Vector3 a = new Vector3(upperValue, refUnit, center);
            Vector3 b = new Vector3(lowerValue, refUnit, center);
            Vector3 c = new Vector3(upperValue, 0, center);
            Vector3 d = new Vector3(lowerValue, 0, center);
            Vector3 e = new Vector3(center, upperValue, refUnit);
            Vector3 f = new Vector3(center, lowerValue, refUnit);
            Vector3 g = new Vector3(center, upperValue, 0);
            Vector3 h = new Vector3(center, lowerValue, 0);
            Vector3 i = new Vector3(refUnit, center, upperValue);
            Vector3 j = new Vector3(0, center, upperValue);
            Vector3 k = new Vector3(refUnit, center, lowerValue);
            Vector3 l = new Vector3(0, center, lowerValue);

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                CreateMeshTriangle(ref vertices, ref triangles, a, b, e);
                CreateMeshTriangle(ref vertices, ref triangles, a, e, i);
                CreateMeshTriangle(ref vertices, ref triangles, a, i, k);
                CreateMeshTriangle(ref vertices, ref triangles, a, k, g);
                CreateMeshTriangle(ref vertices, ref triangles, a, g, b);
                CreateMeshTriangle(ref vertices, ref triangles, b, g, l);
                CreateMeshTriangle(ref vertices, ref triangles, b, l, j);
                CreateMeshTriangle(ref vertices, ref triangles, b, j, e);
                CreateMeshTriangle(ref vertices, ref triangles, c, f, d);
                CreateMeshTriangle(ref vertices, ref triangles, c, d, h);
                CreateMeshTriangle(ref vertices, ref triangles, c, h, k);
                CreateMeshTriangle(ref vertices, ref triangles, c, k, i);
                CreateMeshTriangle(ref vertices, ref triangles, c, i, f);
                CreateMeshTriangle(ref vertices, ref triangles, d, f, j);
                CreateMeshTriangle(ref vertices, ref triangles, d, j, l);
                CreateMeshTriangle(ref vertices, ref triangles, d, l, h);
                CreateMeshTriangle(ref vertices, ref triangles, l, g, h);
                CreateMeshTriangle(ref vertices, ref triangles, h, g, k);
                CreateMeshTriangle(ref vertices, ref triangles, f, i, e);
                CreateMeshTriangle(ref vertices, ref triangles, f, e, j);
            }
            if (drawSecondFace)
            {
                CreateMeshTriangle(ref vertices, ref triangles, a, e, b);
                CreateMeshTriangle(ref vertices, ref triangles, a, i, e);
                CreateMeshTriangle(ref vertices, ref triangles, a, k, i);
                CreateMeshTriangle(ref vertices, ref triangles, a, g, k);
                CreateMeshTriangle(ref vertices, ref triangles, a, b, g);
                CreateMeshTriangle(ref vertices, ref triangles, b, l, g);
                CreateMeshTriangle(ref vertices, ref triangles, b, j, l);
                CreateMeshTriangle(ref vertices, ref triangles, b, e, j);
                CreateMeshTriangle(ref vertices, ref triangles, c, d, f);
                CreateMeshTriangle(ref vertices, ref triangles, c, h, d);
                CreateMeshTriangle(ref vertices, ref triangles, c, k, h);
                CreateMeshTriangle(ref vertices, ref triangles, c, i, k);
                CreateMeshTriangle(ref vertices, ref triangles, c, f, i);
                CreateMeshTriangle(ref vertices, ref triangles, d, j, f);
                CreateMeshTriangle(ref vertices, ref triangles, d, l, j);
                CreateMeshTriangle(ref vertices, ref triangles, d, h, l);
                CreateMeshTriangle(ref vertices, ref triangles, l, h, g);
                CreateMeshTriangle(ref vertices, ref triangles, h, k, g);
                CreateMeshTriangle(ref vertices, ref triangles, f, e, i);
                CreateMeshTriangle(ref vertices, ref triangles, f, j, e);
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
            int g = 6;
            int h = 7;
            int i = 8;
            int j = 9;
            int k = 10;
            int l = 11;

            float upperValue = 0.809f * refUnit;
            float lowerValue = 0.191f * refUnit;
            float center = refUnit * 0.5f;

            vertices.Add(new Vector3(upperValue, refUnit, center));
            vertices.Add(new Vector3(lowerValue, refUnit, center));
            vertices.Add(new Vector3(upperValue, 0, center));
            vertices.Add(new Vector3(lowerValue, 0, center));
            vertices.Add(new Vector3(center, upperValue, refUnit));
            vertices.Add(new Vector3(center, lowerValue, refUnit));
            vertices.Add(new Vector3(center, upperValue, 0));
            vertices.Add(new Vector3(center, lowerValue, 0));
            vertices.Add(new Vector3(refUnit, center, upperValue));
            vertices.Add(new Vector3(0, center, upperValue));
            vertices.Add(new Vector3(refUnit, center, lowerValue));
            vertices.Add(new Vector3(0, center, lowerValue));

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                AddMeshTriangleThreeVertexLinked(ref triangles, a, b, e);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, e, i);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, i, k);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, k, g);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, g, b);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, g, l);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, l, j);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, j, e);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, f, d);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, d, h);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, h, k);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, k, i);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, i, f);
                AddMeshTriangleThreeVertexLinked(ref triangles, d, f, j);
                AddMeshTriangleThreeVertexLinked(ref triangles, d, j, l);
                AddMeshTriangleThreeVertexLinked(ref triangles, d, l, h);
                AddMeshTriangleThreeVertexLinked(ref triangles, l, g, h);
                AddMeshTriangleThreeVertexLinked(ref triangles, h, g, k);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, i, e);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, e, j);
            }
            if (drawSecondFace)
            {
                AddMeshTriangleThreeVertexLinked(ref triangles, a, e, b);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, i, e);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, k, i);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, g, k);
                AddMeshTriangleThreeVertexLinked(ref triangles, a, b, g);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, l, g);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, j, l);
                AddMeshTriangleThreeVertexLinked(ref triangles, b, e, j);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, d, f);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, h, d);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, k, h);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, i, k);
                AddMeshTriangleThreeVertexLinked(ref triangles, c, f, i);
                AddMeshTriangleThreeVertexLinked(ref triangles, d, j, f);
                AddMeshTriangleThreeVertexLinked(ref triangles, d, l, j);
                AddMeshTriangleThreeVertexLinked(ref triangles, d, h, l);
                AddMeshTriangleThreeVertexLinked(ref triangles, l, h, g);
                AddMeshTriangleThreeVertexLinked(ref triangles, h, k, g);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, e, i);
                AddMeshTriangleThreeVertexLinked(ref triangles, f, j, e);
            }
        }
    }
}

