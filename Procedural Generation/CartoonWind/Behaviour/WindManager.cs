using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.CartoonWind
{
    public class WindManager : UPDBBehaviour
    {
        [SerializeField, Tooltip("active pattern")]
        private PatternState _currentPattern = PatternState.curve;


        #region Private Variables

        /// <summary>
        /// trail of Wind
        /// </summary>
        private TrailRenderer _trailRenderer;

        /// <summary>
        /// wind instancier that contains all parameters
        /// </summary>
        private WindInstancier _windInstancier;

        /// <summary>
        /// speed of wind
        /// </summary>
        private float _speed = 0;

        /// <summary>
        /// life time of wind
        /// </summary>
        private float _life = 0;

        /// <summary>
        /// represent time of wind instanciation
        /// </summary>
        private float _timer = 0;

        /// <summary>
        /// timer that is rebooted every sec
        /// </summary>
        private float _loopTimer = 0;

        /// <summary>
        /// size of wind loop when wind is looping
        /// </summary>
        private int _loopSize = 50;

        /// <summary>
        /// side of looping
        /// </summary>
        private int _loopSide = 0;

        /// <summary>
        /// scale of wind perlin noise on x
        /// </summary>
        private float _perlinNoiseX = 0;

        /// <summary>
        /// scale of wind perlin noise on y
        /// </summary>
        private float _perlinNoiseY = 0;

        /// <summary>
        /// last timer saved, not active when object don't move
        /// </summary>
        private float _timerMemo = 0;

        #endregion

        #region Public API

        /// <summary>
        /// represent different possible pattern that wind can do
        /// </summary>
        public enum PatternState
        {
            curve,
            loop,
        }

        #endregion

        /// <summary>
        /// called at build of instance
        /// </summary>
        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// called every frame
        /// </summary>
        private void Update()
        {
            if (_timer < _life)
            {
                Color color = _trailRenderer.material.color;
                color.a = _windInstancier.WindConfig.TrailOpacityCurve.Evaluate(_timer / _life);
                _trailRenderer.material.color = color;

                transform.position += (transform.forward * _speed) * Time.deltaTime;

                if (_currentPattern == PatternState.curve)
                    CurveEffect();
                else if (_currentPattern == PatternState.loop)
                    LoopEffect();

                _timerMemo = _timer;
            }
            else
            {
                Color color = _trailRenderer.material.color;
                color.a = _windInstancier.WindConfig.TrailEndOpacityCurve.Evaluate((_timer - _timerMemo) / _trailRenderer.time);
                _trailRenderer.material.color = color;
            }

            _timer += Time.deltaTime;
            _loopTimer += Time.deltaTime;
        }

        /// <summary>
        /// called at awake or, if necessary, on editor
        /// </summary>
        private void Init()
        {
            if (_trailRenderer == null)
                if (!TryGetComponent(out _trailRenderer))
                    _trailRenderer = gameObject.AddComponent<TrailRenderer>();

            if (_windInstancier == null)
                if (!transform.parent.gameObject.TryGetComponent(out _windInstancier))
                    _windInstancier = FindObjectOfType<WindInstancier>();

            SetWindAttributes();
        }

        /// <summary>
        /// set up wind attribute
        /// </summary>
        private void SetWindAttributes()
        {
            _life = Random.Range(_windInstancier.WindConfig.LifeTimeRange.x, _windInstancier.WindConfig.LifeTimeRange.y);
            _speed = Random.Range(_windInstancier.WindConfig.SpeedRange.x, _windInstancier.WindConfig.SpeedRange.y);
            _perlinNoiseX = Random.Range(_windInstancier.WindConfig.PerlinNoiseXRange.x, _windInstancier.WindConfig.PerlinNoiseXRange.y);
            _perlinNoiseY = Random.Range(_windInstancier.WindConfig.PerlinNoiseYRange.x, _windInstancier.WindConfig.PerlinNoiseYRange.y);

            SetTrailAttributes();
        }

        /// <summary>
        /// set up trail variables
        /// </summary>
        private void SetTrailAttributes()
        {
            _trailRenderer.material = _windInstancier.WindConfig.Material;
            _trailRenderer.widthCurve = _windInstancier.WindConfig.TrailSizeCurve;
            _trailRenderer.widthMultiplier = Random.Range(_windInstancier.WindConfig.SizeRange.x, _windInstancier.WindConfig.SizeRange.y);
            _trailRenderer.minVertexDistance = _windInstancier.WindConfig.MinVertexDistance;
            _trailRenderer.time = Random.Range(_windInstancier.WindConfig.DurationRange.x, _windInstancier.WindConfig.DurationRange.y);
            _trailRenderer.autodestruct = true;
        }

        /// <summary>
        /// update the trajectory of object, following the perlin noise pattern
        /// </summary>
        private void CurveEffect()
        {
            transform.forward = _windInstancier.WindDirection;
            transform.localEulerAngles += new Vector3((Mathf.PerlinNoise(transform.localPosition.z * _perlinNoiseX, 0) * _perlinNoiseY) - (_perlinNoiseY / 2), 0, 0);

            if (_loopTimer >= 1)
            {
                float proba = Random.Range(0f, 1f);

                if (proba <= _windInstancier.WindConfig.LoopProba)
                {
                    _currentPattern = PatternState.loop;
                    _loopSize = Random.Range(_windInstancier.WindConfig.LoopSizeRange.x, _windInstancier.WindConfig.LoopSizeRange.y);
                    _loopSide = (Random.Range(1, 3) * 2) - 3;
                }

                _loopTimer = 0;
            }
        }

        /// <summary>
        /// update the trajectory of object, following the loop pattern
        /// </summary>
        private void LoopEffect()
        {
            if (_loopTimer >= _loopSize)
                _currentPattern = PatternState.curve;

            float moveValue = _loopSize;
            transform.Rotate(new Vector3(_loopSide * (360 / moveValue), 0, 0), Space.Self);

            _loopTimer++;
        }
    } 
}
