using System.Collections.Generic;
using UnityEngine;

public class GlobalValuesManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("enable/disable physics for every affectors")]
    private bool physicGlobalEnabled = false;

    private bool physicGlobalEnabledSwitch = false;

    [SerializeField]
    [Tooltip("the list where this object will be stored and called(in a scriptableObject)")]
    private AffectorsList affectorsList;    public AffectorsList AffectorsAsset => affectorsList;

    [SerializeField]
    [HideInInspector]
    private List<Affector> fixedAffectorList;
    public List<Affector> FixedAffectorList
    {
        get { return fixedAffectorList; }
        set { fixedAffectorList = value; }
    }

    [SerializeField]
    [Tooltip("Gravitational scale for constant G, x means that a 1 meter object will render gravity as a x meter object VALUE 1 IS THE ACCURATE SIMULATION OF REAL WORLD")]
    private float _gravityScale = 1000;    public float GravityScale => _gravityScale; 

    private void Awake()
    {
        if (physicGlobalEnabled) physicGlobalEnabledSwitch = false; 
        else physicGlobalEnabledSwitch = true;
    }

    private void Update()
    {
        PhysicEnableOrDisableAll();
    }

    private void PhysicEnableOrDisableAll()
    {
        if (physicGlobalEnabled)
        {
            if (physicGlobalEnabledSwitch)
            {
                foreach(Affector affector in fixedAffectorList)
                {
                    affector.PhysicEnabled = physicGlobalEnabled;
                }

                physicGlobalEnabledSwitch = false;
            }
        }
        else
        {
            if (!physicGlobalEnabledSwitch)
            {
                foreach (Affector affector in fixedAffectorList)
                {
                    affector.PhysicEnabled = physicGlobalEnabled;
                }

                physicGlobalEnabledSwitch = true;
            }
        }
    }
}
