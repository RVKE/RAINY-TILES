using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    [SerializeField] private Vector3 colliderSize;

    void Update()
    {
        this.GetComponent<BoxCollider>().size = new Vector3(transform.InverseTransformVector(this.transform.localScale).x, 0.2f, transform.InverseTransformVector(this.transform.localScale).z);
    }
}
