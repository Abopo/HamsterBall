using UnityEngine;
using System.Collections;

public class FallState : PlayerState {
	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);
		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;
	}

	// Update is called once per frame
	public override void Update(){
        JumpMaxCheck();

        // Fall
        playerController.ApplyGravity();

		playerController.Physics.CheckBelow ();

		// check if the player has landed
		if(playerController.Physics.IsTouchingFloor) {
			playerController.ChangeState(PLAYER_STATE.IDLE);
		}
	}

	public override void CheckInput(InputState inputState) {
		if(inputState.swing.isJustPressed && !playerController.IsInvuln) {
			if(playerController.heldBubble == null) {
                if (playerController.CanBubble) {
                    playerController.ChangeState(PLAYER_STATE.BUBBLE);
                }
            } else if(playerController.CanAim) {
				playerController.ChangeState(PLAYER_STATE.THROW);
			}
		} else if (inputState.attack.isJustPressed && playerController.CanAttack && playerController.heldBubble == null) {
            playerController.ChangeState(PLAYER_STATE.ATTACK);
        }

        BaseJumpMovement(inputState);
	}

	// returns the PLAYER_STATE that represents this state
	public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.FALL;
	}

	//	use this for destruction
	public override void End(){
	}
}
