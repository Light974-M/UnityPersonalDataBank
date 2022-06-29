using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName ="new RangeMixerGroup", menuName ="UPDB/Sound/AmbianceMixer/RangeMixerGroup")]
    public class RangeMixerGroup : ScriptableObject
    {
        [SerializeField, Tooltip("config used by group, when assigned, override element config")]
        private AudioRandomizerConfig _config = new AudioRandomizerConfig(null);

        /// <summary>
        /// make a tooltip that explain audioRandomizer Group on label of list element 
        /// </summary>
        private string _propertyTooltip = "";

        /// <summary>
        /// use this to change color of font when display element on custom property group
        /// </summary>
        private Color _propertyFontColor = new Color(0, 0, 0, 0.3f);

        /// <summary>
        /// use this ti change color of every text fields in custom property group
        /// </summary>
        private Color _propertyTextColor = Color.white;

        #region Public API

        public AudioRandomizerConfig Config
        {
            get { return _config; }
            set { _config = value; }
        }


        ///<inheritdoc cref="_propertyTooltip"/>
        public string PropertyTooltip
        {
            get { return _propertyTooltip; }
            set { _propertyTooltip = value; }
        }

        ///<inheritdoc cref="_propertyFontColor"/>
        public Color PropertyFontColor
        {
            get { return _propertyFontColor; }
            set { _propertyFontColor = value; }
        }

        ///<inheritdoc cref="_propertyTextColor"/>
        public Color PropertyTextColor
        {
            get { return _propertyTextColor; }
            set { _propertyTextColor = value; }
        }

        #endregion
    } 
}
