using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myLineX : MonoBehaviour
{ 
    public Transform posFinale;

    void Start()
    {
        LineRenderer lineRenderer =GetComponent<LineRenderer>(); 
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;   
    }

    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();        
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, posFinale.position);        
    }
}