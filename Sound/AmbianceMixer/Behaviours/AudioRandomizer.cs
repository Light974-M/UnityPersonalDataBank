using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// Component to manage audioSource, when called, take randomly a song in its song list and play it
    /// </summary>
    [AddComponentMenu("UPDB/Sound/AmbianceMixer/Audio Randomizer"), HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    public class AudioRandomizer : UPDBBehaviour
    {
        /*****************************************SERIALIZED PROPERTIES**********************************************/

        [SerializeField, Tooltip("audioSource used to render songs")]
        private AudioSource _audioSource;

        [Header("RANDOMIZER SETTINGS")]
        [SerializeField, Tooltip("array that contains songs for randomizer")]
        private List<AudioClip> _randomizerList;

        [SerializeField, Tooltip("play only one song a time and disable multi song playing")]
        private bool _playOneSongATime = false;

        [SerializeField, Tooltip("if enabled, random will use smart algorithm to use every song in list randomly without using 2 times the same.")]
        private bool _useSmartRandomizer = true;


        /*****************************************UNSERIALIZED PROPERTIES**********************************************/

        /// <summary>
        /// make a tooltip that explain audioRandomizer on label of list element 
        /// </summary>
        private string _propertyTooltip = "";

        /// <summary>
        /// use this to change color of font when display element on custom property
        /// </summary>
        private Color _propertyFontColor = new Color(0, 0, 0, 0.3f);

        /// <summary>
        /// use this ti change color of every text fields in custom property
        /// </summary>
        private Color _propertyTextColor = Color.white;

        /// <summary>
        /// limitate the index max of random pick(min is 0) in the song list
        /// </summary>
        private int _randomIndex = -1;

        /// <summary>
        /// make that onDrawGizmo call init One time
        /// </summary>
        private bool _isFirstSceneUpdate = true;


        //public API is every public and unserialized properties, like accessors
        #region Public API

        public AudioSource AudioSource => _audioSource;


        ///<inheritdoc cref="_propertyTooltip"/>
        public string PropertyTooltip
        {
            get => _propertyTooltip;
            set => _propertyTooltip = value;
        }

        ///<inheritdoc cref="_propertyFontColor"/>
        public Color PropertyFontColor
        {
            get => _propertyFontColor;
            set => _propertyFontColor = value;
        }

        ///<inheritdoc cref="_propertyTextColor"/>
        public Color PropertyTextColor
        {
            get => _propertyTextColor;
            set => _propertyTextColor = value;
        }

        #endregion


        /*****************************************CLASS METHODS**********************************************/

        /// <summary>
        /// called at build of instance
        /// </summary>
        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// called when scene is refreshing
        /// </summary>
        private void OnDrawGizmos()
        {
            //when object call OnDrawGizmo, pass one unique time in this condition, then, set boolean to false
            if (_isFirstSceneUpdate)
            {
                Init();
                _isFirstSceneUpdate = false;
            }
        }


        /// <summary>
        /// called at awake and every inspector refresh in editor
        /// </summary>
        public void Init()
        {
            //if audio source is null, try to get one in gameobject, and create one if none is found
            if (_audioSource == null)
                if (!TryGetComponent(out _audioSource))
                    _audioSource = gameObject.AddComponent<AudioSource>();
        }

        //input functions represent every override of main "OnRandomize" function, public and callable everywhere.
        #region INPUT FUNCTIONS

        /// <summary>
        /// Pick a random song in the list, and play it, and setup random values for audioSource
        /// </summary>
        /// <param name="volume">volume to set with this clip</param>
        /// <param name="pitch">pitch to set with this clip</param>
        public void OnRandomize(float volume, float pitch)
        {
            //set up values for audioSource at given parameters of OnRandomize
            SetUp(volume, pitch);
        }

        /// <inheritdoc cref="OnRandomize"/>
        public void OnRandomize()
        {
            //set up values for audioSource without changing volume and pitch of it
            SetUp(_audioSource.volume, _audioSource.pitch);
        }

        #endregion

        /// <summary>
        /// called by OnRandomizer functions, intermediate function that set values of audioSource, then, call Randomize
        /// </summary>
        private void SetUp(float volume, float pitch)
        {
            //set volume and pitch to function arguments
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;

            //if one song a time is enabled, stop every other sound played by this audioRandomizer
            if (_playOneSongATime)
                _audioSource.Stop();

            //if song list has songs and is not null, generate random index and play song at this position
            if (_randomizerList != null && _randomizerList.Count > 0)
                Randomize();
        }

        /// <summary>
        /// called by SetUp, pick random clip and play it
        /// </summary>
        private void Randomize()
        {
            //if audioRandomizer use smartRandomizer to avoid playing two times the same song, else, call random song normally
            if (_useSmartRandomizer)
            {
                //if randomIndex is less than 0, loop it to max list value
                if (_randomIndex < 0)
                    _randomIndex = _randomizerList.Count - 1;

                //make a random between 0 and randomIndex, and select the song to this index
                int randomNumber = Random.Range(0, _randomIndex);
                AudioClip selectedClip = _randomizerList[randomNumber];

                //put selected song at the end 
                _randomizerList.Remove(_randomizerList[randomNumber]);
                _randomizerList.Add(selectedClip);

                //play selected song
                _audioSource.PlayOneShot(selectedClip);

                //put -1 to max index to call, so that last songs(that have been played) are outside the range of this value
                _randomIndex--;
            }
            else
            {
                //play fully random song in list
                _audioSource.PlayOneShot(_randomizerList[Random.Range(0, _randomizerList.Count)]);
            }
        }
    }
}
