using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum POWERUPS { ATK_UP = 0, HAM_SPEED_UP, INFINISHIFT, JUNK_SHIELD, MONOCHROME, PLAY_SPEED_DOWN, SMOKE, NUM_POWERUPS };

// A power up in Party mode, moves like a hamster, activates when caught
public class PowerUp : Hamster {
    public POWERUPS powerUp;

    protected bool _isActive = false;
    protected float _activateTime = 0f;
    protected float _activateTimer = 0f;

    float _stuckTimer = 0f;
    float _stuckTime = 0.5f;

    protected HamsterSpawner _parentSpawner;
    protected SpriteRenderer _spriteRenderer;
    protected AudioSource _audioSource;
    protected PlayerController _caughtPlayer; // The player that caught this power up

    protected PowerUpText _powerText;

    protected override void Awake() {
        base.Awake();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _powerText = GetComponentInChildren<PowerUpText>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _moveSpeed = 3.75f;
        curMoveSpeed = _moveSpeed;
        exitedPipe = false;
    }

    // Update is called once per frame
    protected override void Update () {
        //base.Update();
        
        // Don't update if the game is over
        if (_gameManager.gameIsOver || _isActive) {
            return;
        }

        _physics.CheckBelow();
        _physics.WallCheck();

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
            } else if (_physics.IsTouchingWallRight && facingRight && !_springing) {
                FaceLeft();
                UpdateVelocity();
            }

            if (_springing && velocity.y <= 0) {
                _springing = false;
                UpdateVelocity();
            }
        }

        _animator.SetInteger("State", _curState);

        if (!_springing) {
            UpdateVelocity();
        }

        _physics.MoveX(velocity.x * Time.deltaTime);
        _physics.MoveY(velocity.y * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        // Pipe traversal
        PipeMovement(other);

        // Line stuff
        LineCollisions(other);
    }

    void OnTriggerStay2D(Collider2D other) {
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

    void ReenterPipe(Transform pipe) {
        // Turn off standard physics
        exitedPipe = false;
        velocity = new Vector2(velocity.x, 0);
        transform.position = new Vector3(pipe.position.x, transform.position.y, transform.position.z);

        // Go down pipe
        FaceDown();
    }

    public void Caught(PlayerController pCon) {
        if(!wasCaught) {
            wasCaught = true;

            _caughtPlayer = pCon;

            Activate();
        }
    }

    protected virtual void Activate() {
        _isActive = true;
        _activateTimer = 0f;

        // Detach and show text
        Vector3 worldPosition = _powerText.transform.position;
        _powerText.transform.SetParent(null);
        _powerText.transform.position = worldPosition;
        _powerText.DisplayText();

        // Turn off stuff
        //_spriteRenderer.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        velocity = Vector2.zero;

        // Move out of play area
        if (_caughtPlayer.team == 0) {
            transform.position = new Vector3(-8, -7.25f, -3);
        } else {
            transform.position = new Vector3(8, -7.25f, -3);
        }

        // Reduce hamsters spawned for hamster spawner
        _parentSpawner.ReduceHamsterCount();

        // Play a sound
        _audioSource.Play();
    }

    protected virtual void Deactivate() {
        Destroy(this.gameObject);
    }
}
