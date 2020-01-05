using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These are speical lackeys summoned by Villain's special ability
// they are generally just a normal AI, but can die when punched enough

public class VillainLackey : CharacterAI {
    int _health = 3;
    bool _inHitState = false;
    PlayerController _playerController;

    protected override void Start() {
        base.Start();

        // Set time for special move
        _specialMoveTime = 25f;

        _playerController = GetComponent<PlayerController>();
    }

    protected override void Update() {
        base.Update();

        // If started shifted
        if(_playerController.shifted) {
            // Stay shifted forever
            _playerController.ResetShiftTimer();
        }

        // If entered into hit state, lower health by one
        if(_playerController.CurState == PLAYER_STATE.HIT && !_inHitState) {
            _health -= 1;
            _inHitState = true;

            // If health is at 0, die
            if(_health <= 0) {
                Destroy(this.gameObject);
            }
        } else if(_playerController.CurState != PLAYER_STATE.HIT) {
            _inHitState = false;
        }
    }

    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);
    }

    public override bool ActionIsRelevant(AIAction action) {
        if(action.requiresShift) {
            return false;
        }
        /*
        if(action.requiresShift && !_playerController.shifted) {
            return false;
        }
        if(!action.requiresShift && _playerController.shifted) {
            return false;
        }
        */

        return true;
    }

    // Lackey's don't have special moves so leave this blank
    public override void SpecialMove() {

    }
}
