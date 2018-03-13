using UnityEngine;
using System.Collections;

public class BubbleState : PlayerState {
	float attackTime;
	float attackTimer;

	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);
		attackTime = 0.1f;
		attackTimer = 0;

        playerController.PlayerAudio.PlayBubbleClip();

		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;

		playerController.attackBubble.SetActive (true);
        playerController.velocity.x = 0f;
        playerController.bubbleCooldownTimer = 0;
    }

	// Update is called once per frame
	public override void Update(){
		attackTimer += Time.deltaTime;
		if (attackTimer > attackTime) {
			playerController.ChangeState(PLAYER_STATE.IDLE);
		}

        JumpMaxCheck();
        
		// Fall
		playerController.ApplyGravity();
	}

	public override void CheckInput(InputState inputState) {
		if(inputState.jump.isJustReleased) {
			playerController.velocity.y /= 2;
		}

        LockedJumpMovement(inputState);
    }

	// returns the PLAYER_STATE that represents this state
	public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.BUBBLE;
	}

	//	use this for destruction
	public override void End(){
		playerController.attackBubble.SetActive (false);
	}
}
