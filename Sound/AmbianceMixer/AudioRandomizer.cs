using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    public class AudioRandomizer : UPDBBehaviour
    {
        /*****************************************SERIALIZED PROPERTIES**********************************************/

        [SerializeField, Tooltip("audioSource used to render songs")]
        private AudioSource _audioSource;

        [SerializeField, Tooltip("explain here what is this config")]
        private string _propertyTooltip = "";

        [Header("RANDOMIZER SETTINGS")]
        [SerializeField, Tooltip("array that contains songs for randomizer")]
        private List<AudioClip> randomizerList;

        [SerializeField, Tooltip("play only one song a time and disable multi song playing")]
        private bool playOneSongATime = false;

        [SerializeField, Tooltip("if enabled, random will use smart algorithm to use every song in list randomly without using 2 times the same.")]
        private bool useSmartRandomizer = true;


        [Header("PICK A CLIP")]
        [SerializeField, Tooltip("play a song from the randomizer(act like a button)")]
        private bool pickRandomClip = false;



        /*****************************************PRIVATE PROPERTIES**********************************************/

        private int _randomIndex = -1;


        #region Public API

        public AudioSource AudioSource => _audioSource;
        public string PropertyTooltip => _propertyTooltip;

        #endregion


        /// <summary>
        /// called at build of instance
        /// </summary>
        private void Awake()
        {
            if (_audioSource == null)
                if (!TryGetComponent(out _audioSource))
                    _audioSource = gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// called every frame
        /// </summary>
        private void Update()
        {
            if (pickRandomClip)
                OnRandomize();
        }

        /// <summary>
        /// Pick a random song in the list, and play it
        /// </summary>
        /// <param name="volume">volume to set with this clip</param>
        /// <param name="pitch">pitch to set with this clip</param>
        public void OnRandomize(float volume, float pitch)
        {
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;

            if (playOneSongATime)
                _audioSource.Stop();

            if (_randomIndex < 0)
                _randomIndex = randomizerList.Count - 1;

            int randomNumber = Random.Range(0, _randomIndex);
            AudioClip selectedClip = randomizerList[randomNumber];

            randomizerList.Remove(randomizerList[randomNumber]);
            randomizerList.Add(selectedClip);

            if (useSmartRandomizer)
                _audioSource.PlayOneShot(selectedClip);
            else
                _audioSource.PlayOneShot(randomizerList[Random.Range(0, randomizerList.Count)]);

            pickRandomClip = false;
            _randomIndex--;
        }

        /// <inheritdoc cref="OnRandomize"/>
        public void OnRandomize()
        {
            if (playOneSongATime)
                _audioSource.Stop();

            if (_randomIndex < 0)
                _randomIndex = randomizerList.Count - 1;

            int randomNumber = Random.Range(0, _randomIndex);
            AudioClip selectedClip = randomizerList[randomNumber];

            randomizerList.Remove(randomizerList[randomNumber]);
            randomizerList.Add(selectedClip);

            if (useSmartRandomizer)
                _audioSource.PlayOneShot(selectedClip);
            else
                _audioSource.PlayOneShot(randomizerList[Random.Range(0, randomizerList.Count)]);

            pickRandomClip = false;
            _randomIndex--;
        }
    } 
}
