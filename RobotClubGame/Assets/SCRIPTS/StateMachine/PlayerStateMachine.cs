using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    //Declare reference variables
    PlayerMovement _playerInput;
    CharacterController _characterController;
    Animator _animator;
    Camera _mainCamera;

    int _isWalkingHash;
    int _isRunningHash;
    int _isFallingHash;
    int _isJumpingHash;

    //Variables to store player input values
    public Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;

    [Header("Movement")]
    [SerializeField] float _speed;
    [SerializeField] float _acceleration;
    [SerializeField] float _runMultiplier = 3.0f;
    float _currentSpeed;
    bool _isMovementPressed;
    bool _isRunPressed;
    float _targetSpeed;

    float _rotationFactorPerFrame = 20f;

    //Gravity Variables
    float _gravity = -9.8f;
    float _groundGravity = -0.05f;

    //Jumping Variables
    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    [Header("Jumping")]
    [SerializeField] float _maxJumpHeight = 1.0f;
    [SerializeField] float _maxJumpTime = 0.5f;
    bool _isJumping = false;
    bool _requireNewJumpPress= false;

    //State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //getters and setters
        
    public CharacterController CharacterController { get { return _characterController; } }
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        //Animation
    public Animator Animator { get { return _animator; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int IsFallingHash { get { return _isFallingHash; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
        //Jumping
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public bool IsJumping { set { _isJumping = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
        //Movement
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }
    public float CurrentMovementZ { get { return _currentMovement.z; } set { _currentMovement.z = value; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public float RunMultiplier { get { return _runMultiplier; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float TargetSpeed { get { return _targetSpeed; } set { _targetSpeed = value; } }
    public float Acceleration { set { _acceleration = value; } }
        //Gravity
    public float GroundGravity { get { return _groundGravity; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }


    void Awake()
    {
        //initially set reference variables
        _playerInput = new PlayerMovement();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isFallingHash = Animator.StringToHash("isFalling");
        _isJumpingHash = Animator.StringToHash("isJumping");

        //Player input callbacks
        _playerInput.CharacterControls.Move.started += onMovementInput;
        _playerInput.CharacterControls.Move.canceled += onMovementInput;
        _playerInput.CharacterControls.Move.performed += onMovementInput;
        _playerInput.CharacterControls.Run.started += onRun;
        _playerInput.CharacterControls.Run.canceled += onRun;
        _playerInput.CharacterControls.Jump.started += onJump;
        _playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();

    }

    //Sets up variables that will be used for jumping
    void setupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    //Checks if the jump button was pressed
    void onJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    //Checks if run button is pressed
    void onRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    //Sets speeds of character movement
    void onMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement = new Vector3(_currentMovementInput.x, _currentMovement.y, _currentMovementInput.y);
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void handleRotation()
    {
        if (_currentMovementInput.sqrMagnitude == 0) return;

        Vector3 positionToLookAt = _currentMovement;

        //set direction for player to look at. Looks in direction of camera
        positionToLookAt = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(_currentMovementInput.x, 0.0f, _currentMovementInput.y);

        //player moves in the direction they are looking
        _currentMovement.x = positionToLookAt.x;    //remember to NEVER even TOUCH currentMovement.y, it will completely fuck up the gravity logic
        _currentMovement.z = positionToLookAt.z;

        Quaternion currentRotation = transform.rotation;
        //rotates player to face correct orientation
        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt, Vector3.up);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleMovement()
    {
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, _acceleration * Time.deltaTime);

        _characterController.Move(new Vector3(_currentMovement.x * _currentSpeed, _appliedMovement.y * _currentSpeed, _currentMovement.z * _currentSpeed) * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {

        handleRotation();
        handleMovement();
        _currentState.UpdateStates();
    }

    void OnEnable()
    {
        //enables character control action map
        _playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //disables character control action map
        _playerInput.CharacterControls.Disable();
    }
}
