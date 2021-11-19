using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affector : MonoBehaviour
{
    [Header("")]
    [Header("PHYSIC_________________________________________________________________________________________________________")]


    [SerializeField] private bool physicEnabled = true;     
    public bool PhysicEnabled 
    { 
        get { return physicEnabled; } 
        set { physicEnabled = value; } 
    }


    [SerializeField] private bool physicAffectorEnabled = true;     
    public bool PhysicAffectorEnabled 
    { 
        get { return physicAffectorEnabled; } 
        set { physicAffectorEnabled = value; } 
    }


    private bool physicEnabledMemo = false;
    private bool physicEnabledSwitch = true;

    [SerializeField]
    private Rigidbody rb;

    private GlobalValuesManager globalValues;     
    public GlobalValuesManager GlobalValues => globalValues;

    [Header("")]
    [Header("AIR RESISTANCE_________________________________________________________________________________________________________")]


    [SerializeField] private bool atmosphereEnabled = true;    
    public bool AtmosphereEnabled 
    { 
        get { return atmosphereEnabled; } 
        set { atmosphereEnabled = value; } 
    }


    [SerializeField] private float atmosphereRange = 0;     
    public float AtmosphereRange 
    { 
        get { return atmosphereRange; } 
        set { atmosphereRange = value; } 
    }


    [SerializeField] private float airDensity = 1;     
    public float AirDensity 
    { 
        get { return airDensity; } 
        set { airDensity = value; } 
    }


    [SerializeField] private bool airResistanceEnabled = true;     
    public bool AirResistanceEnabled 
    { 
        get { return airResistanceEnabled; } 
        set { airResistanceEnabled = value; } 
    }


    [Header("")]
    [Header("GRAVITY_________________________________________________________________________________________________________")]


    [SerializeField] private bool clampGeneratedForce = false;
    public bool ClampGeneratedForce
    {
        get { return clampGeneratedForce; }
        set { clampGeneratedForce = value; }
    }


    [SerializeField] private float maxGeneratedForce = 0;
    public float MaxGeneratedForce
    {
        get { return maxGeneratedForce; }
        set { maxGeneratedForce = value; }
    }


    [SerializeField] private bool clampAffectedForces = false;
    public bool ClampAffectedForces
    {
        get { return clampAffectedForces; }
        set { clampAffectedForces = value; }
    }


    [SerializeField] private float maxAffectedForces = 0;
    public float MaxAffectedForces
    {
        get { return maxAffectedForces; }
        set { maxAffectedForces = value; }
    }


    [SerializeField] private float matterDensity = 1.90986f;     
    public float MatterDensity 
    { 
        get { return matterDensity; } 
        set { matterDensity = value; } 
    }


    [SerializeField] private bool[] calculationArray = { true, false, false, false } ;
    public bool[] CalculationArray => calculationArray ;


    [Header("")]
    [Header("CUSTOM INSPECTOR_________________________________________________________________________________________________________")]


    [SerializeField] private bool drawDefaultInspector = false;
    public bool DrawDefaultInspector
    {
        get { return drawDefaultInspector; }
        set { drawDefaultInspector = value; }
    }


    private int memoCalculation = 0;

    const float gravitationConstant = 0.0000000000667f;

    private void Awake()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody>();

        globalValues = FindObjectOfType<GlobalValuesManager>();
        globalValues.FixedAffectorList.Add(this);

        physicEnabledMemo = physicEnabled;
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

    #region Forces Calculation And Apply
    private void GravityApply(Affector affectedObj)
    {
        Rigidbody rbToAffect = affectedObj.rb;

        Vector3 direction = rb.position - rbToAffect.position;
        float distance = direction.magnitude;

        if (distance == 0f)
            return;

        float G = gravitationConstant * globalValues.GravityScale;

        float forceMagnitude = G * (rb.mass * rbToAffect.mass) / Mathf.Pow(distance, 2);

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
            affectedObj.rb.AddForce(-affectedObj.rb.velocity * airDensity);
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
        rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        if (calculationArray[0])
            m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;

        if (calculationArray[1])
            m3 = X * Y * Z;

        if (calculationArray[2])
            m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

        rb.mass = matterDensity * m3;
    }

    public void SetMatterDensity()
    {
        rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        if (calculationArray[0])
            m3 = ((4 * Mathf.PI) * (Mathf.Pow(X / 2, 3))) / 3;
        if (calculationArray[1])
            m3 = X * Y * Z;
        if (calculationArray[2])
            m3 = Mathf.PI * ((X / 2) * (Z / 2)) * (Y * 2);

        matterDensity = rb.mass / m3;
    }

    public void SetScale()
    {
        rb = GetComponent<Rigidbody>();
        float m3 = 0;
        float diameter = 0;
        float X = transform.localScale.x; float Y = transform.localScale.y; float Z = transform.localScale.z;

        m3 = rb.mass / matterDensity;

        if (calculationArray[0])
            diameter = 2 * Mathf.Pow((m3 * 3) / (4 * Mathf.PI), 1 / 3f);
        if (calculationArray[1])
            diameter = Mathf.Pow(m3, 1 / 3f);
        if (calculationArray[2])
            diameter = Mathf.Pow(m3 / (Mathf.PI / 2), 1 / 3f);

        transform.localScale = new Vector3(diameter, diameter, diameter);
    } 
    #endregion
}
