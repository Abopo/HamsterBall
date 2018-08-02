using UnityEngine;
using UnityEngine.Events;
using Photon;
using System.Collections;

[RequireComponent (typeof(EntityPhysics))]
[RequireComponent (typeof(Animator))]
public class PlayerController : Entity {
	public float walkForce; // 50
	public float walkSpeed; // 5
    public float speedModifier = 1.0f;
	public float jumpForce; // 8.2
	public float jumpMoveForce; // 50
	public float jumpMoveMax; // 5
	public PlayerState currentState;
	public PLAYER_STATE curState;
	public GameObject attackBubble;
	public Bubble heldBubble;
    public AttackObject attackObj;
    public ShiftPortal shiftPortal;
    public int direction;
    public bool aimAssist;
    public bool canBeHit;
    
	//public bool isLeftTeam;
    public int team; // -1 = no team, 0 = left team, 1 = right team
    public int playerNum;
	public InputState inputState;

    public int atkModifier; // Modifies the amount of junk generated when making matches

	bool _canShift;
    public bool CanShift {
        get { return _canShift; }
    }
    float _shiftCooldownTime;
	public float ShiftCooldownTime {
		get {return _shiftCooldownTime;}
	}
	float _shiftCooldownTimer;
	public float ShiftCooldownTimer {
		get {return _shiftCooldownTimer;	}
        set { _shiftCooldownTimer = value; }
	}

	public bool shifted;
	float _shiftTime;
	float _shiftTimer;
    public float ShiftTimer {
        get { return _shiftTimer; }
        set { _shiftTimer = value; }
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

    PlayerState[] _states = new PlayerState[9];

    // The grace period time after getting hit
    bool _isInvuln;
    float _invulnTimer;
    float _invulnTime = 1.15f;
    bool _blinked;
    float _blinkTimer;
    float _blinkTime = 0.1f;
    public bool IsInvuln {
        get { return _isInvuln; }
    }

    float _traction = 1.0f;
    public float Traction {
        get { return _traction; }
    }

    public bool aiControlled;
    public bool springing;

    PlayerManager _playerManager;
    GameManager _gameManager;
    SpriteRenderer _spriteRenderer;
    PlayerAudio _playerAudio;
    BubbleManager _homeBubbleManager;
    Vector3 _spawnPos;

    public BubbleManager HomeBubbleManager {
        get { return _homeBubbleManager; }
    }

    bool _justChangedState; // Can only change state once per frame

    // Events
    public UnityEvent significantEvent;

    public static int totalThrowCount;

    private void Awake() {
        canBeHit = true;

        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();

        inputState = new InputState();
    }

    // Use this for initialization
    protected override void Start () {
		base.Start ();

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.gameOverEvent.AddListener(GameEnded);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerAudio = GetComponent<PlayerAudio>();

        _spawnPos = transform.position;

		attackBubble.SetActive (false);
        attackObj.team = team;

		_canShift = false;
		_shiftCooldownTime = 12.0f;
		_shiftCooldownTimer = 0;
		_shiftTime = 6.0f;
		_shiftTimer = 0;
        direction = Animator.GetBool("FacingRight") ? 1 : -1;

        if (team == 0) {
            _homeBubbleManager = GameObject.FindGameObjectWithTag("BubbleManager1").GetComponent<BubbleManager>();
        } else if(team == 1) {
            _homeBubbleManager = GameObject.FindGameObjectWithTag("BubbleManager2").GetComponent<BubbleManager>();
        }

        InitStates();

        totalThrowCount = 0;

        //	start in idle
        ChangeState (PLAYER_STATE.IDLE);
	}

    void InitStates() {
        _states[0] = new IdleState();
        _states[1] = new WalkState();
        _states[2] = new JumpState();
        _states[3] = new FallState();
        _states[4] = new BubbleState();
        _states[5] = new ThrowState();
        _states[6] = new HitState();
        _states[7] = new ShiftState();
        _states[8] = new AttackState();
    }

    public void SetPlayerNum(int pNum) {
        playerNum = pNum;
        inputState.controllerNum = _playerManager.GetControllerNum(playerNum);
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        // Don't update if the game is over
        if(_gameManager.gameIsOver) {
            return;
        }

        // Don't do any of this stuff if the game is paused
        if (!_gameManager.isPaused) {
            CheckInput();
            direction = Animator.GetBool("FacingRight") ? 1 : -1;

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
                currentState.CheckInput(inputState);
                currentState.Update();
            } else {
                ChangeState(PLAYER_STATE.IDLE);
            }

            _physics.MoveX(velocity.x * Time.deltaTime);
            _physics.MoveY(velocity.y * Time.deltaTime);

            _justChangedState = false;
        }

        // Failsafe check
        if(heldBubble != null && heldBubble.locked) {
            heldBubble = null;
        }
	}

