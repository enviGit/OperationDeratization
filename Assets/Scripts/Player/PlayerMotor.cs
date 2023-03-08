using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jump = 1f;
    [SerializeField]
    private Gun currentGun;

    //
    public bool isCrouching = false;
    public float crouchSpeed = 2f;
    [SerializeField]
    private Animator animator;
    //

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        isGrounded = controller.isGrounded;

        //
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("verticalSpeed", playerVelocity.y);
        animator.SetBool("isCrouching", isCrouching);
        //
    }
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        
        //

        //

        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;

        if(isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -5f;

        controller.Move(playerVelocity * Time.deltaTime);
    }
    public void Jump()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jump * -3f * gravity);
    }
    public void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            Debug.Log("Hit: " + hit.collider.name);
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

            if (damageable != null)
                damageable.DealDamage(Random.Range(currentGun.minimumDamage, currentGun.maximumDamage));

        }
    }
    /*public void Crouch()
    {

    }*/
}
