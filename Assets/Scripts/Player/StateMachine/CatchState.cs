using UnityEngine;
using System.Collections;

public class CatchState : PlayerState {
    bool _swingDone;
    float sign;

    // Use this for initialization
    public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;

        playerController.bubbleCooldownTimer = 0;

        _swingDone = false;
    }

    // Update is called once per frame
    public override void Update(){
        JumpMaxCheck();
        
		// Fall
		playerController.ApplyGravity();

        // Fail safe so Ai doesn't get stuck in this state
        if(playerController.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
            Debug.Log("Catch animation failsafe triggered");
            playerController.ChangeState(PLAYER_STATE.IDLE);
        }
	}

	public override void CheckInput(InputState inputState) {
		if(inputState.jump.isJustReleased) {
			playerController.velocity.y /= 2;
		}

        //BaseJumpMovement(inputState);
        if (inputState.right.isDown) {
            if (_direction < 0) {
                _direction = 1;
                playerController.Flip();
            }
            playerController.velocity.x += playerController.jumpMoveForce * playerController.WaterMultiplier * playerController.Traction * playerController.speedModifier * Time.deltaTime * _direction;
        } else if (inputState.left.isDown) {
            if (_direction > 0) {
                _direction = -1;
				playerController.Flip();
            }
            playerController.velocity.x += playerController.jumpMoveForce * playerController.WaterMultiplier * playerController.Traction * playerController.speedModifier * Time.deltaTime * _direction;
        } else {
            if (Mathf.Abs(playerController.velocity.x) > 0.5f) {
                sign = Mathf.Sign(playerController.velocity.x);
                playerController.velocity.x -= sign * (playerController.walkForce / 1.5f) * playerController.Traction * Time.deltaTime;
                if (sign > 0 && playerController.velocity.x < 0 ||
                    sign < 0 && playerController.velocity.x > 0) {
                    playerController.velocity.x = 0;
                }
            } else {
                playerController.velocity.x = 0;
            }
        }

        if (_swingDone) {
            if(inputState.jump.isJustPressed && playerController.CanJump) {
                playerController.ChangeState(PLAYER_STATE.JUMP);
            } else if(inputState.attack.isJustPressed && playerController.heldBall == null) {
                playerController.ChangeState(PLAYER_STATE.ATTACK);
            }
        }
    }

    public void Activate() {
        playerController.swingObj.SetActive(true);

        // If the player is not on ice (is on normal ground)
        if(playerController.Traction == 1f) {
            // Halt momentum for the swing
            playerController.velocity.x = 0f;
        }
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
		return PLAYER_STATE.CATCH;
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
