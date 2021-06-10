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
        DisplayMapRecords();
    }

    public void ResetTutorials()
    {
        for (int i = 0; i < _levelRecordTexts.Length; i++)
        {
            if (PlayerPrefs.HasKey("Level0" + (i + 1) + " Tutorial"))    // TODO: Make this work for levels greater than 9
            {
                PlayerPrefs.DeleteKey("Level0" + (i + 1) + " Tutorial");
            }
        }
    }

    private void DisplayMapRecords()
    {
        for (int i = 0; i < _levelRecordTexts.Length; i++)
        {
            _levelRecordTexts[i].text = GetRecord(i + 1);
        }
    }

    private string GetRecord(int level)
    {
        string record = "";

        // Check what is the highest score for this level
        if (PlayerPrefs.HasKey("Level0" + level + " Highscore"))    // TODO: Make this work for levels greater than 9
        {
            float bestLevelTime = PlayerPrefs.GetFloat("Level0" + level + " Highscore");
            decimal d = (decimal)bestLevelTime;
            decimal truncated = decimal.Truncate(d * 100m) / 100m;
            record = truncated.ToString();
        }

        return record;
    }
}
