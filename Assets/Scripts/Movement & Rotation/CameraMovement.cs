using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float speed;

    public float orthographicSize;

    private float desiredCameraPosY;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        desiredCameraPosY = 7.5f;
    }

    void Update()
    {
        Vector3 Pos = transform.position;

        transform.position = Vector3.Lerp(transform.position, new Vector3(Pos.x, desiredCameraPosY, Pos.z), speed * Time.deltaTime);
    }

    public void mouseEnterButton()
    {
        desiredCameraPosY = 4.5f;
    }

    public void mouseLeaveButton()
    {
        desiredCameraPosY = 7.5f;
    }
}
