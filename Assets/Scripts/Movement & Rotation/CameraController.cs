using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool zoomEnabled;
    [SerializeField] private bool movementEnabled;
    private float zoomLerpTime;
    private float movementLerpTime;

    private Camera cam;
    private Transform camHolderTransform;
    private float startZoomLevel;
    private float desiredZoomLevel;

    private Vector3 startPosition;
    private Vector3 desiredPosition;


    private void Start()
    {
        cam = GetComponent<Camera>();
        camHolderTransform = this.transform.parent;
        startZoomLevel = cam.orthographicSize; // (3.1)
        desiredZoomLevel = startZoomLevel;
        startPosition = camHolderTransform.position; // (5, 7.8, -5)
        desiredPosition = startPosition;
    }

    private void Update()
    {
        if (zoomEnabled)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, desiredZoomLevel, zoomLerpTime * Time.deltaTime);
        if (movementEnabled)
            camHolderTransform.position = Vector3.Lerp(camHolderTransform.position, desiredPosition, movementLerpTime * Time.deltaTime);
    }

    public void ChangeZoom(float newZoomLevel, float lerpTime)
    {
        zoomLerpTime = lerpTime;
        desiredZoomLevel = newZoomLevel;
    }

    public void ChangePosition(Vector3 newPosition, float lerpTime)
    {
        movementLerpTime = lerpTime;
        desiredPosition = newPosition;
    }

    public void ResetZoom(float lerpTime)
    {
        ChangeZoom(startZoomLevel, lerpTime);
    }

    public void ResetPosition(float lerpTime)
    {
        ChangePosition(startPosition, lerpTime);
    }
}
