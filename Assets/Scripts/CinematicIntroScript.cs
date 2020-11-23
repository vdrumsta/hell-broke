using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicIntroScript : MonoBehaviour
{
    public GameObject[] cinematic;
    public string[] MyText;
    public float speed = 8f;
    public string SceneAtEnd = "MainMenu";
    private int count = 0;
    public float waitAfterText = 3f;
    public TextMesh TextPoint;
    public float TypingSpeed = 0.08f;
    string OriginalText = "";
    bool finito = false;
    // Start is called before the first frame update
    void Start()
    { 
        count = 0;
        OriginalText = MyText[count];
        TextPoint.text = "";
        
        foreach (var o1 in cinematic)
        {
            o1.SetActive(false);
        }
         
        cinematic[0].SetActive(true);
        StartCoroutine(ShowText());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) {   TextPoint.text = ""; finito = true;StopAllCoroutines(); count = cinematic.Length; }
        if (finito)
        { 
            count++;
            if (count >= cinematic.Length)
            {
                FindObjectOfType<LevelLoader>().LoadScene(SceneAtEnd);
                waitAfterText = speed;
            }
            else
            {
                OriginalText = MyText[count];
                TextPoint.text = "";
                waitAfterText = speed;
                cinematic[count].SetActive(true);
                StartCoroutine(ShowText());
            }
        }
    }

    IEnumerator ShowText()
    {
        finito = false;
        var Str = OriginalText.Replace("<br>","\n\r").ToCharArray();
        foreach (var a1 in Str)
        {
            TextPoint.text += a1;
            yield return new WaitForSeconds(TypingSpeed);
        }
        yield return new WaitForSeconds(waitAfterText);
        TextPoint.text = "";
        finito = true;
        cinematic[count].SetActive(false);


    }

}
