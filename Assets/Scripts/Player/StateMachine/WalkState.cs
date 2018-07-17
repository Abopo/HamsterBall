using UnityEngine;
using System.Collections;

public class WalkState : PlayerState {
	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);
		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;
	}

	// Update is called once per frame
	public override void Update(){
		playerController.velocity.x += playerController.walkForce * playerController.WaterMultiplier * playerController.Traction * playerController.speedModifier * Time.deltaTime * _direction;
		
		if(playerController.velocity.x >= playerController.walkSpeed * playerController.WaterMultiplier * playerController.speedModifier) {
			playerController.velocity.x = playerController.walkSpeed * playerController.WaterMultiplier * playerController.speedModifier;
		} else if(playerController.velocity.x <= -playerController.walkSpeed * playerController.WaterMultiplier * playerController.speedModifier) {
			playerController.velocity.x = -playerController.walkSpeed * playerController.WaterMultiplier * playerController.speedModifier;
		}
		
		// Check below the player to make sure they 
		// are walking on something
		playerController.Physics.CheckBelow ();
		playerController.CheckPosition();
	}

	public override void CheckInput(InputState inputState) {
		if (inputState.jump.isJustPressed) {
			playerController.ChangeState (PLAYER_STATE.JUMP);
		} else if (!PlayerController.Grounded) {
			playerController.ChangeState (PLAYER_STATE.FALL);
		} 

		if(inputState.bubble.isJustPressed && !playerController.IsInvuln) {
			if(playerController.heldBubble == null) {
                if (playerController.CanBubble) {
                    playerController.ChangeState(PLAYER_STATE.BUBBLE);
                }
            } else if (playerController.CanAim) {
                playerController.ChangeState(PLAYER_STATE.THROW);
			}
		} else if (inputState.attack.isJustPressed && playerController.heldBubble == null) {
            playerController.ChangeState(PLAYER_STATE.ATTACK);
        }

        if (inputState.right.isDown) {
			if(_direction < 0) {
				_direction = 1;
				playerController.Flip();
			}
		} else if(inputState.left.isDown) {
			if(_direction > 0) {
				_direction = -1;
				playerController.Flip();
			}
		} else {
			playerController.ChangeState(PLAYER_STATE.IDLE);
		}
	}

	// returns the PLAYER_STATE that represents this state
	public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.WALK;
	}

	//	use this for destruction
	public override void End(){
	}
}
