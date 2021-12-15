using System.Collections.Generic;
using UnityEngine;

public class Affector : MonoBehaviour
{
    #region Serialized And Public Variables

    #region PHYSIC

    [Header("PHYSIC")]

    #region physicEnabled

    [SerializeField, Tooltip("determine if object is in the list and is detected")]
    private bool physicEnabled = true;
    public bool PhysicEnabled
    {
        get { return physicEnabled; }
        set { physicEnabled = value; }
    }

    #endregion

    #region physicAffectorEnabled

    [SerializeField, Tooltip("determine if object generate gravity and apply it to other object")]
    private bool physicAffectorEnabled = true;
    public bool PhysicAffectorEnabled
    {
        get { return physicAffectorEnabled; }
        set { physicAffectorEnabled = value; }
    }

    #endregion

    #region rb

    [SerializeField, Tooltip("rigidbody used to calculate physics for current object")]
    private Rigidbody _rb;
    public Rigidbody Rb
    {
        get { return _rb; }
        set { _rb = value; }
    }

    #endregion

    #region collider

    [SerializeField, Tooltip("collider used by object")]
    private Collider _collider;
    public Collider Collider
    {
        get { return _collider; }
        set { _collider = value; }
    }

    #endregion

    #endregion

    #region AIR RESISTANCE

    [Header("AIR RESISTANCE")]

    #region atmosphereEnabled

    [SerializeField, Tooltip("determine if object as atmosphere and generate air resistance")]
    private bool atmosphereEnabled = true;
    public bool AtmosphereEnabled
    {
        get { return atmosphereEnabled; }
        set { atmosphereEnabled = value; }
    }

    #endregion

    #region atmosphereRange

    [SerializeField, Tooltip("range of object atmosphere")]
    private float atmosphereRange = 0;
    public float AtmosphereRange
    {
        get { return atmosphereRange; }
        set { atmosphereRange = value; }
    }

    #endregion

    #region airDensity

    [SerializeField, Tooltip("density of air and strength of resistance")]
    private float airDensity = 1;
    public float AirDensity
    {
        get { return airDensity; }
        set { airDensity = value; }
    }

    #endregion

    #region airResistanceEnabled

    [SerializeField, Tooltip("determine if the object can detect and calculate its air resistance")]
    private bool airResistanceEnabled = true;
    public bool AirResistanceEnabled
    {
        get { return airResistanceEnabled; }
        set { airResistanceEnabled = value; }
    }

    #endregion

    #endregion

    #region GRAVITY

    [Header("GRAVITY")]

    #region clampGeneratedForce

    [SerializeField, Tooltip("determine if object should have maximum force strength to apply to other objects")]
    private bool clampGeneratedForce = false;
    public bool ClampGeneratedForce
    {
        get { return clampGeneratedForce; }
        set { clampGeneratedForce = value; }
    }

    #endregion

    #region maxGeneratedForce

    [SerializeField, Tooltip("maximum force strength that object can apply to other objects")]
    private float maxGeneratedForce = 0;
    public float MaxGeneratedForce
    {
        get { return maxGeneratedForce; }
        set { maxGeneratedForce = value; }
    }

    #endregion

    #region clampAffectedForces

    [SerializeField, Tooltip("determine if object should clamp every forces apply to it")]
    private bool clampAffectedForces = false;
    public bool ClampAffectedForces
    {
        get { return clampAffectedForces; }
        set { clampAffectedForces = value; }
    }

    #endregion

    #region maxAffectedForces

    [SerializeField, Tooltip("maximum forces strength that object can receive from other objects")]
    private float maxAffectedForces = 0;
    public float MaxAffectedForces
    {
        get { return maxAffectedForces; }
        set { maxAffectedForces = value; }
    }

    #endregion

    #region matterDensity

    [SerializeField, Tooltip("density of mass object(in kg/m3)")]
    private float matterDensity = 50;
    public float MatterDensity
    {
        get { return matterDensity; }
        set { matterDensity = value; }
    }

    #endregion

    #region calculationArray

    [SerializeField, Tooltip("mass of current object in kilogram(Kg)")]
    private bool[] calculationArray = { true, false, false, false };
    public bool[] CalculationArray => calculationArray;

    #endregion

    #endregion

    #region CUSTOM INSPECTOR

    [Header("CUSTOM INSPECTOR")]

    #region drawDefaultInspector

    [SerializeField, Tooltip("determine is, instead of custom inspector, program should render default values for editing at source variables")]
    private bool drawDefaultInspector = false;
    public bool DrawDefaultInspector
    {
        get { return drawDefaultInspector; }
        set { drawDefaultInspector = value; }
    }

    #endregion

    #endregion

    #endregion

    #region Private Variables

    #region SCRIPTS
    //script used to link every Affector with global values
    private GlobalValuesManager globalValues;
    public GlobalValuesManager GlobalValues => globalValues;

    #endregion

    #region PHYSIC

    //used in beta switch module for shape calculation type(waiting for enum), put in memory place of boolean in array and discard multiples booleans set to true at the same time.
    private int memoCalculation = 0;

    //put in memory if physic of current object is enabled, and reuse it to keep in memory state of object at disable/enable, switch is used to make a Flip Flop gate with physicEnabled boolean. 
    private bool physicEnabledMemo = false;
    private bool physicEnabledSwitch = true;

    #endregion

    #region GRAVITY

    //gravitationnal constant(reel one) that will be scaled by globalValuesManager to increase speed of simulation(with a scale of 1, real simulations are extremely slow !)
    const float gravitationConstant = 0.0000000000667f;

    #endregion

    #endregion

