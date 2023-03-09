using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jump = 1f;
    [SerializeField]
    private Gun currentGun;
    private InputManager currentState;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentState = FindObjectOfType<InputManager>();
    }
    private void Update()
    {
        isGrounded = controller.isGrounded;
    }
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;

        if(isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -5f;

        controller.Move(playerVelocity * Time.deltaTime);
    }
    public void Jump()
    {
        if (isGrounded)
        {
            currentState.playerStance = PlayerStance.Stance.Stand;
            playerVelocity.y = Mathf.Sqrt(jump * -3f * gravity);
        }
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
    public void Crouch()
    {
        if (!isGrounded)
            return;
        if (currentState.playerStance == PlayerStance.Stance.Crouch)
        {
            if (currentState.StanceCheck(currentState.playerStandStance.collider.height))
                return;

            currentState.playerStance = PlayerStance.Stance.Stand;
            speed = 5f;
        }
        else
        {
            if (currentState.StanceCheck(currentState.playerCrouchStance.collider.height))
                return;

            currentState.playerStance = PlayerStance.Stance.Crouch;
            speed = 2f;
        }
    }
}
