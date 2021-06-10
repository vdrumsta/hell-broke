using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    [SerializeField] private AudioClip clip;
    
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clip;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (source != null)
        {
            source.Play();
        }
    }

}
