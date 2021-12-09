using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceMixer : MonoBehaviour
{
    //SERIALIZED VARIABLES________________________________________________

    [Header("AMBIANCE MIXER")]
    [SerializeField, Tooltip("array of clip array that call randomly a song")]
    private AudioRandomizer[] randomClipMixer;

    [SerializeField, Tooltip("array that contains random values for the randomizer ambiance playing"), Range(0,1)]
    private List<float> frequencyMixer;

    [SerializeField, Tooltip("array that contains random values for the randomizer ambiance playing")]
    private List<Vector2> volumeMixer;

    [SerializeField, Tooltip("if disabled, a same song list can possibly play across itself")]
    private bool allowCrossFade = false;

    [SerializeField, Tooltip("stop every sound")]
    private bool stopAll = false;

    private void Awake()
    {
        if(randomClipMixer.Length == 0)
        {
            randomClipMixer = FindObjectsOfType<AudioRandomizer>();
        }
    }

    private void Update()
    {
        MixerRandomValueGetterSetter();

        CallRandom();
    }

    private void MixerRandomValueGetterSetter()
    {
        if (randomClipMixer.Length != frequencyMixer.Count)
        {
            int difference = randomClipMixer.Length - frequencyMixer.Count;

            for (int i = 0; i < Mathf.Abs(difference); i++)
            {
                if (difference > 0)
                    frequencyMixer.Add(0);
                else
                    frequencyMixer.Remove(frequencyMixer[frequencyMixer.Count - 1]);
            }
        }

        if (randomClipMixer.Length != volumeMixer.Count)
        {
            int difference = randomClipMixer.Length - volumeMixer.Count;

            for (int i = 0; i < Mathf.Abs(difference); i++)
            {
                if (difference > 0)
                    volumeMixer.Add(new Vector2(0,1));
                else
                    volumeMixer.Remove(volumeMixer[volumeMixer.Count - 1]);
            }
        }
    }

    private void CallRandom()
    {
        for (int i = 0; i < randomClipMixer.Length; i++)
        {
            volumeMixer[i] = new Vector2(Mathf.Clamp(volumeMixer[i].x, 0, 2), Mathf.Clamp(volumeMixer[i].y, 0, 2));

            if (randomClipMixer[i] != null)
            {
                if (allowCrossFade)
                {
                    float random = Random.Range(0f, 1f);

                    if (Mathf.Pow(frequencyMixer[i], 4) > random || frequencyMixer[i] == 1)
                    {
                        float volume = Random.Range(volumeMixer[i].x, volumeMixer[i].y);
                        randomClipMixer[i].audioSource.volume = volume;
                        randomClipMixer[i].OnRandomizer();
                    }
                }
                else
                {
                    if (!randomClipMixer[i].audioSource.isPlaying)
                    {
                        float random = Random.Range(0f, 1f);

                        if (Mathf.Pow(frequencyMixer[i], 4) > random || frequencyMixer[i] == 1)
                        {
                            float volume = Random.Range(volumeMixer[i].x, volumeMixer[i].y);
                            randomClipMixer[i].audioSource.volume = volume;

                            randomClipMixer[i].OnRandomizer();
                        }
                    }
                } 
            }

            if (stopAll)
            {
                randomClipMixer[i].audioSource.Stop();
            }
        }

        stopAll = false;
    }
}
