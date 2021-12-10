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

    [SerializeField, Tooltip("array that contains random timer clamp for randomizer, with a min and a max values")]
    private List<Vector2> timeClampMixer;

    [SerializeField, Tooltip("array that contains random values for the randomizer ambiance playing")]
    private List<Vector2> volumeMixer;

    [SerializeField, Tooltip("array that contains random values for the randomizer pitch")]
    private List<Vector2> pitchMixer;

    [SerializeField, Tooltip("if disabled, a same song list can possibly play across itself")]
    private bool allowCrossFade = false;

    [SerializeField, Tooltip("if disabled, mixer will use classic per frame randomization with frequencyMixer, if enabled, game will use random number between two timeClamps")]
    private bool disableFrequencyMixer = false;

    [SerializeField, Tooltip("stop every sound")]
    private bool stopAll = false;


    //PRIVATE VARIABLES_____________________________________________________

    [SerializeField, HideInInspector]
    private List<float> timerList;

    [SerializeField, HideInInspector]
    private List<float> randomTimerList;

    private List<float>[] floatListsArray;

    private List<Vector2>[] vector2ListsArray;


    private void Awake()
    {
        if(randomClipMixer.Length == 0)
            randomClipMixer = FindObjectsOfType<AudioRandomizer>();

        floatListsArray = new List<float>[] { frequencyMixer, timerList, randomTimerList, };

        vector2ListsArray = new List<Vector2>[] { timeClampMixer, volumeMixer, pitchMixer, };
    }

    private void Update()
    {
        MixerRandomValueGetterSetter();

        CallRandom();
    }

    private void MixerRandomValueGetterSetter()
    {
        foreach(List<float> list in floatListsArray)
        {
            if (randomClipMixer.Length != list.Count)
            {
                int difference = randomClipMixer.Length - list.Count;

                for (int i = 0; i < Mathf.Abs(difference); i++)
                {
                    if (difference > 0)
                        list.Add(0);
                    else
                        list.Remove(list[list.Count - 1]);
                }
            }
        }

        foreach (List<Vector2> list in vector2ListsArray)
        {
            if (randomClipMixer.Length != list.Count)
            {
                int difference = randomClipMixer.Length - list.Count;

                for (int i = 0; i < Mathf.Abs(difference); i++)
                {
                    if (difference > 0)
                        list.Add(new Vector2(0, 1));
                    else
                        list.Remove(list[list.Count - 1]);
                }
            }
        }
    }

    private void CallRandom()
    {
        for (int i = 0; i < randomClipMixer.Length; i++)
        {
            volumeMixer[i] = new Vector2(Mathf.Clamp(volumeMixer[i].x, 0, 2), Mathf.Clamp(volumeMixer[i].y, 0, 2));
            pitchMixer[i] = new Vector2(Mathf.Clamp(pitchMixer[i].x, -12f, 12f), Mathf.Clamp(pitchMixer[i].y, -12f, 12f));

            timerList[i] += Time.deltaTime;

            if (randomClipMixer[i] != null)
            {
                if (disableFrequencyMixer)
                {
                    if (allowCrossFade)
                        RandomPreset(true, i);
                    else if (!randomClipMixer[i].audioSource.isPlaying)
                        RandomPreset(true, i);
                }
                else
                {
                    if (allowCrossFade)
                        RandomPreset(false, i);
                    else if (!randomClipMixer[i].audioSource.isPlaying)
                        RandomPreset(false, i); 

                    if(timerList[i] >= timeClampMixer[i].y)
                    {
                        if (!randomClipMixer[i].audioSource.isPlaying)
                        {
                            float minPitch = ((pitchMixer[i].x + 12f) / 16f) + 0.5f;
                            float maxPitch = ((pitchMixer[i].y + 12f) / 16f) + 0.5f;
                            float volume = Random.Range(volumeMixer[i].x, volumeMixer[i].y);
                            float pitch = Random.Range(minPitch, maxPitch);

                            randomClipMixer[i].audioSource.volume = volume;
                            randomClipMixer[i].audioSource.pitch = pitch;

                            randomClipMixer[i].OnRandomizer();
                            timerList[i] = 0;
                        }
                    }
                }
            }

            if (stopAll)
                randomClipMixer[i].audioSource.Stop();
        }

        stopAll = false;
    }

    private void RandomPreset(bool timerRandom, int i)
    {
        float minPitch = ((pitchMixer[i].x + 12f) / 16f) + 0.5f;
        float maxPitch = ((pitchMixer[i].y + 12f) / 16f) + 0.5f;

        if (timerRandom)
        {
            if (timerList[i] >= randomTimerList[i])
            {
                float volume = Random.Range(volumeMixer[i].x, volumeMixer[i].y);
                float pitch = Random.Range(minPitch, maxPitch);

                randomClipMixer[i].audioSource.volume = volume;
                randomClipMixer[i].audioSource.pitch = pitch;

                randomClipMixer[i].OnRandomizer();
                timerList[i] = 0;
                randomTimerList[i] = Random.Range(timeClampMixer[i].x, timeClampMixer[i].y);
            }
        }
        else
        {
            float random = Random.Range(0f, 1f);

            if (Mathf.Pow(frequencyMixer[i], 4) > random || frequencyMixer[i] == 1)
            {
                float volume = Random.Range(volumeMixer[i].x, volumeMixer[i].y);
                float pitch = Random.Range(minPitch, maxPitch);

                randomClipMixer[i].audioSource.volume = volume;
                randomClipMixer[i].audioSource.pitch = pitch;

                if (timerList[i] >= timeClampMixer[i].x)
                {
                    randomClipMixer[i].OnRandomizer();
                    timerList[i] = 0;
                }
            }
        }
    }
}
