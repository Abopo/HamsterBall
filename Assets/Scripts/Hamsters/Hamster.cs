using UnityEngine;
using System.Collections.Generic;

public enum HAMSTER_TYPES { NO_TYPE = -1, GREEN = 0, RED, YELLOW, GRAY, BLUE, PINK, PURPLE, NUM_NORM_TYPES,
                            RAINBOW = 8, SKULL, BOMB, PLASMA = 50, NUM_SPEC_TYPES = 4,
                            SPECIAL = 15}; // SPECIAL is only used for special bubbles

public class Hamster : Entity {
    public HAMSTER_TYPES type;
    public int team;
    public int hamsterNum; // just a number specifying each individual hamster
    public bool wasCaught; // used to prevent the same hamster from being caught twice ont he same frame.
    public bool special; // Specifically hamsters spawned via a special bubble, they ignore the hamster line

    public GameObject spiralEffectObj;
	public GameObject spiralEffectInstance;
	public bool isPlasma;

    public bool testMode;

    public bool exitedPipe;
    public bool inRightPipe;
    public bool inLine;
    public bool exitedLine;

    public float curMoveSpeed; //  base - 3, rainbow - 4, dead - 2, gravity - 3.5
    public float moveSpeedModifier; // Is added to the move speed
    protected float _moveSpeed = 3;

    protected int _curState = 0; // The state the hamster is in. 0 = idle, 1 = walk, 2 = fall

    HamsterSpawner _parentSpawner;
    List<int> _okTypes;

    float rainbowMoveSpeed = 4f;
    float skullMoveSpeed = 2.25f;
    float gravityMoveSpeed = 3.5f;

    float _stuckTimer = 0f;
    float _stuckTime = 0.5f;

    float _longIdleTimer = 0f;
    float _longIdleTime = 5f;

    public HamsterSpawner ParentSpawner {
        set { _parentSpawner = value; }
    }

    public int CurState {
        get { return _curState; }
    }

    SpriteOutline _spriteOutline;

    protected override void Awake() {
        base.Awake();

        _spriteOutline = GetComponentInChildren<SpriteOutline>();
    }
    // Use this for initialization
    protected override void Start () {
		base.Start ();
        _gameManager = FindObjectOfType<GameManager>();

        wasCaught = false;

        if (type == HAMSTER_TYPES.NO_TYPE) {
            if(isPlasma) {
                SetType(HAMSTER_TYPES.PLASMA, (HAMSTER_TYPES)SelectValidNormalType());
            }
        }

        _curState = 1;
        _animator.SetInteger("State", _curState);

        moveSpeedModifier = 0;

        UpdateVelocity();
	}

    public void Initialize(int inTeam) {
        team = inTeam;

        _gameManager = FindObjectOfType<GameManager>();

        HamsterScan hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        if (team == 0) {
            _okTypes = hamsterScan.OkTypesLeft;
        } else if (team == 1) {
            _okTypes = hamsterScan.OkTypesRight;
        }

        exitedPipe = false;
    }

    // If setType is -1, will randomly generate a type.
    public void SetType(int setType) {
        if(setType == (int)HAMSTER_TYPES.RAINBOW) {
            _moveSpeed = rainbowMoveSpeed;
        } else if(setType == (int)HAMSTER_TYPES.SKULL) {
            _moveSpeed = skullMoveSpeed;
        } else if(setType == (int)HAMSTER_TYPES.PLASMA) {
            setType = SelectValidNormalType();
            PlasmaInitialize();
        } else {
            _moveSpeed = 3;
        }
        type = (HAMSTER_TYPES)setType;
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.SetInteger("Type", (int)type);

        SetOutlineColor();

        curMoveSpeed = _moveSpeed;
    }

    // This overload is specifically used to set a special type with a color.
    public void SetType(HAMSTER_TYPES sType, HAMSTER_TYPES cType) {
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }

        if (sType == HAMSTER_TYPES.PLASMA) {
            PlasmaInitialize();
        }

        type = cType;
        _animator.SetInteger("Type", (int)type);

        SetOutlineColor();

