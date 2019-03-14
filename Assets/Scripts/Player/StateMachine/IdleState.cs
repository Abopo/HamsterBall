using UnityEngine;
using System.Collections;

public class IdleState : PlayerState {
    float sign;

    float _longIdleTime = 5.0f;
    float _longIdleTimer = 0f;

    void Start() {
	}

	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

        // Double check holding bubble
        if (playerController.heldBall != null) {
            playerController.Animator.SetBool("HoldingBall", true);
        } else {
            playerController.Animator.SetBool("HoldingBall", false);
        }

        _longIdleTimer = 0f;

        // Make sure extra hitboxes are turned off
        playerController.swingObj.SetActive(false);
        playerController.attackObj.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void Update(){
		//Just Idling man, don't do shit
		
		// Just kidding, slow the player to a stop
		if (Mathf.Abs (playerController.velocity.x) > 0.5f) {
            sign = Mathf.Sign(playerController.velocity.x);
            playerController.velocity.x -= sign * (playerController.walkForce/1.5f) * playerController.Traction * Time.deltaTime;
            if(sign > 0 && playerController.velocity.x < 0 ||
                sign < 0 && playerController.velocity.x > 0) {
                playerController.velocity.x = 0;
            }
		} else {
			playerController.velocity.x = 0;
		}

        // Update for the long idle animation
        _longIdleTimer += Time.deltaTime;
        if(_longIdleTimer >= _longIdleTime) {
            // Play long idle animation
            playerController.Animator.SetBool("LongIdle", true);

            _longIdleTimer = 0f;
        }

        playerController.ApplyGravity();

        // Check below the player to make sure they 
        // are standing on something
        playerController.Physics.CheckBelow ();
		playerController.CheckPosition();
	}

	public override void CheckInput(InputState inputState) {
		if (inputState.jump.isJustPressed) {
			playerController.ChangeState (PLAYER_STATE.JUMP);
			return;
		} else if (inputState.left.isDown || inputState.right.isDown) {
			playerController.ChangeState (PLAYER_STATE.WALK);
			return;
		} else if (inputState.swing.isJustPressed && !playerController.IsInvuln) {
			if(playerController.heldBall == null) {
                if (playerController.CanBubble) {
                    playerController.ChangeState(PLAYER_STATE.BUBBLE);
                }
			} else if (playerController.CanAim) {
                playerController.ChangeState(PLAYER_STATE.THROW);
			}
		} else if (inputState.attack.isJustPressed && playerController.CanAttack && playerController.heldBall == null) {
            playerController.ChangeState(PLAYER_STATE.ATTACK);
        }
	}

	// returns the PLAYER_STATE that represents this state
	public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.IDLE;
	}

	//	use this for destruction
	public override void End(){
        playerController.Animator.SetBool("LongIdle", false);
    }
}
