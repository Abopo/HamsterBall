using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectResources : SceneResources {

    List<RuntimeAnimatorController>[] _charaAnimators = new List<RuntimeAnimatorController>[3];

    public List<RuntimeAnimatorController>[] CharaAnimators {
        get { return _charaAnimators; }
    }

    private void Awake() {
        LoadCharacterAnimators();
    }

    void LoadCharacterAnimators() {
        // TODO: Add based on what colors are unlocked
        // Boy
        _charaAnimators[0] = new List<RuntimeAnimatorController>();
        _charaAnimators[0].Add(Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController);
        _charaAnimators[0].Add(Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController);
        _charaAnimators[0].Add(Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController);
        _charaAnimators[0].Add(Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController);
        // Girl
        _charaAnimators[1] = new List<RuntimeAnimatorController>();
        _charaAnimators[1].Add(Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController);
        _charaAnimators[1].Add(Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl2") as RuntimeAnimatorController);
        // Rooster
        _charaAnimators[2] = new List<RuntimeAnimatorController>();
        _charaAnimators[2].Add(Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster1") as RuntimeAnimatorController);
        _charaAnimators[2].Add(Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster2") as RuntimeAnimatorController);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
