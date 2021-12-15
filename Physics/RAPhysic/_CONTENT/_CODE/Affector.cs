using System.Collections.Generic;
using UnityEngine;

namespace UPDB.physic.RAPhysic
{
    /// <summary>
    /// main class of RAPhysic
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/Physics/RAPhysic/README.md"), AddComponentMenu("UPDB/Physics/RAPhysics/RAPhysics Affector")]
    public class Affector : MonoBehaviour
    {
        #region Serialized And Public Variables

        /****** PHYSIC ******/

        [Header("PHYSIC")]

        [SerializeField, Tooltip("determine if object is in the list and is detected")]
        private bool _physicEnabled = true;

        [SerializeField, Tooltip("determine if object generate gravity and apply it to other object")]
        private bool _physicAffectorEnabled = true;

        [SerializeField, Tooltip("rigidbody used to calculate physics for current object")]
        private Rigidbody _rb;

        [SerializeField, Tooltip("collider used by object")]
        private Collider _collider;


        /****** AIR RESISTANCE ******/

        [Header("AIR RESISTANCE")]

        [SerializeField, Tooltip("determine if object as atmosphere and generate air resistance")]
        private bool _atmosphereEnabled = true;

        [SerializeField, Tooltip("range of object atmosphere")]
        private float _atmosphereRange = 0;

        [SerializeField, Tooltip("density of air and strength of resistance")]
        private float _airDensity = 1;

        [SerializeField, Tooltip("determine if the object can detect and calculate its air resistance")]
        private bool _airResistanceEnabled = true;


        /****** GRAVITY ******/

        [Header("GRAVITY")]

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


        /****** CUSTOM INSPECTOR ******/

        [Header("CUSTOM INSPECTOR")]

        [SerializeField, Tooltip("determine is, instead of custom inspector, program should render default values for editing at source variables")]
        private bool _drawDefaultInspector = false;

        #endregion

        #region Private Variables

        /// <summary>
        /// script used to link every Affector with global values
        /// </summary>
        private GlobalValuesManager _globalValues;

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
        /// gravitationnal constant(reel one) that will be scaled by globalValuesManager to increase speed of simulation(with a scale of 1, real simulations are extremely slow !)
        /// </summary>
        const float gravitationConstant = 0.0000000000667f;

        #endregion

        #region Accessor

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


        /// <inheritdoc cref="_collider"/>
        public Collider Collider
        {
            get { return _collider; }
            set { _collider = value; }
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


        /// <inheritdoc cref="_globalValues"/>
        public GlobalValuesManager GlobalValues => _globalValues;

        #endregion

        /// <summary>
        /// called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            physicEnabledMemo = _physicEnabled;
            InitVariables();

            //init fixedList that will be kept all along
            _globalValues.FixedAffectorList.Add(this);

            CheckPhysicEnabled();
        }

        /// <summary>
        /// called every Physics update
        /// </summary>
        private void FixedUpdate()
        {
            //for each affector enabled and PhysicEnabled, apply forces of current object
            foreach (Affector affector in _globalValues.AffectorsAsset.AffectorList)
            {
                if (affector != this && _physicAffectorEnabled)
                {
                    GravityApply(affector);
                    AirResistanceApply(affector);
                }
            }
        }

        /// <summary>
        /// called every frame
        /// </summary>
        private void Update()
        {
            CheckPhysicEnabled();
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
        /// initialize every variables that, if set to null, can broke code, called in awake and OnInspectorGUI
        /// </summary>
        public void InitVariables()
        {
            //rigidbody Exceptions Manager(force default Rigidbody gravity to disable)
            if (_rb == null)
                if (!TryGetComponent(out _rb))
                {
                    //if there is no rigidbody to use.
                    _rb = gameObject.AddComponent<Rigidbody>();
                    _rb.mass = 26.17994f;
                }
            _rb.useGravity = false;

            //globalValues Exceptions Manager
            if (_globalValues == null)
                if (!UsableMethods.TryFindObjectOfType(out _globalValues))
                {
                    //if there is no GlobalValuesManager usable in Scene.
                    _globalValues = new GameObject("GlobalValuesManager").AddComponent<GlobalValuesManager>();
                    _globalValues.AffectorsAsset = new AffectorsList();
                }

            //collider Exceptions Manager
            if (_collider == null)
                if (!TryGetComponent(out _collider))
                    _collider = gameObject.AddComponent<SphereCollider>();
        }

        #region Forces Calculation And Apply

        /// <summary>
        /// apply gravity forces that current object set to every enabled and physicEnabled objects
        /// </summary>
        /// <param name="affectedObj"> object that will receive forces from current object</param>
        private void GravityApply(Affector affectedObj)
        {
            //get Rigidbody of object to affect, then calculate direction to apply and distance of objects
            Rigidbody rbToAffect = affectedObj._rb;

            Vector3 direction = _rb.position - rbToAffect.position;
            float distance = direction.magnitude;

            //if the two objects have the same position, stop here.
            if (distance == 0f)
                return;

            //gravitationalConstant that will be used in calculations
            float G = gravitationConstant * _globalValues.GravityScale;

            //magnitude force to apply
            float forceMagnitude = G * (_rb.mass * rbToAffect.mass) / Mathf.Pow(distance, 2);

            //clamp force to apply depending on clamp values
            if (_clampGeneratedForce)
                forceMagnitude = Mathf.Clamp(forceMagnitude, 0, _maxGeneratedForce);
            if (affectedObj._clampAffectedForces)
                forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

            //apply final Vector to rigidBody
            Vector3 force = direction.normalized * forceMagnitude;
            rbToAffect.AddForce(force);
        }

        /// <summary>
        /// apply air resistance forces that current object set to every enabled and airResistanceEnabled objects
        /// </summary>
        /// <param name="affectedObj">object that will receive air resistance forces from current object</param>
        private void AirResistanceApply(Affector affectedObj)
        {
            //get distance of two objects, if affected object is in range, and is detectable for airResistance.
            float distanceObj = Vector3.Distance(transform.position, affectedObj.transform.position);
            bool isInRange = distanceObj <= _atmosphereRange;
            bool detectAirResistance = affectedObj._airResistanceEnabled;

            //if object is in range and affector has atmosphere and affected object can detect air Resistance, apply air forces.
            if (isInRange && _atmosphereEnabled && detectAirResistance)
                affectedObj._rb.AddForce(-affectedObj._rb.velocity * _airDensity);
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
                    listToChange.Add(this); physicEnabledSwitch = false;
                }
            }
            else
            {
                if (!physicEnabledSwitch)
                {
                    //when physicEnabled comes to false, called one time, and remove object of affector list
                    listToChange.Remove(this); physicEnabledSwitch = true;
                }
            }
        }

        #endregion

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
            _rb = GetComponent<Rigidbody>();
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
            _rb.mass = _matterDensity * m3;
        }

        /// <summary>
        /// set matter density calculated trough mass and scale of current object
        /// </summary>
        public void SetMatterDensity()
        {
            _rb = GetComponent<Rigidbody>();
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
            _matterDensity = _rb.mass / m3;
        }

        /// <summary>
        /// set scale calculated trough matter density and mass of current object
        /// </summary>
        public void SetScale()
        {
            _rb = GetComponent<Rigidbody>();
            float m3 = 0;
            float diameter = 0;
            float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

            //set volume with mass divided by matter density
            m3 = _rb.mass / _matterDensity;

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
            transform.localScale = new Vector3(diameter, diameter, diameter);
        }

        #endregion
    } 
}
