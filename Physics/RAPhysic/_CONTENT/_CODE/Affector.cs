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
    [SerializeField]
    private GlobalValuesManager globalValues;
    public GlobalValuesManager GlobalValues => globalValues;

    #endregion

    #region PHYSIC

    private int memoCalculation = 0;

    private bool physicEnabledMemo = false;
    private bool physicEnabledSwitch = true;
    private bool firstFrame = true;

    #endregion

    #region GRAVITY

    const float gravitationConstant = 0.0000000000667f; 

    #endregion

    #endregion

    private void Awake()
    {
        physicEnabledMemo = physicEnabled;

        InitVariables();

        globalValues.FixedAffectorList.Add(this);
        CheckPhysicEnabled();
    }

    private void FixedUpdate()
    {
        foreach (Affector affector in globalValues.AffectorsAsset.AffectorList)
        {
            if (affector != this && physicAffectorEnabled)
            {
                GravityApply(affector);
                AirResistanceApply(affector);
            }
        }
    }

    private void Update()
    {
        CheckPhysicEnabled();
    }

    private void OnEnable()
    {
        physicEnabled = physicEnabledMemo;
        CheckPhysicEnabled();
    }

    private void OnDisable()
    {
        physicEnabledMemo = physicEnabled;
        physicEnabled = false;
        CheckPhysicEnabled();
    }

    public void InitVariables()
    {
        if (_rb == null)
            if (!TryGetComponent(out _rb))
            {
                _rb = gameObject.AddComponent<Rigidbody>();
                _rb.mass = 26.17994f;
            }
        _rb.useGravity = false;

        if (globalValues == null)
            if(!TryFindObjectOfType(out globalValues))
            {
                globalValues = new GameObject("GlobalValuesManager").AddComponent<GlobalValuesManager>();
                globalValues.AffectorsAsset = new AffectorsList();
            }

        if(_collider == null)
            if (!TryGetComponent(out _collider))
                _collider = gameObject.AddComponent<SphereCollider>();
    }

    #region Forces Calculation And Apply
    private void GravityApply(Affector affectedObj)
    {
        Rigidbody rbToAffect = affectedObj._rb;

        Vector3 direction = _rb.position - rbToAffect.position;
        float distance = direction.magnitude;

        if (distance == 0f)
            return;

        float G = gravitationConstant * globalValues.GravityScale;

        float forceMagnitude = G * (_rb.mass * rbToAffect.mass) / Mathf.Pow(distance, 2);

        if(clampGeneratedForce)
            forceMagnitude = Mathf.Clamp(forceMagnitude, 0, maxGeneratedForce);
        if(affectedObj.clampAffectedForces)
            forceMagnitude = Mathf.Clamp(forceMagnitude, 0, affectedObj.MaxAffectedForces);

        Vector3 force = direction.normalized * forceMagnitude;
        rbToAffect.AddForce(force);
    }

    private void AirResistanceApply(Affector affectedObj)
    {
        float distanceObj = Vector3.Distance(transform.position, affectedObj.transform.position);
        bool isInRange = distanceObj <= atmosphereRange;
        bool detectAirResistance = affectedObj.airResistanceEnabled;

        if (isInRange && atmosphereEnabled && detectAirResistance)
            affectedObj._rb.AddForce(-affectedObj._rb.velocity * airDensity);
    }

    private void CheckPhysicEnabled()
    {
        List<Affector> listToChange = globalValues.AffectorsAsset.AffectorList;

        if (physicEnabled)
        {
            if (physicEnabledSwitch)
            {
                listToChange.Add(this); physicEnabledSwitch = false;
            }
        }
        else
        {
            if (!physicEnabledSwitch)
            {
                listToChange.Remove(this); physicEnabledSwitch = true;
            }
        }
    }
    #endregion

    #region Shape Management
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

    public void SetMass()
    {
        _rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        if (calculationArray[0])
            m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;

        if (calculationArray[1])
            m3 = X * Y * Z;

        if (calculationArray[2])
            m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

        _rb.mass = matterDensity * m3;
    }

    public void SetMatterDensity()
    {
        _rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        if (calculationArray[0])
            m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;
        if (calculationArray[1])
            m3 = X * Y * Z;
        if (calculationArray[2])
            m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

        matterDensity = _rb.mass / m3;
    }

    public void SetScale()
    {
        _rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float diameter = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        m3 = _rb.mass / matterDensity;

        if (calculationArray[0])
            diameter = 2 * Mathf.Pow((m3 * 3) / (4 * Mathf.PI), 1 / 3f);
        if (calculationArray[1])
            diameter = Mathf.Pow(m3, 1 / 3f);
        if (calculationArray[2])
            diameter = Mathf.Pow(m3 / (Mathf.PI / 2), 1 / 3f);

        transform.localScale = new Vector3(diameter, diameter, diameter);
    }
    #endregion

    private bool TryFindObjectOfType(out GlobalValuesManager variable)
    {
        if (FindObjectOfType<GlobalValuesManager>() != null)
        {
            variable = FindObjectOfType<GlobalValuesManager>();
            return true;
        }
        else
        {
            variable = null;
            return false;
        }
    }
}
