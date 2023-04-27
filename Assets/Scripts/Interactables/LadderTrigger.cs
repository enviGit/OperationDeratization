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

    [Header("Mouse Rotation Limits")]
    [SerializeField] private float minRotation = -65f;
    [SerializeField] private float maxRotation = 65f;

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
        playerTransform.position = new Vector3(ladderBottom.x, ladderBottom.y, ladderBottom.z);
        characterController.enabled = false;
    }

    private void DetachFromLadder()
    {
        characterController.enabled = true;
        isClimbing = false;
        //playerTransform.position = new Vector3(ladderBottom.x, playerTransform.position.y, playerTransform.position.y);
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

            /*float horizontalInput = Input.GetAxis("Mouse X");
            Vector3 rotation = playerTransform.localRotation.eulerAngles;
            rotation.y += horizontalInput;
            rotation.y = Mathf.Clamp(rotation.y, minRotation, maxRotation);
            playerTransform.localRotation = Quaternion.Euler(rotation);*/
        }
    }
}