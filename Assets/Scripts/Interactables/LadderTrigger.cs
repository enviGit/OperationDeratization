using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class LadderTrigger : Interactable
    {
        [Header("References")]
        private Transform playerTransform;
        private CharacterController characterController;

        [Header("Ladder")]
        [SerializeField] private float climbSpeed = 3f;
        public bool isClimbing = false;
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
            playerTransform.position = new Vector3(ladderBottom.x, playerTransform.position.y, ladderBottom.z + 0.3f);
            characterController.enabled = false;
        }
        public void DetachFromLadder()
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
                {
                    playerTransform.Translate(Vector3.up * climbSpeed * Time.deltaTime);

                    if (playerTransform.position.y >= ladderTop.y)
                    {
                        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, ladderTop.z);
                        playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, 25f * Time.deltaTime);
                    }
                }
                else if (verticalInput < 0 && playerTransform.position.y > ladderBottom.y)
                    playerTransform.Translate(Vector3.down * climbSpeed * Time.deltaTime);
                else if (verticalInput == 0)
                    playerTransform.Translate(Vector3.zero);
                else
                    DetachFromLadder();
            }
        }
    }
}