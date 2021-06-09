using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public bool IsWaitingToStartTutorial;

    [SerializeField] private GameObject _tutorialUI;

    private PlayerController _playerScript;
    private LavaController _lavaScript;
    private ScoreMng _timerScript;

    private bool _isTutorialNeeded;

    void Start()
    {
        _playerScript = FindObjectOfType<PlayerController>();
        _lavaScript = FindObjectOfType<LavaController>();
        _timerScript = FindObjectOfType<ScoreMng>();
        //if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + " Tutorial"))
        //{
        //    _isTutorialNeeded = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + " Tutorial") == 1 ? true : false;
        //}
        //else
        {
            _isTutorialNeeded = true;
            //PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + " Tutorial", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTutorialNeeded)
        {
            ShowTutorial();
        }
    }

    private void ShowTutorial()
    {
        // Check if it's the right moment to show a tutorial
        if (_playerScript.IsGrounded)
        {
            _isTutorialNeeded = false;

            // Pause lava and timer
            _lavaScript.StopRising();
            if (_timerScript.LevelTimer != null)
            {
                _timerScript.LevelTimer.Stop();
            }

            _tutorialUI.SetActive(true);
        }
    }

    public void DismissTutorial()
    {
        _lavaScript.StopRising();
        if (_timerScript.LevelTimer != null)
        {
            _timerScript.LevelTimer.Start();
        }
        _tutorialUI.SetActive(false);
    }
}
