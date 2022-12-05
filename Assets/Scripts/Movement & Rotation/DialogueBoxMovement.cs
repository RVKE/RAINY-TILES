using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxMovement : MonoBehaviour
{
    [SerializeField] private float intensity;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;

    private Vector3 desiredPosition;

    private Vector3 startPos;

    private void OnEnable()
    {
        startPos = this.transform.position;
        desiredPosition = startPos;
        StartCoroutine(Wait());
    }

    private void OnDisable()
    {
        StopCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        desiredPosition = startPos + Random.insideUnitSphere * intensity;
        StartCoroutine(Wait());
    }
}
