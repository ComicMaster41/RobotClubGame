using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallrunState : PlayerBaseState
{
    public PlayerWallrunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        if(Ctx.IsWallRunning)
            WallRunMovement();
    }

    public void WallRunMovement()
    {
        Ctx.RB.useGravity = false;
        Ctx.RB.velocity = new Vector3(Ctx.RB.velocity.x, 0f, Ctx.RB.velocity.z);
        Debug.Log("I'm moving");
        Vector3 wallNormal = Ctx.WallRight ? Ctx.RightWallHit.normal : Ctx.LeftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, Ctx.Orientation.up);

        Ctx.RB.AddForce(wallForward * Ctx.WallRunForce, ForceMode.Force);
    }

    public override void ExitState()
    {
        Ctx.RB.useGravity = true;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsJumpPressed)
            SwitchState(Factory.Jump());
        else if (!Ctx.IsWallRunning && !Ctx.Grounded)
            SwitchState(Factory.Falling());
        else if (!Ctx.IsWallRunning && Ctx.Grounded)
            SwitchState(Factory.Grounded());
    }

    public override void InitializeSubState()
    {

    }
}
