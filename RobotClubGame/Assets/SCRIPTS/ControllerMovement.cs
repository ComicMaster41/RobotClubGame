using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    //Declare reference variables
    PlayerMovement playerInput;
    CharacterController characterController;
    Animator animator;
    Camera mainCamera;

    int isWalkingHash;
    int isRunningHash;
    int isFallingHash;
    int isJumpingHash;

    //Variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    [SerializeField]float speed;
    [SerializeField]float acceleration;
    [SerializeField] float runMultiplier = 3.0f;
    float currentSpeed;
    bool isMovementPressed;
    bool isRunPressed;
    float direction;

    float rotationFactorPerFrame = 20f;

    //Gravity Variables
    float gravity = -9.8f;
    float groundGravity = -0.05f;

    //Jumping Variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    [SerializeField] float maxJumpHeight = 1.0f;
    [SerializeField] float maxJumpTime = 0.5f;
    bool isJumping = false;
    bool isJumpAnimating = false;

    void Awake()
    {
        //initially set reference variables
        playerInput = new PlayerMovement();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isFallingHash = Animator.StringToHash("isFalling");
        isJumpingHash = Animator.StringToHash("isJumping");

        //Player input callbacks
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();

    }

    void handleRotation()
    {
        if (currentMovementInput.sqrMagnitude == 0) return;

        Vector3 positionToLookAt = currentMovement;

        //set direction for player to look at. Looks in direction of camera
        positionToLookAt = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(currentMovementInput.x, 0.0f, currentMovementInput.y);

        //player moves in the direction they are looking
        currentMovement.x = positionToLookAt.x;    //remember to NEVER even TOUCH currentMovement.y, it will completely fuck up the gravity logic
        currentMovement.z = positionToLookAt.z;

        Quaternion currentRotation = transform.rotation;
        //rotates player to face correct orientation
        if(isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt, Vector3.up);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    //Sets up variables that will be used for jumping
    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    //Checks if the jump button was pressed
    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    //Makes the character jump
    void handleJump()
    {
        if(!isJumping && characterController.isGrounded && isJumpPressed)
        {
            //handle jumping animation
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isFallingHash, false);
            isJumpAnimating = true;

            isJumping = true;
            currentMovement.y = initialJumpVelocity * .5f;
        }
        else if(!isJumpPressed && isJumping && characterController.isGrounded) 
        { 
            isJumping = false; 
        }
    }

    //Checks if run button is pressed
    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    //Sets speeds of character movement
    void onMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement = new Vector3(currentMovementInput.x, 0.0f, currentMovementInput.y);
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    //Handles the gravity and the falling
    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;

        //REMINDER TO NOT TOUCH currentMovement.y IF YOU ARE NOT RAF THEN YOU'RE GOOD
        if(characterController.isGrounded)
        {
            //I can't remember what I did here, but it was probably in preparation for a falling animation
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            animator.SetBool(isFallingHash, false);

            //gravity when grounded
            currentMovement.y = groundGravity;
        }
        else if(isFalling)
        {
            animator.SetBool(isFallingHash, true);
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity =Mathf.Max((previousYVelocity + newYVelocity) * .5f, -20.0f);
            currentMovement.y = nextYVelocity;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
        }
    }

    //sets animation booleans and handles running/walking states
    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isFalling = animator.GetBool(isFallingHash);
        bool isJumping = animator.GetBool(isJumpingHash);

        if((isMovementPressed && !isWalking) && !isRunPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    void handleMovement()
    {
        var targetSpeed = isRunPressed ? speed * runMultiplier : speed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        characterController.Move(new Vector3(currentMovement.x * currentSpeed, currentMovement.y, currentMovement.z * currentSpeed) * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();
        handleMovement();
        handleGravity();
        handleJump();

    }

    void OnEnable()
    {
        //enables character control action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable() 
    {
        //disables character control action map
        playerInput.CharacterControls.Disable();
    }
}
