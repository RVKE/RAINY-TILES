using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RGBBackground : MonoBehaviour
{
    public Gradient colorGradient;
    public float gradientDuration = 10f; //seconds
    private float currentTime = 0f;

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > gradientDuration) currentTime = 0f;
            GetComponent<Image>().color = colorGradient.Evaluate(currentTime / gradientDuration);
    }
}
