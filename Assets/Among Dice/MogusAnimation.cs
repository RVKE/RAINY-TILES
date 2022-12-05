using UnityEngine;

public class MogusAnimation : MonoBehaviour
{

    private Animator animatorController;
    private Vector3 movement;
    private Vector3 lastPosition;
    private float currentSpeed;

    [SerializeField] private GameObject characterModel;
    [SerializeField] private float walkSpeed;

    private void Start()
    {
        animatorController = characterModel.GetComponent<Animator>();
    }

    private void Update()
    {
        movement = (this.transform.position - lastPosition);
        currentSpeed = movement.magnitude;

        animatorController.SetFloat("walkSpeed", currentSpeed * walkSpeed);

        if (currentSpeed == 0)
            animatorController.Play("Walking", -1, 0f);

        characterModel.transform.LookAt(transform.position + movement.normalized);

        lastPosition = this.transform.position;

    }
}
