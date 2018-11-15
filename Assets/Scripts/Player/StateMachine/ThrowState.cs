using UnityEngine;
using System.Collections;

public class ThrowState : PlayerState {

    public Transform aimingArrow;

	float rotateSpeed = 60f;
    float bubbleSpeed = 7f; // How fast the bubble goes when thrown

    // Small cooldown between readying aim and throwing.
    float throwTimer = 0;
    float throwTime = 0.2f;

    bool _hasThrown;

    AimingLine _aimingLine;

    public Vector2 AimDirection {
        get {
            return aimingArrow.GetChild(0).transform.position - aimingArrow.position;
        }
    }

    void Start() {
	}
	
	// Use this for initialization
	public override void Initialize(PlayerController playerIn){
		base.Initialize(playerIn);

        aimingArrow = playerIn.transform.GetComponentInChildren<AimingLine>(true).transform;
		//aimingArrow = playerIn.transform.GetChild (3);
		aimingArrow.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
		aimingArrow.gameObject.SetActive (true);

        _aimingLine = aimingArrow.GetComponent<AimingLine>();
        if (_aimingLine != null && playerController.aimAssist) {
            _aimingLine.Begin();
        }

        _direction = playerController.Animator.GetBool("FacingRight") ? 1 : -1;
        _hasThrown = false;

        // For now this is only for the AI
        playerController.significantEvent.Invoke();
    }

    // Update is called once per frame
    public override void Update(){
        // Slow the player to a stop, even if in midair
        StopPlayerMovement();

        throwTimer += Time.deltaTime;

        if (!_hasThrown) {
            // Make sure the aiming arrow is in the right position
            aimingArrow.transform.position = new Vector3(playerController.bubblePosition.position.x,
                                                        playerController.bubblePosition.position.y,
                                                        -15);
        }

        // If we've thrown the bubble and it has locked onto the board
        if (_hasThrown && (playerController.heldBubble == null || playerController.heldBubble.locked)) {
            playerController.heldBubble = null;
            
            // Leave the Throw state
            playerController.ChangeState(PLAYER_STATE.IDLE);
        } else if(!_hasThrown && playerController.heldBubble != null) {
            // Make sure the player can't get stuck in the aim state before throwing
            playerController.Animator.SetBool("HoldingBall", true);
        }
    }

    void StopPlayerMovement() {
        if (Mathf.Abs(playerController.velocity.x) > 0.5f) {
            //playerController.velocity.x -= Mathf.Sign(playerController.velocity.x) * playerController.walkForce * 1.5f * Time.deltaTime;
            float sign = Mathf.Sign(playerController.velocity.x);
            playerController.velocity.x -= sign * (playerController.walkForce/5f) * Time.deltaTime;
            if (sign > 0 && playerController.velocity.x < 0 ||
                sign < 0 && playerController.velocity.x > 0) {
                playerController.velocity.x = 0;
            }
        } else {
            playerController.velocity.x = 0;
        }
        if (Mathf.Abs(playerController.velocity.y) > 0.5f) {
            //playerController.velocity.y -= Mathf.Sign(playerController.velocity.y) * playerController.jumpForce * 5f * Time.deltaTime;
            float sign = Mathf.Sign(playerController.velocity.y);
            playerController.velocity.y -= sign * playerController.jumpForce * 3.75f * Time.deltaTime;
            if (sign > 0 && playerController.velocity.y < 0 ||
                sign < 0 && playerController.velocity.y > 0) {
                playerController.velocity.y = 0;
            }
        } else {
            playerController.velocity.y = 0;
        }
    }

    public override void CheckInput(InputState inputState) {
        // Don't use this function if we are networked and not the master client
        if (_hasThrown) {
            return;
        }

        if (inputState.left.isDown) {
            aimingArrow.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime/* * _direction*/);
            if (_aimingLine != null) {
                _aimingLine.Stop();
            }
        } else if (inputState.right.isDown) {
            aimingArrow.Rotate(Vector3.forward, -rotateSpeed * Time.deltaTime/* * _direction*/);
            if (_aimingLine != null) {
                _aimingLine.Stop();
            }
        } else if (playerController.aimAssist) {
            if (_aimingLine != null) {
                _aimingLine.Begin();
            }
        }

        LimitArrowRotation();

        if (inputState.swing.isJustPressed && throwTimer >= throwTime) {
            // Networking
            if (PhotonNetwork.connectedAndReady) {
                if(playerController.GetComponent<PhotonView>().owner == PhotonNetwork.player) {
                    // If we are the master client
                    if (PhotonNetwork.isMasterClient) {
                        // Just go ahead and throw
                        playerController.GetComponent<PhotonView>().RPC("ThrowBubble", PhotonTargets.Others, aimingArrow.localRotation);
                        Throw();
                    } else {
                        // Check with the master client to see if we are ok to throw
                        playerController.GetComponent<PhotonView>().RPC("TryThrowBubble", PhotonTargets.MasterClient, aimingArrow.localRotation);
                    }
                }
            } else {
                // Tell the animator we don't have a bubble anymore
                playerController.Animator.SetBool("HoldingBall", false);

                _hasThrown = true;
                // Throw bubble!
                // Throw();
            }
        }

        if(inputState.attack.isJustPressed) {
            // Reset aim cooldown
            playerController.aimCooldownTimer = 0f;

            playerController.ChangeState(PLAYER_STATE.IDLE);
        }
	}

    void LimitArrowRotation() {
        if (aimingArrow.localEulerAngles.z < 45) {
            aimingArrow.localEulerAngles = new Vector3(0.0f, 0.0f, 45f);
        }
        if (aimingArrow.localEulerAngles.z > 135) {
            aimingArrow.localEulerAngles = new Vector3(0.0f, 0.0f, 135f);
        }
    }

    public void Throw() {
        if(playerController.heldBubble == null) {
            playerController.ChangeState(PLAYER_STATE.IDLE);
        }

        if (playerController.shifted) {
            // If you are on the opponent side, make the bubble work on their board.
            //playerController.heldBubble.SwitchTeams();
        }
        Vector2 dir = aimingArrow.GetChild(0).position - aimingArrow.position;
        dir.Normalize();

        // Make sure the position is exactly right before releasing
        playerController.heldBubble.transform.position = new Vector3(aimingArrow.position.x, 
                                                                    aimingArrow.position.y, 
                                                                    playerController.heldBubble.transform.position.z);
        playerController.heldBubble.GetComponent<Rigidbody2D>().velocity = new Vector2(bubbleSpeed * dir.x, bubbleSpeed * dir.y);
        playerController.heldBubble.GetComponent<CircleCollider2D>().enabled = true;
        playerController.heldBubble.wasThrown = true;

        // Increase score a little
        if (playerController.heldBubble.type < HAMSTER_TYPES.NUM_NORM_TYPES) {
            playerController.HomeBubbleManager.IncreaseScore(10);
        } else {
            playerController.HomeBubbleManager.IncreaseScore(20);
        }

        playerController.PlayerAudio.PlayThrowClip();

        if (_aimingLine != null) {
            _aimingLine.Stop();
        }

        // For now this is only for the AI
        playerController.significantEvent.Invoke();

        PlayerController.totalThrowCount++;
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType(){
		return PLAYER_STATE.THROW;
	}
	
	//	use this for destruction
	public override void End(){
        aimingArrow.localEulerAngles = new Vector3(0.0f, 0.0f, 90.01f);
        aimingArrow.gameObject.SetActive (false);

        if (_aimingLine != null) {
            _aimingLine.Stop();
        }
    }
}
