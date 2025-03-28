using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UPDB.CoreHelper.Usable;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainPanel;

    [SerializeField]
    private GameObject _settingsPanel;

    public void OnPlayButton()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnSettingsButton()
    {
        _mainPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnQuitSettingsButton()
    {
        _mainPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }

    public void OnVolumeSetSlider(float volume)
    {
        GameManager.Instance.VolumeMainMixer.SetFloat("MasterVolume", volume);
    }
}
