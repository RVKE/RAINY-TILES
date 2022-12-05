using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAppearOnEnable : MonoBehaviour
{
    [SerializeField] private float animationLength;
    [SerializeField] private float animationDelay;
    [SerializeField] private bool playSound;
    [SerializeField] private string soundName;

    private bool animationPlaying = false;
    private Vector3 targetScale;
    private Vector3 normalScale;

    void OnEnable()
    {
        normalScale = gameObject.transform.localScale;
        this.gameObject.transform.localScale = Vector3.zero;
        StartCoroutine(StartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if (animationPlaying)
        {
            this.gameObject.transform.localScale += new Vector3(Time.deltaTime / animationLength, Time.deltaTime / animationLength, Time.deltaTime / animationLength);
        } else
        {
            this.gameObject.transform.localScale = targetScale;
        }
    }

    IEnumerator StartAnimation()
    {
        animationPlaying = false;
        if (animationDelay > 0)
            yield return new WaitForSeconds(animationDelay);
        targetScale = normalScale;
        animationPlaying = true;

        if (playSound)
            FindObjectOfType<AudioManager>().Play(soundName);

        yield return new WaitForSeconds(animationLength);
        animationPlaying = false;
    }
}
