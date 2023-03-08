using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions OnFoot;
    private PlayerMotor playerMotor;
    private PointerPosition pointerPosition;

    private void Awake()
    {
        playerInput = new PlayerInput();
        OnFoot = playerInput.OnFoot;
        playerMotor = GetComponent<PlayerMotor>();
        pointerPosition = GetComponent<PointerPosition>();
        OnFoot.Jump.performed += ctx => playerMotor.Jump();
        OnFoot.Shoot.performed += ctx => playerMotor.Shoot();
        //
        //OnFoot.Crouch.performed += ctx => playerMotor.Crouch();
        //
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
}
