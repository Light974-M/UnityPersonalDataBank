using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UPDB.CoreHelper.Templates;
using UPDB.CoreHelper.Usable;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _pausePanel;

    [SerializeField]
    private GameObject _settingsPanel;

    public void OnResumeButton()
    {
        TemplateLevelManager.Instance.IsPaused = false;

        _pausePanel.SetActive(false);
        _settingsPanel.SetActive(false);
    }

    public void OnPause()
    {
        TemplateLevelManager.Instance.IsPaused = true;

        _pausePanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }


    public void OnSettingsButton()
    {
        _pausePanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void OnQuitToMainMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnQuitApplicationButton()
    {
        Application.Quit();
    }

    public void OnQuitSettingsButton()
    {
        _pausePanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }

    public void OnVolumeSetSlider(float volume)
    {
        GameManager.Instance.VolumeMainMixer.SetFloat("MasterVolume", volume);
    }
}
