using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterpump : MonoBehaviour
{

    [SerializeField] private float lerpTime;

    [SerializeField] private GameObject waterParticlesObject;
    private ParticleSystem waterParticles;
    [SerializeField] private GameObject secondaryWaterParticlesObject;
    private ParticleSystem secondaryWaterParticles;

    [SerializeField] private GameObject engineAudioObject;
    [SerializeField] private float pitchShiftAmount;

    [SerializeField] private GameObject waterSound;
    private float desiredWaterSoundVolume;
    [SerializeField] private float waterSoundMaxVolume;
    [SerializeField] private float waterSoundLerpTime;

    [SerializeField] private float minBladeSpeed;
    [SerializeField] private float maxBladeSpeed;
    [SerializeField] public GameObject blade;
    [SerializeField] public float bladeLerpTime;
    private float desiredBladeSpeed;

    private float startPitch;
    private float desiredEnginePitch;
    private GameObject tileParent;

 

    private void Start()
    {
        desiredWaterSoundVolume = 0.0f;

        tileParent = GameObject.FindGameObjectWithTag("TileParent");
        startPitch = engineAudioObject.GetComponent<AudioSource>().pitch;
        desiredEnginePitch = startPitch;
        engineAudioObject.GetComponent<AudioSource>().pitch = 0;
        waterParticles = waterParticlesObject.GetComponent<ParticleSystem>();

        if (blade != null)
        {
            desiredBladeSpeed = minBladeSpeed;
        }

        if (secondaryWaterParticlesObject != null)
            secondaryWaterParticles = secondaryWaterParticlesObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {


        var emmision = waterParticles.emission;

        engineAudioObject.GetComponent<AudioSource>().pitch = Mathf.Lerp(engineAudioObject.GetComponent<AudioSource>().pitch, desiredEnginePitch, lerpTime * Time.deltaTime);
        waterSound.GetComponent<AudioSource>().volume = Mathf.Lerp(waterSound.GetComponent<AudioSource>().volume, desiredWaterSoundVolume, waterSoundLerpTime * Time.deltaTime);

        if (blade != null)
            blade.GetComponent<RotateObject>().speed = Mathf.Lerp(blade.GetComponent<RotateObject>().speed, desiredBladeSpeed, bladeLerpTime * Time.deltaTime);



        if (tileParent.GetComponent<WaterMechanics>().waterAmount > 0)
        {
            emmision.enabled = true;

            desiredWaterSoundVolume = waterSoundMaxVolume;

            if (blade != null)
                desiredBladeSpeed = maxBladeSpeed;

            desiredEnginePitch = startPitch + pitchShiftAmount;
        } else
        {
            emmision.enabled = false;

            desiredWaterSoundVolume = 0.0f;

            if (blade != null)
                desiredBladeSpeed = minBladeSpeed;

            desiredEnginePitch = startPitch;
        }

        if (secondaryWaterParticlesObject != null)
        {

            var secondaryEmission = secondaryWaterParticles.emission;

            if (tileParent.GetComponent<WaterMechanics>().waterAmount > 0)
            {
                if (secondaryWaterParticlesObject != null)
                    secondaryEmission.enabled = true;
            }
            else
            {
                if (secondaryWaterParticlesObject != null)
                    secondaryEmission.enabled = false;
            }
        }
    }
}
