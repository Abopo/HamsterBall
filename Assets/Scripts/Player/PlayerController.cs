﻿using UnityEngine;
using UnityEngine.Events;
using Rewired;
using Photon;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(EntityPhysics))]
public class PlayerController : Entity {
	public float walkForce; // 50
	public float walkSpeed; // 5
    public float speedModifier = 1.0f;
	public float jumpForce; // 8.2
	public float jumpMoveForce; // 50
	public float jumpMoveMax; // 5
	public PlayerState currentState;
	public GameObject swingObj;
    public Transform bubblePosition;
    public GameObject attackObj;
	public Bubble heldBall;
    public int direction;
    public bool aimAssist;
    public bool canBeHit;

	//public bool isLeftTeam;
    public int team; // -1 = no team, 0 = left team, 1 = right team
    public int playerNum;
	public InputState inputState;
    public bool hasControl = true;

    public int atkModifier; // Modifies the amount of junk generated when making matches

    [SerializeField]
    private PLAYER_STATE _curState;
    public PLAYER_STATE CurState {
        get {
            if(currentState != null) {
                _curState = currentState.GetStateType();
                return currentState.GetStateType();
            } else {
                return PLAYER_STATE.IDLE;
            }
        }
    }
    public bool CanJump {
        get { return GetComponent<EntityPhysics>().IsTouchingFloor; }
    }
	public bool shifted;
    protected bool _canShift;
    public bool CanShift {
        get { return (_canShift && !_isInvuln && (CurState == PLAYER_STATE.IDLE || CurState == PLAYER_STATE.WALK || CurState == PLAYER_STATE.JUMP || CurState == PLAYER_STATE.FALL)); }
    }
    float _shiftCooldownTime;
	public float ShiftCooldownTime {
		get {return _shiftCooldownTime;}
	}
	float _shiftCooldownTimer;
	public float ShiftCooldownTimer {
		get { return _shiftCooldownTimer;	}
        set { _shiftCooldownTimer = value; }
	}

    public float bubbleCooldownTimer;
    public float bubbleCooldownTime; // 0.15f
    public bool CanBubble {
        get {
            if(bubbleCooldownTimer >= bubbleCooldownTime) {
                return true;
            }
            return false;
        }
    }
    public float attackCooldownTimer;
    public float attackCooldownTime; // 0.3f
    public bool CanAttack {
        get {
            if (attackCooldownTimer >= attackCooldownTime && !_isInvuln) {
                return true;
            }
            return false;
        }
    }
    public float aimCooldownTimer;
    public float aimCooldownTime; //0.2f
    public bool CanAim {
        get {
            if (aimCooldownTimer >= aimCooldownTime) {
                return true;
            }
            return false;
        }
    }

    public PlayerAudio PlayerAudio {
        get { return _playerAudio; }
    }

    protected PlayerState[] _states = new PlayerState[10];
    List<PLAYER_STATE> _lockedStates = new List<PLAYER_STATE>();

    // The grace period time after getting hit
    protected bool _isInvuln;
    float _invulnTimer;
    float _invulnTime = 1.15f;
    bool _blinked;
    float _blinkTimer;
    float _blinkTime = 0.1f;
    public bool IsInvuln {
        get { return _isInvuln; }
    }

    bool _freeze; // Stops all player movement and input
    public bool Freeze { get => _freeze; }
    float _freezeTimer = 0f;
    float _freezeTime = 0f;

    float _traction = 1.0f;
    public float Traction {
        get { return _traction; }
        set { _traction = value; }
    }
    public int platformIndex; // This changes based on the kind of platform the player is standing on

    public bool aiControlled;
    public bool springing;

    protected CharaInfo _charaInfo = new CharaInfo();
    public CharaInfo CharaInfo {
        get { return _charaInfo; }
    }

    PlayerAudio _playerAudio;
    Vector3 _spawnPos;

    BubbleManager _homeBubbleManager;
    public BubbleManager HomeBubbleManager {
        get { return _homeBubbleManager; }
    }
    LevelManager _levelManager;
    public LevelManager LevelManager {
        get { return _levelManager; }
    }
    protected SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer {
        get { return _spriteRenderer; }
    }

    PlayerEffects _playerEffects;
    public PlayerEffects PlayerEffects {
        get { return _playerEffects; }
    }

    public bool _justChangedState; // Can only change state once per frame

