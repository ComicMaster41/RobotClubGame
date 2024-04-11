using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallrunState : PlayerBaseState
{
    public PlayerWallrunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        Ctx.Animator.SetBool(Ctx.IsFallingHash, true);
        //Ctx.Speed = Ctx.RunSpeed;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if(Ctx.IsWallRunning) // Called in PlayerStateMachine
            WallRunMovement(); // If true, call WallRunMovement
    }

    public void WallRunMovement()
    {
        // Gravity is false
        Ctx.RB.useGravity = false;

        // Set velocity to be a new vector of x and z
        Ctx.RB.velocity = new Vector3(Ctx.RB.velocity.x, 0f, Ctx.RB.velocity.z);
        Debug.Log("WallRunMovement called");

        // Create a new vector that checks if right, run right, else run left
        Vector3 wallNormal = Ctx.WallRight ? Ctx.RightWallHit.normal : Ctx.LeftWallHit.normal;

        // New vector for running forward
        Vector3 wallForward = Vector3.Cross(wallNormal, Ctx.Orientation.up);

        Ctx.RB.AddForce(wallForward * Ctx.WallRunForce, ForceMode.Force);

        //Ctx.StartCoroutine(StartGravity());
    }

    // Temporary function that waits a second, then sets the gravity to true
    IEnumerator StartGravity()
    {
        yield return new WaitForSeconds(1f);
        Ctx.RB.useGravity = true;
        SwitchState(Factory.Grounded());
    }

    // This doesn't appear to be called
    public override void ExitState()
    {
        Ctx.RB.useGravity = true;
    }

    public override void CheckSwitchStates()
    {
        /*
         * ISSUES:
         * player does wall running, but jumps high in the air and even when touching ground, still considered falling
         * player jumps really high in the air
         * player doesn't exit their animation state, probably because it is seeing that it is falling
         */


        if (Ctx.IsJumpPressed)
        {
            Debug.Log("Is Jumping");
            SwitchState(Factory.Jump());
        }

        else if (!Ctx.IsWallRunning && !Ctx.Grounded)
        {
            SwitchState(Factory.Falling());
            Debug.Log("Is falling");
        }

        if (!Ctx.IsWallRunning && Ctx.Grounded)
        {
            SwitchState(Factory.Grounded());
            Debug.Log("Not wall running, is Grounded");
        }

      //  if (Ctx.IsWallRunning) Debug.Log("Wall running right now");
    }

    public override void InitializeSubState()
    {

    }
}
