using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    public AudioClip[] clips;

    public AudioMixerGroup mixer;

    public bool playRandomClip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    [Range(.1f, 3f)]
    public float randomPitchMin;

    [Range(.1f, 3f)]
    public float randomPitchMax;

    public float pitchIncrease;

    public bool randomPitchEnabled;

    public bool pitchIncreaseEnabled;

    public bool loop;

    public bool gameAudio;

    [HideInInspector]
    public AudioSource source;
}
