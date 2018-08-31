using UnityEngine;
using System.Collections;

public class ShiftState : PlayerState {

    bool _shifted = false;
    float _shiftTimer = 0;
    float _shiftTime = 0.65f;
    Vector3 _oldScale = new Vector3();

    void Start() {
    }

    // Use this for initialization
    public override void Initialize(PlayerController playerIn) {
        base.Initialize(playerIn);

        if(playerIn.shifted) {
            playerIn.ResetShiftTimer();
        }

        _shifted = false;
        _shiftTimer = 0f;
        _oldScale = playerController.transform.localScale;

        // Slow the player to a stop, even if in midair
        StopPlayerMovement();

        playerController.PlayerAudio.PlayShiftClip();

        ActivateShiftPortal();
    }

    void ActivateShiftPortal() {
        int dir = 0;
        if (!playerController.shifted) {
            if (playerController.team == 0) {
                dir = 1;
            } else if (playerController.team == 1) {
                dir = - 1;
            }
        } else {
            if (playerController.team == 0) {
                dir = - 1;
            } else if (playerController.team == 1) {
                dir = 1;
            }
        }

        playerController.shiftPortal.Activate(dir);
    }
    
    // Update is called once per frame
    public override void Update() {
        _shiftTimer += Time.deltaTime;
        if(_shiftTimer >= _shiftTime/2) {
            if (!_shifted) {
                Vector3 tempScale = playerController.transform.localScale;
                playerController.transform.rotation = Quaternion.identity;
                //playerController.Shift();
                playerController.MirrorShift();
                _shifted = true;
                playerController.transform.localScale = tempScale;
            } else {
                // Undo Rotate/Scale from before
                playerController.transform.Rotate(0f, 0f, -520f * Time.deltaTime);
                playerController.transform.localScale = new Vector3(playerController.transform.localScale.x + (1f * Time.deltaTime), 
                                                                    playerController.transform.localScale.y + (1f * Time.deltaTime), _oldScale.z);
            }
        } else {
            // Rotate/Scale player for little teleport animation
            playerController.transform.Rotate(0f, 0f, 520f * Time.deltaTime);
            playerController.transform.localScale = new Vector3(playerController.transform.localScale.x - (1f * Time.deltaTime), 
                                                                playerController.transform.localScale.y - (1f * Time.deltaTime), _oldScale.z);
        }

        if (_shiftTimer >= _shiftTime) {
            playerController.ChangeState(PLAYER_STATE.IDLE);
        }
    }

    public override void CheckInput(InputState inputState) {

    }

    void StopPlayerMovement() {
        //if (Mathf.Abs(playerController.velocity.x) > 0.5f) {
        //    playerController.velocity.x -= Mathf.Sign(playerController.velocity.x) * playerController.walkForce * 1.5f * Time.deltaTime;
        //} else {
            playerController.velocity.x = 0;
        //}
        //if (Mathf.Abs(playerController.velocity.y) > 0.5f) {
        //    playerController.velocity.y -= Mathf.Sign(playerController.velocity.y) * playerController.jumpForce * 5f * Time.deltaTime;
        //} else {
            playerController.velocity.y = 0;
        //}
    }

    // returns the PLAYER_STATE that represents this state
    public override PLAYER_STATE getStateType() {
        return PLAYER_STATE.SHIFT;
    }

    //	used when changing to another state
    public override void End() {
        playerController.transform.rotation = Quaternion.identity;
        playerController.transform.localScale = _oldScale;
        playerController.shiftPortal.Deactivate();
    }
}
