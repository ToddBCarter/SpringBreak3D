using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlightManager : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerCameraManager playerCameraManager;
    [SerializeField] private PlayerStatsSO playerStats;

	//These can be merged into PlayerStats later
    public float flightSpeed = 12f;
    public float glideFallSpeed = 3f;
    public float flapStrength = 4f;
    public float flapCooldown = 0.4f;
    public float flightAcceleration = 5f;
    public float bankStrength = 0.4f;
	public float brakeStrength = 4f;

    public float exitGravity = 12f;

    private bool _isFlying = false;
    private float _currentFlightSpeed = 0f;
    private float _verticalVelocity = 0f;
    private float _lastFlapTime = -999f;

    private InputAction _flyToggleAction;
    private InputAction _flapAction;
    private InputAction _moveAction;
	
    public bool IsFlying => _isFlying;
	
	public bool brakeCheck;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _flapAction = InputSystem.actions.FindAction("Jump");
        _flyToggleAction = InputSystem.actions.FindAction("Flight"); //F key
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

	//Code for bird-like flight.
	//This should be refined further to better emulate taking off.
	//Current parameters have very floaty-feeling flight.
    public void UpdateFlight()
    {
        if (!_isFlying) return;

        Vector2 input = _moveAction.ReadValue<Vector2>();

		//Stay facing forward
        Vector3 camForward = playerCameraManager.CameraForward;
        Vector3 camRight   = Vector3.Cross(Vector3.up, camForward);
        Vector3 cameraForwardFull = Camera.main.transform.forward;
		
		brakeCheck = input.y < -0.1f; //Check the S key (negative Y)
		
		if(brakeCheck)
		{
			//Decelerate with brake strength
			_currentFlightSpeed = Mathf.MoveTowards(
				_currentFlightSpeed,
				0f,
				brakeStrength * Time.deltaTime
			);
		}
		else
		{
			//Accelerate forward up to flightSpeed
			_currentFlightSpeed = Mathf.MoveTowards(
				_currentFlightSpeed,
				flightSpeed,
				flightAcceleration * Time.deltaTime
			);
		}



        //Move forward towards camera
        Vector3 flightVelocity = cameraForwardFull * _currentFlightSpeed;

        //A/D banking
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