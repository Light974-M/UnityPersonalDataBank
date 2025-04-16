using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;
using UPDB.CoreHelper;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// Main script of AmbianceMixer tool, randomize and call randomly a list of AudioRandomizer
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    [AddComponentMenu(NamespaceID.SoundPath + "/" + NamespaceID.AmbianceMixer + "/Ambiance Mixer")]
    public class AmbianceMixer : UPDBBehaviour
    {
        /*****************************************SERIALIZED PROPERTIES**********************************************/

        [SerializeField, Tooltip("array of clip array settings")]
        private List<AudioRandomizerConfig> _randomClipConfig;


        /****************************************UNSERIALIZED PROPERTIES*********************************************/

        /// <summary>
        /// used to save every values of RandomClipConfig when user disable it with "disableAll" buttons
        /// </summary>
        private List<AudioRandomizer> _randomClipSave = new List<AudioRandomizer>();

        /// <summary>
        /// if enabled, a same song list can possibly play across itself
        /// </summary>
        private bool _allowCrossfade = false;

        /// <summary>
        /// if enabled, list will set to 0 and no elements will be in
        /// </summary>
        private bool _disableAll = false;

        /// <summary>
        /// when script is adding a new audioRandomizer, what should be its default name
        /// </summary>
        private string _newClipName = "New AudioRandomizer";


        //public API is every public and unserialized properties, like accessors
        #region Public API

        public List<AudioRandomizerConfig> RandomClipConfig
        {
            get
            {
                //if randomClipConfig is null, create it and return it
                if (_randomClipConfig == null)
                    _randomClipConfig = new List<AudioRandomizerConfig>();

                return _randomClipConfig;
            }
            set { _randomClipConfig = value; }
        }

        /// <inheritdoc cref="_allowCrossfade"/> 
        public bool AllowCrossfade
        {
            get { return _allowCrossfade; }
            set { _allowCrossfade = value; }
        }

        ///<inheritdoc cref="_newClipName"/>
        public string NewClipName
        {
            get { return _newClipName; }
            set { _newClipName = value; }
        }

        ///<inheritdoc cref="_disableAll"/>
        public bool DisableAll
        {
            get { return _disableAll; }
            set { _disableAll = value; }
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
        /// called every frame
        /// </summary>
        private void Update()
        {
            //for every element in RandomClipConfig, if element has a randomizer, update state of randomizer
            for (int i = 0; i < RandomClipConfig.Count; i++)
                if (RandomClipConfig[i].Randomizer != null)
                    UpdateClipCallState(i);
        }


        /// <summary>
        /// called at scene update in editor and at awake
        /// </summary>
        public void Init()
        {
            //if list is has no elements, automatically search every randomizer in scene, and put them in list, else, update state of some elements of configs in list
            if (RandomClipConfig.Count == 0)
            {
                //get every randomizer in scene, and create a list of config that will handle randomizers
                AudioRandomizer[] randomClipMixer = FindObjectsByType<AudioRandomizer>(FindObjectsSortMode.InstanceID);
                List<AudioRandomizerConfig> audioSettingsFinder = new List<AudioRandomizerConfig>();

                //for every randomizer found, add it to config list
                for (int i = 0; i < randomClipMixer.Length; i++)
                    audioSettingsFinder.Add(new AudioRandomizerConfig(randomClipMixer[i]));

                //put final list into RandomClipConfig
                RandomClipConfig = audioSettingsFinder;
            }


            //if user disable all and save is null, then make a save of RandomClipConfig
            if (_disableAll && _randomClipSave.Count == 0)
            {
                for (int i = 0; i < RandomClipConfig.Count; i++)
                    _randomClipSave.Add(RandomClipConfig[i].Randomizer);
            }
            //while user disable all, delete every randomizer not set ot null
            else if (_disableAll)
            {
                //for each element of RandomClipConfig, if there is a randomizer, delete it
                for (int i = 0; i < RandomClipConfig.Count; i++)
                    if (RandomClipConfig[i].Randomizer != null)
                        RandomClipConfig[i].Randomizer = null;
            }
            //if there is no disableAll, but save list has elements saved, and there is same number of elements that in RandomClipConfig, load every randomizer
            else if (_randomClipSave.Count != 0 && _randomClipSave.Count == RandomClipConfig.Count)
            {
                //for each randomizer in save, put it in corresponding element of RandomClipConfig
                for (int i = 0; i < _randomClipSave.Count; i++)
                    RandomClipConfig[i].Randomizer = _randomClipSave[i];

                //clear save list
                _randomClipSave.Clear();
            } 
        }

        /// <summary>
        /// called by inspector when simulating button is enabled
        /// </summary>
        public void Simulate()
        {
            //make the same thing that update, but can be called at any moment
            for (int i = 0; i < RandomClipConfig.Count; i++)
                if (RandomClipConfig[i].Randomizer != null)
                    UpdateClipCallState(i);
        }

        /// <summary>
        /// if called, every playing song will stop
        /// </summary>
        public void StopAll()
        {
            //for each randomizer in RandomClipConfig, if is playing a song, stop it
            foreach (AudioRandomizerConfig randomizer in RandomClipConfig)
                randomizer.Randomizer.AudioSource.Stop();
        }

        /// <summary>
        /// called every frame, for each audioRandomize in list
        /// </summary>
        /// <param name="config">config to update</param>
        /// <param name="randomizer">audioRandomizer to update</param>
        private void UpdateClipCallState(int i)
        {
            //update timer of config element
            RandomClipConfig[i].Timer += Time.deltaTime;

            //if current song is ended, or if allowCrossfade is enabled, set to true
            bool isNotCrossedOrAllowed = _allowCrossfade || (!_allowCrossfade && !RandomClipConfig[i].Randomizer.AudioSource.isPlaying);

            //if timer has reach time, set to true
            bool hasReachedTime = RandomClipConfig[i].Timer >= RandomClipConfig[i].RandomTime;

            //if randomizer as to call a new song, and current song is, crossfaded if true, or ended if not
            if (isNotCrossedOrAllowed && hasReachedTime)
                RandomPreset(i);
        }

        /// <summary>
        /// generate a random preset for audioRandomizer, and call it
        /// </summary>
        /// <param name="config">config to generate</param>
        /// <param name="randomizer">audioRandomizer to generate</param>
        private void RandomPreset(int i)
        {
            //put volume and pitch to random values, following curves
            float volume = RandomClipConfig[i].VolumeProbabilityCurve.Evaluate(Random.Range(0f, 1f));
            float pitch = RandomClipConfig[i].PitchProbabilityCurve.Evaluate(Random.Range(0f, 1f));

            //call OnRandomize of Randomizer, with volume and pitch in arguments
            RandomClipConfig[i].Randomizer.OnRandomize(volume, pitch);

            //reboot tier of config element
            RandomClipConfig[i].Timer = 0;

            //generate a new random time, following time curve
            RandomClipConfig[i].RandomTime = RandomClipConfig[i].TimeProbabilityCurve.Evaluate(Random.Range(0f, 1f));
        }
    }
}
