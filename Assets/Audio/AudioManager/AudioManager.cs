using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

     

    void Awake()
    {
        
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            if (s.clip.Count() == 1)
            {
                s.source.clip = s.clip[0];
            }
            else
            {
                var rr = UnityEngine.Random.Range(0, s.clip.Length);
                s.source.clip = s.clip[rr];
            }
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = mixerGroup;
        }

        if (PlayerPrefs.GetInt("VolumeSet") != 1)
        {
            PlayerPrefs.SetFloat("ThemeVolume", 0.5f);
            PlayerPrefs.SetInt("VolumeSet", 1);
            PlayerPrefs.SetInt("SFX", 1);
        }
        var ThemeVolume = PlayerPrefs.GetFloat("ThemeVolume");
        ChangeThemeVolume("Theme", ThemeVolume);
    }


    public void StopSound(string Name)
    {
        var Snd = sounds.SingleOrDefault(s => s.name == Name);
        if (Snd != null)
        {
            Debug.Log("StopSound " + Name);
            Snd.source.Stop();
        }
    }

    public void StopAllSounds()
    { 
        foreach (var s in sounds)
        {
            s.source.Stop();
        }
    }


    void Start()
    {
        Play("Theme", new Vector3(0, 0, 0));
    }

    public bool GetSFXStatus()
    {
        return PlayerPrefs.GetInt("SFX") == 1;
    }

    public void ChangeSFX()
    {
        var SFX_F = PlayerPrefs.GetInt("SFX");
        SFX_F = (SFX_F == 0 ? 1 : 0);
        PlayerPrefs.SetInt("SFX", SFX_F);
    }

    public float GetThemeVolume(string Theme)
    {
        Sound s = Array.Find(sounds, item => item.name == Theme);
        return s.volume;
    }
    public float ChangeThemeVolume(string Theme, float newVolume)
    {
        Sound s = Array.Find(sounds, item => item.name == Theme);
        if (s == null) return 0;
        s.volume = newVolume;
        s.source.volume = newVolume;
        PlayerPrefs.SetInt("VolumeSet", 1);
        PlayerPrefs.SetFloat("ThemeVolume", newVolume);
        return s.volume;
    }

    public void PlayZeroCoor(string sound)
    {
        Play(sound, Vector3.zero);
    }

    public void Play(string sound, Vector3 Position)
    {
        try
        {
            bool Suonare = false;
            if (sound.Contains("Theme"))
            { 
                Suonare = true;
            }
            else
            {
                if (GetSFXStatus()) Suonare = true;
            }

            if (Suonare)
            {
                Sound s = Array.Find(sounds, item => item.name == sound);
                if (s == null)
                {
                    Debug.LogWarning($"Sound: {sound} not found!");
                    return;
                }

                if (s.clip.Count() > 1)
                {
                    var rr = UnityEngine.Random.Range(0, s.clip.Length);
                    s.source.clip = s.clip[rr];
                }


                s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
                s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
                s.source.transform.position = Position;
                s.source.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error Play sound {sound} position : {Position.x} {Position.y}: {ex.Message}");
        }
    }

    public void Pause(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {sound} not found!");
            return;
        }
        Debug.Log("Pause:" + sound);
        s.source.Pause();
    }

    public void MusicON_OFF(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        if (s.source.isPlaying)
        {
            s.source.Pause();
        }
        else
        {
            s.source.Play();
        }

    }
}
