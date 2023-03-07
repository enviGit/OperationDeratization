using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerMotor playerMotor;
    private PointerPosition pointerPosition;

    private void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        playerMotor = GetComponent<PlayerMotor>();
        pointerPosition = GetComponent<PointerPosition>();
        onFoot.Jump.performed += ctx => playerMotor.Jump();
    }
    private void FixedUpdate()
    {
        playerMotor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {

        pointerPosition.ProcessLook(onFoot.PointerPosition.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }
}
