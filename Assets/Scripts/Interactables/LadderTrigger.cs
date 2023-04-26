using UnityEngine;

public class LadderTrigger : Interactable
{
    [Header("References")]
    private Transform playerTransform;
    private CharacterController characterController;

    [Header("Ladder")]
    [SerializeField] private float climbSpeed = 3f;
    private bool isClimbing;
    private Vector3 ladderTop;
    private Vector3 ladderBottom;

    protected override void Interact()
    {
        prompt = "Climb ladder";

        if (isClimbing)
            DetachFromLadder();
        else
            AttachToLadder();
    }

    private void AttachToLadder()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        characterController = playerTransform.GetComponent<CharacterController>();
        isClimbing = true;
        ladderTop = transform.GetChild(0).position;
        ladderBottom = transform.GetChild(1).position;
        playerTransform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z + -0.5f); //-0.5f is to change, we don't know how the ladder will be rotated
        playerTransform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        characterController.enabled = false;
    }

    private void DetachFromLadder()
    {
        characterController.enabled = true;
        isClimbing = false;
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");

            if (verticalInput > 0 && playerTransform.position.y < ladderTop.y)
                playerTransform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
            else if (verticalInput < 0 && playerTransform.position.y > ladderBottom.y)
                playerTransform.Translate(Vector3.down * climbSpeed * Time.deltaTime);
            else if (verticalInput == 0)
                playerTransform.Translate(Vector3.zero);
            else
                DetachFromLadder();
        }
    }
}