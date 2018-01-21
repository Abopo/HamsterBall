using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Special AI for the General Hamster
public class GeneralHamAI : CharacterAI {

    public override void AdjustActionWeight(AIAction action) {
        base.AdjustActionWeight(action);
    }

    public override bool ActionIsRelevant(AIAction action) {
        if(action.requiresShift) {
            return false;
        }

        return true;
    }
}
