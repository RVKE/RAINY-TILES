using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISmoothAppear : MonoBehaviour
{
    [SerializeField] private float animationLength;
    [SerializeField] private bool playAutomatically;

    private Vector3 targetScale;
    private Vector3 currentScale;

    void Start()
    {
        targetScale = gameObject.transform.localScale;
        if (playAutomatically)
        {
            this.gameObject.transform.localScale = Vector3.zero;
            StartCoroutine(LerpAnimation());
        } else
        {
            currentScale = targetScale;
        }
    }

    private void Update()
    {

        this.transform.localScale = currentScale;

        if (this.gameObject.activeSelf && playAutomatically)
            StartCoroutine(LerpAnimation());
    }

    public void PlayAnimation()
    {
        currentScale = Vector3.zero;
        this.transform.localScale = currentScale;
        StartCoroutine(LerpAnimation());
    }

    IEnumerator LerpAnimation()
    {
        float timeElapsed = 0;
        while (timeElapsed < animationLength)
        {
            currentScale = Vector3.Lerp(Vector3.zero, targetScale, timeElapsed / animationLength);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        currentScale = targetScale;
    }
}