    //called when the script instance is being loaded
    private void Awake()
    {
        physicEnabledMemo = physicEnabled;
        InitVariables();

        //init fixedList that will be kept all along
        globalValues.FixedAffectorList.Add(this);

        CheckPhysicEnabled();
    }

    //called every Physics update
    private void FixedUpdate()
    {
        //for each affector enabled and PhysicEnabled, apply forces of current object
        foreach (Affector affector in globalValues.AffectorsAsset.AffectorList)
        {
            if (affector != this && physicAffectorEnabled)
            {
                GravityApply(affector);
                AirResistanceApply(affector);
            }
        }
    }

    //called every frame
    private void Update()
    {
        CheckPhysicEnabled();
    }

    //called when the object becomes enabled and active
    private void OnEnable()
    {
        //keep in memory state of PhysicEnabled to reuse it at disable/enable
        physicEnabled = physicEnabledMemo;

        CheckPhysicEnabled();
    }

    //called when the behaviour becomes disabled
    private void OnDisable()
    {
        //keep in memory state of PhysicEnabled to reuse it at disable/enable
        physicEnabledMemo = physicEnabled;
        physicEnabled = false;

        CheckPhysicEnabled();
    }

    //initialize every variables that, if set to null, can broke code, called in awake and OnInspectorGUI
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
        if (globalValues == null)
            if(!TryFindObjectOfType(out globalValues))
            {
                //if there is no GlobalValuesManager usable in Scene.
                globalValues = new GameObject("GlobalValuesManager").AddComponent<GlobalValuesManager>();
                globalValues.AffectorsAsset = new AffectorsList();
            }

        //collider Exceptions Manager
        if(_collider == null)
            if (!TryGetComponent(out _collider))
                _collider = gameObject.AddComponent<SphereCollider>();
    }

    #region Forces Calculation And Apply

    //apply gravity forces that current object set to every enabled and physicEnabled objects
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
        float G = gravitationConstant * globalValues.GravityScale;

        //magnitude force to apply
        float forceMagnitude = G * (_rb.mass * rbToAffect.mass) / Mathf.Pow(distance, 2);

        //clamp force to apply depending on clamp values
        if(clampGeneratedForce)
            forceMagnitude = Mathf.Clamp(forceMagnitude, 0, maxGeneratedForce);
        if(affectedObj.clampAffectedForces)
            forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

        //apply final Vector to rigidBody
        Vector3 force = direction.normalized * forceMagnitude;
        rbToAffect.AddForce(force);
    }

    //apply air resistance forces that current object set to every enabled and airResistanceEnabled objects
    private void AirResistanceApply(Affector affectedObj)
    {
        //get distance of two objects, if affected object is in range, and is detectable for airResistance.
        float distanceObj = Vector3.Distance(transform.position, affectedObj.transform.position);
        bool isInRange = distanceObj <= atmosphereRange;
        bool detectAirResistance = affectedObj.airResistanceEnabled;

        //if object is in range and affector has atmosphere and affected object can detect air Resistance, apply air forces.
        if (isInRange && atmosphereEnabled && detectAirResistance)
            affectedObj._rb.AddForce(-affectedObj._rb.velocity * airDensity);
    }

    //check if object is PhysicEnabled and put it in or out of list depends on it
    private void CheckPhysicEnabled()
    {
        List<Affector> listToChange = globalValues.AffectorsAsset.AffectorList;

        if (physicEnabled)
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

    //switch calculation mode using switch boolean module
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

    //set mass calculated trough matter density and scale of current object
    public void SetMass()
    {
        _rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        //set volume considering a sphere
        if (calculationArray[0])
            m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;

        //set volume considering a cube
        if (calculationArray[1])
            m3 = X * Y * Z;

        //set volume considering a cylinder
        if (calculationArray[2])
            m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

        //set mass with matter density multiplied by volume
        _rb.mass = matterDensity * m3;
    }

    //set matter density calculated trough mass and scale of current object
    public void SetMatterDensity()
    {
        _rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        //set volume considering a sphere
        if (calculationArray[0])
            m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;

        //set volume considering a cube
        if (calculationArray[1])
            m3 = X * Y * Z;

        //set volume considering a cylinder
        if (calculationArray[2])
            m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

        //apply matter density with mass divided by volume
        matterDensity = _rb.mass / m3;
    }

    //set scale calculated trough matter density and mass of current object
    public void SetScale()
    {
        _rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float diameter = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        //set volume with mass divided by matter density
        m3 = _rb.mass / matterDensity;

        //find XYZ values with volume, considering a perfect sphere with same XYZ values
        if (calculationArray[0])
            diameter = 2 * Mathf.Pow((m3 * 3) / (4 * Mathf.PI), 1 / 3f);

        //find XYZ values with volume, considering a perfect cube with same XYZ values
        if (calculationArray[1])
            diameter = Mathf.Pow(m3, 1 / 3f);

        //find XYZ values with volume, considering a perfect cylinder with same XYZ values
        if (calculationArray[2])
            diameter = Mathf.Pow(m3 / (Mathf.PI / 2), 1 / 3f);

        //set scale
        transform.localScale = new Vector3(diameter, diameter, diameter);
    }

    #endregion

    //try to find Object, and, if not, let an exception parameter
    private bool TryFindObjectOfType(out GlobalValuesManager variable)
    {
        if (FindObjectOfType<GlobalValuesManager>() != null)
        {
            //if usable GlobalValuesManager exist, affect it and return true
            variable = FindObjectOfType<GlobalValuesManager>();
            return true;
        }
        else
        {
            //if there is no usable Globalvaluesmanager, returen false
            variable = null;
            return false;
        }
    }
}
