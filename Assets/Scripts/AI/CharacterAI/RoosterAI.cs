using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoosterAI : CharacterAI {
    PlayerController _pController;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 30f;

        _pController = GetComponent<PlayerController>();
        
        // Rooster's special ability is that he can't be punched.
        _pController.canBeHit = false;
    }

    protected override void Update() {
        base.Update();

    }

    // Rooster is aggressive and likes to attack the opponent directly
    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);

        int addWeight = 0;

        if(action.requiresShift) {
            addWeight += 25;
        }

        if(action.opponent != null) {
            addWeight += 50;
        }

        action.weight += addWeight;
    }

    public override bool ActionIsRelevant(AIAction action) {


        return true;
    }

    public override void SpecialMove() {
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // If Rooster gets punched
        if (collider.gameObject.layer == 12 && _pController.team != collider.GetComponent<AttackObject>().team && _pController.CurState != PLAYER_STATE.SHIFT) {
            // TODO: Play some effect to show it doesn't affect him
        }
    }
}
