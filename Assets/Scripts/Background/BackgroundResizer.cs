using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundResizer : MonoBehaviour
{
    void Start()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        GetComponent<RectTransform>().position = new Vector2(960, 540);
    }

}
