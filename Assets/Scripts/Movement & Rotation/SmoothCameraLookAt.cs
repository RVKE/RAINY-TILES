using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraLookAt : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float smoothSpeed;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}
