using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSelectInfo {
    public RuntimeAnimatorController animator;
    public Material material;
    public bool isTaken;
}

public class CharacterSelectResources : MonoBehaviour {
    public CharacterSelector[] charaSelectors;

    List<CharaSelectInfo>[] _charaInfo = new List<CharaSelectInfo>[(int)CHARACTERS.NUM_CHARACTERS];
    List<Sprite>[] _charaPortraits = new List<Sprite>[(int)CHARACTERS.NUM_CHARACTERS];

    string[] _charaNames;

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
        LoadCharacterAnimators();
        
        LoadCharacterPortraits();
        LoadCharacterNames();
    }

    void LoadCharacterAnimators() {
        bool[] paletteData;
        CharaSelectInfo tempInfo = new CharaSelectInfo();

        // Boy
        _charaInfo[(int)CHARACTERS.BOY] = new List<CharaSelectInfo>();
        RuntimeAnimatorController boyAnimator = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("BoyPalettes", new bool[0]);

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

        // Owl
        _charaInfo[(int)CHARACTERS.OWL] = new List<CharaSelectInfo>();
        RuntimeAnimatorController owlAnimator = Resources.Load("Art/Animations/Player/Owl/Animation Objects/Owl") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("OwlPalettes", new bool[0]);

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = owlAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Owl/Owl" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.OWL].Add(tempInfo);
            }
        }

        // Goat
        _charaInfo[(int)CHARACTERS.GOAT] = new List<CharaSelectInfo>();
        RuntimeAnimatorController goalAnimator = Resources.Load("Art/Animations/Player/Goat/Animation Objects/Goat") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("GoatPalettes", new bool[0]);

        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempInfo = new CharaSelectInfo();
                tempInfo.animator = goalAnimator;
                tempInfo.material = Resources.Load<Material>("Materials/Character Palettes/Goat/Goat" + (i + 1));
                tempInfo.isTaken = false;
                _charaInfo[(int)CHARACTERS.GOAT].Add(tempInfo);
            }
        }



        // Snail
        _charaInfo[(int)CHARACTERS.SNAIL] = new List<CharaSelectInfo>();
        RuntimeAnimatorController snailAnimator = Resources.Load("Art/Animations/Player/Snail/Animation Objects/Snail1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("SnailPalettes", new bool[0]);

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

        // Lizard
        _charaInfo[(int)CHARACTERS.LIZARD] = new List<CharaSelectInfo>();
        RuntimeAnimatorController lizardAnimator = Resources.Load("Art/Animations/Player/Lizard/Animation Objects/Lizard1") as RuntimeAnimatorController;
        paletteData = ES3.Load<bool[]>("LizardPalettes", new bool[0]);

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
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[27]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[28]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[29]);
        _charaPortraits[(int)CHARACTERS.ROOSTER].Add(sprites[30]);

        // Bat
        _charaPortraits[(int)CHARACTERS.BAT] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[0]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[1]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[2]);
        _charaPortraits[(int)CHARACTERS.BAT].Add(sprites[3]);


        // Owl
        _charaPortraits[(int)CHARACTERS.OWL] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.OWL].Add(sprites[23]);
        _charaPortraits[(int)CHARACTERS.OWL].Add(sprites[24]);
        _charaPortraits[(int)CHARACTERS.OWL].Add(sprites[25]);
        _charaPortraits[(int)CHARACTERS.OWL].Add(sprites[26]);

        // Goat
        _charaPortraits[(int)CHARACTERS.GOAT] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.GOAT].Add(sprites[12]);
        _charaPortraits[(int)CHARACTERS.GOAT].Add(sprites[13]);
        _charaPortraits[(int)CHARACTERS.GOAT].Add(sprites[14]);
        _charaPortraits[(int)CHARACTERS.GOAT].Add(sprites[15]);

        // Snail
        _charaPortraits[(int)CHARACTERS.SNAIL] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[31]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[32]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[33]);
        _charaPortraits[(int)CHARACTERS.SNAIL].Add(sprites[34]);

        // Lizard
        _charaPortraits[(int)CHARACTERS.LIZARD] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[19]);
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[20]);
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[21]);
        _charaPortraits[(int)CHARACTERS.LIZARD].Add(sprites[22]);

        // Lackey
        _charaPortraits[(int)CHARACTERS.LACKEY] = new List<Sprite>();
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[16]);
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[17]);
        _charaPortraits[(int)CHARACTERS.LACKEY].Add(sprites[18]);

    }

    void LoadCharacterNames() {
        _charaNames = new string[(int)CHARACTERS.NUM_CHARACTERS];

        _charaNames[(int)CHARACTERS.BOY] = "KADEN";
        _charaNames[(int)CHARACTERS.GIRL] = "QUINN";
        _charaNames[(int)CHARACTERS.ROOSTER] = "ROOBEN";
        _charaNames[(int)CHARACTERS.BAT] = "CARMELA";
        _charaNames[(int)CHARACTERS.OWL] = "GAIL";
        _charaNames[(int)CHARACTERS.GOAT] = "BEXAL";
        _charaNames[(int)CHARACTERS.SNAIL] = "DON";
        _charaNames[(int)CHARACTERS.LIZARD] = "JODI";
        _charaNames[(int)CHARACTERS.LACKEY] = "CARL";
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
