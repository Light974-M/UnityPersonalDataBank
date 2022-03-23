using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
    /// <summary>
    /// procedural generation for custom shapes
    /// </summary>
    [AddComponentMenu("UPDB/ProceduralGeneration/ProShapeBuilder/Shape Generator")]
    public class ShapeGenerator : MonoBehaviour
    {
        //SERIALIZED VARIABLES___________________________________________________________________________________________________

        [Header("MESH TRANSFORM")]

        [SerializeField, Tooltip("mesh scale")]
        private Vector3 _scale = Vector3.one;
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }


        [Header("OBJ AFFECTATIONS")]

        [SerializeField, Tooltip("mesh filter used to filter generated mesh")]
        private MeshFilter meshFilter;

        [SerializeField, Tooltip("mesh filter used to render generated mesh")]
        private MeshRenderer meshRenderer;

        [SerializeField, Tooltip("mesh material")]
        private Material material;


        [Header("TERRAIN PARAMETERS")]

        [SerializeField, Tooltip("number of vertices in x")]
        private int _verticesNumber = 20;
        public int VerticesNumber
        {
            get { return _verticesNumber; }
            set { _verticesNumber = value; }
        }

        [SerializeField, Tooltip("is current object a static mesh or procedurally animated ?")]
        private bool _enableProceduralAnimations = false;
        public bool EnableProceduralAnimations
        {
            get { return _enableProceduralAnimations; }
            set { _enableProceduralAnimations = value; }
        }

        [Header("GIZMOS")]

        [SerializeField, Tooltip("does editor draw vertices")]
        private bool _drawVerticesGizmo = true;
        public bool DrawVerticesGizmo
        {
            get { return _drawVerticesGizmo; }
            set { _drawVerticesGizmo = value; }
        }

        [SerializeField, Tooltip("size of vertex gizmos"), Range(0, 5)]
        private float _gizmoSize = 1;
        public float GizmoSize
        {
            get { return _gizmoSize; }
            set { _gizmoSize = value; }
        }

        [SerializeField, Tooltip("does gizmo size follow camera distance")]
        private bool _fixedGizmos = false;
        public bool FixedGizmos
        {
            get { return _fixedGizmos; }
            set { _fixedGizmos = value; }
        }

        [SerializeField, Tooltip("size of vertex gizmos constantly trough camera")]
        private float _constantGizmoSize = 1;
        public float ConstantGizmoSize
        {
            get { return _constantGizmoSize; }
            set { _constantGizmoSize = value; }
        }


        [Header("CUSTOM INSPECTOR PARAMETERS")]

        [SerializeField, Tooltip("should game generated a new shape every frame")]
        private bool _generateEveryFrame = false;
        public bool GenerateEveryFrame
        {
            get { return _generateEveryFrame; }
            set { _generateEveryFrame = value; }
        }

        [SerializeField, Tooltip("access to root variables")]
        private bool _drawDefaultInspector = false;
        public bool DrawDefaultInspector
        {
            get { return _drawDefaultInspector; }
            set { _drawDefaultInspector = value; }
        }

        //PRIVATE VARIABLES_______________________________________________________________________________________________________

        Mesh mesh;

        Vector3[] vertices;
        [SerializeField]
        int[] triangles;
        Color[] colors;

        private int trianglesListStartPos = 0;

        private void Awake()
        {
            LoadElements();
        }

        private void Update()
        {
            if (_enableProceduralAnimations)
            {
                CreateShape();
                UpdateMesh();
            }
        }

        public void Clean()
        {
            meshFilter = null;
            meshRenderer = null;
            material = null;
            mesh = null;
            DestroyImmediate(GetComponent<MeshFilter>());
            DestroyImmediate(GetComponent<MeshRenderer>());
        }

        public void LoadElements()
        {
            if (meshFilter == null)
                if (!TryGetComponent(out meshFilter))
                    meshFilter = gameObject.AddComponent<MeshFilter>();

            if (meshRenderer == null)
                if (!TryGetComponent(out meshRenderer))
                    meshRenderer = gameObject.AddComponent<MeshRenderer>();

            if (material == null)
                material = new Material(Shader.Find("Standard"));

            //if (meshRenderer.material == null)
            //    meshRenderer.material = material;

            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        public void CreateShape()
        {
            int surface = (_verticesNumber + 1) * (_verticesNumber + 1);
            int perimeter = (4 * (_verticesNumber + 1)) - 4;

            int verticesCount = (2 * surface) + ((_verticesNumber - 1) * perimeter);

            vertices = new Vector3[verticesCount];

            CreatePerimeterVertices(CreateFaceVertices(_verticesNumber, CreateFaceVertices(0, 0)));

            CreateTriangles();
        }

        public int CreateFaceVertices(float _height, int listStartPos)
        {
            int i = listStartPos;

            for (int z = 0; z <= _verticesNumber; z++)
            {
                for (int x = 0; x <= _verticesNumber; x++)
                {
                    vertices[i] = new Vector3((x) * _scale.x, _height * _scale.y, (z) * _scale.z);
                    i++;
                }
            }

            return i;
        }

        public void CreatePerimeterVertices(int listStartPos)
        {
            int perimeter = (4 * (_verticesNumber + 1)) - 4;
            int i = listStartPos;

            for (int y = 1; y < _verticesNumber; y++)
            {
                for (int xy = 0; xy < perimeter; xy++)
                {
                    int x = 0;
                    int z = 0;

                    if (xy < (perimeter / 4))
                    {
                        x = xy;
                    }
                    else if (xy < perimeter / 2)
                    {
                        x = _verticesNumber;
                        z = xy - (perimeter / 4);
                    }
                    else if (xy < (perimeter / 4) * 3)
                    {
                        x = _verticesNumber - (xy - (perimeter / 2));
                        z = _verticesNumber;
                    }
                    else if (xy < perimeter)
                    {
                        z = _verticesNumber - (xy - ((perimeter / 4) * 3));
                    }

                    vertices[i] = new Vector3(x * _scale.x, y * _scale.y, z * _scale.z);
                    i++;
                }
            }
        }

        public void CreateTriangles()
        {
            int surface = (_verticesNumber + 1) * (_verticesNumber + 1);
            int perimeter = (4 * (_verticesNumber + 1)) - 4;

            int verticesCount = (2 * surface) + ((_verticesNumber - 1) * perimeter);

            triangles = new int[verticesCount * 6];

            trianglesListStartPos = 0;
            CreatePerimeterTriangles(CreateFaceTriangles(CreateFaceTriangles(0, 0, true) + _verticesNumber + 1, trianglesListStartPos, false) + _verticesNumber, trianglesListStartPos);
        }

        public int CreateFaceTriangles(int _verticesListStartPos, int _trianglesListStartPos, bool isReversed)
        {
            int vert = _verticesListStartPos;
            int tris = _trianglesListStartPos;

            for (int z = 0; z < _verticesNumber; z++)
            {
                for (int x = 0; x < _verticesNumber; x++)
                {
                    if (isReversed)
                    {
                        triangles[tris + 0] = vert + 0;
                        triangles[tris + 1] = vert + 1;
                        triangles[tris + 2] = vert + _verticesNumber + 1;
                        triangles[tris + 3] = vert + 1;
                        triangles[tris + 4] = vert + _verticesNumber + 2;
                        triangles[tris + 5] = vert + _verticesNumber + 1;
                    }
                    else
                    {
                        triangles[tris + 0] = vert + 0;
                        triangles[tris + 1] = vert + _verticesNumber + 1;
                        triangles[tris + 2] = vert + 1;
                        triangles[tris + 3] = vert + 1;
                        triangles[tris + 4] = vert + _verticesNumber + 1;
                        triangles[tris + 5] = vert + _verticesNumber + 2;
                    }

                    vert++;
                    tris += 6;
                }

                vert++;
            }

            trianglesListStartPos = tris;
            return vert;
        }

        public void CreatePerimeterTriangles(int _verticesListStartPos, int _trianglesListStartPos)
        {
            int vert = _verticesListStartPos;
            int tris = _trianglesListStartPos;

            int trisIndex = 0;
            for (int y = 0; y < _verticesNumber; y++)
            {
                for (int xz = 0; xz < _verticesNumber * 4; xz++)
                {
                    //int vertIndex = 0;
                    //int triIndex = 0;
                    if (_verticesNumber == 1)
                    {
                        if (xz == 0)
                        {
                            triangles[tris + trisIndex] = 0;
                            triangles[tris + trisIndex + 1] = 4;
                            triangles[tris + trisIndex + 2] = 1;
                            triangles[tris + trisIndex + 3] = 1;
                            triangles[tris + trisIndex + 4] = 4;
                            triangles[tris + trisIndex + 5] = 5;
                        }
                        else if (xz == 1)
                        {
                            triangles[tris + trisIndex] = 1;
                            triangles[tris + trisIndex + 1] = 5;
                            triangles[tris + trisIndex + 2] = 3;
                            triangles[tris + trisIndex + 3] = 3;
                            triangles[tris + trisIndex + 4] = 5;
                            triangles[tris + trisIndex + 5] = 7;
                        }
                        else if (xz == 2)
                        {
                            triangles[tris + trisIndex] = 3;
                            triangles[tris + trisIndex + 1] = 7;
                            triangles[tris + trisIndex + 2] = 2;
                            triangles[tris + trisIndex + 3] = 2;
                            triangles[tris + trisIndex + 4] = 7;
                            triangles[tris + trisIndex + 5] = 6;
                        }
                        else if (xz == 3)
                        {
                            triangles[tris + trisIndex] = 2;
                            triangles[tris + trisIndex + 1] = 6;
                            triangles[tris + trisIndex + 2] = 0;
                            triangles[tris + trisIndex + 3] = 0;
                            triangles[tris + trisIndex + 4] = 6;
                            triangles[tris + trisIndex + 5] = 4;
                        }
                    }
                    else
                    {
                        if (y == 0)
                        {
                            if (xz < _verticesNumber)
                            {
                                triangles[tris + trisIndex] = xz;
                                triangles[tris + trisIndex + 1] = vert + xz + 1;
                                triangles[tris + trisIndex + 2] = xz + 1;
                                triangles[tris + trisIndex + 3] = xz + 1;
                                triangles[tris + trisIndex + 4] = vert + xz + 1;
                                triangles[tris + trisIndex + 5] = vert + xz + 2;
                            }
                            else if (xz < _verticesNumber * 2)
                            {
                                triangles[tris + trisIndex] = ((_verticesNumber) * ((xz - _verticesNumber) + 1)) + (xz - _verticesNumber);
                                triangles[tris + trisIndex + 1] = vert + 1 + _verticesNumber + (xz - _verticesNumber);
                                triangles[tris + trisIndex + 2] = ((_verticesNumber) * ((xz - _verticesNumber) + 2)) + (xz - _verticesNumber + 1);
                                triangles[tris + trisIndex + 3] = ((_verticesNumber) * ((xz - _verticesNumber) + 2)) + (xz - _verticesNumber + 1);
                                triangles[tris + trisIndex + 4] = vert + 1 + _verticesNumber + (xz - _verticesNumber);
                                triangles[tris + trisIndex + 5] = vert + 1 + _verticesNumber + (xz - _verticesNumber + 1);
                            }
                            else if (xz < _verticesNumber * 3)
                            {
                                //Debug.Log(((_verticesNumber + 1) * (_verticesNumber + 1) - 1) - (xz - (_verticesNumber * 2) + 1));
                                triangles[tris + trisIndex] = ((_verticesNumber + 1) * (_verticesNumber + 1) - 1) - (xz - (_verticesNumber * 2));
                                triangles[tris + trisIndex + 1] = vert + 1 + (_verticesNumber * 2) + (xz - _verticesNumber * 2);
                                triangles[tris + trisIndex + 2] = ((_verticesNumber + 1) * (_verticesNumber + 1) - 1) - (xz - (_verticesNumber * 2) + 1);
                                //triangles[tris + trisIndex + 3] = ((_verticesNumber) * ((xz - _verticesNumber) + 2)) + (xz - _verticesNumber + 1);
                                //triangles[tris + trisIndex + 4] = vert + 1 + _verticesNumber + (xz - _verticesNumber);
                                //triangles[tris + trisIndex + 5] = vert + 1 + _verticesNumber + (xz - _verticesNumber + 1);
                            }
                            else
                            {
                                Debug.Log("fourth");
                            }
                        }
                        else if (y == _verticesNumber - 1)
                        {
                            Debug.Log("last");
                        }
                        else
                        {
                            Debug.Log("middle");
                        }
                    }

                    trisIndex += 6;
                }
            }
        }

        private void OnDrawGizmos()
        {

            if (_drawVerticesGizmo)
            {
                if (vertices == null)
                    return;

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (_fixedGizmos)
                    {
                        Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), _constantGizmoSize);
                    }
                    else
                    {
                        float gizmoDist = Vector3.Distance(Camera.current.transform.position, transform.TransformPoint(vertices[i]));
                        Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), gizmoDist / 100 * _gizmoSize);
                    }
                }
            }
        }

        public void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
        }

        public void ClearMesh()
        {
            mesh.Clear();
        }

        public void ClearVerticesAndTriangles()
        {
            vertices = null;
            triangles = null;
        }

        public void ClearAll()
        {
            _verticesNumber = 1;
            _scale = Vector3.one;
            _generateEveryFrame = false;
            _enableProceduralAnimations = true;
            _drawVerticesGizmo = true;
            _gizmoSize = 1;
            _fixedGizmos = false;
            _constantGizmoSize = 0.5f;

            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;

            ClearVerticesAndTriangles();
            Clean();
            UpdateMesh();
        }
    } 
}
