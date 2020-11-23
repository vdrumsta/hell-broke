using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManagementScript : MonoBehaviour
{
    public GameObject CreditMenu;
    public GameObject OptionMenu;

    public GameObject SFXButton;

    public GameObject MusicVolumeSlider;

    public GameObject HighScoreText;

    public Color SelectedColor = Color.green;
    public Color DisableColor = Color.gray;

    // Start is called before the first frame update
    void Start()
    {
        HideCredits();
        HideOptions();
        var mySlider = MusicVolumeSlider.GetComponent<Slider>();
       /* var volume = FindObjectOfType<AudioManager>().GetThemeVolume("Theme");
        mySlider.normalizedValue = volume;

        var SFX = FindObjectOfType<AudioManager>().GetSFXStatus();
        var tt = SFXButton.GetComponent<Text>();
        tt.text = (SFX == true ? "ON" : "OFF");
       */

        HighScoreText.GetComponent<Text>().text = "HighScore : " + PlayerPrefs.GetInt("HighScore").ToString("D6");
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) FindObjectOfType<LevelLoader>().PlayGame();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<LevelLoader>().QuitApp();
        }
    }
     

    public void ShowCredits()
    {
        CreditMenu.SetActive(true);
    }

    public void HideCredits()
    {
        CreditMenu.SetActive(false);
    }


    public void ShowOptions()
    {
        OptionMenu.SetActive(true);
    }

    public void HideOptions()
    {
        OptionMenu.SetActive(false);
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SFXClic()
    {
       /* FindObjectOfType<AudioManager>().ChangeSFX();
        var SFX = FindObjectOfType<AudioManager>().GetSFXStatus();
        var tt = SFXButton.GetComponent<Text>();
        tt.text = (SFX == true ? "ON" : "OFF");
       */
    }

    public void SetVolume()
    {
       /* var mySlider = MusicVolumeSlider.GetComponent<Slider>();
        FindObjectOfType<AudioManager>().ChangeThemeVolume("Theme", mySlider.value);
       */
    } 

}
