using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions OnFoot;
    private PlayerMotor playerMotor;
    public PointerPosition pointerPosition;

    [Header("References")]
    public Transform cam;
    public Transform feetTransform;

    [Header("Settings")]
    public LayerMask playerMask;

    [Header("Stance")]
    public PlayerStance.Stance playerStance;
    public float playerStanceSmoothing = 0.2f;
    public PlayerStance playerStandStance;
    public PlayerStance playerCrouchStance;
    private float stanceCheckErrorMargin = 0.05f;
    private float camHeight;
    private float camHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    private void Awake()
    {
        playerInput = new PlayerInput();
        OnFoot = playerInput.OnFoot;
        playerMotor = GetComponent<PlayerMotor>();
        pointerPosition = GetComponent<PointerPosition>();
        OnFoot.Jump.performed += ctx => playerMotor.Jump();
        OnFoot.Shoot.performed += ctx => playerMotor.Shoot();
        OnFoot.Crouch.performed += ctx => playerMotor.Crouch();
        camHeight = cam.localPosition.y;
        playerStandStance.camHeight = 3.4f;
        playerCrouchStance.camHeight = 1.5f;
    }
    private void Update()
    {
        CalculateStance();
    }
    private void FixedUpdate()
    {
        playerMotor.ProcessMove(OnFoot.Movement.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {
        pointerPosition.ProcessLook(OnFoot.PointerPosition.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        OnFoot.Enable();
    }
    private void OnDisable()
    {
        OnFoot.Disable();
    }
    private void CalculateStance()
    {
        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Stance.Crouch)
            currentStance = playerCrouchStance;

        camHeight = Mathf.SmoothDamp(cam.localPosition.y, currentStance.camHeight, ref camHeightVelocity, playerStanceSmoothing);
        cam.localPosition = new Vector3(cam.localPosition.x, camHeight, cam.localPosition.z);
        playerMotor.controller.height = Mathf.SmoothDamp(playerMotor.controller.height, currentStance.collider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        playerMotor.controller.center = Vector3.SmoothDamp(playerMotor.controller.center, currentStance.collider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    public bool StanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + playerMotor.controller.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - playerMotor.controller.radius - stanceCheckErrorMargin + stanceCheckHeight, 
            feetTransform.position.z);

        return Physics.CheckCapsule(start, end, playerMotor.controller.radius, playerMask);
    }
}
