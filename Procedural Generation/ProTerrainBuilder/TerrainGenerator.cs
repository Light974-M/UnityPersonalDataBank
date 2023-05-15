using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //SERIALIZED VARIABLES___________________________________________________________________________________________________

    [Header("MESH TRANSFORM")]

    [SerializeField, Tooltip("mesh scale")]
    private Vector3 _scale = Vector3.one;
    public Vector3 Scale
    {
        get {return _scale; }
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
    private int _xSize = 20;
    public int XSize
    {
        get { return _xSize; }
        set { _xSize = value; }
    }

    [SerializeField, Tooltip("number of vertices in z")]
    private int _zSize = 20;
    public int ZSize
    {
        get { return _zSize; }
        set { _zSize = value; }
    }

    [SerializeField, Tooltip("does it resize noise scale while resize x and z values")]
    private bool _resizeAndNoiseScale = false;
    public bool ResizeAndNoiseScale
    {
        get { return _resizeAndNoiseScale; }
        set { _resizeAndNoiseScale = value; }
    }

    [SerializeField, Tooltip("is current object a static mesh or procedurally animated ?")]
    private bool _enableProceduralAnimations = false;
    public bool EnableProceduralAnimations
    {
        get { return _enableProceduralAnimations; }
        set { _enableProceduralAnimations = value; }
    }

    [SerializeField, Tooltip("is game generate height color")]
    private bool _heightColor = false;
    public bool HeightColor
    {
        get { return _heightColor; }
        set { _heightColor = value; }
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

    [SerializeField, Tooltip("should game generated noise every frame")]
    private bool _noiseEveryFrame = false;
    public bool NoiseEveryFrame
    {
        get { return _noiseEveryFrame; }
        set { _noiseEveryFrame = value; }
    }

    [SerializeField, Tooltip("scale of noise")]
    private Vector2 _noiseScale = Vector3.one;
    public Vector2 NoiseScale
    {
        get { return _noiseScale; }
        set { _noiseScale = value; }
    }

    [SerializeField, Tooltip("scale of noise")]
    private Vector2 _noiseOffset = Vector3.one;
    public Vector2 NoiseOffset
    {
        get { return _noiseOffset; }
        set { _noiseOffset = value; }
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
    int[] triangles;
    Color[] colors;
    float yOffset = 0;

    private void Awake()
    {
        LoadElements();
    }

    private void Update()
    {
        if(_enableProceduralAnimations)
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

        if(meshRenderer.material == null)
            meshRenderer.material = material;

        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    public void CreateShape()
    {
        vertices = new Vector3[(_xSize + 1) * (_zSize + 1)];

        for(int i = 0, z = 0; z <= _zSize; z++)
        {
            for (int x = 0; x <= _xSize; x++)
            {
                yOffset = 0;
                vertices[i] = new Vector3((x) * _scale.x, yOffset * _scale.y, (z) * _scale.z);
                i++;
            }
        }

        CreateTriangles();
    }

    public void CreateTriangles()
    {
        triangles = new int[_xSize * _zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < _zSize; z++)
        {
            for (int x = 0; x < _xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + _xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + _xSize + 1;
                triangles[tris + 5] = vert + _xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }
    }

    public void CreatePerlinNoise()
    {
        for (int i = 0, z = (int)_noiseOffset.y; z <= _zSize + (int)_noiseOffset.y; z++)
        {
            for (int x = (int)_noiseOffset.x; x <= _xSize + (int)_noiseOffset.x; x++)
            {
                yOffset = 0;
                float xCoord = (x / (float)_xSize) * _noiseScale.x;
                float zCoord = (z / (float)_zSize) * _noiseScale.y;

                float sample = Mathf.PerlinNoise(xCoord, zCoord);

                vertices[i] += new Vector3(0, sample * _scale.y, 0);
                i++;
            }
        }

        if (_heightColor)
            HeightColorize();
    }

    private void OnDrawGizmos()
    {

        if (_drawVerticesGizmo)
        {
            if (vertices == null)
                return;

            for (int i = 0; i < vertices.Length; i++)
            {
                if(_fixedGizmos)
                {
                    Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), _constantGizmoSize);
                }
                else
                {
                    float gizmoDist = Vector3.Distance(Camera.current.transform.position, transform.TransformPoint(vertices[i]));
                    Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), gizmoDist/100 * _gizmoSize);
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
        _xSize = 1;
        _zSize = 1;
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

    public void HeightColorize()
    {
        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Color.black;
        }
    }
}
