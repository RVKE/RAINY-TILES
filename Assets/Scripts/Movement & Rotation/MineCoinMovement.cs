using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCoinMovement : MonoBehaviour
{
    private GameObject player;
    private bool readyToMove = false;
    private float mineCoinTime;
    [SerializeField] private float mineCoinSoundSpeed;
    private bool playedSpawnSound = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (readyToMove == true)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, 6.5f * Time.deltaTime);
        } else
        {
            transform.position += new Vector3(0, 2, 0) * Time.deltaTime;
            StartCoroutine(WaitForCoin());
        }
    }

    IEnumerator WaitForCoin()
    {

        if (!playedSpawnSound)
        {
            FindObjectOfType<AudioManager>().Play("MineCoinSpawn");
            playedSpawnSound = true;
        }

        readyToMove = false;

        if (mineCoinTime == 0)
        {
            FindObjectOfType<AudioManager>().Play("MineCoin");
        }

        mineCoinTime += Time.deltaTime * mineCoinSoundSpeed;
        if (mineCoinTime >= 1f)
        {
            mineCoinTime = 0;
        }

        yield return new WaitForSeconds(0.5f);
        readyToMove = true;
        playedSpawnSound = false;
    }
}
