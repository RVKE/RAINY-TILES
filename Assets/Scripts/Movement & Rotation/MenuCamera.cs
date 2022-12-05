using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [Header("Start")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;

    [Header("Lerp Time")]
    [SerializeField] private float movementLerpTime;
    [SerializeField] private float rotationLerpTime;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start()
    {
        targetPosition = startPosition;
        targetRotation = startRotation;
    }

    private void Update()
    {
        transform.position = Vector3.Slerp(this.transform.position, targetPosition, movementLerpTime * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationLerpTime * Time.deltaTime);
    }

    public void changeTransform(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }

    public void ResetTransform()
    {
        targetPosition = startPosition;
        targetRotation = startRotation;
    }
}
