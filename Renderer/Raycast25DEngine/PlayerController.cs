using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class PlayerController : UPDBBehaviour
    {
        [Header("Player Parameters")]
        [SerializeField]
        private float _speed = 5;

        [SerializeField]
        private float _rotationSpeed = 5;

        [SerializeField]
        private int _verticalRotationSpeed = 5;

        [SerializeField]
        private float _fovTweakSpeed = 5;

        [SerializeField]
        private int _tweakResSpeed = 10;

        [SerializeField]
        private float _walkingSineWaveWidth = 1f;

        [SerializeField]
        private Vector3 _walkingSineWaveAmplitude = Vector3.zero;

        [SerializeField]
        private float _simulateZRotMultplier = 0;

        [SerializeField]
        private float _cameraRotationSimulateZRotMultplier = 0;

        [SerializeField]
        private float _camRotationZRotationTime = 1;

        [SerializeField]
        private AnimationCurve _camRotationZRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        private float _walkingModifiersLerpTime = 1;

        [SerializeField]
        private AnimationCurve _walkingModifiersLerpCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Raycast Parameters")]
        [SerializeField]
        private LayerMask _rayLayerMask;

        [SerializeField]
        private bool _debugRays = false;

        [Header("Shader Renderer Parameters")]
        [SerializeField]
        private Material _rendererMat;

        [Header("CPU Mode Parameters")]
        [SerializeField]
        private float _CPUFieldOfView = 45;

        [SerializeField]
        private int _CPURaysNumber = 100;

        [SerializeField]
        private float _CPURenderDistance = 15;

        [SerializeField]
        private int _CPUVerticalPixelsNumber = 1080;

        [SerializeField]
        private Vector3 _CPUCameraPosOffset = new Vector3(0, 0.6f, 0);

        [SerializeField]
        private int _CPUTexturesMaxSize = 128;

        #region Non Serialized API

        private List<RaycastHit2D> _raysList = new List<RaycastHit2D>();

        private float _verticalLookingValue = 0;
        private float _sineWaveXValue = 0;
        private float _camRotationSimulateZRotValue = 0;
        private float _walkingAndRotationEffectLerpValue = 0;

        private Texture2D _textureToDrawCoordsArray = null;
        private Texture2D _raycastAndHorizonParameters = null;
        private Texture2DArray _texturesToUseArray = null;
        Texture2D _floorTypeMap = null;
        Texture2DArray _floorTexturesArray = null;

        private int _raysNumberMemo = 0;
        private int _verticalPixelsNumberMemo = 0;
        private Rigidbody2D _rb;

        #endregion

        #region Public API

        public List<RaycastHit2D> RaysList
        {
            get => _raysList;
            set => _raysList = value;
        }

        public int CPUVerticalPixelNumbers
        {
            get => _CPUVerticalPixelsNumber;
            set => _CPUVerticalPixelsNumber = value;
        }

        public int CPURayNumbers => _CPURaysNumber;

        public float CPURenderDistance => _CPURenderDistance;

        public int RayNumbers
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return (int)_rendererMat.GetFloat("_RaysNumber");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_RaysNumber", value);
            }
        }

        public int VerticalPixelsNumber
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return (int)_rendererMat.GetFloat("_VerticalPixelsNumber");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_VerticalPixelsNumber", value);
            }
        }

        public float RenderDistance
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return _rendererMat.GetFloat("_RenderDistance");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_RenderDistance", value);
            }
        }

        public float FieldOfView
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return _rendererMat.GetFloat("_FieldOfView");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_FieldOfView", value);
            }
        }

        public Vector3 CPUCameraPosOffset => _CPUCameraPosOffset;

        public int TexturesMaxSize
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return (int)_rendererMat.GetFloat("_TexturesMaxSize");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_TexturesMaxSize", value);
            }
        }

        public float VerticalLookingValue
        {
            get => _verticalLookingValue;
        }

        public Vector3 CameraPosOffset
        {
            get
            {
                return _rendererMat.GetVector("_CameraPosOffset");
            }

            set
            {
                _rendererMat.SetVector("_CameraPosOffset", value);
            }
        }

        #endregion

        private void Awake()
        {
            MakeNonNullable(ref _rb, gameObject);

            _raysNumberMemo = RayNumbers;
            _verticalPixelsNumberMemo = VerticalPixelsNumber;

            if (Raycast2DLevelBuilder.Instance.LevelData)
                GenerateFloorTypeMap(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);

            RebuildParameters();
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _rb.position += new Vector2(transform.up.x, transform.up.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _rb.position -= new Vector2(transform.up.x, transform.up.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _rb.position -= new Vector2(transform.right.x, transform.right.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _rb.position += new Vector2(transform.right.x, transform.right.y) * _speed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(transform.forward * _rotationSpeed * Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(-transform.forward * _rotationSpeed * Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                _verticalLookingValue += _verticalRotationSpeed;
                _verticalLookingValue = Mathf.Clamp(_verticalLookingValue, -1 / Time.fixedDeltaTime, 1 / Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                _verticalLookingValue -= _verticalRotationSpeed;
                _verticalLookingValue = Mathf.Clamp(_verticalLookingValue, -1 / Time.fixedDeltaTime, 1 / Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.E))
            {
                FieldOfView += _fovTweakSpeed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                FieldOfView -= _fovTweakSpeed * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.J))
            {
                VerticalPixelsNumber = Mathf.Clamp(VerticalPixelsNumber - _tweakResSpeed, 0, Screen.height);
                RayNumbers = Mathf.Clamp((int)(VerticalPixelsNumber * (16 / 9f)), 0, 2048);
            }

            if (Input.GetKey(KeyCode.K))
            {
                VerticalPixelsNumber = Mathf.Clamp(VerticalPixelsNumber + _tweakResSpeed, 0, Screen.height);
                RayNumbers = Mathf.Clamp((int)(VerticalPixelsNumber * (16 / 9f)), 0, 2048);
            }

            PlayerMovementsEffectsLerps();
        }

        private void Update()
        {
            //TrowAndStoreRaycastsCPUMode();
            TrowAndStoreRaycastsGPUMode();

            if (_raysNumberMemo != RayNumbers || _verticalPixelsNumberMemo != VerticalPixelsNumber)
                RebuildParameters();

            _raysNumberMemo = RayNumbers;
            _verticalPixelsNumberMemo = VerticalPixelsNumber;
        }

        private void PlayerMovementsEffectsLerps()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                _sineWaveXValue += _walkingSineWaveWidth * Time.fixedDeltaTime;

                if (_walkingAndRotationEffectLerpValue < 1)
                    _walkingAndRotationEffectLerpValue += Time.fixedDeltaTime / _walkingModifiersLerpTime;
            }
            else
            {
                if (_walkingAndRotationEffectLerpValue > 0)
                    _walkingAndRotationEffectLerpValue -= Time.fixedDeltaTime / _walkingModifiersLerpTime;

                if (_walkingAndRotationEffectLerpValue <= Time.fixedDeltaTime / _walkingModifiersLerpTime)
                    _walkingAndRotationEffectLerpValue = 0;
            }

            float value = Time.fixedDeltaTime / _camRotationZRotationTime;

            if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                if (_camRotationSimulateZRotValue < 1)
                    _camRotationSimulateZRotValue += value;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                if (_camRotationSimulateZRotValue > -1)
                    _camRotationSimulateZRotValue -= value;
            }
            else
            {
                if (_camRotationSimulateZRotValue > 0)
                    _camRotationSimulateZRotValue -= value;
                else if (_camRotationSimulateZRotValue < 0)
                    _camRotationSimulateZRotValue += value;

                if (Mathf.Abs(_camRotationSimulateZRotValue) <= value)
                    _camRotationSimulateZRotValue = 0;
            }
        }

        private void RebuildParameters()
        {
            _textureToDrawCoordsArray = new Texture2D(RayNumbers, 1, TextureFormat.RGBAFloat, false);
            _raycastAndHorizonParameters = new Texture2D(RayNumbers, 1, TextureFormat.RGBAFloat, false);
            _texturesToUseArray = new Texture2DArray(TexturesMaxSize, TexturesMaxSize, RayNumbers, TextureFormat.ARGB32, false);
            _textureToDrawCoordsArray.filterMode = FilterMode.Point;
            _raycastAndHorizonParameters.filterMode = FilterMode.Point;
            _texturesToUseArray.filterMode = FilterMode.Point;

            _rendererMat.SetTexture("_textureToDrawCoords", _textureToDrawCoordsArray);
            _rendererMat.SetTexture("_raycastAndHorizonParameters", _raycastAndHorizonParameters);
            _rendererMat.SetTexture("_texturesToUseArray", _texturesToUseArray);
            _rendererMat.SetTexture("_FloorTypeMap", _floorTypeMap);
            _rendererMat.SetTexture("_FloorTexturesArray", _floorTexturesArray);

            if (Raycast2DLevelBuilder.Instance.LevelData)
                _rendererMat.SetVector("_LevelMapSize", new Vector4(Raycast2DLevelBuilder.Instance.LevelData.LevelSize.x, Raycast2DLevelBuilder.Instance.LevelData.LevelSize.y, 0, 0));
        }

        public void GenerateFloorTypeMap(Cell[,] levelArray, Vector2Int size)
        {
            Dictionary<Texture2D, int> textureToIndex = new Dictionary<Texture2D, int>();
            List<Texture2D> _groundTexturesList = new List<Texture2D>();
            _floorTypeMap = new Texture2D(size.x, size.y, TextureFormat.RG32, false);
            _floorTypeMap.filterMode = FilterMode.Point;
            _floorTypeMap.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    CellData cellData = levelArray[x, y].CellType;
                    Texture2D floorTex = cellData.GroundTexture;

                    if (!textureToIndex.TryGetValue(floorTex, out int index))
                    {
                        index = _groundTexturesList.Count;
                        textureToIndex[floorTex] = index;
                        _groundTexturesList.Add(floorTex);
                    }
                }
            }

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    _floorTypeMap.SetPixel(x, y, new Color(textureToIndex[levelArray[x, y].CellType.GroundTexture] / (float)(_groundTexturesList.Count - 1), 0, 0, 0));

            _floorTypeMap.Apply();

            // Crée le Texture2DArray à partir de la liste
            _floorTexturesArray = new Texture2DArray(TexturesMaxSize, TexturesMaxSize, _groundTexturesList.Count, TextureFormat.ARGB32, false);

            _floorTexturesArray.filterMode = FilterMode.Point;
            _floorTexturesArray.wrapMode = TextureWrapMode.Repeat;

            for (int i = 0; i < _groundTexturesList.Count; i++)
                if (_groundTexturesList[i])
                    Graphics.CopyTexture(_groundTexturesList[i], 0, 0, _floorTexturesArray, i, 0);
        }

        private void TrowAndStoreRaycastsCPUMode()
        {
            _raysList.Clear();
            float angleRotate = _CPURaysNumber > 1 ? -_CPUFieldOfView / 2f : 0;

            for (int i = 0; i < _CPURaysNumber; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, RotateVector(transform.up, angleRotate), _CPURenderDistance, _rayLayerMask);

                _raysList.Add(hit);

                if (_debugRays)
                    Debug.DrawRay(transform.position, RotateVector(transform.up, angleRotate) * (hit ? hit.distance : _CPURenderDistance), Color.blue);

                if (_CPURaysNumber > 1)
                    angleRotate += _CPUFieldOfView / (float)(_CPURaysNumber - 1);
            }
        }

        //private void TrowAndStoreRaycastsGPUMode()
        //{
        //    if (!_rendererMat)
        //        return;

        //    float angleRotate = RayNumbers > 1 ? FieldOfView / 2f : 0;

        //    for (int i = 0; i < RayNumbers; i++)
        //    {
        //        float simulateZAngleBaseDiagonal = ((i / (float)RayNumbers) - 0.5f) * 2;
        //        float simulateZAngleWalkingSin = Mathf.Sin(_sineWaveXValue / 2f) * _simulateZRotMultplier * _simulateZRotLerpValue;
        //        float simulateZAngleTurningValue = (_camRotationSimulateZRotValue * _camRotationZRotationCurve.Evaluate(Mathf.Abs(_camRotationSimulateZRotValue))) * _cameraRotationSimulateZRotMultplier;
        //        float simulateZAngleValue = simulateZAngleBaseDiagonal * (simulateZAngleWalkingSin + simulateZAngleTurningValue);

        //        Vector2 angle = RotateVector(transform.up, angleRotate);
        //        float walkingMoveSinXOffset = Mathf.Sin(_sineWaveXValue / 2f) * _walkingSineWaveAmplitude.x;
        //        float walkingMoveSinZOffset = Mathf.Sin(_sineWaveXValue / 2f) * _walkingSineWaveAmplitude.z;
        //        Vector2 rayOrigin = transform.position + (transform.right * _cameraPosOffset.x) + (transform.up * _cameraPosOffset.z) + (transform.right * walkingMoveSinXOffset) + (transform.up * walkingMoveSinZOffset);

        //        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, angle, RenderDistance, _rayLayerMask);

        //        float height = hit ? (1 / (hit.distance)) : 0;

        //        float baseHeightDefault = ((1 - height) / 2);
        //        float yLookingValue = (float)_verticalLookingValue * Time.fixedDeltaTime;
        //        float playerYPosition = _cameraPosOffset.y - 0.5f;
        //        float walkingMoveSinYOffset = Mathf.Sin(_sineWaveXValue) * _walkingSineWaveAmplitude.y * _simulateZRotLerpValue;

        //        float baseHeight = (baseHeightDefault - yLookingValue) + (height * -(playerYPosition + walkingMoveSinYOffset)) + simulateZAngleValue;

        //        Vector2 collidedPos = Vector2.zero;
        //        Vector2Int collidedWall = Vector2Int.zero;
        //        CellData collidedCellData = null;
        //        Texture2D cellTexture = null;
        //        Vector2 textureCoords = Vector2Int.zero;
        //        float textureXPos = 0;
        //        float multipliedHeight = 0;

        //        if (hit)
        //        {
        //            collidedPos = hit.transform.position;
        //            collidedWall = new Vector2Int((int)(collidedPos.x), (int)(collidedPos.y));

        //            collidedCellData = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[collidedWall.x, collidedWall.y].CellType;
        //            cellTexture = collidedCellData.WallTexture;

        //            if (cellTexture && _texturesToUseArray.depth == RayNumbers)
        //                Graphics.CopyTexture(cellTexture, 0, 0, _texturesToUseArray, i, 0);

        //            textureCoords = hit.point - collidedPos;
        //            textureXPos = (textureCoords.x) + (textureCoords.y);

        //            multipliedHeight = (height * collidedCellData.WallHeight);
        //        }

        //        float invMultipliedHeight = 1f / (float)multipliedHeight;


        //        _textureToDrawCoordsArray.SetPixel(i, 0, new Color(textureXPos, baseHeight, multipliedHeight, hit ? 1 : 0));
        //        _distanceAndOtherParametersArray.SetPixel(i, 0, new Color(hit.distance / RenderDistance, (angle.x + 1) / 2, (angle.y + 1) / 2, 0));

        //        if (_debugRays)
        //            Debug.DrawRay(rayOrigin, RotateVector(transform.up, angleRotate) * (hit ? hit.distance : RenderDistance), Color.blue);

        //        if (RayNumbers > 1)
        //            angleRotate -= FieldOfView / (float)(RayNumbers - 1);
        //    }

        //    _textureToDrawCoordsArray.Apply();
        //    _distanceAndOtherParametersArray.Apply();
        //    _rendererMat.SetVector("_PlayerPos", new Vector4(transform.position.x, transform.position.y, 0, 0));
        //}

        private void TrowAndStoreRaycastsGPUMode()
        {
            if (!_rendererMat)
                return;

            float angleRotate = RayNumbers > 1 ? FieldOfView / 2f : 0;
            float yLookingValue = (float)_verticalLookingValue * Time.fixedDeltaTime;

            float walkingMoveSinXOffset = Mathf.Sin(_sineWaveXValue / 2f) * _walkingSineWaveAmplitude.x * _walkingAndRotationEffectLerpValue;
            float walkingMoveSinZOffset = Mathf.Sin(_sineWaveXValue / 2f) * _walkingSineWaveAmplitude.z * _walkingAndRotationEffectLerpValue;
            float walkingMoveSinYOffset = Mathf.Sin(_sineWaveXValue) * _walkingSineWaveAmplitude.y * _walkingAndRotationEffectLerpValue;

            Vector2 rayOrigin = transform.position + (transform.right * CameraPosOffset.x) + (transform.up * CameraPosOffset.z) + (transform.right * walkingMoveSinXOffset) + (transform.up * walkingMoveSinZOffset);
            _rendererMat.SetVector("_RaycastOrigin", rayOrigin);

            float playerYPosition = CameraPosOffset.y - 0.5f;

            for (int i = 0; i < RayNumbers; i++)
            {
                float simulateZAngleBaseDiagonal = ((i / (float)RayNumbers) - 0.5f) * 2;
                float simulateZAngleWalkingSin = Mathf.Sin(_sineWaveXValue / 2f) * _simulateZRotMultplier * _walkingAndRotationEffectLerpValue;
                float simulateZAngleTurningValue = (_camRotationSimulateZRotValue * _camRotationZRotationCurve.Evaluate(Mathf.Abs(_camRotationSimulateZRotValue))) * _cameraRotationSimulateZRotMultplier;
                float simulateZAngleValue = simulateZAngleBaseDiagonal * (simulateZAngleWalkingSin + simulateZAngleTurningValue);

                Vector2 angle = RotateVector(transform.up, angleRotate);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, angle, RenderDistance, _rayLayerMask);

                float height = hit ? (1 / (hit.distance)) : 0;
                float baseHeightDefault = ((1 - height) / 2);

                float baseHeight = (baseHeightDefault - yLookingValue) + (height * -(playerYPosition + walkingMoveSinYOffset)) + simulateZAngleValue;
                float horizon = (0.5f - yLookingValue) + (height * -(walkingMoveSinYOffset)) + simulateZAngleValue;

                Vector2 collidedPos = Vector2.zero;
                Vector2Int collidedWall = Vector2Int.zero;
                CellData collidedCellData = null;
                Texture2D cellTexture = null;
                Vector2 textureCoords = Vector2Int.zero;
                float textureXPos = 0;
                float multipliedHeight = 0;

                if (hit)
                {
                    collidedPos = hit.transform.position;
                    collidedWall = new Vector2Int((int)(collidedPos.x), (int)(collidedPos.y));

                    collidedCellData = Raycast2DLevelBuilder.Instance.LevelData.LevelArray[collidedWall.x, collidedWall.y].CellType;
                    cellTexture = collidedCellData.WallTexture;

                    if (cellTexture && _texturesToUseArray.depth == RayNumbers)
                        Graphics.CopyTexture(cellTexture, 0, 0, _texturesToUseArray, i, 0);

                    textureCoords = hit.point - collidedPos;
                    textureXPos = (textureCoords.x) + (textureCoords.y);

                    multipliedHeight = (height * collidedCellData.WallHeight);
                }

                float invMultipliedHeight = 1f / (float)multipliedHeight;

                _textureToDrawCoordsArray.SetPixel(i, 0, new Color(textureXPos, baseHeight, multipliedHeight, hit ? 1 : 0));
                _raycastAndHorizonParameters.SetPixel(i, 0, new Color(hit.distance / RenderDistance, (angle.x + 1) / 2, (angle.y + 1) / 2, horizon));

                if (_debugRays)
                    Debug.DrawRay(rayOrigin, RotateVector(transform.up, angleRotate) * (hit ? hit.distance : RenderDistance), Color.blue);

                if (RayNumbers > 1)
                    angleRotate -= FieldOfView / (float)(RayNumbers - 1);
            }

            _textureToDrawCoordsArray.Apply();
            _raycastAndHorizonParameters.Apply();
            _rendererMat.SetVector("_PlayerPos", new Vector4(transform.position.x, transform.position.y, 0, 0));
        }
    }
}
