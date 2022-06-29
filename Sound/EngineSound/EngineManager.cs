using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip engineSound;

    [SerializeField]
    private bool isPlaying = true;

    [SerializeField]
    private float playerSpeedMultiplier = 3;

    [SerializeField]
    private float RPM = 0;

    [SerializeField]
    private bool manualVitesse = true;

    [SerializeField]
    private List<float> vitesseSpeedList;

    [SerializeField]
    private List<float> startVitesseList;

    [SerializeField]
    private int actualVitesse = 1;

    [SerializeField]
    private float _offset = 0;

    private bool changePhase = false;
    private bool changePhaseSwitch = false;
    private float startPitchAtChange = 0;
    
    private float timer = 0;

    float usedVitesse = 0;

    private void Awake()
    {
        if (audioSource == null)
            if (!TryGetComponent(out audioSource))
                audioSource = gameObject.AddComponent<AudioSource>();

        actualVitesse = 1;
        usedVitesse = vitesseSpeedList[actualVitesse - 1];
    }

    private void Update()
    {
        audioSource.pitch -= _offset;

        if (isPlaying)
            EngineSoundPlayer();

        RPMManager();

        if (manualVitesse)
        {
            if (Input.GetKeyDown(KeyCode.B) && actualVitesse > 1)
            {
                actualVitesse--;
                changePhase = true;
            }

            if (Input.GetKeyDown(KeyCode.A) && actualVitesse < 6)
            {
                actualVitesse++;
                changePhase = true;
            }
        }

        audioSource.pitch += _offset;
    }

    private void EngineSoundPlayer()
    {
        //if(!audioSource.isPlaying)
        //{
        //    audioSource.PlayOneShot(engineSound);
        //}

        if(timer >= (3 - audioSource.pitch) * playerSpeedMultiplier)
        {
            audioSource.PlayOneShot(engineSound);


            timer = 0;
        }

        timer += Time.deltaTime;
    }

    private void RPMManager()
    {
        usedVitesse = 0;
        int vitesseIndex = actualVitesse - 1;

        if (Input.GetKey(KeyCode.Space))
        {
            if (!changePhase)
            {
                if (changePhaseSwitch)
                {

                    changePhaseSwitch = false;
                }

                usedVitesse = vitesseSpeedList[vitesseIndex];

                if(audioSource.pitch <= 3)
                    audioSource.pitch += usedVitesse;

                if(!manualVitesse)
                {
                    if (audioSource.pitch >= 2.9f)
                    {
                        actualVitesse++;
                        changePhase = true;
                    }  
                }
            }
            else
            {
                if (!changePhaseSwitch)
                {
                    startPitchAtChange = audioSource.pitch;
                    changePhaseSwitch = true;
                }

                if (audioSource.pitch > startPitchAtChange - startVitesseList[vitesseIndex])
                {
                    audioSource.pitch -= 0.1f;
                }
                else
                {
                    changePhase = false;
                }
            }
        }
        else
        {
            if (!changePhase)
            {
                if (changePhaseSwitch)
                {

                    changePhaseSwitch = false;
                }

                usedVitesse = vitesseSpeedList[vitesseIndex];

                if (audioSource.pitch > 0.5f)
                    audioSource.pitch -= 0.01f;

                if(!manualVitesse)
                {
                    if (audioSource.pitch < 1f && actualVitesse > 1)
                    {
                        actualVitesse--;
                        changePhase = true;
                    } 
                }
            }
            else
            {
                if (!changePhaseSwitch)
                {
                    startPitchAtChange = audioSource.pitch;
                    changePhaseSwitch = true;
                }

                if (audioSource.pitch < startPitchAtChange + startVitesseList[vitesseIndex])
                {
                    audioSource.pitch += 0.1f;
                }
                else
                {
                    changePhase = false;
                }
            }
        }
    }
}
