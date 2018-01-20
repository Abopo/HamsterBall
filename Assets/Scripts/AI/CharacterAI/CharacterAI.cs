using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public virtual void AdjustActionWeight(AIAction action) {

    }

    public virtual bool ActionIsRelevant(AIAction action) {
        return true;
    }
}
