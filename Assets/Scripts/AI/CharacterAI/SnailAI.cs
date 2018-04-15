using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailAI : CharacterAI {
    WaterBubbleGenerator[] _bubbleGenerators = new WaterBubbleGenerator[2];

    bool _isSpawning;
    int _spawnCount = 0;
    float _spawnTime = 0.5f;
    float _spawnTimer = 0f;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 20f;

        // Get the two water bubble generators from the player's field
        WaterBubbleGenerator[] generators = FindObjectsOfType<WaterBubbleGenerator>();
        foreach(WaterBubbleGenerator wbg in generators) {
            if(wbg.team == 0) {
                if (_bubbleGenerators[0] == null) {
                    _bubbleGenerators[0] = wbg;
                } else {
                    _bubbleGenerators[1] = wbg;
                }
            }
        }
    }

    protected override void Update() {
        base.Update();

        if(_isSpawning) {
            _spawnTimer += Time.deltaTime;
            if(_spawnTimer >= _spawnTime) {
                int rand = Random.Range(0, 2);
                _bubbleGenerators[rand].SpawnBubble();
                _spawnTimer = 0f;

                _spawnCount++;
                if(_spawnCount > 6) {
                    _isSpawning = false;
                }
            }
        }
    }

    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);
    }

    public override bool ActionIsRelevant(AIAction action) {

        return true;
    }

    // Snail's special move spawns several water bubbles on the player's field
    public override void SpecialMove() {
        // Set spawning to true, bubbles will be spawned over time
        _isSpawning = true;
        _spawnTimer = 0f;
    }
}
