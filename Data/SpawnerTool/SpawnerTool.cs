using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.Data.CustomTransform;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Data.UPDBSpawner
{
    [AddComponentMenu(NamespaceID.DataPath + "/" + NamespaceID.SpawnerTool + "/Spawner")]
    public class SpawnerTool : UPDBBehaviour
    {
        #region Serialized API

        //[Header("BASE SPAWN PARAMETERS")]
        [SerializeField, Tooltip("object to make spawn")]
        private List<GameObject> _objectsToSpawnArray;

        [SerializeField, Tooltip("does spawner spawn a first object on awake ?")]
        private bool _spawnOnAwake = false;

        [SerializeField, Tooltip("one generation include how many objects ? random range")]
        private Vector2Int _spawnNumberRange = Vector2Int.one;

        [SerializeField, Tooltip("range of time where to instantiate it")]
        private Vector2 _spawnRate = Vector2.one;

        [SerializeField, Tooltip("wich direction of spawned object is rotated ?")]
        private RotationDirection _objectsRotationDirection;

        [SerializeField, Tooltip("wich direction spawned objects are rotated towards")]
        private RotationDirection _spawnerRotationDirection;

        [SerializeField, Tooltip("min rotation range for offset rotation randomization")]
        private Vector3 _minRotationOffset;

        [SerializeField, Tooltip("max rotation range for offset rotation randomization")]
        private Vector3 _maxRotationOffset;

        [Header("SPAWNER SHAPE PARAMETERS")]
        [SerializeField, Tooltip("shape of spawner")]
        private Shape _spawnerShape;

        [SerializeField, Tooltip("all datas about position rotation, and scale offsets of spawner")]
        private CustomTransformManager _spawnerOffsetTransform;

        [SerializeField, Tooltip("list of vertice for custom shape")]
        private Vector3[] _customShapeVertice;

        [SerializeField, Tooltip("list of edges for custom shape")]
        private Vector2Int[] _customShapeEdges;

        #endregion

        #region Private API

        private float _spawnTimer = 0f;
        private float _spawnTime = 0f;
        private bool _eventSubscribeTrigger = true;
        private int _spawnNumber = 1;

        #endregion

        #region Public API

        public bool SpawnOnAwake
        {
            get => _spawnOnAwake;
            set => _spawnOnAwake = value;
        }
        public Vector2Int SpawnNumberRange
        {
            get => _spawnNumberRange;
            set => _spawnNumberRange = value;
        }
        public Vector2 SpawnRate
        {
            get => _spawnRate;
            set => _spawnRate = value;
        }
        public RotationDirection ObjectsRotationDirection
        {
            get => _objectsRotationDirection;
            set => _objectsRotationDirection = value;
        }
        public RotationDirection SpawnerRotationDirection
        {
            get => _spawnerRotationDirection;
            set => _spawnerRotationDirection = value;
        }
        public Vector3 MinRotationOffset
        {
            get => _minRotationOffset;
            set => _minRotationOffset = value;
        }
        public Vector3 MaxRotationOffset
        {
            get => _maxRotationOffset;
            set => _maxRotationOffset = value;
        }
        public Shape SpawnerShape
        {
            get => _spawnerShape;
            set => _spawnerShape = value;
        }
        public CustomTransformManager SpawnerOffsetTransform
        {
            get
            {
                MakeNonNullable(ref _spawnerOffsetTransform, gameObject);

                if (_spawnerShape == Shape.Sphere)
                {
                    if (_spawnerOffsetTransform.LocalScale.x != 0 && _spawnerOffsetTransform.LocalScale.y != 0 && _spawnerOffsetTransform.LocalScale.z != 0 && !(_spawnerOffsetTransform.LocalScale.x == _spawnerOffsetTransform.LocalScale.y && _spawnerOffsetTransform.LocalScale.x == _spawnerOffsetTransform.LocalScale.z))
                    {
                        float factor = (_spawnerOffsetTransform.LocalScale.x + _spawnerOffsetTransform.LocalScale.y + _spawnerOffsetTransform.LocalScale.z) / 3;
                        _spawnerOffsetTransform.LastRegisteredproportions = Vector3.one * factor;
                    }

                    _spawnerOffsetTransform.ProportionsConstraint = true;
                }

                return _spawnerOffsetTransform;
            }

            set
            {
                _spawnerOffsetTransform = value;
            }
        }
        public bool EventSubcribeTrigger
        {
            get => _eventSubscribeTrigger;
            set => _eventSubscribeTrigger = value;
        }

        #endregion

        /*************************************BUILT-IN METHODS****************************************/

        private void Awake()
        {
            _spawnTime = GenerateRandomTime();
            _spawnNumber = Random.Range(_spawnNumberRange.x, _spawnNumberRange.y + 1);
        }

        private void Start()
        {
            if (_spawnOnAwake)
                OnSpawnObjects();
        }

        private void Update()
        {
            if (_spawnTimer.TestAndReset(_spawnTimer >= _spawnTime))
                OnSpawnObjects();

            _spawnTimer += Time.deltaTime;
        }

        /*************************************UPDB BUILT-IN METHODS****************************************/

        protected override void OnSceneSelected()
        {
            if (_eventSubscribeTrigger)
            {
                _eventSubscribeTrigger = false;

                SpawnerOffsetTransform.OnChange -= SceneViewUpdate;
                SpawnerOffsetTransform.OnChange += SceneViewUpdate;
            }

            DrawSpawnerShape();
        }

        /*************************************CUSTOM METHODS****************************************/

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void OnProjectLoad()
        {
            SpawnerTool[] spawners = FindObjectsOfType<SpawnerTool>();

            for (int i = 0; i < spawners.Length; i++)
                spawners[i].EventSubcribeTrigger = true;
        }
#endif

        public void DrawSpawnerShape()
        {
#if UNITY_EDITOR
            if (_spawnerShape == Shape.Box)
            {
                DebugDrawCube(SpawnerOffsetTransform.Position, SpawnerOffsetTransform.Scale, SpawnerOffsetTransform.Right, SpawnerOffsetTransform.Up, SpawnerOffsetTransform.Forward, Color.blue);
                return;
            }
            if (_spawnerShape == Shape.Sphere)
            {
                Color GizmoColor = Gizmos.color;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(SpawnerOffsetTransform.Position, SpawnerOffsetTransform.LocalScale.x / 2);
                Gizmos.color = GizmoColor;

                return;
            }
            if(_spawnerShape == Shape.FreeShape)
            {
                DebugDrawWireShape(_customShapeVertice, _customShapeEdges, _spawnerOffsetTransform.Position, _spawnerOffsetTransform.Scale, _spawnerOffsetTransform.Right, _spawnerOffsetTransform.Up, _spawnerOffsetTransform.Forward, Color.blue);

                //Vector3[][] tetrahedrons = GenerateTetrahedrons(_customShapeVertice, _customShapeEdges);
                //List<Vector3> vertice = new List<Vector3>();
                //List<Vector2Int> edges = new List<Vector2Int>();

                //for (int i = 0; i < tetrahedrons.Length; i++)
                //{
                //    vertice.Add(tetrahedrons[i][0]);
                //    int aIndex = vertice.Count - 1;
                //    vertice.Add(tetrahedrons[i][1]);
                //    int bIndex = vertice.Count - 1;
                //    vertice.Add(tetrahedrons[i][2]);
                //    int cIndex = vertice.Count - 1;
                //    vertice.Add(tetrahedrons[i][3]);
                //    int dIndex = vertice.Count - 1;

                //    edges.Add(new Vector2Int(aIndex, bIndex));
                //    edges.Add(new Vector2Int(aIndex, cIndex));
                //    edges.Add(new Vector2Int(aIndex, dIndex));
                //    edges.Add(new Vector2Int(bIndex, cIndex));
                //    edges.Add(new Vector2Int(bIndex, dIndex));
                //    edges.Add(new Vector2Int(cIndex, dIndex));
                //}

                //DebugDrawWireShape(vertice.ToArray(), edges.ToArray(), _spawnerOffsetTransform.Position, _spawnerOffsetTransform.Scale, _spawnerOffsetTransform.Right, _spawnerOffsetTransform.Up, _spawnerOffsetTransform.Forward, Color.blue);

                return;
            }
#endif
        }

        public void SceneViewUpdate()
        {
            SceneView.lastActiveSceneView.Repaint();
        }

        private void OnSpawnObjects()
        {
            if (_objectsToSpawnArray.Count > 0)
                for (int i = 0; i < _spawnNumber; i++)
                    SpawnObject();

            _spawnTime = GenerateRandomTime();
            _spawnNumber = Random.Range(_spawnNumberRange.x, _spawnNumberRange.y + 1);
        }

        private void SpawnObject()
        {
            int i = Random.Range(0, _objectsToSpawnArray.Count);

            GameObject seed = Instantiate(_objectsToSpawnArray[i], GenerateRandomPos(), Quaternion.identity, transform);

            Vector3 spawnerDir = SpawnerOffsetTransform.Forward;

            if (_spawnerRotationDirection == RotationDirection.Right)
                spawnerDir = SpawnerOffsetTransform.Right;
            if (_spawnerRotationDirection == RotationDirection.Up)
                spawnerDir = SpawnerOffsetTransform.Up;

            if (_objectsRotationDirection == RotationDirection.Forward)
                seed.transform.forward = spawnerDir;
            if (_objectsRotationDirection == RotationDirection.Up)
                seed.transform.up = spawnerDir;
            if (_objectsRotationDirection == RotationDirection.Right)
                seed.transform.right = spawnerDir;

            Vector3 rotationOffset = new Vector3(Random.Range(_minRotationOffset.x, _maxRotationOffset.x), Random.Range(_minRotationOffset.y, _maxRotationOffset.y), Random.Range(_minRotationOffset.z, _maxRotationOffset.z));

            seed.transform.Rotate(rotationOffset);
        }

        private Vector3 GenerateRandomPos()
        {
            Vector3 localRandomPos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

            Vector3 randomPos = Vector3.zero;

            if (_spawnerShape == Shape.Box)
            {
                randomPos = Point3LocalToWorld(localRandomPos, SpawnerOffsetTransform.Position, SpawnerOffsetTransform.Right * SpawnerOffsetTransform.Scale.x, SpawnerOffsetTransform.Up * SpawnerOffsetTransform.Scale.y, SpawnerOffsetTransform.Forward * SpawnerOffsetTransform.Scale.z);
            }
            if (_spawnerShape == Shape.Sphere)
            {
                randomPos = Point3LocalToWorld(localRandomPos, SpawnerOffsetTransform.Position, SpawnerOffsetTransform.Right * SpawnerOffsetTransform.LocalScale.x, SpawnerOffsetTransform.Up * SpawnerOffsetTransform.LocalScale.y, SpawnerOffsetTransform.Forward * SpawnerOffsetTransform.LocalScale.z);

                Vector3 centerToPosDistanceToApply = (randomPos - SpawnerOffsetTransform.Position).normalized * (SpawnerOffsetTransform.LocalScale.x / 2);

                randomPos = SpawnerOffsetTransform.Position + centerToPosDistanceToApply;
            }
            if (_spawnerShape == Shape.FreeShape)
            {
                localRandomPos = GenerateRandomPosInVolume(_customShapeVertice, _customShapeEdges);
                randomPos = Point3LocalToWorld(localRandomPos, SpawnerOffsetTransform.Position, SpawnerOffsetTransform.Right * SpawnerOffsetTransform.Scale.x, SpawnerOffsetTransform.Up * SpawnerOffsetTransform.Scale.y, SpawnerOffsetTransform.Forward * SpawnerOffsetTransform.Scale.z);
            }

            return randomPos;
        }

        private float GenerateRandomTime()
        {
            return Random.Range(_spawnRate.x, _spawnRate.y);
        }

    }

    public enum Shape
    {
        Box,
        Sphere,
        Capsule,
        Cylinder,
        FreeShape,
        Circle,
        Tetrahedron,
        Triangle,
    }

    public enum RotationDirection
    {
        Forward,
        Right,
        Up,
    }
}