using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.CartoonWind
{
    [AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.ProceduralGeneration + "/" + NamespaceID.CartoonWind + "/Wind Instancier")]
    public class WindInstancier : UPDBBehaviour
    {
        /*********************ACTIVE CONFIG**************************/

        [Header("GENERIC PARAMETERS")]
        [SerializeField, Tooltip("direction the wind is going")]
        private Vector3 _windDirection = Vector3.forward;

        [SerializeField, Tooltip("parent object containing all wind objects")]
        private Transform _windParent;

        [Header("ACTIVE CONFIG")]
        [SerializeField, Tooltip("scriptable asset that is used to take every values and parameters of wind")]
        private WindAsset _windConfig;


        #region Private Variables

        /// <summary>
        /// wind prefab, used to replicate wind
        /// </summary>
        private GameObject _windPrefab;

        /// <summary>
        /// count time since last instanciation
        /// </summary>
        private float _timer = 0;

        /// <summary>
        /// time to wait until next instanciation
        /// </summary>
        private float _time = 0;

        #endregion

        #region Public API

        public WindAsset WindConfig
        {
            get
            {
                if (_windConfig == null)
                {
                    _windConfig = ScriptableObject.CreateInstance<WindAsset>();
                    _windConfig.name = "Medium Wind";
                }

                return _windConfig;
            }
            set
            { _windConfig = value; }
        }

        public Vector3 WindDirection
        {
            get
            {
                _windDirection = _windDirection.normalized;
                return _windDirection;
            }
            set
            {
                _windDirection = value.normalized;
            }
        }

        public Transform WindParent
        {
            get
            {
                if(!_windParent)
                    _windParent = transform;

                return _windParent;
            }
            set
            {
                _windParent = value;
            }
        }

        #endregion


        /// <summary>
        /// called at build of instance
        /// </summary>
        private void Awake()
        {
            if (_windPrefab == null)
            {
                _windPrefab = new GameObject();
                _windPrefab.name = "WindPrimitive";
                _windPrefab.SetActive(false);
                _windPrefab.transform.SetParent(transform);
                _windPrefab.transform.localPosition = Vector3.zero;
                _windPrefab.transform.localEulerAngles = Vector3.zero;
            }

            _time = Random.Range(WindConfig.WindTimeRange.x, WindConfig.WindTimeRange.y);
        }

        private void Start()
        {
            if (!_windPrefab.TryGetComponent(out WindManager wind))
                _windPrefab.AddComponent<WindManager>();
        }

        /// <summary>
        /// called every frame
        /// </summary>
        private void Update()
        {
            if (_timer >= _time)
            {
                InstantiateWind();

                _time = Random.Range(WindConfig.WindTimeRange.x, WindConfig.WindTimeRange.y);
                _timer = 0;
            }

            _timer += Time.deltaTime;
        }

        /// <summary>
        /// called when gizmo is drawing in unity editor, only if gameobject is selected in inspector
        /// </summary>
        protected override void OnSceneSelected()
        {
            Vector3 center = new Vector3(((WindConfig.PosRangeTwo.x + WindConfig.PosRangeOne.x) / 2), ((WindConfig.PosRangeTwo.y + WindConfig.PosRangeOne.y) / 2), ((WindConfig.PosRangeTwo.z + WindConfig.PosRangeOne.z) / 2));
            Vector3 size = new Vector3((WindConfig.PosRangeTwo.x - WindConfig.PosRangeOne.x), (WindConfig.PosRangeTwo.y - WindConfig.PosRangeOne.y), (WindConfig.PosRangeTwo.z - WindConfig.PosRangeOne.z));
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(center, size);
            Debug.DrawRay(transform.position, _windDirection * 10, Color.red);
        }

        /// <summary>
        /// instanciate wind object
        /// </summary>
        private void InstantiateWind()
        {
            GameObject wind = Instantiate(_windPrefab, _windPrefab.transform.position, Quaternion.identity);
            wind.transform.SetParent(WindParent);
            wind.SetActive(true);

            wind.transform.position += (transform.right * Random.Range(WindConfig.PosRangeOne.x, WindConfig.PosRangeTwo.x)) + (transform.up * Random.Range(WindConfig.PosRangeOne.y, WindConfig.PosRangeTwo.y)) + (transform.forward * Random.Range(WindConfig.PosRangeOne.z, WindConfig.PosRangeTwo.z));
            wind.transform.eulerAngles = _windPrefab.transform.eulerAngles;
        }

        public void SetBuiltInConfig(BuiltInConfig config)
        {
            if (config == BuiltInConfig.LowWind)
            {
                WindConfig = ScriptableObject.CreateInstance<WindAsset>();
                WindConfig.name = "Low Wind";
                WindConfig.SpeedRange = new Vector2(2, 2);
                WindConfig.WindTimeRange = new Vector2(0.6f, 2f);
                WindConfig.LifeTimeRange = new Vector2(5, 8);
                WindConfig.PerlinNoiseYRange = new Vector2(75, 150);
                WindConfig.DurationRange = new Vector2(2, 4);
            }
            else if (config == BuiltInConfig.MediumWind)
            {
                WindConfig = ScriptableObject.CreateInstance<WindAsset>();
                WindConfig.name = "Medium Wind";
            }
            else if (config == BuiltInConfig.HighWind)
            {
                WindConfig = ScriptableObject.CreateInstance<WindAsset>();
                WindConfig.name = "High Wind";
                WindConfig.SpeedRange = new Vector2(10, 10);
                WindConfig.WindTimeRange = new Vector2(0.1f, 0.4f);
                WindConfig.LifeTimeRange = new Vector2(2, 4);
                WindConfig.PerlinNoiseYRange = new Vector2(25, 50);
                WindConfig.DurationRange = new Vector2(0.5f, 2);
            }
            else if (config == BuiltInConfig.EveryFrameDebug)
            {
                WindConfig = ScriptableObject.CreateInstance<WindAsset>();
                WindConfig.name = "EveryFrameDebug";
                WindConfig.WindTimeRange = Vector2.zero;
                WindConfig.PerlinNoiseYRange = Vector2.zero;
                WindConfig.LoopProba = 0;
                WindConfig.DurationRange = new Vector2(10, 10);
            }
            else if (config == BuiltInConfig.LoopDebug)
            {
                WindConfig = ScriptableObject.CreateInstance<WindAsset>();
                WindConfig.name = "LoopDebug";
                WindConfig.LoopProba = 0.5f;
                WindConfig.LoopSizeRange = new Vector2Int(10, 350);
                WindConfig.DurationRange = new Vector2(10, 10);
            }
            else if (config == BuiltInConfig.NoLoop)
            {
                WindConfig.LoopProba = 0;
            }
            else if (config == BuiltInConfig.NormalLoop)
            {
                WindConfig.LoopProba = 0.15f;
            }
            else if (config == BuiltInConfig.FrequentLoop)
            {
                WindConfig.LoopProba = 0.4f;
            }
            else if (config == BuiltInConfig.SmallWidth)
            {
                WindConfig.SizeRange = Vector3.one * 0.25f;
            }
            else if (config == BuiltInConfig.NormalWidth)
            {
                WindConfig.SizeRange = Vector3.one;
            }
            else if (config == BuiltInConfig.LargeWidth)
            {
                WindConfig.SizeRange = Vector3.one * 4;
            }
            else if (config == BuiltInConfig.SmallLength)
            {
                WindConfig.DurationRange = new Vector2(0.2f, 1);
            }
            else if (config == BuiltInConfig.NormalLength)
            {
                WindConfig.DurationRange = new Vector2(1, 3);
            }
            else if (config == BuiltInConfig.HighLength)
            {
                WindConfig.DurationRange = new Vector2(5, 10);
            }
        }

        public void ClearConfig()
        {
            WindManager[] windManagers = FindObjectsByType<WindManager>(FindObjectsSortMode.InstanceID);

            foreach (WindManager wind in windManagers)
                if (wind.gameObject.activeSelf)
                    Destroy(wind.gameObject);
        }
    }

    public enum BuiltInConfig
    {
        LowWind,
        MediumWind,
        HighWind,
        EveryFrameDebug,
        LoopDebug,
        NoLoop,
        NormalLoop,
        FrequentLoop,
        SmallWidth,
        NormalWidth,
        LargeWidth,
        SmallLength,
        NormalLength,
        HighLength,
    }
}
