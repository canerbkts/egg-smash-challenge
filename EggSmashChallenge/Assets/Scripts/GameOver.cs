using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    public void PlayAgain()
    {
        GameControl.situTouch = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    public void BackToMenu()
    {
        GameControl.situTouch = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }


    public GameObject bg, playAgain, resumeGame, pause;
    public Text gameSitu;
    public GameControl GameControl;

    public void PauseGame()
    {
        GameControl.situTouch = false;
        gameSitu.text = "PAUSED";
        Time.timeScale = 0;
        pause.SetActive(false);
        bg.SetActive(true);
        playAgain.SetActive(false);
        resumeGame.SetActive(true);
    }

    public void ResumeGame()
    {
        GameControl.situTouch = true;
        Time.timeScale = 1;
        pause.SetActive(true);
        bg.SetActive(false);
        playAgain.SetActive(true);
        resumeGame.SetActive(false);
    }
}
