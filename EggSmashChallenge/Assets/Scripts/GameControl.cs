using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameControl : MonoBehaviour
{
    Animator anim;
    string[,]animations = { { "FPressCube", "FGetCube","","" }, 
        { "SPressCube", "SGetCube", "SPressFloor", "SSmashEgg" },
        { "FSmashEgg", "FPressFloor","FLeaveCube","WON" },
        { "WON", "SLeaveCube","SGetCube","SPressCube" }
    };
    string won = "",animName;
    public int side = 0, queue, rnd = 0, tempRnd = -1,scorePlayer1=0,scorePlayer2=0;
    public bool situTouch = true, pressLoop = false, sideSitu = false,paused=false,resume=false;
    public GameObject bg, pauseButton;
    public Text gameWon,timeText,scoreText1,scoreText2;
    float timeValue = 60,pauseButtonHeight,pauseButtonWidth;
    RectTransform rectTransform;
    AudioSource smashAnimationSound, pressCubeAnimationSound;
    private bool isMuted;

    void Start()
    {
        //FPS SECTION
        Application.targetFrameRate = 30;   
        

        //AUDIO SECTION               
        AudioSource[] audios = GetComponents<AudioSource>();
        smashAnimationSound = audios[0];
        pressCubeAnimationSound = audios[1];
        //Volume Control      
        isMuted = PlayerPrefs.GetInt("Muted") == 1;
        smashAnimationSound.enabled = !isMuted;
        pressCubeAnimationSound.enabled = !isMuted;


        //Set Score Texts
        scoreText1.text = scorePlayer1.ToString();
        scoreText2.text = scorePlayer2.ToString();

        //Pause Button Touching Settings
        rectTransform= pauseButton.transform.GetComponent<RectTransform>();
        pauseButtonHeight = rectTransform.rect.height;
        pauseButtonWidth = rectTransform.rect.width;


        bg.SetActive(false);
        queue = 0;

        //Get The Animator
        anim = GetComponent<Animator>();
    }

    //COUNTDOWN SECTION
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    //When The Game Paused
    public void IsPaused()
    {
        paused = true;
    }
    //When The Game Resumed
    public void Resume()
    {
        resume = true;
    }
    void Update()
    {   
        //Time Control
        if (timeValue > 0 && (won!="YOU WON" || won!= "YOU LOSE"))
        {
            timeValue -= Time.deltaTime;
        }
        //Score Control 
        else
        {           
            if (won=="")
            {
                if (scorePlayer1 == scorePlayer2)
                {
                    won = "DRAW";
                    gameWon.text = won;
                    pauseButton.SetActive(false);
                    bg.SetActive(true);
                    timeText.enabled = false;
                    Time.timeScale = 0;
                }
                else if (scorePlayer1 > scorePlayer2)
                {
                    GameOver(true);
                }
                else
                {
                    GameOver(false);
                }
            }
            timeValue = 0;
        }
        DisplayTime(timeValue);


        //Touch Control
        if (!paused)
        {
            if (situTouch)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.position.y < (pauseButton.transform.position.y-pauseButtonHeight-60) )
                    {
                        if (Input.touchCount == 1)
                        {
                            side = 0;
                            if (sideSitu)
                            {
                                side += 2;
                                sideSitu = false;
                            }
                            AnimPlay(queue, side);
                        }
                        if (Input.touchCount == 2)
                        {
                            side = 1;
                            if (sideSitu)
                            {
                                side = 2;
                                sideSitu = false;
                            }
                            AnimPlay(queue, side);
                        }
                    }
                    
                }
            }
        }
        else if (resume)
        {
            paused = false;
            resume = false;
        }
        
    }
    float waitNum;

    //ANIMATION SECTION
    public void AnimPlay(int n1,int n2)
    {        
        if (queue != -1)
        {
            situTouch = false;
            animName = animations[n1, n2];
            anim.Play(animName);
            PlayCubeAnimation();

            waitNum = anim.GetCurrentAnimatorStateInfo(1).length;
            StartCoroutine(AnimationWait(waitNum));
        }
        
    }
    
    //Waiting For The Animation Ending
    IEnumerator AnimationWait(float animTime)
    {
        yield return new WaitForSeconds(animTime);
        situTouch = true;        
        AfterAnim();
        if (pressLoop)
        {            
            pressLoop = false;
            yield break;
        }
    }

    //When The Animation Is Over
    public void AfterAnim()
    {
        if(animName== "SPressFloor")
        {
            scorePlayer2++;
            scoreText2.text = scorePlayer2.ToString();
        }
        else if(animName== "FPressFloor")
        {
            scorePlayer1++;
            scoreText1.text = scorePlayer1.ToString();
        }
        rnd = UnityEngine.Random.Range(0, 2);
        if (queue == 0)
        {
            queue++;
            if (side == 1)
            {
                rnd += 2;
            }
            tempRnd = rnd;
            AnimPlay(queue, rnd);
        }
        else if (queue == 1)
        {
            queue++;
            if (tempRnd == 0)
            {
                //Back To Top
                queue = 0;
                pressLoop = true;
            }
            else if (tempRnd ==2)
            {
                sideSitu = true;
            }
            else if (tempRnd == 3)
            {
                // First Player Won
                ScoreTextHide();
                GameOver(true);

                //For Clicking Problem
                queue = -1;
                pressLoop = true;
            }
            
        }
        else if (queue == 2)
        {
            queue++;
            if (side == 0)
            {
                // Second Player Won
                ScoreTextHide();
                GameOver(false);
                queue = -1;
                pressLoop = true;
            }
            if (side == 2)
            {
                queue = 1;
                side = rnd;
                tempRnd = side;
                pressLoop = true;
            }
            AnimPlay(queue, side);
        }
        else if (queue == 3)
        {
            if (side == 1)
            {
                queue = 0;
                pressLoop = true;
            }
        }
    }

    //Cube Animations
    public void PlayCubeAnimation()
    {
        if (queue == 0 && side==1)
        {
            anim.Play("FCubeAction");
        }
        else if (queue == 1 && rnd==1)
        {
            anim.Play("SCubeAction");
        }
        else if(queue==2 && side == 2)
        {
            anim.Play("FCubeLeave");
        }
        else if(queue==3 && side == 1)
        {
            anim.Play("SCubeLeave");
        }
    }

    //When The Game Over
    public void GameOver(bool fWon)
    {
        if (fWon)
        {
            won = "YOU WON";
        }
        else 
        {
            won = "YOU LOST";
        }
        gameWon.text = won;
        pauseButton.SetActive(false);
        bg.SetActive(true);
        Time.timeScale = 0;
        timeText.enabled = false;
    }

    //Hide The Score Texts
    public void ScoreTextHide()
    {
        scoreText1.enabled = false;
        scoreText2.enabled = false;
    }

    //Sound Effects
    private void EggSmashSound()
    {
        smashAnimationSound.Play();
    }
    private void PressCubeSound()
    {
        pressCubeAnimationSound.Play();
    }
}
