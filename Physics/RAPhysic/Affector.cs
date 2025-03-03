using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.RAPhysic
{
    /// <summary>
    /// main class of RAPhysic
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/Physics/RAPhysic/README.md"), AddComponentMenu("UPDB/Physics/RAPhysics/RAPhysics Affector")]
    public class Affector : UPDBBehaviour
    {
        #region Serialized And Public Variables

        /****** PHYSIC ******/

        [Header("PHYSIC")]

        [SerializeField, Tooltip("determine if object is in the list and is detected")]
        private bool _physicEnabled = true;

        [SerializeField, Tooltip("determine if object generate gravity and apply it to other object")]
        private bool _physicAffectorEnabled = true;

        [SerializeField, Tooltip("if enabled, planets will flatten compare to their rotation speed")]
        private bool _rotationDeformation = false;

        [SerializeField, Tooltip("factor for rotation deformation")]
        private float _rotationDeformationFactor = 1;

        [SerializeField, Tooltip("determine wich physic system to use for applying forces")]
        private PhysicType _usedPhysicSystemBase;

        [SerializeField, Tooltip("rigidbody used to calculate physics for current object")]
        private Rigidbody _rb;

        [SerializeField, Tooltip("characterController used for calculations")]
        private CharacterController _charaController;

        [SerializeField, Tooltip("custom Rigidbody used for calculations")]
        private CustomRigidbody _customRb;


        /****** AIR RESISTANCE ******/

        [Header("AIR RESISTANCE")]

        [SerializeField, Tooltip("determine if the object can detect and calculate its air resistance")]
        private bool _airResistanceEnabled = true;

        [SerializeField, Tooltip("determine if object as atmosphere and generate air resistance")]
        private bool _atmosphereEnabled = true;

        [SerializeField, Tooltip("range of object atmosphere")]
        private float _atmosphereRange = 0;

        [SerializeField, Tooltip("density of air and strength of resistance at sea level(or at the very bottom of planet)")]
        private float _airDensity = 1;

        [SerializeField, Tooltip("if disabled, atmosphere density will be maximal everywhere")]
        private bool _graduateAtmosphere = true;

        [SerializeField, Tooltip("choose degrade shape")]
        private AnimationCurve _atmosphereGradiantShape = AnimationCurve.Linear(0, 0, 1, 1);



        /****** GRAVITY ******/

        [Header("GRAVITY")]

        [SerializeField, Tooltip("mass value to exceed basic rigidbody value")]
        private float _mass;

        [SerializeField, Tooltip("average radius of the planet or object, value used to set default height")]
        private float _averageRadius;

        [SerializeField, Tooltip("radius of the planet at equator level(in m), differ from average radius if _rotationDeformation is enabled")]
        private float _equatorialRadius;

        [SerializeField, Tooltip("radius of the planet at the p�les(in m), differ from average radius if _rotationDeformation is enabled")]
        private float _polarRadius;

        [SerializeField, Tooltip("determine if object should have maximum force strength to apply to other objects")]
        private bool _clampGeneratedForce = false;

        [SerializeField, Tooltip("maximum force strength that object can apply to other objects")]
        private float _maxGeneratedForce = 0;

        [SerializeField, Tooltip("determine if object should clamp every forces apply to it")]
        private bool _clampAffectedForces = false;

        [SerializeField, Tooltip("maximum forces strength that object can receive from other objects")]
        private float _maxAffectedForces = 0;

        [SerializeField, Tooltip("density of mass object(in kg/m3)")]
        private float _matterDensity = 50;

        [SerializeField, Tooltip("mass of current object in kilogram(Kg)")]
        private bool[] _calculationArray = { true, false, false, false };

        [SerializeField, Tooltip("used for debug, will consider gravity is applied as object is on the surface of planet or object")]
        private bool _ignoreDistance = false;

        [SerializeField, Tooltip("if enabled, will automatically update mass when density or scale change")]
        private bool _autoUpdateMass = true;


        /****** DEBUG ******/

        [Header("DEBUG")]

        [SerializeField, Tooltip("determine is, instead of custom inspector, program should render default values for editing at source variables")]
        private bool _drawDefaultInspector = false;

        [SerializeField, Tooltip("if enabled, will render atmosphere preview in scene with volumes")]
        private bool _volumetricAtmospherePreview = true;

        [SerializeField, Tooltip("detect if this isfirst frame the script has been loaded, to implement default components")]
        private OnAwakeValuesAsset _onAwakeAsset;

        #endregion

        #region Private Variables

        /// <summary>
        /// script used to link every Affector with global values
        /// </summary>
        private GlobalValuesManager _globalValues;

        /// <summary>
        /// true value used in engine of used physic system
        /// </summary>
        private PhysicType _usedPhysicSystem = PhysicType.Rigidbody;

        /// <summary>
        /// used in beta switch module for shape calculation type(waiting for enum), put in memory place of boolean in array and discard multiples booleans set to true at the same time.
        /// </summary>
        private int memoCalculation = 0;

        /// <summary>
        /// put in memory if physic of current object is enabled, and reuse it to keep in memory state of object at disable/enable, switch is used to make a Flip Flop gate with physicEnabled boolean. 
        /// </summary>
        private bool physicEnabledMemo = false;
        private bool physicEnabledSwitch = true;

        /// <summary>
        /// list all colliders that collided with atmosphere, is there is no atmosphere, value is null
        /// </summary>
        private Collider[] _listOfObjCollidedWithAtmosphere;

        /// <summary>
        /// if enabled, will show clamp force panels
        /// </summary>
        private bool _clampForces = false;

        /// <summary>
        /// get the mesh of primitive sphere to use it as a gizmo for atmosphere
        /// </summary>
        private Mesh _primitiveSphereMesh;

        private Vector3 _charaControllerCustomVelocity = Vector3.zero;

        private float _averageRadiusMemo = 0;
        private float _matterDensityMemo = 0;

        private RigidbodyPhysicFunctionsCaller _useRbPhysicSystem;
        private CharacterControllerPhysicFunctionsCaller _useCharaControllerPhysicSystem;
        private NativePhysicFunctionsCaller _useNativePhysicSystem;

        #endregion

        #region Public API

        /// <inheritdoc cref="_physicEnabled"/>
        public bool PhysicEnabled
        {
            get { return _physicEnabled; }
            set { _physicEnabled = value; }
        }

        /// <inheritdoc cref="_physicAffectorEnabled"/>
        public bool PhysicAffectorEnabled
        {
            get { return _physicAffectorEnabled; }
            set { _physicAffectorEnabled = value; }
        }

        /// <inheritdoc cref="_rb"/>
        public Rigidbody Rb
        {
            get { return _rb; }
            set { _rb = value; }
        }

        /// <inheritdoc cref="_atmosphereEnabled"/>
        public bool AtmosphereEnabled
        {
            get { return _atmosphereEnabled; }
            set { _atmosphereEnabled = value; }
        }

        /// <inheritdoc cref="_atmosphereRange"/>
        public float AtmosphereRange
        {
            get { return _atmosphereRange; }
            set { _atmosphereRange = value; }
        }

        /// <inheritdoc cref="_airDensity"/>
        public float AirDensity
        {
            get { return _airDensity; }
            set { _airDensity = value; }
        }

        /// <inheritdoc cref="_airResistanceEnabled"/>
        public bool AirResistanceEnabled
        {
            get { return _airResistanceEnabled; }
            set { _airResistanceEnabled = value; }
        }

        /// <inheritdoc cref="_clampGeneratedForce"/>
        public bool ClampGeneratedForce
        {
            get { return _clampGeneratedForce; }
            set { _clampGeneratedForce = value; }
        }

        /// <inheritdoc cref="_maxGeneratedForce"/>
        public float MaxGeneratedForce
        {
            get { return _maxGeneratedForce; }
            set { _maxGeneratedForce = value; }
        }

        /// <inheritdoc cref="_clampAffectedForces"/>
        public bool ClampAffectedForces
        {
            get { return _clampAffectedForces; }
            set { _clampAffectedForces = value; }
        }

        /// <inheritdoc cref="_maxAffectedForces"/>
        public float MaxAffectedForces
        {
            get { return _maxAffectedForces; }
            set { _maxAffectedForces = value; }
        }

        /// <inheritdoc cref="_matterDensity"/>
        public float MatterDensity
        {
            get { return _matterDensity; }
            set { _matterDensity = value; }
        }

        /// <inheritdoc cref="_calculationArray"/>
        public bool[] CalculationArray => _calculationArray;

        /// <inheritdoc cref="_physicEnabled"/>
        public bool DrawDefaultInspector
        {
            get { return _drawDefaultInspector; }
            set { _drawDefaultInspector = value; }
        }

        public float Mass
        {
            get => _mass;
            set => _mass = value;
        }

        /// <inheritdoc cref="_globalValues"/>
        public GlobalValuesManager GlobalValues => _globalValues;

        public bool GraduateAtmosphere
        {
            get => _graduateAtmosphere;
            set => _graduateAtmosphere = value;
        }

        public bool IgnoreDistance
        {
            get => _ignoreDistance;
            set => _ignoreDistance = value;
        }

        /// <inheritdoc cref="_clampForces"/>
        public bool ClampForces
        {
            get => _clampForces;
            set => _clampForces = value;
        }

        ///<inheritdoc cref="_atmosphereGradiantShape"/>
        public AnimationCurve AtmosphereGradiantShape
        {
            get => _atmosphereGradiantShape;
            set => _atmosphereGradiantShape = value;
        }

        ///<inheritdoc cref="_usedPhysicSystemBase"/>
        public PhysicType UsedPhysicSystemBase
        {
            get => _usedPhysicSystemBase;
            set => _usedPhysicSystemBase = value;
        }

        ///<inheritdoc cref="_usedPhysicSystem"/>
        public PhysicType UsedPhysicSystem
        {
            get => _usedPhysicSystem;
            set => _usedPhysicSystem = value;
        }

        public bool RotationDeformation
        {
            get => _rotationDeformation;
            set => _rotationDeformation = value;
        }

        public float RotationDeformationFactor
        {
            get => _rotationDeformationFactor;
            set => _rotationDeformationFactor = value;
        }

        public float AverageRadius
        {
            get => _averageRadius;
            set => _averageRadius = value;
        }

        public float EquatorialRadius
        {
            get => _equatorialRadius;
            set => _equatorialRadius = value;
        }

        public float PolarRadius
        {
            get => _polarRadius;
            set => _polarRadius = value;
        }

        public bool VolumetricAtmospherePreview
        {
            get => _volumetricAtmospherePreview;
            set => _volumetricAtmospherePreview = value;
        }

        public OnAwakeValuesAsset OnAwakeAsset
        {
            get
            {
                if (_onAwakeAsset == null)
                    _onAwakeAsset = new OnAwakeValuesAsset();

                return _onAwakeAsset;
            }
        }

        public CharacterController CharaController
        {
            get => _charaController;
            set => _charaController = value;
        }

        public CustomRigidbody CustomRb
        {
            get => _customRb;
            set => _customRb = value;
        }

        public Vector3 CharaControllerCustomVelocity
        {
            get => _charaControllerCustomVelocity;
            set => _charaControllerCustomVelocity = value;
        }

        public bool AutoUpdateMass
        {
            get => _autoUpdateMass;
            set => _autoUpdateMass = value;
        }

        public enum RigidbodyPhysicFunctionsCaller { }
        public enum CharacterControllerPhysicFunctionsCaller { }
        public enum NativePhysicFunctionsCaller { }

        #endregion

        /*********************************NATIVE METHODS**************************************/

        /// <summary>
        /// called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            physicEnabledMemo = _physicEnabled;
            _averageRadiusMemo = _averageRadius;
            _matterDensityMemo = _matterDensity;

            InitVariables();

            //init fixedList that will be kept all along
            _globalValues.FixedAffectorList.Add(this);

            CheckPhysicEnabled();

            _charaControllerCustomVelocity.x -= 0.00001746202f;
        }

        /// <summary>
        /// called every Physics update
        /// </summary>
        private void FixedUpdate()
        {
            UsedPhysicSystemValueUpdate();

            if (_usedPhysicSystem == PhysicType.Native)
                FixedUpdateNative();
            else if (_usedPhysicSystem == PhysicType.Rigidbody)
                FixedUpdateRb();
            else if (_usedPhysicSystem == PhysicType.CharacterController)
                FixedUpdateCharaController();
        }

        /// <summary>
        /// called every frame
        /// </summary>
        private void Update()
        {
            UsedPhysicSystemValueUpdate();

            if (_usedPhysicSystem == PhysicType.Native)
                UpdateNative();
            else if (_usedPhysicSystem == PhysicType.Rigidbody)
                UpdateRb();
            else if (_usedPhysicSystem == PhysicType.CharacterController)
                UpdateCharaController();
        }

        /// <summary>
        /// called when the object becomes enabled and active
        /// </summary>
        private void OnEnable()
        {
            //keep in memory state of PhysicEnabled to reuse it at disable/enable
            _physicEnabled = physicEnabledMemo;

            CheckPhysicEnabled();
        }

        /// <summary>
        /// called when the behaviour becomes disabled
        /// </summary>
        private void OnDisable()
        {
            //keep in memory state of PhysicEnabled to reuse it at disable/enable
            physicEnabledMemo = _physicEnabled;
            _physicEnabled = false;

            CheckPhysicEnabled();
        }

        /// <summary>
        /// called when unity draw gismos in scene window, only if gameObject is selected in inspector
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (_usedPhysicSystem == PhysicType.Native)
                OnDrawGizmosSelectedNative();
            else if (_usedPhysicSystem == PhysicType.Rigidbody)
                OnDrawGizmosSelectedRb();
            else if (_usedPhysicSystem == PhysicType.CharacterController)
                OnDrawGizmosSelectedCharaController();
        }

        /// <summary>
        /// called when unity draw gismos in scene window
        /// </summary>
        private void OnDrawGizmos()
        {
            UsedPhysicSystemValueUpdate();
            InitVariables();

            if (_usedPhysicSystem == PhysicType.Native)
                OnDrawGizmosNative();
            else if (_usedPhysicSystem == PhysicType.Rigidbody)
                OnDrawGizmosRb();
            else if (_usedPhysicSystem == PhysicType.CharacterController)
                OnDrawGizmosCharaController();
        }


        /*****************************NATIVE METHODS SPLIT********************************/

        #region Rb System Native Functions Split

        /// <summary>
        /// called in FixeUpdate when rigidbody system is used
        /// </summary>
        private void FixedUpdateRb()
        {
            //for each affector enabled and PhysicEnabled, apply forces of current object
            foreach (Affector affector in _globalValues.AffectorsAsset.AffectorList)
                if (affector != this && _physicAffectorEnabled)
                    GravityApply(affector, _useRbPhysicSystem);

            //detect all collisions with atmosphere if there is one
            if (_atmosphereEnabled)
            {
                _listOfObjCollidedWithAtmosphere = Physics.OverlapSphere(transform.TransformPoint(_rb.centerOfMass), _atmosphereRange);

                foreach (Collider collider in _listOfObjCollidedWithAtmosphere)
                    if (collider.gameObject != gameObject)
                        if (collider.gameObject.TryGetComponent(out Affector affectedObj))
                            if (affectedObj.AirResistanceEnabled)
                                AirResistanceApply(affectedObj, _useRbPhysicSystem);
            }

            if (_rotationDeformation)
                RotationDeform(_useRbPhysicSystem);
            else
            {
                _equatorialRadius = _averageRadius;
                _polarRadius = _averageRadius;
            }

            transform.localScale = new Vector3(_equatorialRadius * 2, _polarRadius * 2, _equatorialRadius * 2);
        }

        /// <summary>
        /// called in Update when rigidbody system is used
        /// </summary>
        private void UpdateRb()
        {
            CheckPhysicEnabled();
        }

        /// <summary>
        /// called in OnDrawGizmosSelected when rigidbody system is used
        /// </summary>
        private void OnDrawGizmosSelectedRb()
        {
            if (_atmosphereEnabled && _volumetricAtmospherePreview)
            {
                if (!_primitiveSphereMesh)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    _primitiveSphereMesh = Instantiate(obj.GetComponent<MeshFilter>().sharedMesh);

                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }

                Gizmos.color = Color.blue;

                float maxAtmosphereAltitude = _atmosphereRange - _averageRadius;
                float altitude = maxAtmosphereAltitude;
                int sphereNumber = 100;

                for (int i = 0; i < sphereNumber; i++)
                {

                    float lerpT = _graduateAtmosphere ? 1 - Mathf.Clamp(altitude / maxAtmosphereAltitude, 0, 1) : 1;
                    float evaluatedT = Mathf.Clamp(_atmosphereGradiantShape.FullyClampedEvaluate(lerpT), 0, 1);

                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, (0.5f / (float)sphereNumber) * (evaluatedT / lerpT));

                    Gizmos.DrawMesh(_primitiveSphereMesh, transform.TransformPoint(_rb.centerOfMass), Quaternion.identity, Vector3.one * (altitude + _averageRadius) * 2);

                    altitude -= maxAtmosphereAltitude / sphereNumber;
                }
            }
        }

        /// <summary>
        /// called in OnDrawGizmos when rigidbody system is used
        /// </summary>
        private void OnDrawGizmosRb()
        {
            if (_atmosphereEnabled /*&& UnityEditor.Selection.activeGameObject != gameObject*/)
            {
                Color save = Gizmos.color;
                Gizmos.color = Color.blue;

                Gizmos.DrawWireSphere(transform.TransformPoint(_rb.centerOfMass), _atmosphereRange);

                Gizmos.color = save;
            }

            if (OnAwakeAsset.IsOnFirstScriptInstanceLoading)
                OnScriptLoadingMethods();

            if (!Application.isPlaying)
                PlanetScalePreview();
        }

        #endregion

        #region CharacterController System Native Functions Split

        /// <summary>
        /// called in FixeUpdate when CharacterController system is used
        /// </summary>
        private void FixedUpdateCharaController()
        {
            //for each affector enabled and PhysicEnabled, apply forces of current object
            foreach (Affector affector in _globalValues.AffectorsAsset.AffectorList)
                if (affector != this && _physicAffectorEnabled)
                    GravityApply(affector, _useCharaControllerPhysicSystem);

            //detect all collisions with atmosphere if there is one
            if (_atmosphereEnabled)
            {
                _listOfObjCollidedWithAtmosphere = Physics.OverlapSphere(transform.TransformPoint(_rb.centerOfMass), _atmosphereRange);

                foreach (Collider collider in _listOfObjCollidedWithAtmosphere)
                    if (collider.gameObject != gameObject)
                        if (collider.gameObject.TryGetComponent(out Affector affectedObj))
                            if (affectedObj.AirResistanceEnabled)
                                AirResistanceApply(affectedObj, _useCharaControllerPhysicSystem);
            }

            if (_rotationDeformation)
                RotationDeform(_useCharaControllerPhysicSystem);
            else
            {
                _equatorialRadius = _averageRadius;
                _polarRadius = _averageRadius;
            }

            transform.localScale = new Vector3(_equatorialRadius * 2, _polarRadius * 2, _equatorialRadius * 2);
            _charaController.Move(_charaControllerCustomVelocity * Time.fixedDeltaTime);
        }

        /// <summary>
        /// called in Update when CharacterController system is used
        /// </summary>
        private void UpdateCharaController()
        {
            CheckPhysicEnabled();
        }

        /// <summary>
        /// called in OnDrawGizmosSelected when CharacterController system is used
        /// </summary>
        private void OnDrawGizmosSelectedCharaController()
        {
            if (_atmosphereEnabled && _volumetricAtmospherePreview)
            {
                if (!_primitiveSphereMesh)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    _primitiveSphereMesh = Instantiate(obj.GetComponent<MeshFilter>().sharedMesh);

                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }

                Gizmos.color = Color.blue;

                float maxAtmosphereAltitude = _atmosphereRange - _averageRadius;
                float altitude = maxAtmosphereAltitude;
                int sphereNumber = 100;

                for (int i = 0; i < sphereNumber; i++)
                {

                    float lerpT = _graduateAtmosphere ? 1 - Mathf.Clamp(altitude / maxAtmosphereAltitude, 0, 1) : 1;
                    float evaluatedT = Mathf.Clamp(_atmosphereGradiantShape.FullyClampedEvaluate(lerpT), 0, 1);

                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, (0.5f / (float)sphereNumber) * (evaluatedT / lerpT));

                    Gizmos.DrawMesh(_primitiveSphereMesh, transform.position, Quaternion.identity, Vector3.one * (altitude + _averageRadius) * 2);

                    altitude -= maxAtmosphereAltitude / sphereNumber;
                }
            }
        }

        /// <summary>
        /// called in OnDrawGizmos when CharacterController system is used
        /// </summary>
        private void OnDrawGizmosCharaController()
        {
            if (_atmosphereEnabled && UnityEditor.Selection.activeGameObject != gameObject)
            {
                Color save = Gizmos.color;
                Gizmos.color = Color.blue;

                Gizmos.DrawWireSphere(transform.TransformPoint(_rb.centerOfMass), _atmosphereRange);

                Gizmos.color = save;
            }

            if (OnAwakeAsset.IsOnFirstScriptInstanceLoading)
                OnScriptLoadingMethods();

            if (!Application.isPlaying)
                PlanetScalePreview();
        }

        #endregion

        #region Transform(native) System Native Functions Split

        /// <summary>
        /// called in FixeUpdate when transform(native) system is used
        /// </summary>
        private void FixedUpdateNative()
        {

        }

        /// <summary>
        /// called in Update when transform(native) system is used
        /// </summary>
        private void UpdateNative()
        {

        }

        /// <summary>
        /// called in OnDrawGizmosSelected when transform(native) system is used
        /// </summary>
        private void OnDrawGizmosSelectedNative()
        {

        }

        /// <summary>
        /// called in OnDrawGizmos when transform(native) system is used
        /// </summary>
        private void OnDrawGizmosNative()
        {

        }

        #endregion


        /// <summary>
        /// initialize every variables that, if set to null, can broke code, called in awake and OnInspectorGUI
        /// </summary>
        public void InitVariables()
        {
            UsedPhysicSystemValueUpdate();

            if (OnAwakeAsset.IsOnFirstScriptInstanceLoading)
                OnScriptLoadingMethods();

            //rigidbody Exceptions Manager(force default Rigidbody gravity to disable) only call if usedSystem is Rigidbody, otherwise, exception will mean using transform
            if (_usedPhysicSystemBase == PhysicType.Rigidbody || _usedPhysicSystemBase == PhysicType.Dynamic)
            {
                if (_rb == null)
                    if (!TryGetComponent(out _rb))
                    {
                        //if there is no rigidbody to use.
                        if (_usedPhysicSystemBase == PhysicType.Rigidbody)
                            _rb = gameObject.AddComponent<Rigidbody>();
                        SetMass();
                    }
                _rb.useGravity = false;
            }

            if (_usedPhysicSystemBase == PhysicType.CharacterController || _usedPhysicSystemBase == PhysicType.Dynamic)
            {
                if (_charaController == null)
                    if (!TryGetComponent(out _charaController))
                    {
                        //if there is no character controller to use.
                        if (_usedPhysicSystemBase == PhysicType.CharacterController)
                            _charaController = gameObject.AddComponent<CharacterController>();
                        SetMass();
                    }
            }

            //globalValues Exceptions Manager
            if (_globalValues == null)
                if (!TryFindObjectOfType(out _globalValues))
                {
                    //if there is no GlobalValuesManager usable in Scene.
                    _globalValues = new GameObject("GlobalValuesManager").AddComponent<GlobalValuesManager>();
                    _globalValues.AffectorsAsset = new AffectorsList();
                }

            if (!Application.isPlaying)
                PlanetScalePreview();

            if (_autoUpdateMass)
            {
                if (_matterDensity != _matterDensityMemo || _averageRadius != _averageRadiusMemo)
                    SetMass();

                _matterDensityMemo = _matterDensity;
                _averageRadiusMemo = _averageRadius;
            }
        }

        /// <summary>
        /// update value of used physic to manage dynamic(automatic) physic used
        /// </summary>
        private void UsedPhysicSystemValueUpdate()
        {
            if (_usedPhysicSystemBase == PhysicType.Rigidbody || (_usedPhysicSystemBase == PhysicType.Dynamic && _rb && (!_rb.isKinematic || _rb.isKinematic && !_charaController)))
                _usedPhysicSystem = PhysicType.Rigidbody;
            else if (_usedPhysicSystemBase == PhysicType.CharacterController || (_usedPhysicSystemBase == PhysicType.Dynamic && _charaController))
                _usedPhysicSystem = PhysicType.CharacterController;
            else if (_usedPhysicSystemBase == PhysicType.Native || (_usedPhysicSystemBase == PhysicType.Dynamic && _customRb))
                _usedPhysicSystem = PhysicType.Native;
        }

        /// <summary>
        /// check if object is PhysicEnabled and put it in or out of list depends on it
        /// </summary>
        private void CheckPhysicEnabled()
        {
            List<Affector> listToChange = _globalValues.AffectorsAsset.AffectorList;

            if (_physicEnabled)
            {
                if (physicEnabledSwitch)
                {
                    //when physicEnabled comes to true, called one time, and add object to affector list
                    listToChange.Add(this);
                    physicEnabledSwitch = false;
                }
            }
            else
            {
                if (!physicEnabledSwitch)
                {
                    //when physicEnabled comes to false, called one time, and remove object of affector list
                    listToChange.Remove(this);
                    physicEnabledSwitch = true;
                }
            }
        }

        /// <summary>
        /// called only when script instance is loading, independ of play mode or not
        /// </summary>
        private void OnScriptLoadingMethods()
        {
            OnAwakeAsset.IsOnFirstScriptInstanceLoading = false;

            _averageRadius = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 6f;
            _equatorialRadius = (transform.localScale.x + transform.localScale.z) / 4f;
            _polarRadius = transform.localScale.y / 2f;
            transform.localScale = new Vector3(_equatorialRadius * 2, _polarRadius * 2, _equatorialRadius * 2);

            GenerateComponents();

            _averageRadiusMemo = _averageRadius;
            _matterDensityMemo = _matterDensity;
        }

        /// <summary>
        /// create all default components of affector at loading of script only
        /// </summary>
        private void GenerateComponents()
        {
            //generate rigidbody
            if (_rb == null)
                if (!TryGetComponent(out _rb))
                {
                    //if there is no rigidbody to use.
                    _rb = gameObject.AddComponent<Rigidbody>();
                    SetMass();
                }
            _rb.useGravity = false;
        }

        /// <summary>
        /// precalculate scale before playing
        /// </summary>
        private void PlanetScalePreview()
        {
            _equatorialRadius = _averageRadius;
            _polarRadius = _averageRadius;

            transform.localScale = new Vector3(_equatorialRadius * 2, _polarRadius * 2, _equatorialRadius * 2);
        }


        /*********************************FORCES APPLY********************************/

        #region Rigidbody Forces Calculation And Apply

        /// <summary>
        /// apply gravity forces that current object set to every enabled and physicEnabled objects
        /// </summary>
        /// <param name="affectedObj"> object that will receive forces from current object</param>
        private void GravityApply(Affector affectedObj, RigidbodyPhysicFunctionsCaller physicSystemUsed)
        {
            if (affectedObj.UsedPhysicSystem == PhysicType.Rigidbody)
            {
                //get Rigidbody of object to affect, then calculate direction to apply and distance of objects
                Rigidbody rbToAffect = affectedObj.Rb;

                Vector3 direction = _rb.transform.TransformPoint(_rb.centerOfMass) - rbToAffect.transform.TransformPoint(rbToAffect.centerOfMass);
                float distance = _ignoreDistance ? _averageRadius : direction.magnitude;

                //if the two objects have the same position, stop here.
                if (distance == 0f)
                    return;

                //gravitationalConstant that will be used in calculations
                float G = UPDBMath.G * _globalValues.GravityScale;

                //magnitude force to apply
                float forceMagnitude = G * (_mass * affectedObj.Mass) / Mathf.Pow(distance, 2);

                //clamp force to apply depending on clamp values
                if (_clampGeneratedForce)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, _maxGeneratedForce);
                if (affectedObj._clampAffectedForces)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

                //apply final Vector to rigidBody
                Vector3 force = direction.normalized * forceMagnitude;

                if (affectedObj.Mass == rbToAffect.mass)
                    rbToAffect.AddForce(force);
                else
                    rbToAffect.AddForce(force / (affectedObj.Mass / rbToAffect.mass));
            }
            else if (affectedObj.UsedPhysicSystem == PhysicType.CharacterController)
            {
                Vector3 direction = _rb.transform.TransformPoint(_rb.centerOfMass) - affectedObj.transform.position;
                float distance = _ignoreDistance ? _averageRadius : direction.magnitude;

                //if the two objects have the same position, stop here.
                if (distance == 0f)
                    return;

                //gravitationalConstant that will be used in calculations
                float G = UPDBMath.G * _globalValues.GravityScale;

                //magnitude force to apply
                float forceMagnitude = G * (_mass * affectedObj.Mass) / Mathf.Pow(distance, 2);

                //clamp force to apply depending on clamp values
                if (_clampGeneratedForce)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, _maxGeneratedForce);
                if (affectedObj._clampAffectedForces)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

                //apply final Vector to rigidBody
                Vector3 force = direction.normalized * forceMagnitude;

                affectedObj.CharaControllerCustomVelocity = AddForce(affectedObj.CharaControllerCustomVelocity, force, affectedObj.Mass);
            }
        }

        /// <summary>
        /// apply air resistance forces that current object set to every enabled and airResistanceEnabled objects
        /// </summary>
        /// <param name="affectedObj">object that will receive air resistance forces from current object</param>
        private void AirResistanceApply(Affector affectedObj, RigidbodyPhysicFunctionsCaller physicSystemUsed)
        {
            if (affectedObj.UsedPhysicSystem == PhysicType.Rigidbody)
            {
                //apply air forces.
                Vector3 objVelocity = affectedObj.Rb.linearVelocity;
                float ObjDistance = Vector3.Distance(_rb.transform.TransformPoint(_rb.centerOfMass), affectedObj.Rb.transform.TransformPoint(affectedObj.Rb.centerOfMass));

                float lerpT = _graduateAtmosphere ? 1 - Mathf.Clamp((ObjDistance - _averageRadius) / (_atmosphereRange - _averageRadius), 0, 1) : 1;
                float evaluatedT = Mathf.Clamp(_atmosphereGradiantShape.FullyClampedEvaluate(lerpT), 0, 1);

                float lerp = Mathf.Lerp(0, _airDensity / 5, evaluatedT);

                affectedObj.Rb.AddForce(-objVelocity.normalized * (Mathf.Pow(objVelocity.magnitude, 2) * lerp));
                affectedObj.Rb.AddTorque(-affectedObj.Rb.angularVelocity * (Mathf.Pow(affectedObj.Rb.angularVelocity.magnitude, 2) * lerp));
            }
            else if (affectedObj.UsedPhysicSystem == PhysicType.CharacterController)
            {

            }
        }

        /// <summary>
        /// if called, flatten planet trough it's rotation speed
        /// </summary>
        private void RotationDeform(RigidbodyPhysicFunctionsCaller physicSystemUsed)
        {
            _equatorialRadius = _averageRadius * (1 + (_rb.angularVelocity.magnitude * _rotationDeformationFactor));
            _polarRadius = _averageRadius / (1 + (_rb.angularVelocity.magnitude * _rotationDeformationFactor));
        }

        #endregion

        #region CharacterController Forces Calculation And Apply

        /// <summary>
        /// apply gravity forces that current object set to every enabled and physicEnabled objects
        /// </summary>
        /// <param name="affectedObj"> object that will receive forces from current object</param>
        private void GravityApply(Affector affectedObj, CharacterControllerPhysicFunctionsCaller physicSystemUsed)
        {
            if (affectedObj.UsedPhysicSystem == PhysicType.Rigidbody)
            {
                //get Rigidbody of object to affect, then calculate direction to apply and distance of objects
                Rigidbody rbToAffect = affectedObj.Rb;

                Vector3 direction = transform.position - rbToAffect.transform.TransformPoint(rbToAffect.centerOfMass);
                float distance = _ignoreDistance ? _averageRadius : direction.magnitude;

                //if the two objects have the same position, stop here.
                if (distance == 0f)
                    return;

                //gravitationalConstant that will be used in calculations
                float G = UPDBMath.G * _globalValues.GravityScale;

                //magnitude force to apply
                float forceMagnitude = G * (_mass * affectedObj.Mass) / Mathf.Pow(distance, 2);

                //clamp force to apply depending on clamp values
                if (_clampGeneratedForce)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, _maxGeneratedForce);
                if (affectedObj._clampAffectedForces)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

                //apply final Vector to rigidBody
                Vector3 force = direction.normalized * forceMagnitude;

                if (affectedObj.Mass == rbToAffect.mass)
                    rbToAffect.AddForce(force);
                else
                    rbToAffect.AddForce(force / (affectedObj.Mass / rbToAffect.mass));
            }
            else if (affectedObj.UsedPhysicSystem == PhysicType.CharacterController)
            {
                Vector3 direction = transform.position - affectedObj.transform.position;
                float distance = _ignoreDistance ? _averageRadius : direction.magnitude;

                //if the two objects have the same position, stop here.
                if (distance == 0f)
                    return;

                //gravitationalConstant that will be used in calculations
                float G = UPDBMath.G * _globalValues.GravityScale;

                //magnitude force to apply
                float forceMagnitude = G * (_mass * affectedObj.Mass) / Mathf.Pow(distance, 2);

                //clamp force to apply depending on clamp values
                if (_clampGeneratedForce)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, _maxGeneratedForce);
                if (affectedObj._clampAffectedForces)
                    forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

                //apply final Vector to rigidBody
                Vector3 force = direction.normalized * forceMagnitude;

                affectedObj.CharaControllerCustomVelocity = AddForce(affectedObj.CharaControllerCustomVelocity, force, affectedObj.Mass);
            }
        }

        /// <summary>
        /// apply air resistance forces that current object set to every enabled and airResistanceEnabled objects
        /// </summary>
        /// <param name="affectedObj">object that will receive air resistance forces from current object</param>
        private void AirResistanceApply(Affector affectedObj, CharacterControllerPhysicFunctionsCaller physicSystemUsed)
        {
            if (affectedObj.UsedPhysicSystem == PhysicType.Rigidbody)
            {
                //apply air forces.
                Vector3 objVelocity = affectedObj.Rb.linearVelocity;
                float ObjDistance = Vector3.Distance(_rb.transform.TransformPoint(_rb.centerOfMass), affectedObj.Rb.transform.TransformPoint(affectedObj.Rb.centerOfMass));

                float lerpT = _graduateAtmosphere ? 1 - Mathf.Clamp((ObjDistance - _averageRadius) / (_atmosphereRange - _averageRadius), 0, 1) : 1;
                float evaluatedT = Mathf.Clamp(_atmosphereGradiantShape.FullyClampedEvaluate(lerpT), 0, 1);

                float lerp = Mathf.Lerp(0, _airDensity / 5, evaluatedT);

                affectedObj.Rb.AddForce(-objVelocity.normalized * (Mathf.Pow(objVelocity.magnitude, 2) * lerp));
                affectedObj.Rb.AddTorque(-affectedObj.Rb.angularVelocity * (Mathf.Pow(affectedObj.Rb.angularVelocity.magnitude, 2) * lerp));
            }
            else if (affectedObj.UsedPhysicSystem == PhysicType.CharacterController)
            {

            }
        }

        /// <summary>
        /// if called, flatten planet trough it's rotation speed
        /// </summary>
        private void RotationDeform(CharacterControllerPhysicFunctionsCaller physicSystemUsed)
        {
            _equatorialRadius = _averageRadius * (1 + (_rb.angularVelocity.magnitude * _rotationDeformationFactor));
            _polarRadius = _averageRadius / (1 + (_rb.angularVelocity.magnitude * _rotationDeformationFactor));
        }

        #endregion


        /****************************SHAPE MANAGEMENT******************************/
        #region Shape Management

        /// <summary>
        /// switch calculation mode using switch boolean module
        /// </summary>
        public void switchCalculationMode()
        {
            int calculationCount = 0;
            int arrayEnd = CalculationArray.Length;
            int i = 0;

            for (i = 0; i < arrayEnd; i++)
            {
                if (CalculationArray[i])
                    calculationCount++;

                if (calculationCount > 1)
                {
                    for (int j = 0; j < arrayEnd; j++)
                    {
                        if (j == memoCalculation)
                        {
                            CalculationArray[j] = false;
                            break;
                        }
                    }
                    break;
                }
            }
            for (i = 0; i < arrayEnd; i++)
            {
                if (CalculationArray[i])
                {
                    memoCalculation = i;
                    break;
                }
            }
        }

        /// <summary>
        /// set mass calculated trough matter density and scale of current object
        /// </summary>
        public void SetMass()
        {
            float m3 = 0;
            float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

            //set volume considering a sphere
            if (_calculationArray[0])
                m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;

            //set volume considering a cube
            if (_calculationArray[1])
                m3 = X * Y * Z;

            //set volume considering a cylinder
            if (_calculationArray[2])
                m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

            //set mass with matter density multiplied by volume
            _mass = _matterDensity * m3;
        }

        /// <summary>
        /// set matter density calculated trough mass and scale of current object
        /// </summary>
        public void SetMatterDensity()
        {
            float m3 = 0;
            float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

            //set volume considering a sphere
            if (_calculationArray[0])
                m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;

            //set volume considering a cube
            if (_calculationArray[1])
                m3 = X * Y * Z;

            //set volume considering a cylinder
            if (_calculationArray[2])
                m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

            //apply matter density with mass divided by volume
            _matterDensity = _mass / m3;
        }

        /// <summary>
        /// set scale calculated trough matter density and mass of current object
        /// </summary>
        public void SetScale()
        {
            float m3 = 0;
            float diameter = 0;
            float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

            //set volume with mass divided by matter density
            m3 = _mass / _matterDensity;

            //find XYZ values with volume, considering a perfect sphere with same XYZ values
            if (_calculationArray[0])
                diameter = 2 * Mathf.Pow((m3 * 3) / (4 * Mathf.PI), 1 / 3f);

            //find XYZ values with volume, considering a perfect cube with same XYZ values
            if (_calculationArray[1])
                diameter = Mathf.Pow(m3, 1 / 3f);

            //find XYZ values with volume, considering a perfect cylinder with same XYZ values
            if (_calculationArray[2])
                diameter = Mathf.Pow(m3 / (Mathf.PI / 2), 1 / 3f);

            //set scale
            _averageRadius = diameter / 2;
        }

        #endregion 
    }

    /// <summary>
    /// type of usable physic system
    /// </summary>
    public enum PhysicType
    {
        Rigidbody,
        Native,
        CharacterController,
        Dynamic,
    }
}
