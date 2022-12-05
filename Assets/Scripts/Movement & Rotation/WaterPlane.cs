using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour
{
    [SerializeField] public Vector3 desiredPos;
    [SerializeField] private float lerpTime;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Slerp(this.transform.position, desiredPos, lerpTime * Time.deltaTime);
    }
}
