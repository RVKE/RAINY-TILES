using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _x, _y;

    void Update()
    {
        if (!this.gameObject.activeInHierarchy)
            return;
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.fixedDeltaTime, _img.uvRect.size);
    }
}
