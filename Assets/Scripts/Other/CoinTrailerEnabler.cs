using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrailerEnabler : MonoBehaviour
{
    private GameObject tileParent;

    private void Start()
    {
        tileParent = GameObject.FindGameObjectWithTag("TileParent");
    }

    private void Update()
    {
        if (tileParent.GetComponent<MoneyMechanics>().treasureMagnetEnabled)
        {
            gameObject.GetComponent<TrailRenderer>().emitting = true;
        } else {
            gameObject.GetComponent<TrailRenderer>().emitting = false;
        }
    }
}
