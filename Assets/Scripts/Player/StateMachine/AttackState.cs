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

        if (playerController.velocity.x >= playerController.jumpMoveMax) {
            playerController.velocity.x = playerController.jumpMoveMax;
        } else if (playerController.velocity.x <= -playerController.jumpMoveMax) {
            playerController.velocity.x = -playerController.jumpMoveMax;
        }

        // Fall
        playerController.ApplyGravity();
    }

    public override void CheckInput(InputState inputState) {
        if (inputState.jump.isJustReleased) {
            playerController.velocity.y /= 2;
        }

        if (inputState.right.isDown) {
            if (_direction < 0) {
                _direction = 1;
            }
            playerController.velocity.x += playerController.jumpMoveForce * Time.deltaTime * _direction;
        } else if (inputState.left.isDown) {
            if (_direction > 0) {
                _direction = -1;
            }
            playerController.velocity.x += playerController.jumpMoveForce * Time.deltaTime * _direction;
        } else {
            playerController.velocity.x /= 1.05f;
        }
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType() {
        return PLAYER_STATE.ATTACK;
    }

    //	use this for destruction
    public override void End() {
        
    }
}
