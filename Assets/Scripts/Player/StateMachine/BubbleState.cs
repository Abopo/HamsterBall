using UnityEngine;
using System.Collections;

public class BubbleState : PlayerState {
    bool _swingDone;

	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

        playerController.PlayerAudio.PlayBubbleClip();

		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;

        playerController.bubbleCooldownTimer = 0;

        _swingDone = false;
    }

    // Update is called once per frame
    public override void Update(){
        JumpMaxCheck();
        
		// Fall
		playerController.ApplyGravity();
	}

	public override void CheckInput(InputState inputState) {
		if(inputState.jump.isJustReleased) {
			playerController.velocity.y /= 2;
		}

        BaseJumpMovement(inputState);

        if(_swingDone) {
            if(inputState.jump.isJustPressed && playerController.CanJump) {
                playerController.ChangeState(PLAYER_STATE.JUMP);
            } else if(inputState.attack.isJustPressed && playerController.heldBall == null) {
                playerController.ChangeState(PLAYER_STATE.ATTACK);
            }
        }
    }

    public void Activate() {
        playerController.swingObj.SetActive(true);
        playerController.velocity.x = 0f;
    }

    public void Deactivate() {
        playerController.swingObj.SetActive(false);
        _swingDone = true;
    }

    public void Finish() {
        playerController.ChangeState(PLAYER_STATE.IDLE);
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.BUBBLE;
	}

	//	use this for destruction
	public override void End(){
		playerController.swingObj.SetActive (false);

        if (playerController.heldBall != null) {
            // Make sure the held bubble is displayed
            playerController.heldBall.DisplaySprites();
        }
    }
}
