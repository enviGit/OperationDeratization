using UnityEngine;

public class LadderTrigger : Interactable
{
    [SerializeField] private float climbSpeed = 3f;
    private bool isClimbing;
    private Vector3 ladderTop;
    private Vector3 ladderBottom;
    private Transform playerTransform;

    protected override void Interact()
    {
        prompt = "Climb Ladder";

        if (isClimbing)
        {
            DetachFromLadder();
            prompt = "";
        }
        else
        {
            prompt = "Climb Ladder";
            AttachToLadder();
        }
    }

    private void AttachToLadder()
    {
        playerTransform = GetComponent<Transform>();
        isClimbing = true;
        ladderTop = transform.GetChild(0).position;
        ladderBottom = transform.GetChild(1).position;
        playerTransform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
        playerTransform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        GetComponent<CharacterController>().enabled = false;
        playerTransform.SetParent(transform);
    }

    private void DetachFromLadder()
    {
        GetComponent<CharacterController>().enabled = true;
        playerTransform.SetParent(null);
        isClimbing = false;
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");

            if (verticalInput > 0 && playerTransform.position.y < ladderTop.y)
            {
                playerTransform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
            }
            else if (verticalInput < 0 && playerTransform.position.y > ladderBottom.y)
            {
                playerTransform.Translate(Vector3.down * climbSpeed * Time.deltaTime);
            }
            else if (verticalInput == 0)
            {
                playerTransform.Translate(Vector3.zero);
            }
            else
            {
                DetachFromLadder();
            }
        }
    }
}