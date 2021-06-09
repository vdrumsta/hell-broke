using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MapMenuRecordController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _levelRecordTexts;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _levelRecordTexts.Length; i++)
        {
            _levelRecordTexts[i].text = GetRecord(i + 1);
        }
    }

    public string GetRecord(int level)
    {
        string record = "";

        // Check what is the highest score for this level
        if (PlayerPrefs.HasKey("Level0" + level + " Highscore"))    // TODO: Make this work for levels greater than 9
        {
            float bestLevelTime = PlayerPrefs.GetFloat("Level0" + level + " Highscore");
            Debug.Log("Level0" + level + " has time " + bestLevelTime);
            decimal d = (decimal)bestLevelTime;
            decimal truncated = decimal.Truncate(d * 100m) / 100m;
            record = truncated.ToString();
        }

        return record;
    }
}
