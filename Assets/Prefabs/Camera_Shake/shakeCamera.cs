using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shakeCamera : MonoBehaviour
{
    private Animator anim;
    public GameObject Camera;
   
  

    public void Shake()
    { 
        anim = Camera.GetComponent<Animator>();
        anim.SetTrigger("shakeNow");
    }
}
