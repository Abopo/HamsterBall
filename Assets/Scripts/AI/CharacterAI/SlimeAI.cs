using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : CharacterAI {
    HamsterScan _hamsterScan;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 30f;

        PlayerController pController = GetComponent<PlayerController>();

        _hamsterScan = FindObjectOfType<HamsterScan>();
    }

    protected override void Update() {
        base.Update();
    }

    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);
    }

    public override bool ActionIsRelevant(AIAction action) {


        return true;
    }

    // Slime's special move turns some of the player's hamsters into dead hamsters
    public override void SpecialMove() {
        // Get valid indexes of the player's hamsters
        List<int> validIndexes = new List<int>();
        for (int i = 0; i < _hamsterScan.AllLeftHamsters.Count; ++i) {
            // Don't choose already dead hamsters
            if (_hamsterScan.AllLeftHamsters[i].type != HAMSTER_TYPES.DEAD) {
                validIndexes.Add(i);
            }
        }

        // Set 3 of the player's hamsters to dead randomly
        int tempIndex = 0;
        for (int i = 0; i < 3; ++i) {
            // Get a random valid index
            tempIndex = validIndexes[Random.Range(0, validIndexes.Count)];

            // Ice the chosen bubble
            _hamsterScan.AllLeftHamsters[tempIndex].SetType((int)HAMSTER_TYPES.DEAD);

            // Remove chosen index from valid indexes
            validIndexes.Remove(tempIndex);
        }
    }
}
