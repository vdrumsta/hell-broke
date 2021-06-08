using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMng : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _winPanel;

    void Start()
    {
        
    }

    private void Update()
    {

    }

    public void GameOver()
    {
        StartCoroutine(SwitchPanelActiveState(_gameOverPanel, true, 1));
    }

    public void GameWin()
    {
        StartCoroutine(SwitchPanelActiveState(_winPanel, true, 0.5f));
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
