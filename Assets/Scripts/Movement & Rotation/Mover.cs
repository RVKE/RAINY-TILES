using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{

    [SerializeField] private Vector3 speedVector;

    // Update is called once per frame
    void Update()
    {
        this.transform.position += speedVector * Time.deltaTime;
    }
}
