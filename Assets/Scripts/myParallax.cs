using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myParallax : MonoBehaviour
{
    public float length;
    private float startpos;
    public GameObject cam;
    public float parallaxEffect;
    public float increaseColor = 0f; 

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            var cc = sr.color;
            sr.color = new Color(cc.r + increaseColor, cc.g + increaseColor, cc.b + increaseColor, cc.a);
        }
    }
   
    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