    void CheckInput() {
        if (!aiControlled) {
            inputState = InputState.GetInput(inputState);
        }

        if (inputState.shift.isJustPressed && _canShift && curState != PLAYER_STATE.HIT && curState != PLAYER_STATE.SHIFT) {
			// Shift to opposite field.
			if(shifted) {
                _shiftCooldownTimer = 0f;
                _shiftTimer = 0f;
                _canShift = false;
				//Shift ();
				//canShift = false;
				//shiftCooldownTimer = 0;
			}
            ChangeState(PLAYER_STATE.SHIFT);
        }

        if(_physics.IsTouchingFloor && inputState.down.isJustPressed) {
            // Move player slightly downward to pass through certain platforms
            transform.Translate(0f, -0.03f, 0f);
        }
    }

	void UpdateBubbles() {
		float dir = facingRight ? 1 : -1;
		attackBubble.transform.position = new Vector3 (transform.position.x + 0.5f * dir,
		                                               transform.position.y,
		                                               transform.position.z);

		if (heldBubble != null && !heldBubble.wasThrown) {
			walkSpeed = 2;
			jumpMoveMax = 2;
			heldBubble.transform.position = new Vector3 (transform.position.x,
			                                             transform.position.y + 0.5f,
			                                             transform.position.z-5);
		} else {
			walkSpeed = 5;
			jumpMoveMax = 5;
		}
	}

    void ShiftUpdates() {
        if (shifted && curState != PLAYER_STATE.SHIFT) {
            _shiftTimer += Time.deltaTime;
            _shiftCooldownTimer -= Time.deltaTime * 2;
            if (_shiftTimer >= _shiftTime) {
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

    public void Shift() {
        if (!shifted) {
            if (team == 0) {
                transform.Translate(12.5f /* * direction* */, 0.0f, 0.0f);
            } else if (team == 1) {
                transform.Translate(-12.5f /* * direction * -1*/, 0.0f, 0.0f);
            }
            //transform.Translate (10.5f * direction * (isLeftTeam ? 1 : -1), 0.0f, 0.0f);
            shifted = true;
        } else {
            if (team == 0) {
                transform.Translate(-12.5f /* * direction * -1*/, 0.0f, 0.0f);
            } else if (team == 1) {
                transform.Translate(12.5f /* * direction */, 0.0f, 0.0f);
            }
            //transform.Translate (10.5f * direction * (isLeftTeam ? -1 : 1), 0.0f, 0.0f);
            shifted = false;
        }

        //_playerAudio.PlayShiftClip();

        _shiftTimer = 0f;
    }

    void InvulnerabilityState() {
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
            _isInvuln = false;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }

	public void CheckPosition() {
		if (!_physics.IsTouchingFloor) {
			ChangeState(PLAYER_STATE.FALL);
		}
	}

	public PlayerState GetPlayerState(PLAYER_STATE state){
        return _states[(int)state];
	}

	//	Call to switch from one state to another
	public void ChangeState(PLAYER_STATE state){
        if (!_justChangedState) {
            //Debug.Log("Change state: " + state.ToString());
            if (null != currentState) {
                currentState.End();
            }
            currentState = GetPlayerState(state);
            if (null != currentState) {
                curState = state;
                _animator.SetInteger("PlayerState", (int)curState);
                _animator.speed = 1;
                currentState.Initialize(this);
            }
            _justChangedState = true;
        }
    }

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == 12 && team != collider.GetComponent<AttackObject>().team/* && shifted */&& !_isInvuln && curState != PLAYER_STATE.SHIFT && canBeHit) {
			ChangeState(PLAYER_STATE.HIT);
            ((HitState)currentState).Knockback((int)Mathf.Sign(transform.position.x - collider.transform.position.x));
            
            //_canShift = false;
			//_shiftCooldownTimer = 0;
		}

        if(collider.name == "Ice Platform") {
            _traction = 0.2f;
        }

        // If we somehow go out of bounds
        if(collider.gameObject.tag == "KillPlane") {
            // Reset to spawn position
            transform.position = _spawnPos;
        }
	}

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.name == "Ice Platform") {
            _traction = 1.0f;
        }
    }

    public override void CollisionResponseX(Collider2D collider) {
		if (collider.gameObject.layer == 9 || collider.gameObject.layer == 13) {
			velocity.x = 0.0f;
		}
	}
	public override void CollisionResponseY(Collider2D collider) {
		if (collider.gameObject.layer == 9 || collider.gameObject.layer == 13 || collider.gameObject.layer == 18) {
			velocity.y = 0.0f;
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

    // Increase the shift cooldown based on how big of a match just happened
    public void IncreaseShiftCooldown(int matches) {
        //_shiftCooldownTimer += (3 + ((matches-3) * 2));
    }

    public void TakeInput(InputState input) {
        inputState = input;
    }

    public void ResetShiftTimer() {
        _shiftTimer = 0f;
    }

    void GameEnded() {
        ChangeState(PLAYER_STATE.IDLE);
        // Do a win/lose animation here or something
    }
}
