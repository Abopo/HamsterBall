﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSelectInfo {
    public RuntimeAnimatorController animator;
    public bool isTaken;
}

public class CharacterSelectResources : SceneResources {

    List<CharaSelectInfo>[] _charaAnimators = new List<CharaSelectInfo>[3];

    List<Sprite>[] _charaPortraits = new List<Sprite>[3];

    public List<CharaSelectInfo>[] CharaAnimators {
        get { return _charaAnimators; }
    }

    public List<Sprite>[] CharaPortraits {
        get { return _charaPortraits; }
    }

    private void Awake() {
        LoadCharacterAnimators();
        LoadCharacterPortraits();
    }

    void LoadCharacterAnimators() {
        // TODO: Add based on what colors are unlocked
        // Boy
        _charaAnimators[0] = new List<CharaSelectInfo>();

        CharaSelectInfo boy1 = new CharaSelectInfo();
        boy1.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
        boy1.isTaken = false;
        _charaAnimators[0].Add(boy1);
        CharaSelectInfo boy2 = new CharaSelectInfo();
        boy2.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
        boy2.isTaken = false;
        _charaAnimators[0].Add(boy2);
        CharaSelectInfo boy3 = new CharaSelectInfo();
        boy3.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
        boy3.isTaken = false;
        _charaAnimators[0].Add(boy3);
        CharaSelectInfo boy4 = new CharaSelectInfo();
        boy4.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
        boy4.isTaken = false;
        _charaAnimators[0].Add(boy4);

        // Girl
        _charaAnimators[1] = new List<CharaSelectInfo>();

        CharaSelectInfo girl1 = new CharaSelectInfo();
        girl1.animator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
        girl1.isTaken = false;
        _charaAnimators[1].Add(girl1);
        CharaSelectInfo girl2 = new CharaSelectInfo();
        girl2.animator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl2") as RuntimeAnimatorController;
        girl2.isTaken = false;
        _charaAnimators[1].Add(girl2);

        // Rooster
        _charaAnimators[2] = new List<CharaSelectInfo>();

        CharaSelectInfo rooster1 = new CharaSelectInfo();
        rooster1.animator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster1") as RuntimeAnimatorController;
        rooster1.isTaken = false;
        _charaAnimators[2].Add(rooster1);
        CharaSelectInfo rooster2 = new CharaSelectInfo();
        rooster2.animator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster2") as RuntimeAnimatorController;
        rooster2.isTaken = false;
        _charaAnimators[2].Add(rooster2);
    }

    void LoadCharacterPortraits() {
        // Boy
        _charaPortraits[0] = new List<Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        _charaPortraits[0].Add(sprites[0]);
        _charaPortraits[0].Add(sprites[1]);
        _charaPortraits[0].Add(sprites[2]);
        _charaPortraits[0].Add(sprites[3]);

        // Girl
        _charaPortraits[1] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Girl-Icon");
        _charaPortraits[1].Add(sprites[0]);
        _charaPortraits[1].Add(sprites[1]);
        _charaPortraits[1].Add(sprites[2]);
        _charaPortraits[1].Add(sprites[3]);

        // Rooster
        _charaPortraits[2] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Rooster-Icon");
        _charaPortraits[2].Add(sprites[0]);
        _charaPortraits[2].Add(sprites[1]);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
