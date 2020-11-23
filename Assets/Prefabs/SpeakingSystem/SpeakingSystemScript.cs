using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakingSystemScript : MonoBehaviour
{
    public bool Enabled = true;

    public TextMesh p1;
    public TextMesh p2;
    public float speed = 1f;
    public float Wait = 5f;
    public SpeakingText[] PlayerText;
    private int startText = 0;
    private int TextPos = 0;
    private float lastRead = 0f;
    private bool WaitWrite = false;

    public Animator Player1Anim;
    public Animator Player2Anim;

    public string SceneToLoad;

    public GameObject ButtonStart;

    void Start()
    {
        startText = 0;
        TextPos = 0;
        p1.text = "";
        p2.text = "";
        WaitWrite = false;
        lastRead = Wait;
        SetAnimation();
        if (ButtonStart != null) ButtonStart.SetActive(false);
    }

    void SetAnimation()
    {
        if (TextPos == 0)
        {
            if (startText < PlayerText.Length)
            {
                var ss = PlayerText[startText];
                if (ss.AnimTrigger != "")
                {
                    if (ss.PlayerSpeak == 1) Player1Anim.SetTrigger(ss.AnimTrigger);
                    if (ss.PlayerSpeak == 2) Player2Anim.SetTrigger(ss.AnimTrigger);
                }
                if (ss.AnimTriggerOther != "")
                {
                    if (ss.PlayerSpeak == 1) Player2Anim.SetTrigger(ss.AnimTriggerOther);
                    if (ss.PlayerSpeak == 2) Player1Anim.SetTrigger(ss.AnimTriggerOther);
                }
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Enabled)
        {
            if (Input.anyKeyDown)
            {
                if (speed == 0) StartCoroutine(StartGame());
                if (startText < PlayerText.Length)
                {
                    TextPos = PlayerText[startText].Text.Length - 1;
                    lastRead = 0;
                    speed = 0;
                    Wait = .1f;
                    ButtonStart.SetActive(true);
                }
            }

            if (lastRead <= 0 && WaitWrite == false)
            {
                if (startText < PlayerText.Length)
                {
                   // FindObjectOfType<AudioManager>().Play("Mouse_over_4", new Vector3(0, 0, 0));
                    TextPos++;
                    lastRead = speed;
                    var ss = PlayerText[startText];
                    if (ss.PlayerSpeak == 1 && TextPos <= ss.Text.Length) p1.text = ss.Text.Substring(0, TextPos);
                    if (ss.PlayerSpeak == 2 && TextPos <= ss.Text.Length) p2.text = ss.Text.Substring(0, TextPos);
                    if (TextPos >= ss.Text.Length)
                    {
                        WaitWrite = true;
                        StartCoroutine(NextText());
                    }
                }
                else
                {
                    StartCoroutine(StartGame());
                }
            }


            if (lastRead > 0) lastRead -= Time.deltaTime;
        }
    }

    private IEnumerator NextText()
    {
        yield return new WaitForSeconds(Wait);
        p1.text = "";
        p2.text = "";
        startText++;
        TextPos = 0;
        WaitWrite = false;
        SetAnimation();
    }
    private IEnumerator StartGame()
    {
        FindObjectOfType<LevelLoader>().LoadScene(SceneToLoad);
        yield return new WaitForSeconds(1);
    }


}
