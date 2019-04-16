using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This shift is only used in Character Select, used to jump between pull down window and the play space.
public class CSShiftState : PlayerState {
    float _shiftTimer = 0;
    float _totalShiftTime = 0.65f;
    Vector3 _oldScale = new Vector3();

    Vector2 _takeOffPosition;
    Vector2 _landingPosition;
    float _scaleT = 0f;
    float _scaleVelocity;
    float _startScale;
    float _endScale;

    void Start() {
    }

    // Use this for initialization. Ran every time the player enters the state
    public override void Initialize(PlayerController playerIn) {
        base.Initialize(playerIn);

        _totalShiftTime = 0.65f;

        _shiftTimer = 0f;

        // Slow the player to a stop, even if in midair
        StopPlayerMovement();

        // If the player has a ball
        if (playerController.heldBall != null) {
            // Hide it so it doesn't look weird
            playerController.heldBall.HideSprites();
        }

        playerController.PlayerAudio.PlayShiftClip();

        _takeOffPosition = playerController.transform.position;

        float shiftDistance = 12.5f;
        //if(playerController.LevelManager.mirroredLevel) {
        shiftDistance = Mathf.Abs(playerController.transform.position.x) * 2;
        //}

        // Find landing point
        if(playerController.shifted) {
            _landingPosition = ((CSPlayerController)playerController).returnPos;
        } else {
            _landingPosition = ((CSPlayerController)playerController).shiftLandingPos.position;
        }

        // Push the player's sprite higher on the draw order (so it renders above the stage)
        playerController.SpriteRenderer.sortingOrder = 8;

        // Save the initial scale to return to at the end
        _startScale = playerController.transform.localScale.x;
        // Calculate the largest scale the sprite will get to
        _endScale = _startScale * 10;
        _scaleT = 0f;
        _scaleVelocity = _totalShiftTime;

        // Turn off main collider so walls and stuff don't get in the way
        playerController.GetComponent<BoxCollider2D>().enabled = false;

        // Save the current scale to return to at the end
        _oldScale = playerController.transform.localScale;
    }

    // Update is called once per frame
    public override void Update() {
        _shiftTimer += _totalShiftTime * Time.deltaTime;

        // Jump towards landing point
        // Apply initial jump force towards landing point
        // Lerp towards landing point
        playerController.transform.position = new Vector3(Mathf.Lerp(_takeOffPosition.x, _landingPosition.x, _shiftTimer / _totalShiftTime),
                                                          Mathf.Lerp(_takeOffPosition.y, _landingPosition.y, _shiftTimer / _totalShiftTime),
                                                            playerController.transform.position.z);

        // Lerp up/down scale to appear as if character is jumping towards the screen.
        _scaleT += _scaleVelocity * Time.deltaTime;
        _scaleVelocity -= (_totalShiftTime * 2) * Time.deltaTime;
        if (_scaleT > 1f) {
            _scaleT = 1f;
        }
        // Apply the scale changes
        float scale = Mathf.Lerp(_startScale, _endScale, _scaleT);
        playerController.transform.localScale = new Vector3(scale, Mathf.Abs(scale), 1);

        if (_shiftTimer >= _totalShiftTime) {
            playerController.ChangeState(PLAYER_STATE.IDLE);
        }
    }

    public override void CheckInput(InputState inputState) {

    }

    void StopPlayerMovement() {
        playerController.velocity.x = 0;
        playerController.velocity.y = 0;
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType() {
        return PLAYER_STATE.SHIFT;
    }

    //	used when changing to another state
    public override void End() {
        playerController.transform.rotation = Quaternion.identity;
        playerController.transform.localScale = _oldScale;
        //playerController.shiftPortal.Deactivate();

        playerController.shifted = !playerController.shifted;
        if (playerController.shifted) {
            ((CSPlayerController)playerController).EnterPlayArea();
        } else {
            ((CSPlayerController)playerController).EnterPullDownWindow();
        }

        // Return player's sprite's draw order back to normal
        playerController.SpriteRenderer.sortingOrder = 0;

        // If the player has a ball
        if (playerController.heldBall != null) {
            // Display it
            playerController.heldBall.DisplaySprites();
        }

        // Turn main collider back on.
        playerController.GetComponent<BoxCollider2D>().enabled = true;
    }
}
