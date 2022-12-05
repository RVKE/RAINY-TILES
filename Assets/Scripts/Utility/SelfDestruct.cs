using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float timeToSelfToDestruct;

    void Start()
    {
        StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown()
    {
        yield return new WaitForSeconds(timeToSelfToDestruct);
        Destroy(this.gameObject);
    }

}
