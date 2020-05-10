using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class CharacterSelectWindow : Menu {
    protected int _boyPaletteIndex = 0;
    protected int _girlPaletteIndex = 0;

    public Image boySprite;
    public Image girlSprite;
    //protected Sprite[] _characterSprites = new Sprite[8];

    protected List<Material> _boyPalettes = new List<Material>();
    protected List<Material> _girlPalettes = new List<Material>();

    public GameObject menuObject;

    // The selected color of the other player
    protected Material _chosenBoyPalette;
    protected Material _chosenGirlPalette;
    //protected int _chosenBoyColor = -1;
    //protected int _chosenGirlColor = -1;

    protected bool _waitFrame;

    PlayerInfoBox _playerInfoBox;
    protected Player _controllingPlayer;

    MenuOption[] _options;

    protected bool _isActive;

    StorySelectMenu _storySelectMenu;

    protected override void Awake() {
        base.Awake();

        _storySelectMenu = FindObjectOfType<StorySelectMenu>();

        LoadSprites();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        //boySprite.sprite = _characterSprites[_boyColor];
        //girlSprite.sprite = _characterSprites[_girlColor+4];
    }

    void LoadSprites() {
        Material tempMaterial;
        bool[] paletteData;

        // Base palette doesn't need material
        tempMaterial = new Material(Shader.Find("Sprites/Default"));
        _boyPalettes.Add(tempMaterial);
        _girlPalettes.Add(tempMaterial);

        paletteData = ES3.Load<bool[]>("BoyPalettes", new bool[0]);
        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempMaterial = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy" + (i + 2));
                _boyPalettes.Add(tempMaterial);
            }
        }
        paletteData = ES3.Load<bool[]>("GirlPalettes", new bool[0]);
        for (int i = 0; i < paletteData.Length; ++i) {
            // If this palette is unlocked
            if (paletteData[i] == true) {
                tempMaterial = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl" + (i + 2));
                _girlPalettes.Add(tempMaterial);
            }
        }

        /*
        Sprite[] boySprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        Sprite[] girlSprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Girl-Icon");
        _characterSprites[0] = boySprites[0];
        _characterSprites[1] = boySprites[1];
        _characterSprites[2] = boySprites[2];
        _characterSprites[3] = boySprites[3];
        _characterSprites[4] = girlSprites[0];
        _characterSprites[5] = girlSprites[1];
        _characterSprites[6] = girlSprites[2];
        _characterSprites[7] = girlSprites[3];
        */
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isActive && !_waitFrame) {
            CheckInput();
        } else if(_waitFrame) {
            _waitFrame = false;
        }
    }

    protected void CheckInput() {
        if (_controllingPlayer.GetButtonDown("Up")) {
            // Figure out which character is highlighted
            if (IsBoyHighlighted()) {
                ChangeBoy(-1);
            } else {
                ChangeGirl(-1);
            }
        }
        if (_controllingPlayer.GetButtonDown("Down")) {
            // Figure out which character is highlighted
            if (IsBoyHighlighted()) {
                ChangeBoy(1);
            } else {
                ChangeGirl(1);
            }
        }
        if (_controllingPlayer.GetButtonDown("Cancel")) {
            Deactivate();
        }
    }

    protected void ChangeBoy(int dir) {
        do {
            // Move down the list one
            _boyPaletteIndex += dir;
            if (_boyPaletteIndex < 0) {
                _boyPaletteIndex = _boyPalettes.Count-1;
            } else if (_boyPaletteIndex >= _boyPalettes.Count) {
                _boyPaletteIndex = 0;
            }
        } while (_boyPalettes[_boyPaletteIndex] == _chosenBoyPalette);

        boySprite.material = _boyPalettes[_boyPaletteIndex];
        // Change the icon to the correct image
        //boySprite.sprite = _characterSprites[_boyColor];
    }
    protected void ChangeGirl(int dir) {
        do {
            // Move down the list one
            _girlPaletteIndex += dir;
            if (_girlPaletteIndex < 0) {
                _girlPaletteIndex = _girlPalettes.Count-1;
            } else if (_girlPaletteIndex >= _girlPalettes.Count) {
                _girlPaletteIndex = 0;
            }
        } while (_girlPalettes[_girlPaletteIndex] == _chosenGirlPalette);

        girlSprite.material = _girlPalettes[_girlPaletteIndex];
        // Change the icon to the correct image
        //girlSprite.sprite = _characterSprites[_girlColor+4];
    }

    public void Activate(PlayerInfoBox pib) {
        _playerInfoBox = pib;
        _controllingPlayer = ReInput.players.GetPlayer(pib.playerID);

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pib.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pib.characterInfo);

        // Block already selected characters
        if(pib.playerID == 0) {
            if(ES3.Load<int>("Player2Character", 0) == (int)CHARACTERS.BOY) {
                //_chosenBoyColor = ES3.Load<int>("Player2Color", 1)-1;
                _chosenBoyPalette = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy" + ES3.Load<int>("Player2Color"));
            } else {
                //_chosenGirlColor = ES3.Load<int>("Player2Color", 1)-1;
                _chosenGirlPalette = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl" + ES3.Load<int>("Player2Color"));
            }
        } else {
            if (ES3.Load<int>("Player1Character", 0) == (int)CHARACTERS.BOY) {
                //_chosenBoyColor = ES3.Load<int>("Player1Color", 1)-1;
                _chosenBoyPalette = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy" + ES3.Load<int>("Player1Color"));
            } else {
                //_chosenGirlColor = ES3.Load<int>("Player1Color", 1)-1;
                _chosenGirlPalette = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl" + ES3.Load<int>("Player1Color"));
            }
        }

        menuObject.SetActive(true);
        _isActive = true;
        _storySelectMenu.DisableUI();

        // Wait a frame to avoid input overflow
        _waitFrame = true;

        TakeFocus();
    }

    protected void SetMenuOptionInputs(int playerID) {
        _options = GetComponentsInChildren<MenuOption>(true);
        foreach (MenuOption option in _options) {
            option.SetPlayer(playerID);
        }
    }

    protected void SetSelectionToCharacter(CharaInfo charaInfo) {
        if (charaInfo.name == CHARACTERS.BOY) {
            // Highlight the boy
            _options[0].isFirstSelection = true;
            _options[1].isFirstSelection = false;

            // Set the boy to the correct name and sprite
            //_boyPaletteIndex = charaInfo.color-1;
            boySprite.material = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy" + charaInfo.color);
            for(int i = 0; i < _boyPalettes.Count; ++i) {
                if(_boyPalettes[i] == boySprite.material) {
                    _boyPaletteIndex = i;
                    break;
                }
            }
            //boySprite.sprite = _characterSprites[_boyColor];
        } else if (charaInfo.name == CHARACTERS.GIRL) {
            // Highlight the girl
            _options[1].isFirstSelection = true;
            _options[0].isFirstSelection = false;

            // Set the girl to the correct name and sprite
            //_girlPaletteIndex = charaInfo.color-1;
            girlSprite.material = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl" + charaInfo.color);
            for (int i = 0; i < _girlPalettes.Count; ++i) {
                if (_girlPalettes[i] == girlSprite.material) {
                    _girlPaletteIndex = i;
                    break;
                }
            }
            //girlSprite.sprite = _characterSprites[_girlColor+4];
        }
    }

    public override void Deactivate() {
        base.Deactivate();

        menuObject.SetActive(false);
        _isActive = false;

        _chosenBoyPalette = null;
        _chosenGirlPalette = null;

        _options = null;

        _storySelectMenu.EnableUI();
    }

    protected bool IsBoyHighlighted() {
        if(_options[0].isHighlighted) {
            return true;
        }

        return false;
    }

    public virtual void ChooseBoy() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.BOY;

        // Get the color out of the material name
        string paletteString = new String(_boyPalettes[_boyPaletteIndex].name.Where(Char.IsDigit).ToArray());
        charaInfo.color = int.Parse(paletteString);
        //charaInfo.color = _boyPaletteIndex+1;

        _playerInfoBox.SetCharacter(charaInfo);

        int tempColor = _boyPaletteIndex;
        ChangeBoy(1);

        Deactivate();
    }

    public virtual void ChooseGirl() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.GIRL;
        // Get the color out of the material name
        string paletteString = new String(_girlPalettes[_girlPaletteIndex].name.Where(Char.IsDigit).ToArray());
        charaInfo.color = int.Parse(paletteString);
        //charaInfo.color = _girlPaletteIndex+1;

        _playerInfoBox.SetCharacter(charaInfo);

        int tempColor = _girlPaletteIndex;
        ChangeGirl(1);

        Deactivate();
    }
}
