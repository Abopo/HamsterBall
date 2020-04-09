using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSelectInfo {
    public RuntimeAnimatorController animator;
    public Material material;
    public bool isTaken;
}

public class CharacterSelectResources : MonoBehaviour {

    Sprite[] _charaSelectors;

    List<CharaSelectInfo>[] _charaAnimators = new List<CharaSelectInfo>[7];

    List<Sprite>[] _charaPortraits = new List<Sprite>[7];

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

        RuntimeAnimatorController boyAnimator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
        CharaSelectInfo boy1 = new CharaSelectInfo();
        boy1.animator = boyAnimator;
        boy1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        boy1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy1);
        CharaSelectInfo boy2 = new CharaSelectInfo();
        boy2.animator = boyAnimator;
        boy2.material = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy2");
        boy2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy2);
        CharaSelectInfo boy3 = new CharaSelectInfo();
        boy3.animator = boyAnimator;
        boy3.material = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy3");
        boy3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy3);
        CharaSelectInfo boy4 = new CharaSelectInfo();
        boy4.animator = boyAnimator;
        boy4.material = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy4");
        boy4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BOY].Add(boy4);

        // Girl
        _charaAnimators[(int)CHARACTERS.GIRL] = new List<CharaSelectInfo>();

        RuntimeAnimatorController girlAnimator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
        CharaSelectInfo girl1 = new CharaSelectInfo();
        girl1.animator = girlAnimator;
        girl1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        girl1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl1);
        CharaSelectInfo girl2 = new CharaSelectInfo();
        girl2.animator = girlAnimator;
        girl2.material = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl2");
        girl2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl2);
        CharaSelectInfo girl3 = new CharaSelectInfo();
        girl3.animator = girlAnimator;
        girl3.material = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl3");
        girl3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl3);
        CharaSelectInfo girl4 = new CharaSelectInfo();
        girl4.animator = girlAnimator;
        girl4.material = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl4");
        girl4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.GIRL].Add(girl4);

        // Rooster
        _charaAnimators[(int)CHARACTERS.ROOSTER] = new List<CharaSelectInfo>();

        RuntimeAnimatorController roosterAnimator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster1") as RuntimeAnimatorController;
        CharaSelectInfo rooster1 = new CharaSelectInfo();
        rooster1.animator = roosterAnimator;
        rooster1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        rooster1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster1);
        CharaSelectInfo rooster2 = new CharaSelectInfo();
        rooster2.animator = roosterAnimator;
        rooster2.material = Resources.Load<Material>("Materials/Character Palettes/Rooster/Rooster2");
        rooster2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster2);
        CharaSelectInfo rooster3 = new CharaSelectInfo();
        rooster3.animator = roosterAnimator;
        rooster3.material = Resources.Load<Material>("Materials/Character Palettes/Rooster/Rooster3");
        rooster3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster3);
        CharaSelectInfo rooster4 = new CharaSelectInfo();
        rooster4.animator = roosterAnimator;
        rooster4.material = Resources.Load<Material>("Materials/Character Palettes/Rooster/Rooster4");
        rooster4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.ROOSTER].Add(rooster4);

        // Bat
        _charaAnimators[(int)CHARACTERS.BAT] = new List<CharaSelectInfo>();

        RuntimeAnimatorController batAnimator = Resources.Load("Art/Animations/Player/Bat/Animation Objects/Bat1") as RuntimeAnimatorController;
        CharaSelectInfo bat1 = new CharaSelectInfo();
        bat1.animator = batAnimator;
        bat1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        bat1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat1);
        CharaSelectInfo bat2 = new CharaSelectInfo();
        bat2.animator = batAnimator;
        bat2.material = Resources.Load<Material>("Materials/Character Palettes/Bat/Bat2");
        bat2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat2);
        CharaSelectInfo bat3 = new CharaSelectInfo();
        bat3.animator = batAnimator;
        bat3.material = Resources.Load<Material>("Materials/Character Palettes/Bat/Bat3");
        bat3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat3);
        CharaSelectInfo bat4 = new CharaSelectInfo();
        bat4.animator = batAnimator;
        bat4.material = Resources.Load<Material>("Materials/Character Palettes/Bat/Bat4");
        bat4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.BAT].Add(bat4);

        // Snail
        _charaAnimators[(int)CHARACTERS.SNAIL] = new List<CharaSelectInfo>();

        RuntimeAnimatorController snailAnimator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail1") as RuntimeAnimatorController;
        CharaSelectInfo snail1 = new CharaSelectInfo();
        snail1.animator = snailAnimator;
        snail1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        snail1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail1);
        CharaSelectInfo snail2 = new CharaSelectInfo();
        snail2.animator = snailAnimator;
        snail2.material = Resources.Load<Material>("Materials/Character Palettes/Snail/Snail2");
        snail2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail2);
        CharaSelectInfo snail3 = new CharaSelectInfo();
        snail3.animator = snailAnimator;
        snail3.material = Resources.Load<Material>("Materials/Character Palettes/Snail/Snail3");
        snail3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail3);
        CharaSelectInfo snail4 = new CharaSelectInfo();
        snail4.animator = snailAnimator;
        snail4.material = Resources.Load<Material>("Materials/Character Palettes/Snail/Snail4");
        snail4.isTaken = false;
        _charaAnimators[(int)CHARACTERS.SNAIL].Add(snail4);

        // Lackey
        // Lackey is special and has more animator palettes than material palette swaps
        _charaAnimators[(int)CHARACTERS.LACKEY] = new List<CharaSelectInfo>();

        CharaSelectInfo lackey1 = new CharaSelectInfo();
        lackey1.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey1") as RuntimeAnimatorController;
        lackey1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        lackey1.isTaken = false;
        _charaAnimators[(int)CHARACTERS.LACKEY].Add(lackey1);
        CharaSelectInfo lackey2 = new CharaSelectInfo();
        lackey2.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey2") as RuntimeAnimatorController;
        lackey2.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        lackey2.isTaken = false;
        _charaAnimators[(int)CHARACTERS.LACKEY].Add(lackey2);
        CharaSelectInfo lackey3 = new CharaSelectInfo();
        lackey3.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey3") as RuntimeAnimatorController;
        lackey3.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        lackey3.isTaken = false;
        _charaAnimators[(int)CHARACTERS.LACKEY].Add(lackey3);

        // Lizard
        _charaAnimators[(int)CHARACTERS.LIZARD] = new List<CharaSelectInfo>();

        CharaSelectInfo lizard1 = new CharaSelectInfo();
        lizard1.animator = Resources.Load("Art/Animations/Player/Lizard/Animation Objects/Lizard1") as RuntimeAnimatorController;
        lizard1.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        lizard1.isTaken = false;
        CharaSelectInfo lizard2 = new CharaSelectInfo();
        lizard2.animator = Resources.Load("Art/Animations/Player/Lizard/Animation Objects/Lizard1") as RuntimeAnimatorController;
        lizard2.material = Resources.Load<Material>("Materials/Character Palettes/Lizard/Lizard2");
        lizard2.isTaken = false;

        _charaAnimators[(int)CHARACTERS.LIZARD].Add(lizard1);
        _charaAnimators[(int)CHARACTERS.LIZARD].Add(lizard2);
        _charaAnimators[(int)CHARACTERS.LIZARD].Add(lizard1);
        _charaAnimators[(int)CHARACTERS.LIZARD].Add(lizard2);
    }

    void LoadCharacterPortraits() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Portraits");

        // Boy
        _charaPortraits[(int)CHARACTERS.BOY] = new List<Sprite>();
        //sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[4]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[5]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[6]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[7]);

        // Girl
        _charaPortraits[(int)CHARACTERS.GIRL] = new List<Sprite>();
        //sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Girl-Icon");
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[8]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[9]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[10]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[11]);

        // Rooster
        _charaPortraits[(int)CHARACTERS.ROOSTER] = new List<Sprite>();
        //sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Rooster-Icon");
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[15]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[16]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[17]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[18]);

        // Bat
        _charaPortraits[(int)CHARACTERS.BAT] = new List<Sprite>();
        //sprites = Resources.LoadAll<Sprite>("Art/Animations/Player/Bat/bat-hit_784x784");
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[2]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[3]);

        // Snail
        _charaPortraits[(int)CHARACTERS.SNAIL] = new List<Sprite>();
        //sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Snail-Icon");
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[19]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[22]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[20]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[21]);

        // Lackey
        _charaPortraits[(int)CHARACTERS.LACKEY] = new List<Sprite>();
        //sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Carl-Icons");
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[12]);
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[13]);
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[14]);

        // Lizard
        _charaPortraits[(int)CHARACTERS.LIZARD] = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Icons-Master-File");
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[24]);
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[25]);
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[24]);
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[25]);
    }

    void LoadCharacterNames() {
        _charaNames = new string[7];

        //Sprite[] names = Resources.LoadAll<Sprite>("Art/UI/Character Select/CharacterNames");
        _charaNames[(int)CHARACTERS.BOY] = "KADEN";
        _charaNames[(int)CHARACTERS.GIRL] = "QUINN";
        _charaNames[(int)CHARACTERS.ROOSTER] = "ROOBEN";
        _charaNames[(int)CHARACTERS.BAT] = "CARMELA";
        _charaNames[(int)CHARACTERS.SNAIL] = "DON";
        _charaNames[(int)CHARACTERS.LACKEY] = "CARL";
        _charaNames[(int)CHARACTERS.LIZARD] = "JODI";
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
