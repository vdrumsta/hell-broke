using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{
    private Animator transition;
    void Start()
    {
        transition = GetComponent<Animator>();
        transition.SetTrigger("Start");
    }
     

    public void PlayGame()
    {        
        PlayerPrefs.SetInt("Lives", 5);
        PlayerPrefs.SetInt("BusLives", 5);
        PlayerPrefs.SetInt("Score", 0);
        StartCoroutine(Cor_LoadScene("Map"));
    }

    public void GotoLevel(int Level)
    { 
        StartCoroutine(Cor_LoadScene("Level0" + Level));
    }


    public void GotoMainMenu()
    {
        StartCoroutine(Cor_LoadScene("MainMenu"));
    }


    public void MusicON_OFF()
    {
       // FindObjectOfType<AudioManager>().MusicON_OFF("Theme");
    }

    public void LoadScene(string SceneName)
    {
        StartCoroutine(Cor_LoadScene(SceneName));
        //SceneManager.LoadScene(SceneName);
    }
    private IEnumerator Cor_LoadScene(string SceneName)
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneName);
    }

    public void QuitApp()
    {
       // FindObjectOfType<AudioManager>().Play("Mouse_over_4", new Vector3(0, 0, 0));
        StartCoroutine(Cor_Quit());
    }

    private IEnumerator Cor_Quit()
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}
