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

    List<CharaSelectInfo>[] _charaInfo = new List<CharaSelectInfo>[7];
    List<Sprite>[] _charaPortraits = new List<Sprite>[7];

    string[] _charaNames;

    public Sprite[] CharaSelectors {
        get { return _charaSelectors; }
    }

    public List<CharaSelectInfo>[] CharaInfo {
        get { return _charaInfo; }
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
        bool[] paletteData;
        CharaSelectInfo tempInfo = new CharaSelectInfo();

        // Boy
        _charaInfo[(int)CHARACTERS.BOY] = new List<CharaSelectInfo>();
        RuntimeAnimatorController boyAnimator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("BoyPalettes", new bool[0]);

        // First info is default
        /*
        tempInfo = new CharaSelectInfo();
        tempInfo.animator = boyAnimator;
        tempInfo.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        tempInfo.isTaken = false;
        _charaInfo[(int)CHARACTERS.BOY].Add(tempInfo);
        */

        for(int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = boyAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy" + (i+1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.BOY].Add(tempInfo);
            }
        }

        // Girl
        _charaInfo[(int)CHARACTERS.GIRL] = new List<CharaSelectInfo>();
        RuntimeAnimatorController girlAnimator = Resources.Load("Art/Animations/Player/Girl/Animation Objects/Girl1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("GirlPalettes", new bool[0]);

        // First info is default
        /*
        tempInfo = new CharaSelectInfo();
        tempInfo.animator = girlAnimator;
        tempInfo.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        tempInfo.isTaken = false;
        _charaInfo[(int)CHARACTERS.GIRL].Add(tempInfo);
        */

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = girlAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.GIRL].Add(tempInfo);
            }
        }

        // Rooster
        _charaInfo[(int)CHARACTERS.ROOSTER] = new List<CharaSelectInfo>();
        RuntimeAnimatorController roosterAnimator = Resources.Load("Art/Animations/Player/Rooster/Animation Objects/Rooster1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("RoosterPalettes", new bool[0]);

        // First info is default
        /*
        tempInfo = new CharaSelectInfo();
        tempInfo.animator = roosterAnimator;
        tempInfo.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        tempInfo.isTaken = false;
        _charaInfo[(int)CHARACTERS.ROOSTER].Add(tempInfo);
        */

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = roosterAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Rooster/Rooster" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.ROOSTER].Add(tempInfo);
            }
        }

        // Bat
        _charaInfo[(int)CHARACTERS.BAT] = new List<CharaSelectInfo>();
        RuntimeAnimatorController batAnimator = Resources.Load("Art/Animations/Player/Bat/Animation Objects/Bat1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("BatPalettes", new bool[0]);

        // First info is default
        /*
        tempInfo = new CharaSelectInfo();
        tempInfo.animator = batAnimator;
        tempInfo.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        tempInfo.isTaken = false;
        _charaInfo[(int)CHARACTERS.BAT].Add(tempInfo);
        */

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = batAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Bat/Bat" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.BAT].Add(tempInfo);
            }
        }

        // Snail
        _charaInfo[(int)CHARACTERS.SNAIL] = new List<CharaSelectInfo>();
        RuntimeAnimatorController snailAnimator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("SnailPalettes", new bool[0]);

        // First info is default
        /*
        tempInfo = new CharaSelectInfo();
        tempInfo.animator = snailAnimator;
        tempInfo.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        tempInfo.isTaken = false;
        _charaInfo[(int)CHARACTERS.SNAIL].Add(tempInfo);
        */

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = snailAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Snail/Snail" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.SNAIL].Add(tempInfo);
            }
        }

        // Lackey
        // Lackey is special and has more animator palettes than material palette swaps
        _charaInfo[(int)CHARACTERS.LACKEY] = new List<CharaSelectInfo>();

        CharaSelectInfo lackey1 = new CharaSelectInfo();
        lackey1.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey1") as RuntimeAnimatorController;
        lackey1.material = new Material(Shader.Find("Sprites/Default")); // Lackey doesn't need material
        lackey1.isTaken = false;
        _charaInfo[(int)CHARACTERS.LACKEY].Add(lackey1);
        CharaSelectInfo lackey2 = new CharaSelectInfo();
        lackey2.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey2") as RuntimeAnimatorController;
        lackey2.material = new Material(Shader.Find("Sprites/Default")); // Lackey doesn't need material
        lackey2.isTaken = false;
        _charaInfo[(int)CHARACTERS.LACKEY].Add(lackey2);
        CharaSelectInfo lackey3 = new CharaSelectInfo();
        lackey3.animator = Resources.Load("Art/Animations/Player/Lackey/Animation Objects/Lackey3") as RuntimeAnimatorController;
        lackey3.material = new Material(Shader.Find("Sprites/Default")); // Lackey doesn't need material
        lackey3.isTaken = false;
        _charaInfo[(int)CHARACTERS.LACKEY].Add(lackey3);

        // Lizard
        _charaInfo[(int)CHARACTERS.LIZARD] = new List<CharaSelectInfo>();
        RuntimeAnimatorController lizardAnimator = Resources.Load("Art/Animations/Player/Lizard/Animation Objects/Lizard1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("LizardPalettes", new bool[0]);

        // First info is default
        /*
        tempInfo = new CharaSelectInfo();
        tempInfo.animator = lizardAnimator;
        tempInfo.material = new Material(Shader.Find("Sprites/Default")); // Default color doesn't need material
        tempInfo.isTaken = false;
        _charaInfo[(int)CHARACTERS.LIZARD].Add(tempInfo);
        */

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = lizardAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Lizard/Lizard" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.LIZARD].Add(tempInfo);
            }
        }
    }

    void LoadCharacterPortraits() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Portraits");

        // Boy
        _charaPortraits[(int)CHARACTERS.BOY] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[4]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[5]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[6]);
        _charaPortraits[(int)CHARACTERS.BOY].Add(sprites[7]);

        // Girl
        _charaPortraits[(int)CHARACTERS.GIRL] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[8]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[9]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[10]);
        _charaPortraits[(int)CHARACTERS.GIRL].Add(sprites[11]);

        // Rooster
        _charaPortraits[(int)CHARACTERS.ROOSTER] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[15]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[16]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[17]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[18]);

        // Bat
        _charaPortraits[(int)CHARACTERS.BAT] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[2]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[3]);

        // Snail
        _charaPortraits[(int)CHARACTERS.SNAIL] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[19]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[22]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[20]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[21]);

        // Lackey
        _charaPortraits[(int)CHARACTERS.LACKEY] = new List<Sprite>();
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
