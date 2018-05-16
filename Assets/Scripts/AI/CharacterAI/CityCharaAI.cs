using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCharaAI : CharacterAI {
    GameObject _bubbleObj;

    List<Bubble> _generatedGravBubbles = new List<Bubble>();

    bool _doingSpecialMove = false;

    int _bubblesSpawned = 0;
    int _bubblesToThrow = 3;
    int _bubblesThrown = 0;

    float _spawnTime = 0.8f;
    float _spawnTimer = 0;
    float _throwTime = 1.2f;
    float _throwTimer = 0;

    Vector3[] _spawnPositions = new Vector3[5];

    BubbleManager _bubbleManager;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 0f;

        PlayerController pController = GetComponent<PlayerController>();

        _bubbleObj = Resources.Load<GameObject>("Prefabs/Level/Bubble");

        _bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager2").GetComponent<BubbleManager>();
        _bubbleManager.boardChangedEvent.AddListener(CheckLowestBubble);

        SetSpawnPositions();
    }

    void SetSpawnPositions() {
        _spawnPositions[0] = new Vector3(-6.1f, -3.5f, -5f);
        _spawnPositions[1] = new Vector3(-4.35f, -3.0f, -5f);
        _spawnPositions[2] = new Vector3(-7.85f, -3.0f, -5f);
        _spawnPositions[3] = new Vector3(-2.6f, -3.5f, -5f);
        _spawnPositions[4] = new Vector3(-9.6f, -3.5f, -5f);
    }

    protected override void Update() {
        base.Update();

        if (_doingSpecialMove) {
            if (_bubblesSpawned < _bubblesToThrow) {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer >= _spawnTime) {
                    SpawnGravBubble();
                }
            }
            if (_bubblesSpawned > 0 && _bubblesThrown < _bubblesToThrow) {
                _throwTimer += Time.deltaTime;
                if (_throwTimer >= _throwTime) {
                    ThrowNextBubble();
                }
                // If we've thrown all the bubbles, stop doing the special move
            } else if (_bubblesThrown >= _bubblesToThrow) {
                StopSpecialMove();
            }
        }
    }

    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);
    }

    public override bool ActionIsRelevant(AIAction action) {

        return true;
    }

    void CheckLowestBubble() {
        // Don't try to start the special move if already doing it
        if (_doingSpecialMove) {
            return;
        }

        foreach (Bubble b in _bubbleManager.Bubbles) {
            if (b != null) {
                if (b.node > 74 && _bubblesToThrow == 3) {
                    // Trigger first wave
                    SpecialMove();
                }
                if (b.node > 112 && _bubblesToThrow == 5) {
                    // Trigger second wave
                    SpecialMove();
                }
            }
        }
    }

    // City chara's special move spawns and shoots several bomb hamsters at their board
    public override void SpecialMove() {
        _doingSpecialMove = true;

        _bubblesSpawned = 0;
        _bubblesThrown = 0;

        // Start the spawn timer
        _spawnTimer = 0f;

        // Stop the AI from doing anything while doing special move

    }

    void SpawnGravBubble() {
        // Spawn a rainbow bubble
        GameObject newBubble = GameObject.Instantiate(_bubbleObj);
        Bubble gravBubble = newBubble.GetComponent<Bubble>();
        gravBubble.team = 0;

        // Randomely choose color
        int color = Random.Range(0, 7);
        gravBubble.Initialize((HAMSTER_TYPES)11 + color);
        _generatedGravBubbles.Add(gravBubble);

        // Place it in a position
        gravBubble.transform.position = _spawnPositions[_bubblesSpawned];

        // Play a spawn sound


        _bubblesSpawned++;
        _spawnTimer = 0f;
    }

    void ThrowNextBubble() {
        Bubble gravBubble = _generatedGravBubbles[_bubblesThrown];

        // Throw it at the board
        gravBubble.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 7f);
        gravBubble.GetComponent<CircleCollider2D>().enabled = true;
        gravBubble.wasThrown = true;

        // Play a throw sound


        _bubblesThrown++;
        _throwTimer = 0f;
    }

    void StopSpecialMove() {
        _doingSpecialMove = false;
        _generatedGravBubbles.Clear();

        // Increase bubbles to throw for next wave
        _bubblesToThrow += 2;
    }
}
