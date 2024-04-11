using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        HandleJump();
        Ctx.ReadyToJump = false;
        Ctx.IsJumping = true;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        Ctx.IsJumping = false;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Grounded && Ctx.ReadyToJump)
            SwitchState(Factory.Grounded());
        else if (Ctx.IsWallRunning)
            SwitchState(Factory.Wallrunning());
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
            SetSubState(Factory.Idle());
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
            SetSubState(Factory.Walk());
        else
            SetSubState(Factory.Run());
    }

    void HandleJump()
    {
        //handle jumping animation
        //Debug.Log("jumpg");
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.Animator.SetBool(Ctx.IsFallingHash, false);

        Ctx.RB.velocity = new Vector3(Ctx.RB.velocity.x, 0f, Ctx.RB.velocity.z);
        Ctx.RB.AddForce(Ctx.RB.transform.up * Ctx.InitialJumpVelocity, ForceMode.Impulse);
    }


}
