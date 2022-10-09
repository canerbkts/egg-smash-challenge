using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject  SettingsPanel,VolumeOnIcon,VolumeOffIcon;
    public void PlayGame()
    {        
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Settings()
    {
        isMuted = PlayerPrefs.GetInt("Muted") == 1;
        if (isMuted)
        {
            VolumeOffIcon.SetActive(true);
        }
        else
        {
            VolumeOnIcon.SetActive(true);
        }
        SettingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        VolumeOnIcon.SetActive(true);
        VolumeOffIcon.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    private bool isMuted;

    //When Pressed The Volume Button
    public void MutePressed()
    {
        isMuted = PlayerPrefs.GetInt("Muted")==1;   
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1:0);

        if (isMuted)
        {
            VolumeOnIcon.SetActive(false);
            VolumeOffIcon.SetActive(true);
        }
        else
        {
            VolumeOffIcon.SetActive(false);
            VolumeOnIcon.SetActive(true);
        }
    }
}
