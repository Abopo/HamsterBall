using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI {
    public virtual void AdjustActionWeight(AIAction action) {

    }

    public virtual bool ActionIsRelevant(AIAction action) {
        return true;
    }
}
