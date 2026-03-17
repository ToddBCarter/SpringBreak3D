using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerStatsSO playerStats;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction slideAction;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private float currentSpeed;
    public bool IsJumpButtonPressed => jumpAction.IsPressed();
    public bool IsSlideButtonPressed => slideAction.IsPressed();
    public bool IsSprintButtonPressed => sprintAction.IsPressed();

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        slideAction = InputSystem.actions.FindAction("Slide");

        currentSpeed = playerStats.WalkSpeed;
    }

    private void Update()
    {   
        if (IsJumpButtonPressed)
        {
            velocity.y = playerStats.JumpForce; //probably wanna mess with this and maybe add maneuverability while jumping?
        }
        
        if (IsSprintButtonPressed)
        {
            currentSpeed = playerStats.SprintSpeed; //need to implement momentum and decide if this will have limitations
        }
        else
        {
            currentSpeed = playerStats.WalkSpeed;
        }
        
        if (IsSlideButtonPressed)
        {
            Debug.Log("slide"); //haven't implemented sliding
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 inputDirection = (transform.right * input.x + transform.forward * input.y).normalized;
        velocity.x = inputDirection.x * currentSpeed;
        velocity.z = inputDirection.z * currentSpeed;
        ApplyGravity();
        characterController.Move(velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y += gravity * playerStats.GravityMultiplier * Time.deltaTime; //gravity multiplier might need to modified to fall faster to feel better
        }
    }
}
