using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSelectInfo {
    public RuntimeAnimatorController animator;
    public bool isTaken;
}

public class CharacterSelectResources : MonoBehaviour {

    Sprite[] _charaSelectors;

    List<CharaSelectInfo>[] _charaAnimators = new List<CharaSelectInfo>[6];

    List<Sprite>[] _charaPortraits = new List<Sprite>[6];

    string[] _charaNames;

    public Sprite[] CharaSelectors {
        get { return _charaSelectors; }
    }

    public List<CharaSelectInfo>[] CharaAnimators {
        get { return _charaAnimators; }
    }

    public List<Sprite>[] CharaPortraits {
        get { return _charaPortraits; }
    }

    public string[] CharaNames {
        get { return _charaNames; }
    }


    private void Awake() {
        LoadCharacterSelectors();
        LoadCharacterAnimators();
        LoadCharacterPortraits();
        LoadCharacterNames();
    }

    void LoadCharacterSelectors() {
        _charaSelectors = Resources.LoadAll<Sprite>("Art/UI/Character Select/CharacterSelectors");
    }

    void LoadCharacterAnimators() {
        // TODO: Add based on what colors are unlocked
        // Boy
        _charaAnimators[(int)CHARACTERS.BOY] = new List<CharaSelectInfo>();

        CharaSelectInfo boy1 = new CharaSelectInfo();
        boy1.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
        boy1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy1);
        CharaSelectInfo boy2 = new CharaSelectInfo();
        boy2.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
        boy2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy2);
        CharaSelectInfo boy3 = new CharaSelectInfo();
        boy3.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
        boy3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy3);
        CharaSelectInfo boy4 = new CharaSelectInfo();
        boy4.animator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
        boy4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy4);

        // Girl
        _charaAnimators[(int)CHARACTERS.GIRL] = new List<CharaSelectInfo>();

        CharaSelectInfo girl1 = new CharaSelectInfo();
        girl1.animator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
        girl1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl1);
        CharaSelectInfo girl2 = new CharaSelectInfo();
        girl2.animator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl2") as RuntimeAnimatorController;
        girl2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl2);
        CharaSelectInfo girl3 = new CharaSelectInfo();
        girl3.animator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl3") as RuntimeAnimatorController;
        girl3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl3);
        CharaSelectInfo girl4 = new CharaSelectInfo();
        girl4.animator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl4") as RuntimeAnimatorController;
        girl4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl4);

        // Rooster
        _charaAnimators[(int)CHARACTERS.ROOSTER] = new List<CharaSelectInfo>();

        CharaSelectInfo rooster1 = new CharaSelectInfo();
        rooster1.animator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster1") as RuntimeAnimatorController;
        rooster1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster1);
        CharaSelectInfo rooster2 = new CharaSelectInfo();
        rooster2.animator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster2") as RuntimeAnimatorController;
        rooster2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster2);
        CharaSelectInfo rooster3 = new CharaSelectInfo();
        rooster3.animator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster3") as RuntimeAnimatorController;
        rooster3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster3);
        CharaSelectInfo rooster4 = new CharaSelectInfo();
        rooster4.animator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster4") as RuntimeAnimatorController;
        rooster4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster4);

        // Bat
        _charaAnimators[(int)CHARACTERS.BAT] = new List<CharaSelectInfo>();

        CharaSelectInfo bat1 = new CharaSelectInfo();
        bat1.animator = Resources.Load("Art/Animations/Player/Bat/Animation Objects/Bat1") as RuntimeAnimatorController;
        bat1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat1);
        CharaSelectInfo bat2 = new CharaSelectInfo();
        bat2.animator = Resources.Load("Art/Animations/Player/Bat/Animation Objects/Bat1") as RuntimeAnimatorController;
        bat2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat2);
        CharaSelectInfo bat3 = new CharaSelectInfo();
        bat3.animator = Resources.Load("Art/Animations/Player/Bat/Animation Objects/Bat3") as RuntimeAnimatorController;
        bat3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat3);
        CharaSelectInfo bat4 = new CharaSelectInfo();
        bat4.animator = Resources.Load("Art/Animations/Player/Bat/Animation Objects/Bat4") as RuntimeAnimatorController;
        bat4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat4);

        // Snail
        _charaAnimators[(int)CHARACTERS.SNAIL] = new List<CharaSelectInfo>();

        CharaSelectInfo snail1 = new CharaSelectInfo();
        snail1.animator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail1") as RuntimeAnimatorController;
        snail1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail1);
        CharaSelectInfo snail2 = new CharaSelectInfo();
        snail2.animator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail1") as RuntimeAnimatorController;
        snail2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail2);
        CharaSelectInfo snail3 = new CharaSelectInfo();
        snail3.animator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail3") as RuntimeAnimatorController;
        snail3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail3);
        CharaSelectInfo snail4 = new CharaSelectInfo();
        snail4.animator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail4") as RuntimeAnimatorController;
        snail4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail4);

        // Lackey
        _charaAnimators[(int)CHARACTERS.LACKEY] = new List<CharaSelectInfo>();

        CharaSelectInfo lackey1 = new CharaSelectInfo();
        lackey1.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey1") as RuntimeAnimatorController;
        lackey1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.LACKEY].Add(lackey1);
        CharaSelectInfo lackey2 = new CharaSelectInfo();
        lackey2.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey2") as RuntimeAnimatorController;
        lackey2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.LACKEY].Add(lackey2);
        CharaSelectInfo lackey3 = new CharaSelectInfo();
        lackey3.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey3") as RuntimeAnimatorController;
        lackey3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.LACKEY].Add(lackey3);
    }

    void LoadCharacterPortraits() {
        // Boy
        _charaPortraits[(int)CHARACTERS.BOY] = new List<Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[2]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[3]);

        // Girl
        _charaPortraits[(int)CHARACTERS.GIRL] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Girl-Icon");
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[2]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[3]);

        // Rooster
        _charaPortraits[(int)CHARACTERS.ROOSTER] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Rooster-Icon");
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[1]);

        // Bat
        _charaPortraits[(int)CHARACTERS.BAT] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/Animations/Player/Bat/bat-hit_784x784");
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);

        // Snail
        _charaPortraits[(int)CHARACTERS.SNAIL] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Snail-Icon");
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[2]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[3]);

        // Lackey
        _charaPortraits[(int)CHARACTERS.LACKEY] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Carl-Icons");
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[1]);
    }

    void LoadCharacterNames() {
        _charaNames = new string[6];

        //Sprite[] names = Resources.LoadAll<Sprite>("Art/UI/Character Select/CharacterNames");
        _charaNames[(int)CHARACTERS.BOY] = "KADEN";
        _charaNames[(int)CHARACTERS.GIRL] = "QUINN";
        _charaNames[(int)CHARACTERS.ROOSTER] = "ROOBEN";
        _charaNames[(int)CHARACTERS.BAT] = "CARMELA";
        _charaNames[(int)CHARACTERS.SNAIL] = "DON";
        _charaNames[(int)CHARACTERS.LACKEY] = "CARL";
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