        curMoveSpeed = _moveSpeed;
    }

    // Initialization for a plasma hamster
    void PlasmaInitialize() {
        isPlasma = true;
        _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Art/Animations/Hamsters/AnimationObjects/Plasma/PlasmaHamster");
        spiralEffectInstance = Instantiate(spiralEffectObj, transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
        spiralEffectInstance.transform.parent = transform;
        spiralEffectInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        _moveSpeed = gravityMoveSpeed;
    }

    void SetOutlineColor() {
        if(_spriteOutline != null) {
            switch (type) {
                case HAMSTER_TYPES.BLUE:
                    _spriteOutline.color = Color.blue;
                    break;
                case HAMSTER_TYPES.GRAY:
                    _spriteOutline.color = Color.gray;
                    break;
                case HAMSTER_TYPES.GREEN:
                    _spriteOutline.color = Color.green;
                    break;
                case HAMSTER_TYPES.PINK:
                    _spriteOutline.color = new Color(1.0f, 0f, 0.5f);
                    break;
                case HAMSTER_TYPES.PURPLE:
                    _spriteOutline.color = new Color(0.8f, 0f, 1f);
                    break;
                case HAMSTER_TYPES.RED:
                    _spriteOutline.color = Color.red;
                    break;
                case HAMSTER_TYPES.YELLOW:
                    _spriteOutline.color = Color.yellow;
                    break;
                case HAMSTER_TYPES.SKULL:
                    _spriteOutline.color = Color.black;
                    break;
                case HAMSTER_TYPES.RAINBOW:
                    _spriteOutline.color = Color.white;
                    break;
            }
        }
    }

    int SelectValidNormalType() {
        int validType = 0;
        // If special types are OK, don't include it here.
        int special = _okTypes.Contains(7) ? 1 : 0;
        int rIndex = Random.Range(0, _okTypes.Count - special);

        validType = _okTypes[rIndex];

        return validType;
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        // Don't update if the game is over
        if (_gameManager.gameIsOver) {
            return;
        }

        _physics.SnapToSlope();

        if (exitedPipe) {
            if (!_physics.IsTouchingFloor) {
                _curState = 2;
                ApplyGravity();
            } else {
                _curState = 1;
            }

            // Wall flips
            if (_physics.IsTouchingWallLeft && !facingRight && !_springing) {
                FaceRight();
                UpdateVelocity();
            } else if(_physics.IsTouchingWallRight && facingRight && !_springing) {
                FaceLeft();
                UpdateVelocity();
            }

            // If we've reached the peak of the spring jump
            if(_springing && velocity.y <= 0) {
                // Stop springing and regain control
                _springing = false;
                // Unrotate sprite back to normal
                transform.GetChild(0).Rotate(0f, 0f, -45f);

                UpdateVelocity();
            }
        }

        // If we are in idle
        if(_curState == 0) {
            // Update long idle timer
            _longIdleTimer += Time.deltaTime;
            if(_longIdleTimer >= _longIdleTime) {
                _animator.SetBool("LongIdle", true);
                _longIdleTimer = -3f - Random.Range(2f, 7f);
            }
        } else {
            _longIdleTimer = 0f;
        }

        _animator.SetInteger("State", _curState);

        if (!_springing) {
            UpdateVelocity();
        }
	}
    
    private void FixedUpdate() {
        if (_curState != 0 && !_gameManager.isPaused && !_gameManager.gameIsOver) {
            _physics.MoveX(velocity.x * Time.deltaTime);
            _physics.MoveY(velocity.y * Time.deltaTime);
        }
    }

    protected void UpdateVelocity() {
		if (facingRight) {
			velocity.x = curMoveSpeed * (exitedPipe ? WaterMultiplier : 1) + moveSpeedModifier;
		} else {
			velocity.x = -curMoveSpeed * (exitedPipe ? WaterMultiplier : 1) - moveSpeedModifier;
		}
	}

    public override void Spring(float springForce) {
        base.Spring(springForce);

        velocity.y = springForce;
        _springing = true;
        // Restrict x velocity while rising
        velocity.x = 0;

        // Rotate sprite so it faces upward a bit
        transform.GetChild(0).Rotate(0f, 0f, 45f);
    }

    public void Caught() {
        if (!wasCaught) {
            // Reduce hamster spawner's hamsterCount
            _parentSpawner.ReduceHamsterCount();

            wasCaught = true;

            // Destroy self
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Pipe traversal
        PipeMovement(other);

        if (!special) {
            // Line collisions
            LineCollisions(other);
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        // Prevent hamsters from overlapping each other
        // If we are touching another hamster that is moving the same direction and speed as us
        if (other.tag == "Hamster" && exitedLine && other.GetComponent<Hamster>().velocity.x == velocity.x) {
            Vector2 toHamster = other.transform.position - transform.position;
            toHamster.Normalize();
            // If the other hamster is in front
            if (Vector2.Dot(transform.right, toHamster) > 0.95f) {
                // Reduce speed to make space
                curMoveSpeed = 2.5f;
                UpdateVelocity();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // We have stopped overlapping with another hamster
        if (other.tag == "Hamster" && exitedLine) {
            // Resume normal speed
            curMoveSpeed = _moveSpeed;
        }
    }

    protected void PipeMovement(Collider2D other) {
        if (other.tag == "Pipe Entrance Left") {
            inRightPipe = false;
            ReenterPipe(other.transform);
        } else if (other.tag == "Pipe Entrance Right") {
            inRightPipe = true;
            ReenterPipe(other.transform);
        } else if (other.tag == "Single Pipe Entrance") {
            SinglePipeEntrance spe = other.GetComponent<SinglePipeEntrance>();

            // For some boards, there are only 1 entrance to the loop so
            // figure out which direction hamsters will go based on their facing
            if (spe.GetDirection()) {
                inRightPipe = false;
            } else {
                inRightPipe = true;
            }
            ReenterPipe(other.transform);
        }

        if (other.tag == "PipeCornerLeft") {
            FaceRight();
            if (other.name == "PipeExit") {
                exitedPipe = true;
            }
        } else if (other.tag == "PipeCornerRight") {
            FaceLeft();
            if (other.name == "PipeExit") {
                exitedPipe = true;
            }
        } else if (other.tag == "Pipe Turn 1") {
            // Turn into connecting pipe
            if (inRightPipe) {
                FaceRight();
            } else {
                FaceLeft();
            }
            UpdateVelocity();
        } else if (other.tag == "Pipe Turn 2") {
            // Turn into main pipe
            FaceUp();

            UpdateVelocity();
        } else if (other.tag == "Bottom Pipe Entrance Left") {
            exitedPipe = false;
            inRightPipe = false;
        } else if (other.tag == "Bottom Pipe Entrance Right") {
            exitedPipe = false;
            inRightPipe = true;
        }
    }

    void ReenterPipe(Transform pipe) {
        // Turn off standard physics
        exitedPipe = false;
        velocity = new Vector2(velocity.x, 0);
        transform.position = new Vector3(pipe.position.x, transform.position.y, transform.position.z);

        // Go down pipe
        FaceDown();
    }

    protected void LineCollisions(Collider2D other) {
        // If we run into another hamster in line, stop moving
        if (other.tag == "Hamster" && !exitedLine) {
            if (other.GetComponent<Hamster>().CurState == 0) {
                inLine = true;
                SetState(0);
            }
        }
        // Or if we run into a power up
        if (other.tag == "PowerUp" && !exitedLine) {
            if (other.GetComponent<PowerUp>().CurState == 0) {
                inLine = true;
                SetState(0);
            }
        }
    }

    public override void CollisionResponseY(Collider2D collider) {
        if (collider.gameObject.layer == 21 /*Platform*/ || collider.gameObject.layer == 18/*Fallthrough*/ || collider.gameObject.layer == 19/*HamOnly*/) {
            velocity.y = 0.0f;
        }
    }

    public void ExitLine() {
        inLine = false;
        exitedLine = true;
    }
    
    public bool IsSpecialType() {
        if (type == HAMSTER_TYPES.RAINBOW || type == HAMSTER_TYPES.SKULL || isPlasma) {
            return true;
        }

        return false;
    }

    public void SetState(int state) {
        _curState = state;
        _animator.SetInteger("State", _curState);

        switch(_curState) {
            case 0: // Idle
                if (inRightPipe) {
                    FaceLeft();
                } else {
                    FaceRight();
                }
                curMoveSpeed = 0;
                _longIdleTimer = 0f;
                break;
            case 1: // Walk
                curMoveSpeed = _moveSpeed;
                break;
            case 2: // Fall
                break;
        }
    }

    public override void Respawn() {
        base.Respawn();

        // Respawn into the center of the stage
        if(team == 0) {
            transform.position = new Vector3(-6.44f, -5.5f, transform.position.z);
            FaceRight();
        } else {
            transform.position = new Vector3(6.44f, -5.5f, transform.position.z);
            FaceLeft();
        }
        exitedPipe = true;
    }
}