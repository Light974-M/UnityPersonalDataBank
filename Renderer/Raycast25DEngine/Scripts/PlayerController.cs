using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class PlayerController : UPDBBehaviour
    {
        [Header("Player Parameters")]
        [SerializeField]
        private float _speed = 5;

        [SerializeField]
        private Vector2 _rotationSpeed = new Vector2(150, 3);

        [SerializeField]
        private Vector2 _verticalLookAngleConstraints = new Vector2(-90, 90);

        [SerializeField]
        private bool _useMouseLook = false;

        [SerializeField]
        private float _fovTweakSpeed = 5;

        [SerializeField]
        private int _tweakResSpeed = 10;

        [SerializeField]
        private Vector3 _cameraPosOffset = new Vector3(0, 0.6f, 0);

        [SerializeField]
        private float _walkingSineWaveWidth = 1f;

        [SerializeField]
        private Vector3 _walkingSineWaveAmplitude = Vector3.zero;

        [SerializeField]
        private float _walkingZRotMultplier = 0;

        [SerializeField]
        private float _cameraRotationZRotMultplier = 0;

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

        [SerializeField]
        private UniversalRendererData _uRenderData;

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

        [SerializeField]
        private GameObject _CPURendererObj;

        [SerializeField]
        private bool _CPUModeEnabled = false;

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
        Texture2D _roofTypeMap = null;
        Texture2DArray _roofTexturesArray = null;
        Texture2D _cellsEnabledMap = null;
        Texture2D _cellsLightSourceMap = null;
        Texture2D _cellsLightSourceColorMap = null;

        private int _raysNumberMemo = 0;
        private int _verticalPixelsNumberMemo = 0;
        private Rigidbody2D _rb;
        private float _FOVMemo = 70f;
        private Vector2 _moveInput = Vector2.zero;
        private Vector2 _turnInput = Vector2.zero;
        private bool _isMoving = false;
        private bool _isTurningX = false;

        private List<Vector2> _raysDirList = new List<Vector2>();
        private Vector2 _raycastOrigin = Vector2.zero;

        #endregion

        #region Public API

        public List<RaycastHit2D> RaysList
        {
            get => _raysList;
            set => _raysList = value;
        }

        public List<Vector2> RaysDirList
        {
            get => _raysDirList;
            set => _raysDirList = value;
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

        public float PlayerHeight
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return _rendererMat.GetFloat("_playerHeight");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_playerHeight", value);
            }
        }

        public Vector2 FieldOfView
        {
            get
            {
                if (!_rendererMat)
                    return Vector2.zero;

                return _rendererMat.GetVector("_FieldOfView");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetVector("_FieldOfView", value);
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

        public float CellLightSourceMaxIntensity
        {
            get
            {
                if (!_rendererMat)
                    return 0;

                return _rendererMat.GetFloat("_CellLightSourceMaxIntensity");
            }

            set
            {
                if (!_rendererMat)
                    return;

                _rendererMat.SetFloat("_CellLightSourceMaxIntensity", value);
            }
        }

        public Texture2D TextureToDrawCoordsArray => _textureToDrawCoordsArray;
        public Texture2D RaycastAndHorizonParameters => _raycastAndHorizonParameters;
        public Vector2 RaycastOrigin => _raycastOrigin;

        #endregion

        private void Awake()
        {
            MakeNonNullable(ref _rb, gameObject);

            _CPURendererObj.SetActive(_CPUModeEnabled);

            _raysNumberMemo = RayNumbers;
            _verticalPixelsNumberMemo = VerticalPixelsNumber;

            _FOVMemo = _rendererMat.GetFloat("_FOV");

            RebuildParameters();
        }

        private void FixedUpdate()
        {
            MoveUpdate();
            TurnUpdate();

            if (Input.GetKey(KeyCode.E))
            {
                FieldOfView += new Vector2(_fovTweakSpeed * Time.fixedDeltaTime, 0);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                FieldOfView -= new Vector2(_fovTweakSpeed * Time.fixedDeltaTime, 0);
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
            if (_CPUModeEnabled)
                TrowAndStoreRaycastsCPUMode();
            else
                TrowAndStoreRaycastsGPUMode();

            if (_raysNumberMemo != RayNumbers || _verticalPixelsNumberMemo != VerticalPixelsNumber)
                RebuildParameters();

            _raysNumberMemo = RayNumbers;
            _verticalPixelsNumberMemo = VerticalPixelsNumber;
        }

        private void MoveUpdate()
        {
            Vector2 up = new Vector2(transform.up.x, transform.up.y);
            Vector2 right = new Vector2(transform.right.x, transform.right.y);
            _rb.position += ((up * _moveInput.y) + (right * _moveInput.x)) * _speed * Time.fixedDeltaTime;
        }

        private void TurnUpdate()
        {
            transform.Rotate((-transform.forward * _turnInput.x) * _rotationSpeed.x * Time.fixedDeltaTime);

            _verticalLookingValue += _turnInput.y * _rotationSpeed.y * Time.fixedDeltaTime;
            _verticalLookingValue = Mathf.Clamp(_verticalLookingValue, _verticalLookAngleConstraints.x, _verticalLookAngleConstraints.y);
        }

        private void PlayerMovementsEffectsLerps()
        {
            if (_isMoving)
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

            if (_isTurningX)
            {
                float dir = _turnInput.x == Mathf.Abs(_turnInput.x) ? 1 : -1;
                _camRotationSimulateZRotValue = Mathf.Clamp(_camRotationSimulateZRotValue + (value * dir), -1, 1);
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

        public void RebuildParameters()
        {
            if (Raycast2DLevelBuilder.Instance.LevelData)
            {
                GenerateFloorTypeMap(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
                GenerateRoofTypeMap(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
                GenerateCellsEnabledMap(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
                GenerateCellsLightSourceMap(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
                GenerateCellsLightSourceColorMap(Raycast2DLevelBuilder.Instance.LevelData.LevelArray, Raycast2DLevelBuilder.Instance.LevelData.LevelSize);
            }

            _textureToDrawCoordsArray = new Texture2D(RayNumbers, 1, TextureFormat.RGBAFloat, false);
            _raycastAndHorizonParameters = new Texture2D(RayNumbers, 1, TextureFormat.RGBAFloat, false);
            _texturesToUseArray = new Texture2DArray(TexturesMaxSize, TexturesMaxSize, RayNumbers, TextureFormat.ARGB32, false);
            _textureToDrawCoordsArray.filterMode = FilterMode.Point;
            _raycastAndHorizonParameters.filterMode = FilterMode.Point;
            _texturesToUseArray.filterMode = FilterMode.Point;

            _rendererMat.SetTexture("_textureToDrawCoords", _textureToDrawCoordsArray);
            _rendererMat.SetTexture("_raycastAndHorizonParameters", _raycastAndHorizonParameters);
            _rendererMat.SetTexture("_texturesToUseArray", _texturesToUseArray);
            _rendererMat.SetTexture("_floorTypeMap", _floorTypeMap);
            _rendererMat.SetTexture("_floorTexturesArray", _floorTexturesArray);
            _rendererMat.SetTexture("_roofTypeMap", _roofTypeMap);
            _rendererMat.SetTexture("_roofTexturesArray", _roofTexturesArray);
            _rendererMat.SetTexture("_cellsEnabledMap", _cellsEnabledMap);
            _rendererMat.SetTexture("_cellsLightSourceMap", _cellsLightSourceMap);
            _rendererMat.SetTexture("_cellsLightSourceColorMap", _cellsLightSourceColorMap);

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

        public void GenerateRoofTypeMap(Cell[,] levelArray, Vector2Int size)
        {
            Dictionary<Texture2D, int> textureToIndex = new Dictionary<Texture2D, int>();
            List<Texture2D> _groundTexturesList = new List<Texture2D>();
            _roofTypeMap = new Texture2D(size.x, size.y, TextureFormat.RG32, false);
            _roofTypeMap.filterMode = FilterMode.Point;
            _roofTypeMap.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    CellData cellData = levelArray[x, y].CellType;
                    Texture2D floorTex = cellData.RoofTexture;

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
                    _roofTypeMap.SetPixel(x, y, new Color(textureToIndex[levelArray[x, y].CellType.RoofTexture] / (float)(_groundTexturesList.Count - 1), 0, 0, 0));

            _roofTypeMap.Apply();

            // Crée le Texture2DArray à partir de la liste
            _roofTexturesArray = new Texture2DArray(TexturesMaxSize, TexturesMaxSize, _groundTexturesList.Count, TextureFormat.ARGB32, false);

            _roofTexturesArray.filterMode = FilterMode.Point;
            _roofTexturesArray.wrapMode = TextureWrapMode.Repeat;

            for (int i = 0; i < _groundTexturesList.Count; i++)
                if (_groundTexturesList[i])
                    Graphics.CopyTexture(_groundTexturesList[i], 0, 0, _roofTexturesArray, i, 0);
        }

        public void GenerateCellsEnabledMap(Cell[,] levelArray, Vector2Int size)
        {
            Dictionary<Texture2D, int> textureToIndex = new Dictionary<Texture2D, int>();
            List<Texture2D> _groundTexturesList = new List<Texture2D>();
            _cellsEnabledMap = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
            _cellsEnabledMap.filterMode = FilterMode.Point;
            _cellsEnabledMap.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    _cellsEnabledMap.SetPixel(x, y, new Color(levelArray[x, y].CellType.HasGround ? 1 : 0, levelArray[x, y].CellType.HasWall ? 1 : 0, levelArray[x, y].CellType.HasRoof ? 1 : 0, 0));

            _cellsEnabledMap.Apply();
        }

        public void GenerateCellsLightSourceMap(Cell[,] levelArray, Vector2Int size)
        {
            Dictionary<Texture2D, int> textureToIndex = new Dictionary<Texture2D, int>();
            List<Texture2D> _groundTexturesList = new List<Texture2D>();
            _cellsLightSourceMap = new Texture2D(size.x, size.y, TextureFormat.RGBAFloat, false);
            _cellsLightSourceMap.filterMode = FilterMode.Point;
            _cellsLightSourceMap.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    _cellsLightSourceMap.SetPixel(x, y, new Color(levelArray[x, y].CellType.LightSourcePosition.x, levelArray[x, y].CellType.LightSourcePosition.y, levelArray[x, y].CellType.LightSourcePosition.z, levelArray[x, y].CellType.LightSource / CellLightSourceMaxIntensity));

            _cellsLightSourceMap.Apply();
        }

        public void GenerateCellsLightSourceColorMap(Cell[,] levelArray, Vector2Int size)
        {
            Dictionary<Texture2D, int> textureToIndex = new Dictionary<Texture2D, int>();
            List<Texture2D> _groundTexturesList = new List<Texture2D>();
            _cellsLightSourceColorMap = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            _cellsLightSourceColorMap.filterMode = FilterMode.Point;
            _cellsLightSourceColorMap.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    _cellsLightSourceColorMap.SetPixel(x, y, levelArray[x, y].CellType.LightSourceColor);

            _cellsLightSourceColorMap.Apply();
        }

        private void TrowAndStoreRaycastsCPUMode()
        {
            _raysList.Clear();
            _raysDirList.Clear();

            float angleRotate = _CPURaysNumber > 1 ? -_CPUFieldOfView / 2f : 0;

            for (int i = 0; i < _CPURaysNumber; i++)
            {
                Vector2 rayAngle = RotateVector(transform.up, angleRotate);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayAngle, _CPURenderDistance, _rayLayerMask);

                _raysList.Add(hit);
                _raysDirList.Add(rayAngle);

                if (_debugRays)
                    Debug.DrawRay(transform.position, rayAngle * (hit ? hit.distance : _CPURenderDistance), Color.blue);

                if (_CPURaysNumber > 1)
                    angleRotate += _CPUFieldOfView / (float)(_CPURaysNumber - 1);
            }
        }

        private void TrowAndStoreRaycastsGPUMode()
        {
            if (!_rendererMat)
                return;

            float fovValue = _rendererMat.GetFloat("_FOV");

            if (_FOVMemo != fovValue)
                _rendererMat.SetVector("_FieldOfView", new Vector2(fovValue, fovValue));

            _FOVMemo = fovValue;

            float angleRotate = RayNumbers > 1 ? FieldOfView.x / 2f : 0;
            float yLookingValue = (float)_verticalLookingValue * Time.fixedDeltaTime;
            _rendererMat.SetFloat("_yLookingValue", yLookingValue);

            float walkingMoveSinXOffset = Mathf.Sin(_sineWaveXValue / 2f) * _walkingSineWaveAmplitude.x * _walkingAndRotationEffectLerpValue;
            float walkingMoveSinZOffset = Mathf.Sin(_sineWaveXValue / 2f) * _walkingSineWaveAmplitude.z * _walkingAndRotationEffectLerpValue;
            float walkingMoveSinYOffset = Mathf.Sin(_sineWaveXValue) * _walkingSineWaveAmplitude.y * _walkingAndRotationEffectLerpValue;

            _raycastOrigin = transform.position + (transform.right * _cameraPosOffset.x) + (transform.up * _cameraPosOffset.z) + (transform.right * walkingMoveSinXOffset) + (transform.up * walkingMoveSinZOffset);
            _rendererMat.SetVector("_RaycastOrigin", _raycastOrigin);

            float playerYPosition = _cameraPosOffset.y - 0.5f;
            PlayerHeight = playerYPosition + walkingMoveSinYOffset;

            for (int i = 0; i < RayNumbers; i++)
            {
                float simulateZAngleBaseDiagonal = ((i / (float)RayNumbers) - 0.5f) * 2;
                float simulateZAngleWalkingSin = Mathf.Sin(_sineWaveXValue / 2f) * _walkingZRotMultplier * _walkingAndRotationEffectLerpValue;
                float simulateZAngleTurningValue = (_camRotationSimulateZRotValue * _camRotationZRotationCurve.Evaluate(Mathf.Abs(_camRotationSimulateZRotValue))) * _cameraRotationZRotMultplier;
                float simulateZAngleValue = simulateZAngleBaseDiagonal * (simulateZAngleWalkingSin + simulateZAngleTurningValue);

                Vector2 angle = RotateVector(transform.up, angleRotate);
                RaycastHit2D hit = Physics2D.Raycast(_raycastOrigin, angle, RenderDistance, _rayLayerMask);

                float height = hit ? ((100f / FieldOfView.y) / (hit.distance)) : 0;
                float baseHeightDefault = ((1 - height) / 2);

                float baseHeightPlayerHeightOffset = (height * -(playerYPosition + walkingMoveSinYOffset));

                float baseHeight = (baseHeightDefault - yLookingValue) + baseHeightPlayerHeightOffset + simulateZAngleValue;
                float horizon = (0.5f - yLookingValue) + simulateZAngleValue;

                float invertedBaseHeight = (baseHeightDefault + yLookingValue) - (baseHeightPlayerHeightOffset + simulateZAngleValue);
                float invertedHorizon = (0.5f + yLookingValue) - simulateZAngleValue;

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
                    Debug.DrawRay(_raycastOrigin, RotateVector(transform.up, angleRotate) * (hit ? hit.distance : RenderDistance), Color.blue);

                if (RayNumbers > 1)
                    angleRotate -= FieldOfView.x / (float)(RayNumbers - 1);
            }

            _textureToDrawCoordsArray.Apply();
            _raycastAndHorizonParameters.Apply();
            _rendererMat.SetVector("_PlayerPos", new Vector4(transform.position.x, transform.position.y, 0, 0));
        }

        #region Event Callback

        public void GetMove(InputAction.CallbackContext callback)
        {
            _moveInput = callback.ReadValue<Vector2>();

            _isMoving = callback.started ? true : callback.canceled ? false : _isMoving;
        }

        public void GetTurn(InputAction.CallbackContext callback)
        {
            if (!_useMouseLook)
            {
                _turnInput = callback.ReadValue<Vector2>() * 50;

                if (Cursor.lockState == CursorLockMode.Locked || !Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
            }
        }

        public void GetIsHorizontalRotate(InputAction.CallbackContext callback)
        {
            if (!_useMouseLook)
                _isTurningX = callback.started ? true : callback.canceled ? false : _isTurningX;
        }

        public void GetMouseTurn(InputAction.CallbackContext callback)
        {
            if (_useMouseLook)
            {
                _turnInput = callback.ReadValue<Vector2>();

                if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        public void GetIsMouseHorizontalRotate(InputAction.CallbackContext callback)
        {
            if (_useMouseLook)
                _isTurningX = callback.started ? true : callback.canceled ? false : _isTurningX;
        }

        #endregion
    }
}
