using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGoatAI : CharacterAI {
    BubbleManager _playerBubbleManager;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 30f;

        PlayerController pController = GetComponent<PlayerController>();
        // Get the opposite bubble manager
        if (pController.team == 0) {
            _playerBubbleManager = GameObject.FindGameObjectWithTag("BubbleManager2").GetComponent<BubbleManager>();
        } else if (pController.team == 1) {
            _playerBubbleManager = GameObject.FindGameObjectWithTag("BubbleManager1").GetComponent<BubbleManager>();
        }
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

    // Mountain Goat's special move turns several bubbles on the player's field to ice
    public override void SpecialMove() {
        // Get valid indexes of the player's bubbles
        List<int> validIndexes = new List<int>();
        for(int i = 0; i < _playerBubbleManager.Bubbles.Length; ++i) {
            // Don't choose already iced bubbles
            if (_playerBubbleManager.Bubbles[i] != null && !_playerBubbleManager.Bubbles[i].isIce) {
                validIndexes.Add(i);
            }
        }

        // Set 5 of the player's bubbles to ice randomly
        int tempIndex = 0;
        for (int i = 0; i < 6; ++i) {
            // Get a random valid index
            tempIndex = validIndexes[Random.Range(0, validIndexes.Count)];

            // Ice the chosen bubble
            _playerBubbleManager.Bubbles[tempIndex].SetIce(true);

            // Remove chosen index from valid indexes
            validIndexes.Remove(tempIndex);
        }
    }
}
