using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;

using Debug = UnityEngine.Debug;

public class ScoreMng : MonoBehaviour
{
    public Stopwatch LevelTimer;

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private TextMeshProUGUI _levelTimerUIObject;

    private bool _isTimerUpdating;

    private void Awake()
    {
        _isTimerUpdating = true;
    }

    private void Update()
    {
        UpdateTimer();
    }

    public void GameOver()
    {
        _isTimerUpdating = false;
        _levelTimerUIObject.text = "--:--";
        StartCoroutine(SwitchPanelActiveState(_gameOverPanel, true, 1));
    }

    public void GameWin()
    {
        _isTimerUpdating = false;
        // TODO record highscore
        StartCoroutine(SwitchPanelActiveState(_winPanel, true, 0.5f));
    }

    private void UpdateTimer()
    {
        if (!_isTimerUpdating) return;
        if (LevelTimer == null) return;

        string timerDisplayString = "<mspace=120.00>";  // Add monospacing

        int minutes = LevelTimer.Elapsed.Minutes;
        if (minutes > 0)
        {
            timerDisplayString += minutes + ":";
        }

        int DoubleDigitMilliseconds = LevelTimer.Elapsed.Milliseconds / 10;
        timerDisplayString += LevelTimer.Elapsed.Seconds + ":" + DoubleDigitMilliseconds.ToString("D2") + "</mspace>";
        _levelTimerUIObject.text = timerDisplayString;
    }


    private IEnumerator SwitchPanelActiveState(GameObject panel, bool state, float time)
    {
        yield return new WaitForSeconds(time);
        panel.SetActive(state);
    }

    public void SetRecord()
    {
        // TODO: change score to lowest time
        //var GetBest = PlayerPrefs.GetInt("HighScore");
        //if (GetBest < myScore) PlayerPrefs.SetInt("HighScore", myScore);
    }
}
