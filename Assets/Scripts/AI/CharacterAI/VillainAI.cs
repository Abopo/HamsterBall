using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillainAI : CharacterAI {

    GameObject lackeyObj;

    GameObject _spawnedLackey;
    Vector3 _rightSpawnPosition = new Vector3(6.2f, -2f, 0f);
    Vector3 _leftSpawnPosition = new Vector3(-6.2f, -2f, 0f);

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 20f;

        lackeyObj = Resources.Load<GameObject>("Prefabs/Entities/VillainLackey");
    }

    protected override void Update() {
        base.Update();

        // Don't increase the spawn timer if there is already a lackey on the board
        if(_spawnedLackey != null) {
            _specialMoveTimer = 0;
        }
    }

    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);
    }

    public override bool ActionIsRelevant(AIAction action) {


        return true;
    }

    // Villain's special move summons a Lackey on either side to help out
    public override void SpecialMove() {
        // Instantiate a Lackey
        _spawnedLackey = Instantiate(lackeyObj);

        // Set difficulty (Lackey's aren't super smart)
        GetComponent<AIBrain>().Difficulty = 5;

        // Set up rest of player info
        PlayerController lackeyPC = _spawnedLackey.GetComponent<PlayerController>();
        lackeyPC.SetPlayerNum(4);
        lackeyPC.team = 1;

        // Randomly choose which side to spawn on
        int rand = Random.Range(0, 3);
        if (rand == 0) {
            lackeyPC.transform.position = _leftSpawnPosition;
            lackeyPC.shifted = true;
        } else {
            lackeyPC.transform.position = _rightSpawnPosition;
            lackeyPC.shifted = false;
        }
        lackeyPC.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Art/Animations/Player/PepsiMan/PepsiMan") as RuntimeAnimatorController;
    }
}
