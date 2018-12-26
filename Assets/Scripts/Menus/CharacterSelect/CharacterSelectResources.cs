using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectResources : SceneResources {

    public RuntimeAnimatorController[] characterAnimators = new RuntimeAnimatorController[6];

    private void Awake() {
        LoadCharacterAnimators();
    }

    void LoadCharacterAnimators() {
        characterAnimators[0] = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
        characterAnimators[1] = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
        characterAnimators[2] = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
        characterAnimators[3] = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
        characterAnimators[4] = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
        characterAnimators[5] = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl2") as RuntimeAnimatorController;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
