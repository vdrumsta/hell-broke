using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMng : MonoBehaviour
{
    private int myScore;
    private int myLives;
    public GameObject GameOverMenu;
    public GameObject PauseMenu;

    public GameObject InfoText;

    private bool isDead;
    public int MaxLives = 8;
    private float ContinueCountdown;
    private bool vinto = false;
    private bool completed = false;

    public GameObject myScoreText;
    public GameObject myLivesText;
     

    void Start()
    { 

        GameOverMenu.SetActive(false);

        myLives = PlayerPrefs.GetInt("Lives");
        myScore = PlayerPrefs.GetInt("Score");
        ShowText("", 0f);

#if UNITY_EDITOR
        AddLife(5);
#endif

        isDead = (myLives == 0);
        UpdateMyUI();

    }



    public void ShowText(string myText, float time)
    {

        StartCoroutine(MostraText(myText, time));
    }

    private IEnumerator MostraText(string myText, float time)
    {
        InfoText.GetComponent<Text>().text = myText;
        yield return new WaitForSeconds(time);
        InfoText.GetComponent<Text>().text = "";
    }


    private void Update()
    {
        CheckWin();


        if (myLives == 0 || vinto == true)
        {
            ShowGameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                HidePause();
            }
            else
            {
                ShowPause();
            }
        }
    }

    public void ShowPause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        AudioListener.pause = true;
    }
    public void HidePause()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        PauseMenu.SetActive(false);
    }

    public void UpdateMyUI()
    {
        myScoreText.GetComponent<Text>().text = myScore.ToString();
        myLivesText.GetComponent<Text>().text = myLives.ToString();
    }


    public void AddScore(int score)
    {
        if (isDead == false)
        {
            myScore += score;

            PlayerPrefs.SetInt("Score", myScore);
            UpdateMyUI();
        }

    }


    public void AddLife(int lives)
    {
        if (isDead == false)
        {
            myLives += lives;
            if (myLives > MaxLives) myLives = MaxLives;
            PlayerPrefs.SetInt("Lives", myLives);
            UpdateMyUI();
        }
    }

    public int GetLives()
    {
        return myLives;
    }

    public int LostLife()
    {
        if (isDead == false)
        {
            ContinueCountdown = 5f;//Secondi a fine gioco
            FindObjectOfType<shakeCamera>().Shake();
            myLives--;
            if (myLives < 0) myLives = 0;
            PlayerPrefs.SetInt("Lives", myLives);
            UpdateMyUI();
            if (myLives <= 0)
            {
                isDead = true;
                GameOverMenu.SetActive(true);
                ShowGameOver();
            }
        }
        return myLives;
    }

    public void CheckWin()
    {
        /*  var Tb = GameObject.FindGameObjectsWithTag("DonutEnemy").Length; 


          if (Tb == 0 && vinto == false)
          {
              ContinueCountdown = 3f;
              MenuTop.SetActive(true);
              WinMessage.SetActive(true);
              LostMessage.SetActive(false); 
              completed = false;
              vinto = true;
              ShowGameOver();
          }*/
    }

    void ShowGameOver()
    {

        if (myLives == 0 || vinto == true)
        {
            if (vinto == true)
            {
                GameOverMenu.SetActive(false);
            }
            else
            {
                GameOverMenu.SetActive(true);
            }


            ContinueCountdown -= Time.deltaTime;
            if (ContinueCountdown <= 0)
            {
                ContinueCountdown = 0;
                if (vinto == true)
                {
                    if (completed == false)
                    {
                        completed = true;
                    }
                }
                else
                {
                    SetRecord();
                    FindObjectOfType<LevelLoader>().GotoMainMenu();
                }
            }
        }

    }

    public void GameOver()
    {
        isDead = true;
        StartCoroutine(SetMenuGameOver());
    }


    private IEnumerator SetMenuGameOver()
    {
        SetRecord();

        //Increase Level
        yield return new WaitForSeconds(3);
        FindObjectOfType<LevelLoader>().PlayGame();
    }
    public void SetRecord()
    {
        var GetBest = PlayerPrefs.GetInt("HighScore");
        if (GetBest < myScore) PlayerPrefs.SetInt("HighScore", myScore);
    }


    public void QuitGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        PauseMenu.SetActive(false);
        FindObjectOfType<LevelLoader>().PlayGame();
    }



}
