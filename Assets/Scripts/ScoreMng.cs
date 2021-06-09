using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using TMPro;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;

public class ScoreMng : MonoBehaviour
{
    public Stopwatch LevelTimer;

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private TextMeshProUGUI _levelTimerText;
    [SerializeField] private TextMeshProUGUI _recordTimeText;
    [SerializeField] private TextMeshProUGUI _recordStatusText;

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
        _levelTimerText.text = "--:--";
        SetEndLevelRecordTime(false);


        StartCoroutine(SwitchPanelActiveState(_gameOverPanel, true, 1));
        StartCoroutine(SwitchPanelActiveState(_recordStatusText.gameObject, true, 1));
        StartCoroutine(SwitchPanelActiveState(_recordTimeText.gameObject, true, 1));
    }

    public void GameWin()
    {
        _isTimerUpdating = false;
        SetEndLevelRecordTime();

        StartCoroutine(SwitchPanelActiveState(_winPanel, true, 0.5f));
        StartCoroutine(SwitchPanelActiveState(_recordStatusText.gameObject, true, 0.5f));
        StartCoroutine(SwitchPanelActiveState(_recordTimeText.gameObject, true, 0.5f));
    }

    private void SetEndLevelRecordTime(bool checkForRecord = true)
    {
        if (checkForRecord && SetRecord())
        {
            _recordStatusText.text = "New Record!";
            _recordTimeText.text = GetFormattedLevelTime();
        }
        else
        {
            _recordStatusText.text = "Current Record:";
            var currentRecord = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + " Highscore");
            decimal d = (decimal)currentRecord;
            decimal truncated = decimal.Truncate(d * 100m) / 100m;
            _recordTimeText.text = truncated.ToString();
        }
    }

    private void UpdateTimer()
    {
        if (!_isTimerUpdating) return;
        
        _levelTimerText.text = GetFormattedLevelTime();
    }

    private string GetFormattedLevelTime()
    {
        if (LevelTimer == null) return "0:00";

        string timerDisplayString = "<mspace=120.00>";  // Add monospacing

        int DoubleDigitMilliseconds = LevelTimer.Elapsed.Milliseconds / 10;
        timerDisplayString += LevelTimer.Elapsed.Seconds + ":" + DoubleDigitMilliseconds.ToString("D2") + "</mspace>";
        return timerDisplayString;
    }


    private IEnumerator SwitchPanelActiveState(GameObject panel, bool state, float time)
    {
        yield return new WaitForSeconds(time);
        panel.SetActive(state);
    }

    public bool SetRecord()
    {
        // Check what is the highest score for this level
        var bestLevelTime = float.MaxValue;
        if (PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + " Highscore"))
        {
            bestLevelTime = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + " Highscore");
        }
        if (LevelTimer.Elapsed.TotalSeconds < bestLevelTime)
        {
            PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + " Highscore", (float)LevelTimer.Elapsed.TotalSeconds);
            Debug.Log("New record");
            return true;
        }

        return false;
    }
}
