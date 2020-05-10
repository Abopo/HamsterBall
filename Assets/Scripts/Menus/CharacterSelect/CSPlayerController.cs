using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CSPlayerController : PlayerController {
    public Transform shiftLandingPos;
    public Vector3 returnPos;
    public CharacterSelector characterSelector;

    public bool underControl;
    public bool inPlayArea;
    public int playerInputID = -1;

    PullDownWindow pullDownWindow;
    CharacterSelect _charaSelect;

    // Networking
    NetworkedCSPlayer _networkedCSPlayer;
    Vector3 _baseScale;

    protected override void Awake() {
        base.Awake();

        _charaSelect = FindObjectOfType<CharacterSelect>();
        _networkedCSPlayer = GetComponent<NetworkedCSPlayer>();

        // Make sure input is set up to the correct player
        SetPlayerNum(playerNum);
        SetInputID(playerNum);
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        pullDownWindow = transform.parent.GetComponent<PullDownWindow>();

        // Replace shift state
        _states[7] = new CSShiftState();

        _justChangedState = false;
        _canShift = true;

        _baseScale = transform.lossyScale;
    }

    // Update is called once per frame
    protected override void Update() {
        if (inputState != null) {
            playerInputID = inputState.playerID;
        } else {
            playerInputID = -1;
        }

        // Don't update if the game in paused
        if(_gameManager.isPaused) {
            return;
        }

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

            _canShift = true; // Make sure we can shift at any time

            _justChangedState = false;
        } else if(!inPlayArea) {
            // Make absolute fucking 100% sure we are in the goddamn idle state when we are in the pull down window jesus fucking christ
            if (CurState != PLAYER_STATE.IDLE) {
                ChangeState(PLAYER_STATE.IDLE);
            }

            // Also make sure we are the correct scale
            if (transform.lossyScale != _baseScale) {
                transform.SetParent(null);
                transform.localScale = _baseScale;
                transform.SetParent(pullDownWindow.transform);
            }
        }

        if (inPlayArea) {
            // Make sure sprite is on correct layer
            if (_spriteRenderer.sortingOrder != 0) {
                _spriteRenderer.sortingOrder = 0;
            }
        }
    }

    private void FixedUpdate() {
    }

    protected override void CheckInput() {
        inputState.GetInput();

        if (inputState.shift.isJustPressed && CanShift) {
            // Shift back to pull down window
            StartShift();
        }

        // If we're first player or an ai and press select while in a team box
        if (inputState.select.isJustPressed && team != -1) {
            // Create AI Player
            _charaSelect.ActivateAI(characterSelector);

            // Control AI player
            //characterSelector.ControlNextAI();
            underControl = false;

            // Set to invulnerable so they can't be messed with
            _isInvuln = true;

            ChangeState(PLAYER_STATE.IDLE);
        }

        /* CSPlayer should never be on a fall through anyway
        if (_physics.IsTouchingFloor && _onFallThrough && inputState.down.isJustPressed) {
            // Move player slightly downward to pass through certain platforms
            transform.Translate(0f, -0.03f, 0f);
        }
        */
    }

    public void ShiftIntoPlayArea() {
        underControl = true;
        returnPos = transform.position;

        // Break off from parent
        //transform.SetParent(null);
        pullDownWindow.PlayerLeft();

        Shift();

        if (_photonView != null && _photonView.isMine) {
            _photonView.RPC("CSShift", PhotonTargets.Others);
        }
    }

    public void EnterPlayArea() {
        PlayArea();

        if(_networkedCSPlayer != null && _networkedCSPlayer.photonView.isMine) {
            _networkedCSPlayer.photonView.RPC("EnterPlayArea", PhotonTargets.Others);
        }
    }
    public void PlayArea() {
        underControl = true;
        inPlayArea = true;

        // Make sure our shifted variable is accurate
        shifted = true;

        transform.SetParent(null);
        transform.localScale = _baseScale;
    }

    public void EnterPullDownWindow() {
        PullDownWindow();

        if (_networkedCSPlayer != null && _networkedCSPlayer.photonView.isMine) {
            _networkedCSPlayer.photonView.RPC("EnterPullDownWindow", PhotonTargets.Others);
            Debug.Log("RPC EnterPullDownWindow Sent");
        }
    }
    public void PullDownWindow() {
        underControl = false;
        inPlayArea = false;

        // Make sure our shifted variable is accurate
        shifted = false;

        // Make sure we are in idle
        ChangeState(PLAYER_STATE.IDLE);

        // Set self to return position
        transform.position = returnPos;
        transform.localScale = _baseScale;

        if(transform.localScale.x > 0) {
            FacingRight = true;
        } else {
            FacingRight = false;
        }

        // Reattach parent
        transform.SetParent(pullDownWindow.transform);

        pullDownWindow.PlayerReturned();

        characterSelector.Unready();
    }

    public void SetInputPlayer(int playerNum) {
        inputState.SetPlayerID(playerNum);
    }
    public void SetInputPlayer(Player player) {
        inputState.SetPlayer(player);
    }

    public void RegainControl() {
        underControl = true;
        _isInvuln = false;
    }
}
