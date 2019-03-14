using UnityEngine;
using System.Collections.Generic;

public enum HAMSTER_TYPES { NO_TYPE = -1, GREEN = 0, RED, ORANGE, GRAY, BLUE, PINK, PURPLE, NUM_NORM_TYPES,
						   RAINBOW = 8, DEAD, BOMB, NUM_SPEC_TYPES = 3};

public class Hamster : Entity {
    public HAMSTER_TYPES type;
    public int team;
    public int hamsterNum; // just a number specifying each individual hamster
    public bool wasCaught; // used to prevent the same hamster from being caught twice ont he same frame.

	public GameObject spiralEffectObj;
	public GameObject spiralEffectInstance;
	public bool isGravity;

    public bool testMode;

    public bool exitedPipe;
    public bool inRightPipe;
    public bool inLine;
    public bool exitedLine;

    public float curMoveSpeed; //  base - 3, rainbow - 4, dead - 2, gravity - 3.5
    public float moveSpeedModifier; // Is added to the move speed
    float moveSpeed = 3;

    int _curState = 0; // The state the hamster is in. 0 = idle, 1 = walk, 2 = fall

    HamsterSpawner _parentSpawner;
    List<int> _okTypes;

    float rainbowMoveSpeed = 4f;
    float deadMoveSpeed = 2.25f;
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

    GameManager _gameManager;
    
    // Use this for initialization
    protected override void Start () {
		base.Start ();
        _gameManager = FindObjectOfType<GameManager>();

        wasCaught = false;

        if (type == HAMSTER_TYPES.NO_TYPE) {
            if(isGravity) {
                SetType(11, (HAMSTER_TYPES)SelectValidNormalType());
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
            moveSpeed = rainbowMoveSpeed;
        } else if(setType == (int)HAMSTER_TYPES.DEAD) {
            moveSpeed = deadMoveSpeed;
        } else if(setType == 11) {
            isGravity = true;
            spiralEffectInstance = Instantiate(spiralEffectObj, transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            spiralEffectInstance.transform.parent = transform;
            spiralEffectInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
            setType = SelectValidNormalType();
            moveSpeed = gravityMoveSpeed;
        } else {
            moveSpeed = 3;
        }
        type = (HAMSTER_TYPES)setType;
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.SetInteger("Type", (int)type);

        curMoveSpeed = moveSpeed;
    }

    // This overload is specifically used to set a special type with a color.
    public void SetType(int sType, HAMSTER_TYPES cType) {
        if (sType == 11) {
            isGravity = true;
            spiralEffectInstance = Instantiate(spiralEffectObj, transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            spiralEffectInstance.transform.parent = transform;
            spiralEffectInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
            moveSpeed = gravityMoveSpeed;
        }

        type = cType;
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.SetInteger("Type", (int)type);

        curMoveSpeed = moveSpeed;
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

        _physics.CheckBelow ();
		_physics.WallCheck ();

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
        } else {
            //_curState = 1;
        }

        // If we are in idle
        if(_curState == 0) {
            // Update long idle timer
            _longIdleTimer += Time.deltaTime;
            if(_longIdleTimer >= _longIdleTime) {
                _animator.SetBool("LongIdle", true);
                _longIdleTimer = -3f - Random.Range(2f, 7f);
            }
        }

        _animator.SetInteger("State", _curState);

        if (!_springing) {
            UpdateVelocity();
        }

		_physics.MoveX (velocity.x * Time.deltaTime);
		_physics.MoveY (velocity.y * Time.deltaTime);
	}

	void UpdateVelocity() {
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
            DestroyObject(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
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
            if (other.name == "Blocker") {
                exitedPipe = true;
            }
        } else if (other.tag == "PipeCornerRight") {
            FaceLeft();
            if (other.name == "Blocker") {
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

        // If we run into another hamster in line, stop moving
        if(other.tag == "Hamster"&& !exitedLine) {
            if(other.GetComponent<Hamster>().CurState == 0) {
                inLine = true;
                SetState(0);
            }
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

        if ((other.tag == "PipeCornerLeft" || other.tag == "PipeCornerRight") && other.name != "Blocker") {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= _stuckTime) {
                // If we're stuck in the collider for some reason, get set back to a good spot
                transform.position = new Vector3(other.transform.position.x, other.transform.position.y + 0.06f, transform.position.z);

                // Turn
                if (inRightPipe) {
                    FaceLeft();
                } else {
                    FaceRight();
                }

                _stuckTimer = 0f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // We have stopped overlapping with another hamster
        if (other.tag == "Hamster" && exitedLine) {
            // Resume normal speed
            curMoveSpeed = moveSpeed;
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

    public void ExitLine() {
        inLine = false;
        exitedLine = true;
    }
    
    public bool IsSpecial() {
        if (type == HAMSTER_TYPES.RAINBOW || type == HAMSTER_TYPES.DEAD || isGravity) {
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
                curMoveSpeed = moveSpeed;
                break;
            case 2: // Fall
                break;
        }
    }

    public override void Respawn() {
        base.Respawn();

        // Respawn into the hamster pipe
        transform.position = _parentSpawner.SpawnPosition;
        exitedPipe = false;
        FaceUp();
    }
}
