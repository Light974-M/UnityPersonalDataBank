using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Sound.AmbianceMixer
{
    public class AudioRandomizer : MonoBehaviour
    {
        //SERIALIZED VARIABLES___________________________________________________________________________

        [SerializeField, Tooltip("audioSource used to render songs")]
        private AudioSource _audioSource; public AudioSource audioSource => _audioSource;

        [Header("RANDOMIZER")]
        [SerializeField, Tooltip("array that contains songs for randomizer")]
        private List<AudioClip> randomizerList;

        [SerializeField, Tooltip("play only one song a time and disable multi song playing")]
        private bool playOneSongATime = false;

        [SerializeField, Tooltip("if enabled, random will use smart algorithm to use every song in list randomly without using 2 times the same.")]
        private bool useSmartRandomizer = true;

        [Header("")]
        [SerializeField, Tooltip("play a song from the randomizer(act like a button)")]
        private bool pickRandomClip = false;


        //PRIVATE VARIABLES______________________________________________________________________________

        private int _randomIndex = -1;


        private void Awake()
        {
            if (_audioSource == null)
                if (!TryGetComponent(out _audioSource))
                    _audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Update()
        {
            if (pickRandomClip)
                OnRandomizer();
        }

        public void OnRandomizer()
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
