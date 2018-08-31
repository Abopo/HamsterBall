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

    float _xMoveTime = 1.0f;
    float _xMoveTimer = 0.5f;
    float _xMoveDir = 0.5f;

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
	}
	
	// Update is called once per frame
	void Update () {
        // If just spawned
        if (_isSpawning) {
            // Expand until normal size
            transform.localScale = new Vector3(transform.localScale.x + 0.2f * Time.deltaTime,
                                                transform.localScale.y + 0.2f * Time.deltaTime,
                                                transform.localScale.z);
            if (transform.localScale.x >= 1f) {
                transform.localScale = new Vector3(1, 1, 1);
                _isSpawning = false;
                _circleCollider.enabled = true;
            }
        } else {
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

            // Destroy self
            DestroyObject(this.gameObject);
        }
    }

    public void CatchHamster(Hamster hamster) {
        if (PhotonNetwork.connectedAndReady) {
            //InstantiateNetworkBubble(hamster);
        } else {
            GameObject bubble = Instantiate(bubbleObj, this.transform) as GameObject;
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

        // Destroy self
        DestroyObject(this.gameObject);
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
                s.GetComponent<HamsterSpawner>().hamsterCount++;
                break;
            }
        }

        // Set the correct type
        if (_caughtBubble.isGravity) {
            hamster.SetType(11, _caughtBubble.type);
        } else {
            hamster.SetType((int)_caughtBubble.type);
        }

        // Randomly choose direction
        if (Random.Range(0, 11) > 5) {
            hamster.Flip();
        }
    }
}
