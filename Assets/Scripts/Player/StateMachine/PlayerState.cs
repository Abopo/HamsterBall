using UnityEngine;
using System.Collections;

public enum PLAYER_STATE { IDLE=0, WALK, JUMP, FALL, BUBBLE, THROW, HIT, SHIFT, ATTACK, NUM_STATES };

public class PlayerState {
	protected int _direction;	// -1 for left, 1 for right

	protected PlayerController playerController;
	public PlayerController PlayerController {
		get {
			return playerController;
		}
	}

	// Use this for initialization
	public virtual void Initialize(PlayerController playerIn) {
		playerController = playerIn;

	}
    // Update is called once per frame
    public virtual void Update(){}
	// GetInput is used for all Input handling
	public virtual void CheckInput(InputState inputState){}
	// returns the PLAYER_STATE that represents this state
	public virtual PLAYER_STATE getStateType(){
		return PLAYER_STATE.NUM_STATES;
	}
	//	use this for destruction
	public virtual void End() {}

    protected void JumpMaxCheck() {
        if (playerController.velocity.x >= playerController.jumpMoveMax * playerController.WaterMultiplier * playerController.speedModifier) {
            playerController.velocity.x = playerController.jumpMoveMax * playerController.WaterMultiplier * playerController.speedModifier;
        } else if (playerController.velocity.x <= -playerController.jumpMoveMax * playerController.WaterMultiplier * playerController.speedModifier) {
            playerController.velocity.x = -playerController.jumpMoveMax * playerController.WaterMultiplier * playerController.speedModifier;
        }
    }

    // Player will change direction here
    protected void BaseJumpMovement(InputState inputState) {
        if (inputState.right.isDown) {
            if (_direction < 0) {
                _direction = 1;
                playerController.Flip();
            }
            playerController.velocity.x += playerController.jumpMoveForce * playerController.WaterMultiplier * playerController.speedModifier * Time.deltaTime * _direction;
        } else if (inputState.left.isDown) {
            if (_direction > 0) {
                _direction = -1;
                playerController.Flip();
            }
            playerController.velocity.x += playerController.jumpMoveForce * playerController.WaterMultiplier * playerController.speedModifier * Time.deltaTime * _direction;
        } else {
            playerController.velocity.x /= 1.05f;
        }
    }

    // Player won't change direction here
    protected void LockedJumpMovement(InputState inputState) {
        if (inputState.right.isDown) {
            if (_direction < 0) {
                _direction = 1;
            }
            playerController.velocity.x += playerController.jumpMoveForce * playerController.WaterMultiplier * playerController.speedModifier * Time.deltaTime * _direction;
        } else if (inputState.left.isDown) {
            if (_direction > 0) {
                _direction = -1;
            }
            playerController.velocity.x += playerController.jumpMoveForce * playerController.WaterMultiplier * playerController.speedModifier * Time.deltaTime * _direction;
        } else {
            playerController.velocity.x /= 1.05f;
        }
    }
}