    // Networking
    protected PhotonView _photonView;
    public PhotonView PhotonView {
        get { return _photonView; }
    }

    // Events
    public UnityEvent significantEvent;

    public static int totalThrowCount;

    protected override void Awake() {
        base.Awake();

        canBeHit = true;

        _playerAudio = GetComponent<PlayerAudio>();
        _spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _playerEffects = GetComponentInChildren<PlayerEffects>();
        _gameManager = GameManager.instance;
        _levelManager = FindObjectOfType<LevelManager>();

        inputState = new InputState();

        swingObj = transform.Find("CatchBubble").gameObject;
        swingObj.SetActive(false);


        _photonView = GetComponent<PhotonView>();

        InitStates();
    }

    // Use this for initialization
    protected override void Start () {
		base.Start ();

        _gameManager.gameOverEvent.AddListener(GameEnded);

        _spawnPos = transform.position;

        _canShift = false;
		_shiftCooldownTime = 12.0f;
		_shiftCooldownTimer = 0f;
        direction = _animator.GetBool("FacingRight") ? 1 : -1;

        // Ignore collision with our own attack object
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), attackObj.GetComponent<CircleCollider2D>());

        FindHomeBubbleManager();

        totalThrowCount = 0;

        // Character specific scripts
        // (right now the rooster is the only character with a special script)
        if (_charaInfo.name == CHARACTERS.ROOSTER) {
            gameObject.AddComponent<Rooster>();
        } else if(_charaInfo.name == CHARACTERS.LIZARD) {
            gameObject.AddComponent<Lizard>();
        }

        //	start in idle
        ChangeState(PLAYER_STATE.IDLE);

        hasControl = true;
    }

    protected virtual void InitStates() {
        _states[0] = new IdleState();
        _states[1] = new WalkState();
        _states[2] = new JumpState();
        _states[3] = new FallState();
        _states[4] = new CatchState();
        _states[5] = new ThrowState();
        _states[6] = new HitState();
        _states[7] = new ShiftState();
        _states[8] = new AttackState();
        _states[9] = new GameOverState();
    }

    public void SetPlayerNum(int pNum) {
        playerNum = pNum;
    }

    public void SetInputID(int id) {
        if (id >= 0) {
            inputState.SetPlayerID(id);
        }
    }

    public void SetCharacterInfo(CharaInfo charaInfo) {
        _charaInfo = charaInfo;

        string animatorPath = "Art/Animations/Player/";
        string palettePath = "";
        switch (_charaInfo.name) {
            case CHARACTERS.BOY:
                animatorPath += "Boy/Animation Objects/Boy1";// + _charaInfo.color;
                palettePath = "Materials/Character Palettes/Boy/Boy" + _charaInfo.color;
                break;
            case CHARACTERS.GIRL:
                animatorPath += "Girl/Animation Objects/Girl1";// + _charaInfo.color;
                palettePath = "Materials/Character Palettes/Girl/Girl" + _charaInfo.color;
                break;
            case CHARACTERS.ROOSTER:
                animatorPath += "Rooster/Animation Objects/Rooster1";// + _charaInfo.color;
                palettePath = "Materials/Character Palettes/Rooster/Rooster" + _charaInfo.color;
                break;
            case CHARACTERS.BAT:
                animatorPath += "Bat/Animation Objects/Bat1"; // + _charaInfo.color;
                palettePath = "Materials/Character Palettes/Bat/Bat" + _charaInfo.color;
                break;
            case CHARACTERS.OWL:
                animatorPath += "Owl/Animation Objects/Owl"; // + _charaInfo.color;
                palettePath = "Materials/Character Palettes/Owl/Owl" + _charaInfo.color;
                break;
            case CHARACTERS.GOAT:
                animatorPath += "Goat/Animation Objects/Goat"; // + _charaInfo.color;
                palettePath = "Materials/Character Palettes/Goat/Goat" + _charaInfo.color;
                break;
            case CHARACTERS.SNAIL:
                animatorPath += "Snail/Animation Objects/Snail1";// + charaInfo.color;
                palettePath = "Materials/Character Palettes/Snail/Snail" + _charaInfo.color;
                break;
            case CHARACTERS.LIZARD:
                animatorPath += "Lizard/Animation Objects/Lizard1";
                palettePath = "Materials/Character Palettes/Lizard/Lizard" + _charaInfo.color;
                break;
            case CHARACTERS.LACKEY:
                animatorPath += "Lackey/Animation Objects/Lackey" + _charaInfo.color;
                // Make sure line sprite is turned off
                GetComponentInChildren<LinesSprite>().turnOff = true;
                break;
        }

        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.runtimeAnimatorController = Resources.Load(animatorPath) as RuntimeAnimatorController;

        if(_spriteRenderer == null) {
            _spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        }
        if (palettePath != "" && _charaInfo.color > 0) {
            _spriteRenderer.material = Resources.Load(palettePath) as Material;
        } else {
            _spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    public void FindHomeBubbleManager() {
        // Find a proper bubble manager
        GameObject bubbleManager = null;
        if (_gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager1");
        } else {
            if (team == 0) {
                bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager1");
            } else if (team == 1) {
                bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager2");
            }
        }

        if (bubbleManager != null) {
            _homeBubbleManager = bubbleManager.GetComponent<BubbleManager>();
        }
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        if(_freeze) {
            _freezeTimer += Time.deltaTime;
            if(_freezeTimer >= _freezeTime) {
                _freeze = false;

                // TODO: right now freeze only happens for airship, if it ends up being used elsewhere
                // this will have to be reworked
                Respawn();
            } else {
                return;
            }
        }

        // Only update some stuff if the game is over or hasn't started yet
        if(_gameManager.gameIsOver || (_levelManager != null && !_levelManager.gameStarted)) {
            currentState.CheckInput(inputState);
            currentState.Update();

            // Allow player to slow movement/fall to ground
            //_physics.MoveX(velocity.x * Time.deltaTime);
            //_physics.MoveY(velocity.y * Time.deltaTime);

            _justChangedState = false;

            return;
        }

        // Don't do any of this stuff if the game is paused
        if (!_gameManager.isPaused) {
            if (hasControl) {
                CheckInput();
            }
            direction = _animator.GetBool("FacingRight") ? 1 : -1;

            UpdateBubbles();

            bubbleCooldownTimer += Time.deltaTime;
            attackCooldownTimer += Time.deltaTime;
            aimCooldownTimer += Time.deltaTime;

            if (!_gameManager.isSinglePlayer) {
                // Shift time stuff
                ShiftUpdates();
            } else {
                _canShift = false;
            }

            // Invuln stuff
            if (_isInvuln) {
                // Be invulnerable
                InvulnerabilityState();
            }

            // State update stuff
            if (currentState != null) {
                if (hasControl) {
                    currentState.CheckInput(inputState);
                }
                if (!_justChangedState) { // Give the game a frame to update animations and stuff before heading into update
                    currentState.Update();
                }
            } else {
                ChangeState(PLAYER_STATE.IDLE);
            }

            // Snap to any slopes as long as we aren't jumping
            if(CurState != PLAYER_STATE.JUMP) {
                _physics.SnapToSlope();
            } else {
                _physics.snappedToSlope = false;
            }

            _justChangedState = false;
        }

        // Failsafe checks
        if (heldBall != null && heldBall.locked) {
            heldBall = null;
        }
        if(CurState != PLAYER_STATE.ATTACK && attackObj.gameObject.activeSelf) {
            attackObj.gameObject.SetActive(false);
        }

		//_playerAnimationTriggers.PlayerFootstepEvent.setParameterValue("Surface", platformIndex);
	}

    private void FixedUpdate() {
        // Don't do any of this stuff if the game is paused
        if ((!_gameManager.isPaused && !_freeze) ||
            _gameManager.gameIsOver || (_levelManager != null && !_levelManager.gameStarted)) {
            _physics.MoveX(velocity.x * Time.deltaTime);
            _physics.MoveY(velocity.y * Time.deltaTime);
        }
    }
    
    protected virtual void CheckInput() {
        if (!aiControlled) {
            //inputState = InputState.GetInput(inputState);
            inputState.GetInput();
        }

        if (inputState.shift.isJustPressed && CanShift) {
            // Shift to opposite field.
            StartShift();
        }

        // If start is pressed
        if(inputState.pause.isJustPressed) {
            if (_levelManager != null) {
                // Open the pause menu
                _levelManager.PauseGame(inputState.playerID);
            }
        }

        if(_physics.IsTouchingFloor && _physics.IsOnPassthrough && inputState.down.isJustPressed && CurState != PLAYER_STATE.THROW) {
            // Move player slightly downward to pass through certain platforms
            //transform.Translate(0f, -0.05f, 0f);
        }

        // If the player is holding down
        if(inputState.down.isDown) {
            // Ignore any passthrough platforms
            _physics.IgnorePassthrough();
        }
        if(inputState.down.isJustReleased) {
            // Don't ignore passthrough platforms anymore
            _physics.DetectPassthrough();
        }
    }

    public void StartShift() {
        if (CurState != PLAYER_STATE.SHIFT) {
            Shift();

            if (_photonView != null && _photonView.isMine && !_gameManager.gameIsOver) {
                _photonView.RPC("ShiftPlayer", PhotonTargets.Others);
            }
        }
    }

    protected void Shift() {
        if (shifted) {
            _shiftCooldownTimer = 0f;
            _canShift = false;
        }
        ChangeState(PLAYER_STATE.SHIFT);
    }

    public override void Flip() {
        base.Flip();

        // Also flip the effects so particles generate properly
        Vector3 theScale = _playerEffects.transform.localScale;
        theScale.x *= -1;
        _playerEffects.transform.localScale = theScale;
    }

    void UpdateBubbles() {
		if (heldBall != null && !heldBall.wasThrown) {
			walkSpeed = 3.25f;
			jumpMoveMax = 3.25f;
			heldBall.transform.position = new Vector3 (bubblePosition.position.x,
                                                         bubblePosition.position.y,
                                                         bubblePosition.position.z-10);
            
		} else {
			walkSpeed = 5;
			jumpMoveMax = 5;
		}
	}

    void ShiftUpdates() {
        if (shifted && CurState != PLAYER_STATE.SHIFT) {
            _shiftCooldownTimer -= Time.deltaTime * 2;
            if (_shiftCooldownTimer <= 0) {
                //Shift ();
                ChangeState(PLAYER_STATE.SHIFT);
                _canShift = false;
                _shiftCooldownTimer = 0;
            }
        } else {
            _shiftCooldownTimer += Time.deltaTime;
            if (_shiftCooldownTimer >= _shiftCooldownTime) {
                if(_canShift == false) {
                    _playerAudio.PlayShiftReadyClip();
                }
                _canShift = true;
                _shiftCooldownTimer = _shiftCooldownTime;
            }
        }
    }

    protected void InvulnerabilityState() {
        _blinkTimer += Time.deltaTime;
        if (_blinkTimer > _blinkTime) {
            // Blink sprite
            if (_blinked) {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                _blinked = false;
            } else {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                _blinked = true;
            }
            _blinkTimer = 0f;
        }

        _invulnTimer += Time.deltaTime;
        if (_invulnTimer >= _invulnTime) {
            // Stop invuln time
            StopInvuln();
        }
    }

	public void CheckPosition() {
		if (!_physics.IsTouchingFloor && !_physics.snappedToSlope) {
			ChangeState(PLAYER_STATE.FALL);
		}
	}

	public PlayerState GetPlayerState(PLAYER_STATE state){
        return _states[(int)state];
	}

	//	Call to switch from one state to another
	public void ChangeState(PLAYER_STATE state){
        if(_lockedStates.Contains(state)) {
            return;
        }

        //if (!_justChangedState) {
            //Debug.Log("Change state: " + state.ToString());
        if (null != currentState) {
            currentState.End();
        }
        currentState = GetPlayerState(state);
        if (null != currentState) {
            _animator.SetInteger("PlayerState", (int)CurState);
            _animator.speed = 1;
            currentState.Initialize(this);
        }
        _justChangedState = true;
        _curState = currentState.GetStateType();

        if(_photonView != null && _photonView.owner == PhotonNetwork.player) {
            _photonView.RPC("ChangePlayerState", PhotonTargets.Others, (int)_curState);
        }
        //Debug.Log("State Changed to: " + currentState.getStateType().ToString());
        //}
    }

    public void LockState(PLAYER_STATE state) {
        if(!_lockedStates.Contains(state)) {
            _lockedStates.Add(state);
        }
    }
    public void UnlockState(PLAYER_STATE state) {
        _lockedStates.Remove(state);
    }

    // Used in case there are multiple player animators that need to be synched
    void SetAnimatorParameter(string parameter, int value) {
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach(Animator anim in animators) {
            anim.SetInteger(parameter, value);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // Attack Object
		if (collider.gameObject.layer == 12 && collider.gameObject != attackObj && 
            (team != collider.GetComponentInParent<PlayerController>().team || team == -1) && 
            !_isInvuln && CurState != PLAYER_STATE.SHIFT && canBeHit) {
			ChangeState(PLAYER_STATE.HIT);
            ((HitState)GetPlayerState(PLAYER_STATE.HIT)).Knockback((int)Mathf.Sign(transform.position.x - collider.transform.position.x));
            
            //_canShift = false;
			//_shiftCooldownTimer = 0;
		}

        if(collider.tag == "LightningBolt") {
            ChangeState(PLAYER_STATE.HIT);
            ((HitState)GetPlayerState(PLAYER_STATE.HIT)).Knockback((int)Mathf.Sign(transform.position.x - collider.transform.position.x));
        }

        if (collider.name == "Ice Platform") {
            _traction = 0.2f;
        }

        // If we've fallen through the airship hole
        if(collider.gameObject.tag == "Bottom") {
            _freeze = true;
            _freezeTimer = 0f;
            _freezeTime = 2f;
        }

        // If we somehow go out of bounds
        if(collider.gameObject.tag == "KillPlane") {
            Respawn();
        }
	}

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Fire" && !_isInvuln) {
            ChangeState(PLAYER_STATE.HIT);
            ((HitState)GetPlayerState(PLAYER_STATE.HIT)).Knockback((int)Mathf.Sign(transform.position.x - collision.transform.position.x));
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.name == "Ice Platform") {
            _traction = 1.0f;
        }
    }

    public override void CollisionResponseX(Collider2D collider) {
        base.CollisionResponseX(collider);

        if (collider.gameObject.layer == 21 /*Platform*/ || collider.gameObject.layer == 9 || collider.gameObject.layer == 13) {
			velocity.x = 0.0f;
		}
	}
	public override void CollisionResponseY(Collider2D collider) {
        //base.CollisionResponseY(collider);

        // Only change the platform type if we're landing on the floor
        if (velocity.y < 0f && GetComponent<EntityPhysics>().IsTouchingFloor) {
            PlatformTypeCheck(collider.gameObject.GetComponent<Platform>());
            if (CurState == PLAYER_STATE.FALL) {
                // We should be in fall state sooo
                ((FallState)currentState).Land();
            }
        }

        velocity.y = 0.0f;

        if (collider.name == "Ice Platform") {
            _traction = 0.2f;
        } else if (collider.name != "Ice Platform" && collider.tag == "Platform") {
            _traction = 1f;
        }
	}

    void PlatformTypeCheck(Platform plat) {
        if (plat != null) {
            platformIndex = plat.platformIndex;

            // React to the player landing on us
            plat.React();
        }
    }
    
    public override void Spring(float springForce) {
        base.Spring(springForce);

        springing = true;
        velocity.x = 0;
        velocity.y = 0;
        float temp = jumpForce;
        jumpForce = springForce+1.0f; // add a little extra oompf for the player
        ChangeState(PLAYER_STATE.JUMP);
        jumpForce = temp;
    }

    public void StartInvulnTime() {
        _isInvuln = true;
        _invulnTimer = 0f;
    }
    public void StopInvuln() {
        _isInvuln = false;
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    // Increase the shift cooldown based on how big of a match just happened
    public void IncreaseShiftCooldown(int matches) {
        //_shiftCooldownTimer += (3 + ((matches-3) * 2));
    }

    public void TakeInput(InputState input) {
        inputState = input;
    }

    public void ResetShiftTimer() {
        _shiftCooldownTimer = 0f;
    }

    public override void Respawn() {
        base.Respawn();

        // Reset to a spawn pos on the correct side of the stage
        if (!shifted) {
            // Reset to spawn position
            transform.position = _spawnPos;
        } else {
            transform.position = new Vector3(_spawnPos.x + ((team == 0 ? 1 : -1) * 12.5f),
                                            _spawnPos.y,
                                            _spawnPos.z);
        }
    }

    void GameEnded() {
        _animator.SetBool("Won Game", _homeBubbleManager.wonGame);

        // Do a win/lose animation here or something
        ChangeState(PLAYER_STATE.GAMEOVER);
    }
}
