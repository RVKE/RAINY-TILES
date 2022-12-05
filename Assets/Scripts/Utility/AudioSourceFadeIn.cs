using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourceFadeIn : MonoBehaviour
{

    [SerializeField] private float fadeLength;

    private float targetVolume;

    void Start()
    {
        targetVolume = this.gameObject.GetComponent<AudioSource>().volume;
        this.gameObject.GetComponent<AudioSource>().volume = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<AudioSource>().volume = Mathf.Lerp(this.gameObject.GetComponent<AudioSource>().volume, targetVolume, fadeLength * Time.deltaTime);
    }

}
