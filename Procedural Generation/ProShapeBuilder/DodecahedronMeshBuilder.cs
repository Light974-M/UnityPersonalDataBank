using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    [HelpURL(URL.baseURL + "/tree/main/ProceduralGeneration/ProShapeBuilder/README.md")]
    [AddComponentMenu(NamespaceID.ProceduralGenerationPath + "/" + NamespaceID.ProShapeBuilder + "/Dodecahedron Mesh Builder")]
    public class DodecahedronMeshBuilder : ShapeMeshBuilder
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
            float firstCoord = 0.80901699f * refUnit;
            float secondCoord = 0.19098301f * refUnit;
            float thirdCoord = 0.69098301f * refUnit;
            float fourthCoord = 0.30901699f * refUnit;
            float center = refUnit * 0.5f;

            Vector3 a = new Vector3(firstCoord, firstCoord, firstCoord);
            Vector3 b = new Vector3(firstCoord, firstCoord, secondCoord);
            Vector3 c = new Vector3(firstCoord, secondCoord, firstCoord);
            Vector3 d = new Vector3(firstCoord, secondCoord, secondCoord);
            Vector3 e = new Vector3(secondCoord, firstCoord, firstCoord);
            Vector3 f = new Vector3(secondCoord, firstCoord, secondCoord);
            Vector3 g = new Vector3(secondCoord, secondCoord, firstCoord);
            Vector3 h = new Vector3(secondCoord, secondCoord, secondCoord);
            Vector3 i = new Vector3(center, thirdCoord, refUnit);
            Vector3 j = new Vector3(center, thirdCoord, 0);
            Vector3 k = new Vector3(center, fourthCoord, refUnit);
            Vector3 l = new Vector3(center, fourthCoord, 0);
            Vector3 m = new Vector3(thirdCoord, refUnit, center);
            Vector3 n = new Vector3(thirdCoord, 0, center);
            Vector3 o = new Vector3(fourthCoord, refUnit, center);
            Vector3 p = new Vector3(fourthCoord, 0, center);
            Vector3 q = new Vector3(refUnit, center, thirdCoord);
            Vector3 r = new Vector3(refUnit, center, fourthCoord);
            Vector3 s = new Vector3(0, center, thirdCoord);
            Vector3 t = new Vector3(0, center, fourthCoord);

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                CreatePentagone(ref vertices, ref triangles, a, m, o, e, i);
                CreatePentagone(ref vertices, ref triangles, a, i, k, c, q);
                CreatePentagone(ref vertices, ref triangles, a, q, r, b, m);
                CreatePentagone(ref vertices, ref triangles, b, r, d, l, j);
                CreatePentagone(ref vertices, ref triangles, b, j, f, o, m);
                CreatePentagone(ref vertices, ref triangles, c, k, g, p, n);
                CreatePentagone(ref vertices, ref triangles, c, n, d, r, q);
                CreatePentagone(ref vertices, ref triangles, d, n, p, h, l);
                CreatePentagone(ref vertices, ref triangles, e, o, f, t, s);
                CreatePentagone(ref vertices, ref triangles, e, s, g, k, i);
                CreatePentagone(ref vertices, ref triangles, f, j, l, h, t);
                CreatePentagone(ref vertices, ref triangles, s, t, h, p, g);
            }
            if (drawSecondFace)
            {
                CreatePentagone(ref vertices, ref triangles, a, i, e, o, m);
                CreatePentagone(ref vertices, ref triangles, a, q, c, k, i);
                CreatePentagone(ref vertices, ref triangles, a, m, b, r, q);
                CreatePentagone(ref vertices, ref triangles, b, j, l, d, r);
                CreatePentagone(ref vertices, ref triangles, b, m, o, f, j);
                CreatePentagone(ref vertices, ref triangles, c, n, p, g, k);
                CreatePentagone(ref vertices, ref triangles, c, q, r, d, n);
                CreatePentagone(ref vertices, ref triangles, d, l, h, p, n);
                CreatePentagone(ref vertices, ref triangles, e, s, t, f, o);
                CreatePentagone(ref vertices, ref triangles, e, i, k, g, s);
                CreatePentagone(ref vertices, ref triangles, f, t, h, l, j);
                CreatePentagone(ref vertices, ref triangles, s, g, p, h, t);
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
            int m = 12;
            int n = 13;
            int o = 14;
            int p = 15;
            int q = 16;
            int r = 17;
            int s = 18;
            int t = 19;

            float firstCoord = 0.80901699f * refUnit;
            float secondCoord = 0.19098301f * refUnit;
            float thirdCoord = 0.69098301f * refUnit;
            float fourthCoord = 0.30901699f * refUnit;
            float center = refUnit * 0.5f;

            vertices.Add(new Vector3(firstCoord, firstCoord, firstCoord));
            vertices.Add(new Vector3(firstCoord, firstCoord, secondCoord));
            vertices.Add(new Vector3(firstCoord, secondCoord, firstCoord));
            vertices.Add(new Vector3(firstCoord, secondCoord, secondCoord));
            vertices.Add(new Vector3(secondCoord, firstCoord, firstCoord));
            vertices.Add(new Vector3(secondCoord, firstCoord, secondCoord));
            vertices.Add(new Vector3(secondCoord, secondCoord, firstCoord));
            vertices.Add(new Vector3(secondCoord, secondCoord, secondCoord));
            vertices.Add(new Vector3(center, thirdCoord, refUnit));
            vertices.Add(new Vector3(center, thirdCoord, 0));
            vertices.Add(new Vector3(center, fourthCoord, refUnit));
            vertices.Add(new Vector3(center, fourthCoord, 0));
            vertices.Add(new Vector3(thirdCoord, refUnit, center));
            vertices.Add(new Vector3(thirdCoord, 0, center));
            vertices.Add(new Vector3(fourthCoord, refUnit, center));
            vertices.Add(new Vector3(fourthCoord, 0, center));
            vertices.Add(new Vector3(refUnit, center, thirdCoord));
            vertices.Add(new Vector3(refUnit, center, fourthCoord));
            vertices.Add(new Vector3(0, center, thirdCoord));
            vertices.Add(new Vector3(0, center, fourthCoord));

            bool drawFirstFace = false;
            bool drawSecondFace = false;

            if (!_flipNormals || _doubleSidedMesh)
                drawFirstFace = true;

            if (_flipNormals || _doubleSidedMesh)
                drawSecondFace = true;

            if (drawFirstFace)
            {
                AddPentagoneFiveVerticesLinked(ref triangles, a, m, o, e, i);
                AddPentagoneFiveVerticesLinked(ref triangles, a, i, k, c, q);
                AddPentagoneFiveVerticesLinked(ref triangles, a, q, r, b, m);
                AddPentagoneFiveVerticesLinked(ref triangles, b, r, d, l, j);
                AddPentagoneFiveVerticesLinked(ref triangles, b, j, f, o, m);
                AddPentagoneFiveVerticesLinked(ref triangles, c, k, g, p, n);
                AddPentagoneFiveVerticesLinked(ref triangles, c, n, d, r, q);
                AddPentagoneFiveVerticesLinked(ref triangles, d, n, p, h, l);
                AddPentagoneFiveVerticesLinked(ref triangles, e, o, f, t, s);
                AddPentagoneFiveVerticesLinked(ref triangles, e, s, g, k, i);
                AddPentagoneFiveVerticesLinked(ref triangles, f, j, l, h, t);
                AddPentagoneFiveVerticesLinked(ref triangles, s, t, h, p, g);
            }
            if (drawSecondFace)
            {
                AddPentagoneFiveVerticesLinked(ref triangles, a, i, e, o, m);
                AddPentagoneFiveVerticesLinked(ref triangles, a, q, c, k, i);
                AddPentagoneFiveVerticesLinked(ref triangles, a, m, b, r, q);
                AddPentagoneFiveVerticesLinked(ref triangles, b, j, l, d, r);
                AddPentagoneFiveVerticesLinked(ref triangles, b, m, o, f, j);
                AddPentagoneFiveVerticesLinked(ref triangles, c, n, p, g, k);
                AddPentagoneFiveVerticesLinked(ref triangles, c, q, r, d, n);
                AddPentagoneFiveVerticesLinked(ref triangles, d, l, h, p, n);
                AddPentagoneFiveVerticesLinked(ref triangles, e, s, t, f, o);
                AddPentagoneFiveVerticesLinked(ref triangles, e, i, k, g, s);
                AddPentagoneFiveVerticesLinked(ref triangles, f, t, h, l, j);
                AddPentagoneFiveVerticesLinked(ref triangles, s, g, p, h, t);
            }
        }
    }
}


