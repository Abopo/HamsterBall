using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubble : MonoBehaviour {
    public int team;
    public GameObject bubbleObj;
    public GameObject hamsterObj;

    Bubble _caughtBubble;
    CircleCollider2D _circleCollider;

    bool _isSpawning;
    bool _popped;

    float _xMoveTime = 1.0f;
    float _xMoveTimer = 0.5f;
    float _xMoveDir = 0.5f;

    float _baseScale;

    Animator _animator;

    public Bubble CaughtBubble {
        get  {return _caughtBubble; }
    }

    public bool IsSpawning {
        get { return _isSpawning; }
    }

    // Use this for initialization
    void Start () {
        _circleCollider = GetComponent<CircleCollider2D>();
        _isSpawning = true;

        _baseScale = transform.localScale.x;
        transform.localScale = new Vector3(_baseScale / 4, _baseScale / 4, _baseScale / 4);

        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        // If just spawned
        if (_isSpawning) {
            // Expand until normal size
            transform.localScale = new Vector3(transform.localScale.x + 1f * Time.deltaTime,
                                                transform.localScale.y + 1f * Time.deltaTime,
                                                transform.localScale.z);
            if (transform.localScale.x >= _baseScale) {
                transform.localScale = new Vector3(_baseScale, _baseScale, _baseScale);
                _isSpawning = false;
                _circleCollider.enabled = true;
            }
        } else if(!_popped) {
            // Float upward
            transform.Translate(_xMoveDir * Time.deltaTime, 1f * Time.deltaTime, 0f, Space.World);

            _xMoveTimer += Time.deltaTime;
            if(_xMoveTimer >= _xMoveTime) {
                _xMoveDir = _xMoveDir * -1f;
                _xMoveTimer = 0f;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Hamster" && other.GetComponent<Hamster>().exitedPipe && _caughtBubble == null) {
            CatchHamster(other.GetComponent<Hamster>());
        } else if (other.tag == "Bubble") {
            if (other.GetComponent<Bubble>().locked) {
                BoardCollide(other.GetComponent<Bubble>().HomeBubbleManager);
            }
        } else if (other.tag == "Ceiling") {
            BoardCollide(other.GetComponent<Ceiling>().bubbleManager);
        } else if (other.tag == "Attack") {
            if (_caughtBubble != null) {
                // Drop the caught bubble
                DropHamster();
            }

            Pop();
        }
    }

    public void CatchHamster(Hamster hamster) {
        if (PhotonNetwork.connectedAndReady) {
            //InstantiateNetworkBubble(hamster);
        } else {
            GameObject bubble = Instantiate(bubbleObj) as GameObject;
            bubble.transform.parent = this.transform;
            _caughtBubble = bubble.GetComponent<Bubble>();
            _caughtBubble.transform.localPosition = new Vector3(0f, 0f, 0f);
            _caughtBubble.team = team;
            _caughtBubble.PlayerController = null;
            _caughtBubble.Initialize(hamster.type);
            _caughtBubble.GetComponent<CircleCollider2D>().enabled = false;

            if (hamster.isGravity) {
                _caughtBubble.isGravity = true;
                GameObject spiralEffect = hamster.spiralEffectInstance;
                spiralEffect.transform.parent = _caughtBubble.transform;
                spiralEffect.transform.position = new Vector3(_caughtBubble.transform.position.x,
                                                              _caughtBubble.transform.position.y,
                                                              _caughtBubble.transform.position.z + 3);
                spiralEffect.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
        }

        // The hamster was caught.
        hamster.Caught();
    }

    void BoardCollide(BubbleManager bubbleManager) {
        if (_caughtBubble != null) {
            // Collide bubble with board
            _caughtBubble.GetComponent<CircleCollider2D>().enabled = true;
            _caughtBubble.CollisionWithBoard(bubbleManager);
        }

        Pop();
    }

    void DropHamster() {
        GameObject hamsterGO = Instantiate(hamsterObj, transform.position, Quaternion.identity) as GameObject;
        Hamster hamster = hamsterGO.GetComponent<Hamster>();

        // Set the correct team and parent spawner
        hamster.Initialize(team);
        hamster.exitedPipe = true;

        // Set a parent spawner
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Hamster Spawner");
        foreach (GameObject s in spawners) {
            if (s.GetComponent<HamsterSpawner>().team == hamster.team) {
                hamster.ParentSpawner = s.GetComponent<HamsterSpawner>();
                s.GetComponent<HamsterSpawner>().releasedHamsterCount++;
                break;
            }
        }

        // Set the correct type
        if (_caughtBubble.isGravity) {
            hamster.SetType(HAMSTER_TYPES.GRAVITY, _caughtBubble.type);
        } else {
            hamster.SetType((int)_caughtBubble.type);
        }

        // Randomly choose direction
        if (Random.Range(0, 11) > 5) {
            hamster.Flip();
        }
    }

    public void Pop() {
        _circleCollider.enabled = false;
        _animator.Play("BubblePop", 0);
        _popped = true;

        if(_caughtBubble != null && !_caughtBubble.locked) {
            // destroy the caught bubble during the pop animation
            Destroy(_caughtBubble.gameObject);
        }
    }

    public void DestroySelf() {
        // Destroy self
        Destroy(gameObject);
    }
}
