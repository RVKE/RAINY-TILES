using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAppear : MonoBehaviour
{
    [SerializeField] private float animationLength;

    private bool animationPlaying = false;
    private Vector3 targetScale;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player.GetComponent<Player>().allowSmoothAppear)
        {
            targetScale = gameObject.transform.localScale;
            return;
        }

        StartCoroutine(StartAnimation());
        targetScale = gameObject.transform.localScale;
        this.gameObject.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetComponent<Player>().allowSmoothAppear)
            return;

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
        animationPlaying = true;
        yield return new WaitForSeconds(animationLength);
        animationPlaying = false;
    }
}
