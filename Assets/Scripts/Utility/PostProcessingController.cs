using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    private void Update()
    {
        GetComponent<Volume>().enabled = GameManager.postProcessingEnabled;
    }
}
