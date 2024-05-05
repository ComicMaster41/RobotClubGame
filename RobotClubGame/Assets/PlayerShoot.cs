using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction shootAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); // Component put in Player Object

        shootAction = playerInput.actions["Shoot"]; // Referencing the Shoot called in ActionMap
    }

    private void OnEnable()
    {
        // Assign function to input to shooting events
        if (shootAction != null)
        {
            // Unity Events subscribed if the game object is "Active"
            shootAction.performed += HandleShootActionPerformed;
            shootAction.canceled += HandleShootActionCanceled;
        }
    }

    void OnDisable()
    {
        // Unassign function from from shooting events
        if (shootAction != null)
        {
            // Unsubscribe from shooting events when the player "dies"
            shootAction.performed -= HandleShootActionPerformed;
            shootAction.canceled -= HandleShootActionCanceled;
        }
    }

    public void HandleShootActionPerformed(InputAction.CallbackContext callback)
    {
        Debug.Log("Player is shooting");
        // Check if player shooting is not disabled (if the game is not paused)
            // If game is paused, return
                // Set lock state to none and show cursor
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;

            // Else, allow the player to shoot
                // Hide cursor
                //Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;

                // Shoot weapon function called here
    }

    public void HandleShootActionCanceled(InputAction.CallbackContext callback)
    {
        Debug.Log("Player is NOT shooting");
        // Call stop functions for weapons that you can hold the mouse down
    }
}
