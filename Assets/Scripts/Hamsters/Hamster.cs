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

    public int _curState = 0; // The state the hamster is in. 0 = idle, 1 = walk, 2 = fall

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
        get { return _parentSpawner; }
        set { _parentSpawner = value; }
    }

    public int CurState {
        get { return _curState; }
    }

    protected override void Awake() {
        base.Awake();
    }
    // Use this for initialization
    protected override void Start () {
		base.Start ();
        _gameManager = GameManager.instance;

        wasCaught = false;

        if (type == HAMSTER_TYPES.NO_TYPE) {
            if(isPlasma) {
                SetType(HAMSTER_TYPES.PLASMA, (HAMSTER_TYPES)SelectValidNormalType());
            }
        }

        _curState = 1;
        _animator.SetInteger("State", _curState);
        _animator.SetInteger("Type", (int)type);

        GameManager.instance.gameOverEvent.AddListener(OnGameOver);

        moveSpeedModifier = 0;

        UpdateVelocity();
	}

    public void Initialize(int inTeam) {
        team = inTeam;

        _gameManager = GameManager.instance;

        GameObject lm = GameObject.FindGameObjectWithTag("LevelManager");
        if (lm != null) {
            HamsterScan hamsterScan = lm.GetComponent<HamsterScan>();
            if (team == 0) {
                _okTypes = hamsterScan.OkTypesLeft;
            } else if (team == 1) {
                _okTypes = hamsterScan.OkTypesRight;
            }
        }
    }

    // If setType is -1, will randomly generate a type.
    public void SetType(int setType) {
        if(setType == (int)HAMSTER_TYPES.RAINBOW) {
            _moveSpeed = rainbowMoveSpeed;
        } else if(setType == (int)HAMSTER_TYPES.SKULL) {
            _moveSpeed = skullMoveSpeed;
        } else if(setType == (int)HAMSTER_TYPES.PLASMA) {
            setType = SelectValidNormalType();
            //setType = (int)HAMSTER_TYPES.RED;
            PlasmaInitialize();
        } else {
            _moveSpeed = 3;
        }
        type = (HAMSTER_TYPES)setType;
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.SetInteger("Type", (int)type);

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

        curMoveSpeed = _moveSpeed;
    }

    // Initialization for a plasma hamster
    void PlasmaInitialize() {
        isPlasma = true;
        _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Art/Animations/Hamsters/AnimationObjects/Plasma/PlasmaHamster");
        _moveSpeed = gravityMoveSpeed;
        gravity = 10;
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

        if (!wasCaught) {
            _physics.SnapToSlope();
        }

        if (_curState == 5) {
            ApplyGravity();
        } else if (exitedPipe) {
            if (!_physics.IsTouchingFloor) {
                _curState = 2;
                ApplyGravity();

            // If we're on the floor, but still in the fall state
            } else if(_curState == 2) {
                // determine which state we should be in
                DetermineState();
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
        if (_curState != 0 && !_gameManager.isPaused) {
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

    void DetermineState() {
        if(GameManager.instance.gameIsOver) {
            // Check if our team won or lost

            // Find corresponding bubble manager
            bool wonGame = false;
            if(team == 0) {
                GameObject bubMan = GameObject.FindGameObjectWithTag("BubbleManager1");
                if (bubMan != null) {
                    wonGame = bubMan.GetComponent<BubbleManager>().wonGame;
                }
            } else {
                GameObject bubMan = GameObject.FindGameObjectWithTag("BubbleManager2");
                if (bubMan != null) {
                    wonGame = bubMan.GetComponent<BubbleManager>().wonGame;
                }
            }

            _curState = wonGame ? 3 : 4;

            // Since we don't have win/lost anims yet, just idle lol
            _curState = 0;

            // If we're in a pipe for this, make sure we are facing a normal direction
            FaceRight();
        } else {
            _curState = 1;
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
            if (_parentSpawner != null) {
                // Reduce hamster spawner's hamsterCount
                _parentSpawner.ReduceHamsterCount();
            }

            wasCaught = true;

            // Destroy self
            DestroySelf();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Pipe traversal
        PipeMovement(other);

        if (!special) {
            // Line collisions
            LineCollisions(other);
        }

        // If we got hit by fire
        if(other.tag == "Fire" && exitedPipe && !wasCaught) {
            // We gotta jump outta the stage
            BAIL();
        }
    }

    void BAIL() {
        // Stop moving
        velocity = Vector2.zero;

        // Set state to 5
        SetState(5);

        // Remove all colliders and stuff?
        GetComponent<Collider2D>().enabled = false;

        // If we were snapped to a slope we aint no more
        _physics.snappedToSlope = false;

        float rX = Random.Range(1f, 2f);
        float rY = Random.Range(0f, 3f);
        GetComponent<Rigidbody2D>().velocity = new Vector2(rX, 10f + rY);

        // Set ourselves to 'caught' so we can't accidentally get for real caught
        wasCaught = true;

        if (_parentSpawner != null) {
            // Reduce our parent spawner's hamsterCount
            _parentSpawner.ReduceHamsterCount();
        }

        // Push our sprite forward
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 20;
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
            // If we are right on top of the other hamster
            } else if(toHamster.magnitude <= 0.1) {
                // Move position a bit so the above works
                transform.position = new Vector3(transform.position.x - 0.1f,
                                                 transform.position.y, transform.position.z);
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
        if (other.tag == "PipeCornerLeft") {
            FaceRight();
            if (other.name == "PipeExit") {
                exitedPipe = true;
                inLine = false;
            }
        } else if (other.tag == "PipeCornerRight") {
            FaceLeft();
            if (other.name == "PipeExit") {
                exitedPipe = true;
                inLine = false;
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
            // Make sure we are lined up in the pipe well
            transform.position = new Vector3(other.transform.position.x, transform.position.y, transform.position.z);

            // Turn into main pipe
            FaceUp();

            UpdateVelocity();
        } else if (other.tag == "Bottom Pipe Entrance Left") {
            ReenterPipe(other.GetComponent<PipeEntrance>().parentSpawner);
            inRightPipe = false;
        } else if (other.tag == "Bottom Pipe Entrance Right") {
            ReenterPipe(other.GetComponent<PipeEntrance>().parentSpawner);
            inRightPipe = true;
        }
    }

    void ReenterPipe(HamsterSpawner spawner) {
        exitedPipe = false;

        // If we entered a pipe different from our original spawner
        if(spawner != null && spawner != _parentSpawner) {
            // Set the new pipe as our parent
            _parentSpawner = spawner;
        }
    }

    protected void LineCollisions(Collider2D other) {
        // If we run into another hamster in line, stop moving
        if ((other.tag == "Hamster" || other.tag == "PowerUp") && !exitedLine) {
            if (other.GetComponent<Hamster>().CurState == 0) {
                inLine = true;
                SetState(0);
                _parentSpawner.UpdateHamstersInLine();
            }
        }
    }

    public override void CollisionResponseY(Collider2D collider) {
        if (collider.gameObject.layer == 21 /*Platform*/ || collider.gameObject.layer == 18/*Fallthrough*/ || collider.gameObject.layer == 19/*HamOnly*/
            || collider.gameObject.layer == 24 /*FallthroughSlope*/ || collider.gameObject.layer == 23 /*Stairstep*/) {
            velocity.y = 0.0f;
            DetermineState();
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

        // Get some kind of null reference here, not sure how
        if(_animator == null && gameObject != null) {
            _animator = GetComponentInChildren<Animator>();
        }
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
            case 3: // Win
                break;
            case 4: // Lose
                break;
            case 5: // Bail
                velocity = Vector2.zero;
                break;
        }
    }

    void OnGameOver() {
        // Stop moving
        curMoveSpeed = 0f;

        // Figure out what state to be in 
        DetermineState();
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

    public void DestroySelf() {
        if (PhotonNetwork.connectedAndReady) {
            if (PhotonNetwork.isMasterClient) {
                // Make sure we destroy ourselves on the network
                PhotonNetwork.Destroy(this.gameObject);
            }
        } else {
            // Destroy self
            Destroy(gameObject);
        }
    }
}