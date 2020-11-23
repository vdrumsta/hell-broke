using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTextTrigger : MonoBehaviour
{
     private TextMesh TextPoint;
     bool found = false;
    string OriginalText="";
    public float TypingSpeed = 0.05f;

    void Start(){
        TextPoint = GetComponent<TextMesh>();
        OriginalText = TextPoint.text;
        TextPoint.text = "";
    }
    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (found == false)
        {            
            if (other.gameObject.CompareTag("Player"))
            {
                found = true;                
                StartCoroutine(ShowText());
            }
        }
    }

   IEnumerator ShowText()
    {
        var Str = OriginalText.ToCharArray();
        foreach(var a1 in Str)
        {
            TextPoint.text += a1;
            yield return new WaitForSeconds(TypingSpeed);
        }
        yield return new WaitForSeconds(3);
        for (int i = 255; i > 0;i-=1)
        {
            TextPoint.color = new Color(i, i, 0, i);
        }
         yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
