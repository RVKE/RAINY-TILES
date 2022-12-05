using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimationBobbing : MonoBehaviour
{
    public float speed;
    public float intensity;
    private Text text;
    private int fontSize;

    private void Start()
    {
        text = this.GetComponent<Text>();
        fontSize = text.fontSize;
    }

    private void Update()
    {
        text.fontSize = fontSize + (int)(Mathf.Sin(Time.time * speed) * intensity);
    }
}
