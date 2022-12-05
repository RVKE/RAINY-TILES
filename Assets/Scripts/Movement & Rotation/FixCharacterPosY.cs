using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCharacterPosY : MonoBehaviour
{
    private float defaultPosY;

    private void Start()
    {
        defaultPosY = this.transform.position.y;
    }


    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Tile")
            {
                if (hit.transform.GetComponent<TileState>().tileLevel == 0) 
                {
                    this.transform.position = new Vector3(this.transform.position.x, defaultPosY, this.transform.position.z);
                } else if (hit.transform.GetComponent<TileState>().tileLevel == 1)
                {
                    this.transform.position = new Vector3(this.transform.position.x, defaultPosY + 0.05f, this.transform.position.z);
                } else
                {
                    this.transform.position = new Vector3(this.transform.position.x, defaultPosY + 0.1f, this.transform.position.z);
                }
            }
        }
    }
}
