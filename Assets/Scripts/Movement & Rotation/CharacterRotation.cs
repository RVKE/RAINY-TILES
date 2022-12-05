using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class CharacterRotation : MonoBehaviour
{

    private Vector3 movement;
    private Vector3 lastPosition;

    [SerializeField] public float buildRotationSpeedy;

    private GameObject player;

    Vector3 oldForward;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        oldForward = transform.forward;
    }

    void Update()
    {
        movement = (this.transform.position - lastPosition);

        this.transform.LookAt(transform.position + new Vector3(movement.normalized.x, 0, movement.normalized.z));

        if (player.GetComponent<Player>().inBuildingMode)
        {
            Vector3 targetPos = new Vector3(player.GetComponent<Player>().tileMouseHitPos.x, transform.position.y, player.GetComponent<Player>().tileMouseHitPos.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - this.transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * buildRotationSpeedy);
        }

        //GekkeMagie();
        
        lastPosition = this.transform.position;
    }

    private void GekkeMagie()
    {
        Vector3 cross = Vector3.Cross(oldForward, transform.forward);

        if (oldForward != transform.forward)
        {
            GetComponent<Animator>().SetBool("IsTurning", true);
        } else
        {
            GetComponent<Animator>().SetBool("IsTurning", false);
        }

        if (cross.y > 0f)
        {
            GetComponent<Animator>().SetFloat("TurnDirection", 1.0f);
        } else if (cross.y < 0f)
        {
            GetComponent<Animator>().SetFloat("TurnDirection", -1.0f);
        } else
        {
            GetComponent<Animator>().SetFloat("TurnDirection", 0.0f);
        }

        oldForward = transform.forward;
    }
}
