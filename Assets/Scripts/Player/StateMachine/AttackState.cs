using UnityEngine;
using System.Collections;

public class AttackState : PlayerState {
    // Use this for initialization
    public override void Initialize(PlayerController playerIn) {
        base.Initialize(playerIn);

        playerController.PlayerAudio.PlayAttackClip();

        _direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;

        // Activate attack animation thingy
        playerController.attackObj.Attack();
        playerController.velocity.x = 0f;
        playerController.attackCooldownTimer = 0;
    }

    // Update is called once per frame
    public override void Update() {
        if (!playerController.attackObj.IsAttacking) {
            playerController.ChangeState(PLAYER_STATE.IDLE);
        }

        JumpMaxCheck();

        // Fall
        playerController.ApplyGravity();
    }

    public override void CheckInput(InputState inputState) {
        if (inputState.jump.isJustReleased) {
            playerController.velocity.y /= 2;
        }

        LockedJumpMovement(inputState);
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType() {
        return PLAYER_STATE.ATTACK;
    }

    //	use this for destruction
    public override void End() {
        
    }
}
