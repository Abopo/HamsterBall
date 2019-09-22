using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPlayerController : PlayerController {
    public Transform shiftLandingPos;
    public Vector3 returnPos;
    public CharacterSelector characterSelector;

    public bool underControl;
    public bool inPlayArea;

    PullDownWindow pullDownWindow;

    protected override void Awake() {
        base.Awake();

        // Make sure input is set up to the correct player
        SetPlayerNum(playerNum);
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        pullDownWindow = transform.parent.GetComponent<PullDownWindow>();

        // Replace shift state
        _states[7] = new CSShiftState();

        _justChangedState = false;
        _canShift = true;
    }

    // Update is called once per frame
    protected override void Update() {
        if (underControl) {
            CheckInput();
            direction = _animator.GetBool("FacingRight") ? 1 : -1;

            bubbleCooldownTimer += Time.deltaTime;
            attackCooldownTimer += Time.deltaTime;
            aimCooldownTimer += Time.deltaTime;

            // Invuln stuff
            if (IsInvuln) {
                // Be invulnerable
                InvulnerabilityState();
            }

            // State update stuff
            if (currentState != null) {
                currentState.CheckInput(inputState);
                currentState.Update();
            } else {
                ChangeState(PLAYER_STATE.IDLE);
            }

            if (underControl) {
                _physics.MoveX(velocity.x * Time.deltaTime);
                _physics.MoveY(velocity.y * Time.deltaTime);
            }

            _justChangedState = false;
        }
    }

    private void FixedUpdate() {
    }

    protected override void CheckInput() {
        inputState.GetInput();

        if (inputState.shift.isJustPressed && CanShift) {
            // Shift back to pull down window
            ChangeState(PLAYER_STATE.SHIFT);
        }

        // If we're first player or an ai and press select while in a team box
        if(inputState.select.isJustPressed && inputState.playerID == 0 && team != -1) {
            // Control AI player
            characterSelector.ControlNextAI();
            underControl = false;

            // Set to invulnerable so they can't be messed with
            _isInvuln = true;
        }

        if (_physics.IsTouchingFloor && _onFallThrough && inputState.down.isJustPressed) {
            // Move player slightly downward to pass through certain platforms
            transform.Translate(0f, -0.03f, 0f);
        }
    }

    public void ShiftIntoPlayArea() {
        underControl = true;
        returnPos = transform.position;

        // Break off from parent
        transform.SetParent(null);
        pullDownWindow.PlayerLeft();

        ChangeState(PLAYER_STATE.SHIFT);
    }

    public void EnterPlayArea() {
        underControl = true;
        inPlayArea = true;
    }

    public void EnterPullDownWindow() {
        underControl = false;

        // Reattach parent
        transform.SetParent(pullDownWindow.transform);
        pullDownWindow.PlayerReturned();

        characterSelector.Unready();
    }

    public void SetInputPlayer(int playerNum) {
        inputState.SetPlayerID(playerNum);
    }

    public void RegainControl() {
        underControl = true;
        _isInvuln = false;
    }
}
