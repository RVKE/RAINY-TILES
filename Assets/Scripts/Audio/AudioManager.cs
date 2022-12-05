using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    Scene currentScene;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixer;

            s.source.volume = s.volume;

            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.playRandomClip)
        {
            s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        }

        s.source.Play();
        if (s.randomPitchEnabled)
            s.source.pitch = UnityEngine.Random.Range(s.randomPitchMin, s.randomPitchMax);
        if (s.pitchIncreaseEnabled)
            s.source.pitch += s.pitchIncrease;
    }

    public void Stop(bool stopSound)
    {
        foreach (Sound s in sounds)
        {
            if (stopSound == true && s.gameAudio == true)
            {
                s.source.volume = 0;
                s.source.Pause();
            } else
            {
                s.source.UnPause();
                s.source.volume = s.volume;
            }
        }
    }

    public void StopSpecific(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.mute = true;
            }
        }
    }

    public void JustStopSpecific(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Stop();
            }
        }
    }

    public void PauseSpecificAudioObject(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Pause();
            }
        }
    }

    public void UnpauseSpecificAudioObject(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.UnPause();
            }
        }
    }

    public void PauseAllAudioObjects()
    {
        GameObject[] sounds = GameObject.FindGameObjectsWithTag("Audio");

        foreach (GameObject audioSource in sounds)
        {
            if (audioSource.GetComponent<AudioSource>().isPlaying == true)
            {
                audioSource.GetComponent<AudioSource>().Pause();
            }
            else
            {
                audioSource.GetComponent<AudioSource>().UnPause();
            }
        }
    }
}
