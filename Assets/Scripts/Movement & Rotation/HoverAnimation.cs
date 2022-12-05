using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    public Vector3 speed;
    public Vector3 intensity;
    public Vector3 offset;
    private Vector3 startPos;
    public float timeOffset;

    private void Start()
    {
        startPos = this.transform.position;
    }

    private void Update()
    {
        float newPosX = calculateDaPossy(speed.x, intensity.x, startPos.x, offset.x);
        float newPosY = calculateDaPossy(speed.y, intensity.y, startPos.y, offset.y);
        float newPosZ = calculateDaPossy(speed.z, intensity.z, startPos.z, offset.z);

        transform.position = new Vector3(newPosX, newPosY, newPosZ);
    }

    private float calculateDaPossy(float speedPossy, float intensityPossy, float startPossy, float offsetPossy)
    {
        float daPossy = startPossy + offsetPossy + (Mathf.Sin((Time.time + timeOffset) * speedPossy) * intensityPossy);
        return daPossy;
    }
}
