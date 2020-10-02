using UnityEngine;
using System.Collections;

public class ShiftState : PlayerState {

    public bool endGameShift; // If this is the shift onto the banner at the end of the game

    float _shiftTimer = 0;
    float _totalShiftTime = 0.65f;
    Vector3 _oldScale = new Vector3();

    Vector2 _takeOffPosition;
    Vector2 _landingPosition;
    float _scaleT = 0f;
    float _scaleVelocity;
    float _startScale;
    float _endScale;

    float _initialZPos;

    SpriteRenderer _targetArrow;

    void Start() {
    }

    // Use this for initialization. Ran every time the player enters the state
    public override void Initialize(PlayerController playerIn) {
        base.Initialize(playerIn);

        if(playerIn.shifted) {
            playerIn.ResetShiftTimer();
        }

        _totalShiftTime = 0.65f;

        _shiftTimer = 0f;

        // Slow the player to a stop, even if in midair
        StopPlayerMovement();

        // If the player has a ball and it wasn't thrown
        if(playerController.heldBall != null && !playerController.heldBall.wasThrown) {
            // Hide it so it doesn't look weird
            playerController.heldBall.HideSprites();
        }

        playerController.PlayerAudio.PlayShiftClip();

        _takeOffPosition = playerController.transform.position;

        float shiftDistance = 12.8f;
        //if(playerController.LevelManager.mirroredLevel) {
        //shiftDistance = Mathf.Abs(playerController.transform.position.x) * 2;
        //}

        if (!endGameShift) {
            // Find landing point
            if (!playerController.shifted) {
                if (playerController.team == 0) {
                    // Going right
                    _landingPosition = new Vector2(playerController.transform.position.x + shiftDistance, playerController.transform.position.y);
                    playerController.FaceRight();
                } else if (playerController.team == 1) {
                    // Going left
                    _landingPosition = new Vector2(playerController.transform.position.x - shiftDistance, playerController.transform.position.y);
                    playerController.FaceLeft();
                }
            } else {
                if (playerController.team == 0) {
                    // Going left
                    _landingPosition = new Vector2(playerController.transform.position.x - shiftDistance, playerController.transform.position.y);
                    playerController.FaceLeft();
                } else if (playerController.team == 1) {
                    // Going right
                    _landingPosition = new Vector2(playerController.transform.position.x + shiftDistance, playerController.transform.position.y);
                    playerController.FaceRight();
                }
            }
        }

        // Make sure the target arrow is off by default
        playerController.PlayerEffects.targetArrow.enabled = false;

        // Push the player's sprite higher on the draw order (so it renders above the stage)
        playerController.SpriteRenderer.sortingOrder = 25;

        // Save the initial scale to return to at the end
        _startScale = playerController.transform.localScale.x;
        // Calculate the largest scale the sprite will get to
        if (endGameShift) {
            _endScale = _startScale * 20;
        } else {
            _endScale = _startScale * 10;
        }
        _scaleT = 0f;
        _scaleVelocity = _totalShiftTime;

        // Save the player's initial z position
        _initialZPos = playerController.transform.position.z;
        // Push the player forward so they render over level stuff
        playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, -10);

        // Turn off main collider so walls and stuff don't get in the way
        playerController.GetComponent<BoxCollider2D>().enabled = false;

        // Save the current scale to return to at the end
        _oldScale = playerController.transform.localScale;

        playerController.PlayerEffects.PlayShiftWind();
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
        if(endGameShift) {
            // If we are shrinking, and the scale drops below 2
            if(_scaleVelocity < 0 && Mathf.Abs(scale) < 2) {
                // Cap the scale at 2
                scale = Mathf.Sign(scale) * 2;
            }
        }
        playerController.transform.localScale = new Vector3(scale, Mathf.Abs(scale), 1);

        if (_shiftTimer >= _totalShiftTime) {
            if (endGameShift) {
                playerController.ChangeState(PLAYER_STATE.GAMEOVER);
            } else {
                playerController.ChangeState(PLAYER_STATE.IDLE);
            }
        }
    }

    public override void CheckInput(InputState inputState) {

    }

    void StopPlayerMovement() {
        playerController.velocity.x = 0;
        playerController.velocity.y = 0;
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE GetStateType() {
        return PLAYER_STATE.SHIFT;
    }

    //	used when changing to another state
    public override void End() {
        playerController.transform.rotation = Quaternion.identity;

        if (endGameShift) {
            playerController.shifted = false;

            // Make sure the player faces the center
            if (playerController.team == 0) {
                //playerController.FaceRight();
                playerController.transform.localScale = new Vector3(2f, 2f, 1f);
            } else if (playerController.team == 1) {
                //playerController.FaceLeft();
                playerController.transform.localScale = new Vector3(-2f, 2f, 1f);
            }
        } else {
            playerController.transform.localScale = _oldScale;

            playerController.shifted = !playerController.shifted;

            // If we are in the opponents area
            if(playerController.shifted) {
                // Turn on out target arrows
                playerController.PlayerEffects.targetArrow.enabled = true;
            }

            // Return player's z pos to normal
            playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, _initialZPos);

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

    public void SetLandingPosition(Vector2 lPos) {
        _landingPosition = lPos;

        // Face towards the direction we are shifting
        if (_landingPosition.x < playerController.transform.position.x) {
            playerController.FaceLeft();
            _startScale *= -1;
            _endScale *= -1;
        } else if (_landingPosition.x > playerController.transform.position.x) {
            playerController.FaceRight();
            _startScale = Mathf.Abs(_startScale);
            _endScale = Mathf.Abs(_endScale);
        }
    }
}
