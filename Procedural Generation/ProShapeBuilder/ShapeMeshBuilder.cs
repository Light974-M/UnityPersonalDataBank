using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    public abstract class ShapeMeshBuilder : UPDBBehaviour
    {
        [SerializeField, Tooltip("set default size of mesh")]
        protected float _scaleFactor = 1;

        protected MeshFilter _meshFilter;
        protected MeshRenderer _meshRenderer;
        protected Mesh _mesh;

        protected Vector3 _relativeCenterPos = Vector3.zero;


        #region Public API

        public Mesh GeneratedMesh
        {
            get
            {
                if (!_mesh)
                {
                    _mesh = new Mesh();
                    _mesh.name = "Generated Mesh";
                }

                return _mesh;
            }
        }

        public float ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
        }

        #endregion

        /*****************************BUILT IN METHODS*****************************/

        private void OnEnable()
        {
            Init();
        }

        /*****************************CUSTOM METHODS*****************************/

        public virtual void Init()
        {
            if (_meshFilter == null)
                if (!TryGetComponent(out _meshFilter))
                    _meshFilter = gameObject.AddComponent<MeshFilter>();

            if (_meshRenderer == null)
                if (!TryGetComponent(out _meshRenderer))
                    _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            if (_meshFilter.sharedMesh != GeneratedMesh)
                _meshFilter.sharedMesh = GeneratedMesh;

            if (_meshRenderer.sharedMaterial == null)
            {
                Shader defaultLitShader = Shader.Find("Universal Render Pipeline/Lit");
                _meshRenderer.sharedMaterial = new Material(defaultLitShader);
            }
        }

        public virtual void ClearAll()
        {
            GeneratedMesh.Clear();
            _mesh = null;
        }

        public virtual void BuildMesh()
        {
            Init();

            GeneratedMesh.Clear();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            OnBuildTrianglesAndVertices(ref vertices, ref triangles);

            GeneratedMesh.vertices = vertices.ToArray();
            GeneratedMesh.triangles = triangles.ToArray();
            GeneratedMesh.CalculateNormals();
        }

        protected virtual void CalculateCenterOfMesh(List<Vector3> vertices)
        {
            _relativeCenterPos = Vector3.zero;

            foreach (Vector3 vertex in vertices)
            {
                _relativeCenterPos += vertex;
            }

            _relativeCenterPos /= vertices.Count;
        }

        protected virtual void OnBuildTrianglesAndVertices(ref List<Vector3> vertices, ref List<int> triangles)
        {

        }

        public int GetIndexWithCoords2D(int x, int y, int width)
        {
            return x + (width * y);
        }
    }

    public enum BaseScaleUnitOfSolid
    {
        DefaultFitInOneMeterCube,
        EdgesOneMeter,
        EdgesSqrtTwo,
        EdgesSqrtThree,
        BiggestRadiusHalfMeter,
        BiggestRadiusHalfSqrtTwo,
        BiggestRadiusHalfSqrtThree,
        BiggestRadiusOneMeter,
    }
}
