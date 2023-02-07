using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.ProceduralGeneration.CartoonWind
{
    [CreateAssetMenu(fileName = "new windAsset", menuName = "ScriptableObjects/WindAssets")]
    public class WindAsset : ScriptableObject
    {
        /*********************INSTANCIATION**************************/

        [SerializeField, Tooltip("Range of time between instanciations")]
        private Vector2 _windTimeRange = new Vector2(0.2f, 1f);

        [SerializeField, Tooltip("range of start position, 1")]
        private Vector3 _posRangeOne = new Vector3(-20, 5, -5);

        [SerializeField, Tooltip("range of start position, 2")]
        private Vector3 _posRangeTwo = new Vector3(20, 15, 10);


        /*********************WIND PARAMETERS**************************/

        [Header("WIND PARAMETERS")]
        [SerializeField, Tooltip("lifetime of particle")]
        private Vector2 _lifeTimeRange = new Vector2(3, 6);

        [SerializeField, Tooltip("speed of wind")]
        private Vector2 _speedRange = Vector2.one * 6;

        [SerializeField, Tooltip("value of perlin noise length")]
        private Vector2 _perlinNoiseXRange = new Vector2(0.05f, 0.3f);

        [SerializeField, Tooltip("value of perlin noise height")]
        private Vector2 _perlinNoiseYRange = new Vector2(50, 100);

        [SerializeField, Tooltip("size of wind looping in the air")]
        private Vector2Int _loopSizeRange = new Vector2Int(30, 70);

        [SerializeField, Tooltip("probability of looping every sec"), Range(0, 1)]
        private float _loopProba = 0.15f;

        /*********************TRAIL PARAMETERS************************/

        [Header("TRAIL PARAMETERS")]
        [SerializeField, Tooltip("material used to render trail")]
        private Material _material;

        [SerializeField, Tooltip("size of trail range")]
        private Vector2 _sizeRange = Vector3.one;

        [SerializeField, Tooltip("duration of trail range")]
        private Vector2 _durationRange = new Vector2(1, 3);

        [SerializeField, Tooltip("detail precision of trail, lower value means more precision")]
        private float _minVertexDistance = 0.05f;

        [SerializeField, Tooltip("curve of trail width")]
        private AnimationCurve _trailSizeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 0.05f), new Keyframe(1, 0) });

        [SerializeField, Tooltip("curve of trail opacity at the end of its life")]
        private AnimationCurve _trailEndOpacityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });

        [SerializeField, Tooltip("curve of trail opacity at the end of its life")]
        private AnimationCurve _trailOpacityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });



        #region Public API

        public Vector2 WindTimeRange
        {
            get => _windTimeRange;
            set => _windTimeRange = value;
        }
        public Vector3 PosRangeOne => _posRangeOne;
        public Vector3 PosRangeTwo => _posRangeTwo;
        public Vector2 LifeTimeRange
        {
            get => _lifeTimeRange;
            set => _lifeTimeRange = value;
        }
        public Vector2 SpeedRange
        {
            get => _speedRange;
            set => _speedRange = value;
        }
        public Vector2 PerlinNoiseXRange
        {
            get => _perlinNoiseXRange;
            set => _perlinNoiseXRange = value;
        }
        public Vector2 PerlinNoiseYRange
        {
            get => _perlinNoiseYRange;
            set => _perlinNoiseYRange = value;
        }
        public Vector2 SizeRange
        {
            get => _sizeRange;
            set => _sizeRange = value;
        }
        public Vector2 DurationRange
        {
            get => _durationRange;
            set => _durationRange = value;
        }
        public Vector2Int LoopSizeRange
        {
            get => _loopSizeRange;
            set => _loopSizeRange = value;
        }
        public float LoopProba
        {
            get => _loopProba;
            set => _loopProba = value;
        }
        public float MinVertexDistance => _minVertexDistance;
        public Material Material => _material;
        public AnimationCurve TrailSizeCurve => _trailSizeCurve;
        public AnimationCurve TrailEndOpacityCurve => _trailEndOpacityCurve;
        public AnimationCurve TrailOpacityCurve => _trailOpacityCurve;

        #endregion

        private void OnEnable()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                _material.color = Color.white;
                _material.name = "white";
            }
        }
    } 
}
