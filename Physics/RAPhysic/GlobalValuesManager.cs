using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.physic.RAPhysic
{
    /// <summary>
    /// manage global values and lists of Affector from RAPhysic (break without it, Affector put it in scene Automatically)
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/Physics/RAPhysic/README.md"), AddComponentMenu("UPDB/Physics/RAPhysics/Global Values Manager")]
    public class GlobalValuesManager : MonoBehaviour
    {
        #region Serialized And Public Variables

        [SerializeField, Tooltip("enable/disable physics for every affectors")]
        private bool _physicGlobalEnabled = false;

        [SerializeField, Tooltip("the list where this object will be stored and called(in a scriptableObject)")]
        private AffectorsList _affectorsList;

        [SerializeField, HideInInspector]
        private List<Affector> _fixedAffectorList;

        [SerializeField, Tooltip("Gravitational scale for constant G, x means that a 1 meter object will render gravity as a x meter object VALUE 1 IS THE ACCURATE SIMULATION OF REAL WORLD")]
        private float _gravityScale = 1000000000;

        #endregion

        #region Private Variables

        /// <summary>
        /// switch used to make a flip flop gate with _physicGlobalEnabled
        /// </summary>
        private bool physicGlobalEnabledSwitch = false;

        #endregion

        #region Accessors

        /// <inheritdoc cref="_affectorsList"/>
        public AffectorsList AffectorsAsset
        {
            get { return _affectorsList; }
            set { _affectorsList = value; }
        }


        /// <inheritdoc cref="_fixedAffectorList"/>
        public List<Affector> FixedAffectorList
        {
            get { return _fixedAffectorList; }
            set { _fixedAffectorList = value; }
        }


        /// <inheritdoc cref="_gravityScale"/>
        public float GravityScale => _gravityScale;

        #endregion

        /// <summary>
        /// called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            //init switch of physicGlobalEnabled
            if (_physicGlobalEnabled) 
                physicGlobalEnabledSwitch = false;
            else 
                physicGlobalEnabledSwitch = true;
        }

        /// <summary>
        /// called every frame
        /// </summary>
        private void Update()
        {
            PhysicEnableOrDisableAll();
        }

        /// <summary>
        /// enable or disable physic for all bojects in scene
        /// </summary>
        private void PhysicEnableOrDisableAll()
        {
            if (_physicGlobalEnabled)
            {
                if (physicGlobalEnabledSwitch)
                {
                    foreach (Affector affector in _fixedAffectorList)
                    {
                        affector.PhysicEnabled = _physicGlobalEnabled;
                    }

                    physicGlobalEnabledSwitch = false;
                }
            }
            else
            {
                if (!physicGlobalEnabledSwitch)
                {
                    foreach (Affector affector in _fixedAffectorList)
                    {
                        affector.PhysicEnabled = _physicGlobalEnabled;
                    }

                    physicGlobalEnabledSwitch = true;
                }
            }
        }
    }
}