using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// Main script of AmbianceMixer tool, randomize and call randomly a list of AudioRandomizer
    /// </summary>
    [AddComponentMenu("UPDB/Sound/AmbianceMixer/Ambiance Mixer Manager"), HelpURL(URL.baseURL + "/tree/main/Audio/AmbianceMixer/README.md")]
    public class AmbianceMixer : UPDBBehaviour
    {
        /*****************************************SERIALIZED PROPERTIES**********************************************/

        [SerializeField, Tooltip("array of clip array settings")]
        private List<AudioRandomizerConfig> _randomClipConfig;


        /****************************************UNSERIALIZED PROPERTIES*********************************************/

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

        #region Public API

        public List<AudioRandomizerConfig> RandomClipConfig
        {
            get
            {
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
            for (int i = 0; i < RandomClipConfig.Count; i++)
                if (RandomClipConfig[i].Randomizer != null)
                    UpdateClipCallState(i);
        }


        /// <summary>
        /// called at scene update in editor and at awake
        /// </summary>
        public void Init()
        {
            if (RandomClipConfig.Count == 0)
            {
                AudioRandomizer[] randomClipMixer = FindObjectsOfType<AudioRandomizer>();
                List<AudioRandomizerConfig> audioSettingsFinder = new List<AudioRandomizerConfig>();

                for (int i = 0; i < randomClipMixer.Length; i++)
                    audioSettingsFinder.Add(new AudioRandomizerConfig(randomClipMixer[i]));

                RandomClipConfig = audioSettingsFinder;
            }
            else
            {
                Vector2 volumeXRange = new Vector2(0, 2);
                Vector2 volumeYRange = new Vector2(0, 2);
                Vector2 pitchXRange = new Vector2(-12, 12);
                Vector2 pitchYRange = new Vector2(-12, 12);

                for (int i = 0; i < RandomClipConfig.Count; i++)
                {
                    Keyframe[] timeKeys = RandomClipConfig[i].TimeProbabilityCurve.keys;

                    if (timeKeys.Length != 0 && (timeKeys[0].time != 0 || timeKeys[0].value != RandomClipConfig[i].TimeRange.x || timeKeys[timeKeys.Length - 1].time != 1 || timeKeys[timeKeys.Length - 1].value != RandomClipConfig[i].TimeRange.y))
                    {
                        timeKeys[0] = new Keyframe(0, RandomClipConfig[i].TimeRange.x);
                        timeKeys[timeKeys.Length - 1] = new Keyframe(1, RandomClipConfig[i].TimeRange.y);
                        RandomClipConfig[i].TimeProbabilityCurve.keys = timeKeys;
                    }

                    Keyframe[] volumeKeys = RandomClipConfig[i].VolumeProbabilityCurve.keys;

                    if (volumeKeys.Length != 0 && (volumeKeys[0].time != 0 || volumeKeys[0].value != RandomClipConfig[i].VolumeRange.x || volumeKeys[volumeKeys.Length - 1].time != 1 || volumeKeys[volumeKeys.Length - 1].value != RandomClipConfig[i].VolumeRange.y))
                    {
                        volumeKeys[0] = new Keyframe(0, RandomClipConfig[i].VolumeRange.x);
                        volumeKeys[volumeKeys.Length - 1] = new Keyframe(1, RandomClipConfig[i].VolumeRange.y);
                        RandomClipConfig[i].VolumeProbabilityCurve.keys = volumeKeys;
                    }

                    Keyframe[] pitchKeys = RandomClipConfig[i].PitchProbabilityCurve.keys;

                    if (pitchKeys.Length != 0 && (pitchKeys[0].time != 0 || pitchKeys[0].value != RandomClipConfig[i].PitchRange.x || pitchKeys[pitchKeys.Length - 1].time != 1 || pitchKeys[pitchKeys.Length - 1].value != RandomClipConfig[i].PitchRange.y))
                    {
                        pitchKeys[0] = new Keyframe(0, RandomClipConfig[i].PitchRange.x);
                        pitchKeys[pitchKeys.Length - 1] = new Keyframe(1, RandomClipConfig[i].PitchRange.y);
                        RandomClipConfig[i].PitchProbabilityCurve.keys = pitchKeys;
                    }
                }
            }

            if (_disableAll && _randomClipSave.Count == 0)
            {
                for (int i = 0; i < RandomClipConfig.Count; i++)
                    _randomClipSave.Add(RandomClipConfig[i].Randomizer);
            }
            else if(_disableAll)
            {
                for (int i = 0; i < RandomClipConfig.Count; i++)
                    if (RandomClipConfig[i].Randomizer != null)
                        RandomClipConfig[i].Randomizer = null;
            }
            else if (_randomClipSave.Count != 0 && _randomClipSave.Count == RandomClipConfig.Count)
            {
                for (int i = 0; i < _randomClipSave.Count; i++)
                    RandomClipConfig[i].Randomizer = _randomClipSave[i];

                _randomClipSave.Clear();
            }
        }

        /// <summary>
        /// called by inspector when simulating button is enabled
        /// </summary>
        public void Simulate()
        {
            for (int i = 0; i < RandomClipConfig.Count; i++)
                if (RandomClipConfig[i].Randomizer != null)
                    UpdateClipCallState(i);
        }

        /// <summary>
        /// if called, every playing song will stop
        /// </summary>
        public void StopAll()
        {
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
            RandomClipConfig[i].Timer += Time.deltaTime;

            bool isNotCrossedOrAllowed = _allowCrossfade || (!_allowCrossfade && !RandomClipConfig[i].Randomizer.AudioSource.isPlaying);
            bool hasReachedTime = RandomClipConfig[i].Timer >= RandomClipConfig[i].RandomTime;

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
            float volume = RandomClipConfig[i].VolumeProbabilityCurve.Evaluate(Random.Range(0f, 1f));
            float pitch = RandomClipConfig[i].PitchProbabilityCurve.Evaluate(Random.Range(0f, 1f));

            RandomClipConfig[i].Randomizer.OnRandomize(volume, pitch);

            RandomClipConfig[i].Timer = 0;
            RandomClipConfig[i].RandomTime = RandomClipConfig[i].TimeProbabilityCurve.Evaluate(Random.Range(0f, 1f));
        }
    }
}
