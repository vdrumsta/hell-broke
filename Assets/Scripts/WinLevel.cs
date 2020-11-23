using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLevel : MonoBehaviour
{

    public string NameNextLevel;
    public int levelFinished;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetInt($"Level_{levelFinished}", 1);
            FindObjectOfType<ScoreMng>().SetRecord();//salvo il High record .. in caso sia l'ultimo livello
            FindObjectOfType<LevelLoader>().LoadScene(NameNextLevel);

        }
    }
}
