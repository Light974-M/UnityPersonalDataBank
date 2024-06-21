using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Sound.AmbianceMixer
{
    /// <summary>
    /// class that contains audioRandomizer and every settings of it
    /// </summary>
    [System.Serializable, HelpURL(URL.baseURL + "/tree/main/Sound/AmbianceMixer/README.md")]
    public class AudioRandomizerConfig
    {
        /*****************************************SERIALIZED PROPERTIES**********************************************/

        [SerializeField, Tooltip("clip array that will play when asked")]
        private AudioRandomizer _randomizer;

        [SerializeField, Tooltip("group used to override default preset values")]
        private RangeMixerGroup _group;

        [SerializeField, Tooltip("clip calls will range randomly with min and max")]
        private Vector2 _timeRange;

        [SerializeField, Tooltip("probability curve, timeRange will follow this curve")]
        private AnimationCurve _timeProbabilityCurve;

        [SerializeField, Tooltip("volume will range randomly between x value and y value")]
        private Vector2 _volumeRange;

        [SerializeField, Tooltip("probability curve, volume will follow this curve")]
        private AnimationCurve _volumeProbabilityCurve;

        [SerializeField, Tooltip("pitch will range randomly between x value and y value")]
        private Vector2 _pitchRange;

        [SerializeField, Tooltip("probability curve, pitch will follow this curve")]
        private AnimationCurve _pitchProbabilityCurve;

        //fields hiden in inspector, attributes that are proper to custom property drawer, like foldout boolean
        #region Property Attributes

        [SerializeField, HideInInspector, Tooltip("only used by custom property drawer, determine if main foldout is displayed or not")]
        private bool _mainFoldoutDisplay;

        [SerializeField, HideInInspector, Tooltip("only used by custom property drawer, set min and max of time slider")]
        private Vector2 _minMaxTimeSlider = new Vector2(0, 10);

        [SerializeField, HideInInspector, Tooltip("only used by custom property drawer, set min and max of volume slider")]
        private Vector2 _minMaxVolumeSlider = new Vector2(0, 10);

        [SerializeField, HideInInspector, Tooltip("only used by custom property drawer, set min and max of pitch slider")]
        private Vector2 _minMaxPitchSlider = new Vector2(0, 10);

        #endregion


        /*****************************************UNSERIALIZED PROPERTIES**********************************************/

        /// <summary>
        /// timer that reset every time a song is played, until it reach randomTimer
        /// </summary>
        private float _timer;

        /// <summary>
        /// random time between current and folloxing song, depend on timeRange
        /// </summary>
        private float _randomTime;


        //public API is every public and unserialized properties, like accessors
        #region Public API

        public AudioRandomizer Randomizer
        {
            get { return _randomizer; }
            set { _randomizer = value; }
        }
        public RangeMixerGroup Group
        {
            get { return _group; }
            set { _group = value; }
        }
        public Vector2 TimeRange
        {
            get { return _timeRange; }
            set { _timeRange = value; }
        }
        public Vector2 VolumeRange
        {
            get { return _volumeRange; }
            set { _volumeRange = value; }
        }
        public Vector2 PitchRange
        {
            get { return _pitchRange; }
            set { _pitchRange = value; }
        }
        public AnimationCurve TimeProbabilityCurve
        {
            get { return _timeProbabilityCurve; }
            set { _timeProbabilityCurve = value; }
        }
        public AnimationCurve VolumeProbabilityCurve
        {
            get { return _volumeProbabilityCurve; }
            set { _volumeProbabilityCurve = value; }
        }
        public AnimationCurve PitchProbabilityCurve
        {
            get { return _pitchProbabilityCurve; }
            set { _pitchProbabilityCurve = value; }
        }
        public Vector2 MinMaxTimeSlider
        {
            get { return _minMaxTimeSlider; }
            set { _minMaxTimeSlider = value; }
        }
        public Vector2 MinMaxVolumeSlider
        {
            get{ return _minMaxVolumeSlider; }
            set { _minMaxVolumeSlider = value; }
        }
        public Vector2 MinMaxPitchSlider
        {
            get { return _minMaxPitchSlider; }
            set {  _minMaxPitchSlider = value; }
        }

        /// <inheritdoc cref="_timer"/>
        public float Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        /// <inheritdoc cref="_randomTime"/>
        public float RandomTime
        {
            get { return _randomTime; }
            set { _randomTime = value; }
        }

        #endregion


        /*****************************************CLASS METHODS**********************************************/

        /// <summary>
        /// Constructor of AudioRandomizerSetting struct
        /// </summary>
        /// <param name="randomizer">AudioRandomizer class referenced</param>
        public AudioRandomizerConfig(AudioRandomizer randomizer)
        {
            //put randomizer to input randomizer
            _randomizer = randomizer;

            //put time, volume, and pitch, to random default value
            _timeRange = new Vector2(1, 3);
            _volumeRange = Vector2.one;
            _pitchRange = Vector2.one;

            //put timer and randomTime to 0
            _timer = 0;
            _randomTime = 0;

            //put time, volume, and pitch curves to initialized values of time, volume, and range time
            _timeProbabilityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(_timeRange.x, 1), new Keyframe(_timeRange.y, 1) });
            _volumeProbabilityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(_volumeRange.x, 1), new Keyframe(_volumeRange.y, 1) });
            _pitchProbabilityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(_pitchRange.x, 1), new Keyframe(_pitchRange.y, 1) });
        }
    }
}
