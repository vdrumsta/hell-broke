using UnityEngine.Audio;
using System;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    //Usarlo con FindObjectOfType<EffectManager>().Get("Stelline")
    //Tipo :  Instantiate(FindObjectOfType<EffectManager>().Get("Stelline"), transform.position, Quaternion.identity);
    public myEffect[] Effect;
     

    public ParticleSystem Get(string TheEffect)
    {
        myEffect s = Array.Find(Effect, item => item.name == TheEffect);
        if (s == null)
        {
            Debug.LogWarning("Effect: " + name + " not found!");
            return null;
        }
        return s.Effect;
    }

    public void PlayEffect(string TheEffect, Vector3 position)
    {
        myEffect s = Array.Find(Effect, item => item.name == TheEffect);
        if (s == null)
        {
            Debug.LogWarning("Effect: " + TheEffect + " not found!");
            return;
        }
        Instantiate(s.Effect, position, Quaternion.identity);
    }

}
