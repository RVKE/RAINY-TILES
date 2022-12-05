using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boombox : MonoBehaviour
{

    private bool musicPlaying;
    private GameObject canvas;

    [SerializeField] private GameObject boomBoxOff;
    [SerializeField] private GameObject boomBoxOn;

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        musicPlaying = false;
    }

    private void OnMouseDown()
    {
        if (musicPlaying == true && canvas.GetComponent<MenuMechanics>().theGameHasEnded == true)
            return;

        musicPlaying = true;

        StartCoroutine(Wait());

        canvas.GetComponent<MenuMechanics>().ToggleMusic();

        boomBoxOn.SetActive(true);
        boomBoxOff.SetActive(false);

        FindObjectOfType<AudioManager>().Play("TileAction");
        FindObjectOfType<AudioManager>().Play("Chune");
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(31);

        boomBoxOn.SetActive(false);
        boomBoxOff.SetActive(true);

        canvas.GetComponent<MenuMechanics>().ToggleMusic();

        yield return new WaitForSeconds(20);
        musicPlaying = false;

    }
}
