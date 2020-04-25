using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlAI : CharacterAI {
    HamsterScan _hamsterScan;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 25f;

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

    // Owl's special move turns some of her hamsters into rainbow hamsters
    public override void SpecialMove() {
        Debug.Log("Owl special move");

        // Get valid indexes of the player's hamsters
        List<int> validIndexes = new List<int>();
        bool anyRainbows = false;
        for (int i = 0; i < _hamsterScan.AllRightHamsters.Count; ++i) {
            // Don't choose already rainbow hamsters
            if (_hamsterScan.AllRightHamsters[i].type != HAMSTER_TYPES.RAINBOW) {
                validIndexes.Add(i);
            } else {
                anyRainbows = true;
            }
        }

        if (!anyRainbows) {
            // Set 3 of the hamsters to rainbow randomly
            int tempIndex = 0;
            for (int i = 0; i < 3; ++i) {
                // Get a random valid index
                tempIndex = validIndexes[Random.Range(0, validIndexes.Count)];

                // Ice the chosen bubble
                _hamsterScan.AllRightHamsters[tempIndex].SetType((int)HAMSTER_TYPES.RAINBOW);

                // Remove chosen index from valid indexes
                validIndexes.Remove(tempIndex);
            }
        }
    }
}
