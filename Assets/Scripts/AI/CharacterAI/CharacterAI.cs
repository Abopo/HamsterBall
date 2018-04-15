using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour {
    protected float _specialMoveTime = 0;
    protected float _specialMoveTimer = 0;

    protected virtual void Start() {

    }

    protected virtual void Update() {
        if(_specialMoveTime > 0) {
            _specialMoveTimer += Time.deltaTime;
            if(_specialMoveTimer >= _specialMoveTime) {
                SpecialMove();
                _specialMoveTimer = 0f;
            }
        }
    }

    public virtual void AdjustActionWeight(AIAction action) {

    }

    public virtual bool ActionIsRelevant(AIAction action) {
        return true;
    }

    // bosses have special moves that do a variety of effects
    public virtual void SpecialMove() {

    }
}
