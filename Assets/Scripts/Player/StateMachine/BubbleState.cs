using UnityEngine;
using System.Collections;

public class BubbleState : PlayerState {
	float attackTime;
	float attackTimer;

    bool _swingDone;

	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);
		attackTime = 0.1f;
		attackTimer = 0;

        playerController.PlayerAudio.PlayBubbleClip();

		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;

        playerController.bubbleCooldownTimer = 0;

        _swingDone = false;
    }

    // Update is called once per frame
    public override void Update(){
		//attackTimer += Time.deltaTime;
		//if (attackTimer > attackTime) {
		//	playerController.ChangeState(PLAYER_STATE.IDLE);
		//}

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
            if(inputState.jump.isJustPressed) {
                playerController.ChangeState(PLAYER_STATE.JUMP);
            } else if(inputState.attack.isJustPressed) {
                playerController.ChangeState(PLAYER_STATE.ATTACK);
            } else if(inputState.shift.isJustPressed) {
                playerController.ChangeState(PLAYER_STATE.SHIFT);
            }
        }
    }

    public void Activate() {
        playerController.attackBubble.SetActive(true);
        playerController.velocity.x = 0f;
    }

    public void Deactivate() {
        playerController.attackBubble.SetActive(false);
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
		playerController.attackBubble.SetActive (false);
	}
}
