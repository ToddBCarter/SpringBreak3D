using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private PlayerCameraManager playerCameraManager;
	[SerializeField] private PlayerFlightManager playerFlightManager;
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
	
	private bool _wasGrounded = true;
	private float groundCheckDistance = 0.2f;
	[SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        slideAction = InputSystem.actions.FindAction("Slide");

        currentSpeed = playerStats.WalkSpeed;
		
		playerFlightManager.enabled = true; //just leave it enabled
		
		Debug.Log("Player spawn position: " + transform.position);
		Debug.Log("Capsule height: " + characterController.height);
		Debug.Log("Capsule center: " + characterController.center);
    }

    private void Update()
    {   
		if (playerFlightManager.IsFlying)
		{
			playerFlightManager.UpdateFlight();
			return; //skip remaining movement
		}
		
        if (IsJumpButtonPressed && characterController.isGrounded)
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
		
		Vector3 camForward = playerCameraManager.CameraForward;
		Vector3 camRight   = Vector3.Cross(Vector3.up, camForward);
		
		//Old movement
        //Vector3 inputDirection = (transform.right * input.x + transform.forward * input.y).normalized;
		
		//New movement based on camera angle
		Vector3 inputDirection = (camRight * input.x + camForward * input.y).normalized;
		
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
