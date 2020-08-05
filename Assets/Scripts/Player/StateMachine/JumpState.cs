using UnityEngine;
using System.Collections;

public class JumpState : PlayerState {
    bool _jumped = false;
    float _jumpForce;

	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

        _jumped = false;
        _jumpForce = playerController.jumpForce;

        playerController.PlayerAudio.PlayJumpClip();

		_direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;

        playerController.Traction = 1.0f;

        playerIn.PlayerEffects.PlayJumping();
    }

    // Update is called once per frame
    public override void Update(){
        if (!_jumped) {
            // TODO: This can potentially get very ugly, look into a better way to check which animation the player is in
            if (playerController.Animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump") ||
                playerController.Animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump_Ball")) {

                Jump();
            }
        } else {
            // Fall
            playerController.ApplyGravity();

            if(playerController.velocity.y < 0.75f) {
                if (playerController.heldBall != null) {
                    playerController.Animator.Play("Player_AirTransition_Ball");
                } else {
                    playerController.Animator.Play("Player_AirTransition");
                }
            }
            if (playerController.velocity.y <= 0) {
                playerController.Physics.CheckBelow();

                // check if the player has landed
                if (playerController.Physics.IsTouchingFloor) {
                    playerController.ChangeState(PLAYER_STATE.IDLE);
                }
            }
            if (playerController.velocity.y < -0.75f) {
                playerController.ChangeState(PLAYER_STATE.FALL);
            }
        }

        JumpMaxCheck();
    }

    public override void CheckInput(InputState inputState) {
        if(playerController.springing) {
            if (_jumped && playerController.velocity.y <= 1) {
                playerController.springing = false;
            } else {
                return;
            }
        }

		if(inputState.jump.isJustReleased) {
			playerController.velocity.y /= 2;
            _jumpForce /= 2;
		}

		if(inputState.swing.isJustPressed && !playerController.IsInvuln) {
			if(playerController.heldBall == null) {
                if (playerController.CanBubble) {
                    playerController.ChangeState(PLAYER_STATE.CATCH);
                }
            } else if (playerController.CanAim) {
                playerController.ChangeState(PLAYER_STATE.THROW);
			}
		} else if (inputState.attack.isJustPressed && playerController.CanAttack && playerController.heldBall == null) {
            playerController.ChangeState(PLAYER_STATE.ATTACK);
        }

        BaseJumpMovement(inputState);
    }

    void Jump() {
        playerController.velocity = new Vector2(playerController.velocity.x, _jumpForce);
        _jumped = true;
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE GetStateType(){
		return PLAYER_STATE.JUMP;
	}

	//	use this for destruction
	public override void End(){
        if(!_jumped) {
            Jump();
        }
    }
}
