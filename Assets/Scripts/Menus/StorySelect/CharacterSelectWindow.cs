using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

// This enum is only used for selecting between the Boy and Girl in story mode
//public enum CHARACTERCOLORS { BOY1 = 0, BOY2, BOY3, BOY4, GIRL1, GIRL2, GIRL3, GIRL4, NUM_COLORS }

public class CharacterSelectWindow : Menu {
    protected int _boyColor = 0;
    protected int _girlColor = 0;

    public Image boySprite;
    public Image girlSprite;
    protected Sprite[] _characterSprites = new Sprite[8];

    public GameObject menuObject;

    // The selected color of the other player
    protected int _chosenBoyColor = -1;
    protected int _chosenGirlColor = -1;

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

        boySprite.sprite = _characterSprites[_boyColor];
        girlSprite.sprite = _characterSprites[_girlColor+4];
    }

    void LoadSprites() {
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
            _boyColor += dir;
            if (_boyColor < 0) {
                _boyColor = 3;
            } else if (_boyColor > 3) {
                _boyColor = 0;
            }
        } while (_boyColor == _chosenBoyColor);

        // Change the icon to the correct image
        boySprite.sprite = _characterSprites[_boyColor];
    }
    protected void ChangeGirl(int dir) {
        do {
            // Move down the list one
            _girlColor += dir;
            if (_girlColor < 0) {
                _girlColor = 3;
            } else if (_girlColor > 3) {
                _girlColor = 0;
            }
        } while (_girlColor == _chosenGirlColor);

        // Change the icon to the correct image
        girlSprite.sprite = _characterSprites[_girlColor+4];
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
                _chosenBoyColor = ES3.Load<int>("Player2Color", 1)-1;
            } else {
                _chosenGirlColor = ES3.Load<int>("Player2Color", 1)-1;
            }
        } else {
            if (ES3.Load<int>("Player1Character", 0) == (int)CHARACTERS.BOY) {
                _chosenBoyColor = ES3.Load<int>("Player1Color", 1)-1;
            } else {
                _chosenGirlColor = ES3.Load<int>("Player1Color", 1)-1;
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
            _boyColor = charaInfo.color-1;
            boySprite.sprite = _characterSprites[_boyColor];
        } else if (charaInfo.name == CHARACTERS.GIRL) {
            // Highlight the girl
            _options[1].isFirstSelection = true;
            _options[0].isFirstSelection = false;

            // Set the girl to the correct name and sprite
            _girlColor = charaInfo.color-1;
            girlSprite.sprite = _characterSprites[_girlColor+4];
        }
    }

    public override void Deactivate() {
        base.Deactivate();

        gameObject.SetActive(false);
        _isActive = false;

        _chosenBoyColor = -1;
        _chosenGirlColor = -1;

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
        charaInfo.color = _boyColor+1;
        _playerInfoBox.SetCharacter(charaInfo);

        int tempColor = _boyColor;
        ChangeBoy(1);

        Deactivate();
    }

    public virtual void ChooseGirl() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.GIRL;
        charaInfo.color = _girlColor+1;
        _playerInfoBox.SetCharacter(charaInfo);

        int tempColor = _girlColor;
        ChangeGirl(1);

        Deactivate();
    }
}
