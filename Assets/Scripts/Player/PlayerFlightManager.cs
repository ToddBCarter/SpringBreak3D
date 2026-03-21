using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlightManager : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerCameraManager playerCameraManager;
    [SerializeField] private PlayerStatsSO playerStats;

    public float flightSpeed = 12f;
    public float glideFallSpeed = 2f;
    public float flapStrength = 6f;
    public float flapCooldown = 0.4f;
    public float flightAcceleration = 3f;
    public float bankStrength = 0.4f;

    public float exitGravity = 12f;

    private bool _isFlying = false;
    private float _currentFlightSpeed = 0f;
    private float _verticalVelocity = 0f;
    private float _lastFlapTime = -999f;

    private InputAction _flyToggleAction;
    private InputAction _flapAction;
    private InputAction _moveAction;
	
    public bool IsFlying => _isFlying;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _flapAction = InputSystem.actions.FindAction("Jump");
        _flyToggleAction = InputSystem.actions.FindAction("Flight");
    }

    private void OnEnable()
    {
        _flyToggleAction.performed += OnFlyToggle;
        _flapAction.performed      += OnFlap;
		Debug.Log("Flight Manager enabled!");
    }

    private void OnDisable()
    {
        _flyToggleAction.performed -= OnFlyToggle;
        _flapAction.performed      -= OnFlap;
		Debug.Log("Flight Manager disabled!");
    }

    private void OnFlyToggle(InputAction.CallbackContext ctx)
    {
        if (!characterController.isGrounded)
        {
            if (_isFlying)
                ExitFlight();
            else
                EnterFlight();
        }
    }

    private void OnFlap(InputAction.CallbackContext ctx)
    {
        if (_isFlying && Time.time - _lastFlapTime >= flapCooldown)
        {
            _verticalVelocity = flapStrength;
            _lastFlapTime = Time.time;
        }
    }

    private void EnterFlight()
    {
        _isFlying = true;
        _currentFlightSpeed = 0f;
        _verticalVelocity = flapStrength;
        _lastFlapTime = Time.time;
    }

    private void ExitFlight()
    {
        _isFlying = false;
        _currentFlightSpeed = 0f;
    }

    public void UpdateFlight()
    {
        if (!_isFlying) return;

        Vector2 input = _moveAction.ReadValue<Vector2>();

		//Stay facing forward
        Vector3 camForward = playerCameraManager.CameraForward;
        Vector3 camRight   = Vector3.Cross(Vector3.up, camForward);
        Vector3 cameraForwardFull = Camera.main.transform.forward;

        //Accelerate forward speed up to flightSpeed
        _currentFlightSpeed = Mathf.MoveTowards(
            _currentFlightSpeed,
            flightSpeed,
            flightAcceleration * Time.deltaTime
        );

        //Move forward towards camera
        Vector3 flightVelocity = cameraForwardFull * _currentFlightSpeed;

        //A/D banking — strafe gently left/right
        flightVelocity += camRight * (input.x * bankStrength * _currentFlightSpeed);

        //Apply glide sink between flaps
        _verticalVelocity -= glideFallSpeed * Time.deltaTime;

        //Add vertical velocity on top of flight direction
        flightVelocity.y += _verticalVelocity;

        characterController.Move(flightVelocity * Time.deltaTime);

        //Auto-exit if grounded while flying
        if (characterController.isGrounded)
		{
            ExitFlight();
		}
    }

}