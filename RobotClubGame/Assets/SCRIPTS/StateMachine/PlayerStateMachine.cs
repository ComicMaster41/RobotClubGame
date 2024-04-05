using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    //Declare reference variables
    public Animator _animator;
    Camera _mainCamera;
    Rigidbody _rb;

    int _isWalkingHash;
    int _isRunningHash;
    int _isFallingHash;
    int _isJumpingHash;

    //Variables to store player input values
    public Vector2 _currentMovementInput;
    Vector3 _currentMovement;

    [Header("Movement")]
    private float _speed;
    public float _walkSpeed;
    public float _runSpeed;
    public float _groundDrag;
    bool _isMovementPressed;
    bool _isRunPressed;
    public Transform _orientation;
    float _rotationFactorPerFrame = 20f;

    [Header("Key Binds")]
    public KeyCode _jumpKey = KeyCode.Space;
    public KeyCode _runKey = KeyCode.LeftShift;
    float _verticalInput;
    float _horizontalInput;

    [Header("WallRunning")]
    public LayerMask _whatIsWall;
    public float _wallRunForce;
    public float _wallRunSpeed;
    public float _maxWallRunTime;
    private float _wallRunTimer;
    private bool _isWallRunning;

    [Header("Wall Detection")]
    public float _wallCheckDistance;
    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    private bool _wallLeft;
    private bool _wallRight;

    [Header("Ground Check")]
    public float _playerHeight;
    public LayerMask _whatIsGround;
    bool _grounded;



    //Gravity Variables
    float _gravity = -9.8f;
    float _groundGravity = -0.05f;

    //Jumping Variables
    bool _isJumpPressed = false;
    [Header("Jumping")]
    [SerializeField] float _initialJumpVelocity;
    [SerializeField] float _jumpCooldown;
    [SerializeField] float _airMultiplier;
    bool _readyToJump = true;
    bool _isJumping = false;

    //State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //getters and setters

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        //Animation
    public Animator Animator { get { return _animator; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int IsFallingHash { get { return _isFallingHash; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
        //Jumping
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool IsJumping { get { return _isJumping; } set { _isJumping = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float AirMultiplier { get { return _airMultiplier; } }
    public bool ReadyToJump { get { return _readyToJump; } set { _readyToJump = value; } }
    public bool Grounded { get { return _grounded; } }
        //Movement
    public Rigidbody RB { get { return _rb; } }
    public Transform Orientation { get { return _orientation; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }
    public float CurrentMovementZ { get { return _currentMovement.z; } set { _currentMovement.z = value; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float WalkSpeed { get { return _walkSpeed; } }
    public float RunSpeed { get { return _runSpeed; } }
    public float VerticalInput { get { return _verticalInput; } }
    public float HorizontalInput { get { return _horizontalInput; } }
        //Gravity
    public float GroundGravity { get { return _groundGravity; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }
        //Wallrunning
    public bool WallRight { get { return _wallRight; } }
    public bool WallLeft { get { return _wallLeft; } }
    public bool IsWallRunning { get { return _isWallRunning; } set { _isWallRunning = value; } }
    public float WallRunForce { get { return _wallRunForce; } }
    public float WallRunTimer { get { return _wallRunTimer; } }
    public float WallRunSpeed { get { return _wallRunSpeed; } }
    public RaycastHit RightWallHit { get { return _rightWallHit; } }
    public RaycastHit LeftWallHit { get { return _leftWallHit; } }


    void Awake()
    {
        //initially set reference variables
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isFallingHash = Animator.StringToHash("isFalling");
        _isJumpingHash = Animator.StringToHash("isJumping");


    }

    //Sets speeds of character movement
    void onInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        _isMovementPressed = _horizontalInput != 0f || _verticalInput != 0f;
        _isRunPressed = Input.GetKey(_runKey);

        _isJumpPressed = Input.GetKey(_jumpKey);
        if (_isJumpPressed && _readyToJump && _grounded)
        {
            Invoke(nameof(ResetJump), _jumpCooldown);
        }
        
    }

    void ResetJump()
    {
        _readyToJump = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void handleMovement()
    {
        //Calculate movement direction
        _currentMovement = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        ///On Ground
        if(_grounded)
            _rb.AddForce(_currentMovement.normalized * _speed * 10f, ForceMode.Force);
        //In Air
        else if(!_grounded)
            _rb.AddForce(_currentMovement.normalized * _speed * 10f * _airMultiplier, ForceMode.Force);
    }

    void handleRotation()
    {
        Vector3 viewDir = transform.position - new Vector3(_mainCamera.transform.position.x, transform.position.y, _mainCamera.transform.position.z);
        _orientation.forward = viewDir.normalized;

        Vector3 inputDir = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        if (inputDir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, Time.deltaTime * _rotationFactorPerFrame);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        //limit velocity if needed
        if(flatVel.magnitude > _speed)
        {
            Vector3 limitedVel = flatVel.normalized * _speed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    private void WallCheck()
    {
        _wallRight = Physics.Raycast(transform.position, _orientation.right, out _rightWallHit, _wallCheckDistance, _whatIsWall);
        _wallLeft = Physics.Raycast(transform.position, -_orientation.right, out _leftWallHit, _wallCheckDistance, _whatIsWall);

        if ((_wallLeft || _wallRight) && _verticalInput > 0)
        {
            _isWallRunning = true;
        }
        else
            _isWallRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);
        //drag
        if (_grounded)
        {
            _rb.drag = _groundDrag;
        }
        else
            _rb.drag = 0;
        WallCheck();
        onInput();
        SpeedControl();
        _currentState.UpdateStates();
    }

    private void FixedUpdate()
    {
        handleRotation();
        handleMovement();
    }
}
