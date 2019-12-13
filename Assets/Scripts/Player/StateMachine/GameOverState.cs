using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The player enters this state when the game has finished
public class GameOverState : PlayerState {
    float sign;

    bool _stopped;
    
    // Use this for initialization
    public override void Initialize(PlayerController playerIn) {
        base.Initialize(playerIn);

        _direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;
        playerController.Animator.SetBool("WinLoop", false);

        // Make sure sprite is solid
        playerController.SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

        // Hide held bubble if there is one
        if (playerController.heldBall != null) {
            playerController.heldBall.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    public override void Update() {
        if(_stopped) {
            return;
        }

        if (Mathf.Abs(playerController.velocity.x) > 0.5f) {
            sign = Mathf.Sign(playerController.velocity.x);
            playerController.velocity.x -= sign * (playerController.walkForce / 1.5f) * Time.deltaTime;
            if (sign > 0 && playerController.velocity.x < 0 ||
                sign < 0 && playerController.velocity.x > 0) {
                playerController.velocity.x = 0;
            }
        } else {
            playerController.velocity.x = 0;
        }

        JumpMaxCheck();

        // check if the player has landed
        if (!playerController.Physics.IsTouchingFloor) {
            // Fall
            playerController.ApplyGravity();

            playerController.Physics.CheckBelow();
        } else if(playerController.velocity.x == 0) {
            _stopped = true;
        }
    }

    public override void CheckInput(InputState inputState) {
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType() {
        return PLAYER_STATE.GAMEOVER;
    }

    //	use this for destruction
    public override void End() {
    }
}
