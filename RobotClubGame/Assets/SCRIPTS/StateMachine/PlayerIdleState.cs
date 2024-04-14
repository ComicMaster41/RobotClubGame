using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Derives from player baste state
// All function calls in PlayerBaseState
public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        Ctx.Speed = Ctx.WalkSpeed;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    // This function checks player state, and checks if player is moving and running
    public override void CheckSwitchStates()
    {
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
            SwitchState(Factory.Run());
        else if (Ctx.IsMovementPressed)
            SwitchState(Factory.Walk());
    }

    public override void InitializeSubState() { }
}
