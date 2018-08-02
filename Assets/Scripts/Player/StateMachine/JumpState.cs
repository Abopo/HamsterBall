using UnityEngine;
using System.Collections;

public class JumpState : PlayerState {
	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

        playerController.PlayerAudio.PlayJumpClip();

		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;
		playerController.velocity = new Vector2(playerController.velocity.x, playerController.jumpForce);
	}

	// Update is called once per frame
	public override void Update(){
        JumpMaxCheck();
		
		// Fall
		playerController.ApplyGravity();

		if (playerController.velocity.y < 0) {
			playerController.ChangeState(PLAYER_STATE.FALL);
		}
	}

	public override void CheckInput(InputState inputState) {
        if(playerController.springing) {
            if (playerController.velocity.y <= 1) {
                playerController.springing = false;
            } else {
                return;
            }
        }

		if(inputState.jump.isJustReleased) {
			playerController.velocity.y /= 2;
		}

		if(inputState.bubble.isJustPressed && !playerController.IsInvuln) {
			if(playerController.heldBubble == null) {
                if (playerController.CanBubble) {
                    playerController.ChangeState(PLAYER_STATE.BUBBLE);
                }
            } else if (playerController.CanAim) {
                playerController.ChangeState(PLAYER_STATE.THROW);
			}
		} else if (inputState.attack.isJustPressed && playerController.CanAttack && playerController.heldBubble == null) {
            playerController.ChangeState(PLAYER_STATE.ATTACK);
        }

        BaseJumpMovement(inputState);
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.JUMP;
	}

	//	use this for destruction
	public override void End(){
	}
}
