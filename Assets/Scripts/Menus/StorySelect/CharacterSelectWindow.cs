﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

// This enum is only used for selecting between the Boy and Girl in story mode
public enum CHARACTERCOLORS { BOY1 = 0, BOY2, BOY3, BOY4, GIRL1, GIRL2, NUM_COLORS }

public class CharacterSelectWindow : MonoBehaviour {
    //protected CHARACTERNAMES _boyName = CHARACTERNAMES.BOY2;
    //protected CHARACTERNAMES _girlName = CHARACTERNAMES.GIRL1;
    protected CHARACTERCOLORS _boyColor = CHARACTERCOLORS.BOY2;
    protected CHARACTERCOLORS _girlColor = CHARACTERCOLORS.GIRL1;

    public Image boySprite;
    public Image girlSprite;
    protected Sprite[] _characterSprites = new Sprite[6];
    //protected Sprite[] _boySprites = new Sprite[4];
    //protected Sprite[] _girlSprites = new Sprite[2];

    //public CHARACTERNAMES _alreadyChosen1 = CHARACTERNAMES.BOY1;
    //public CHARACTERNAMES _alreadyChosen2 = CHARACTERNAMES.NUM_CHARACTERS;

    public CHARACTERCOLORS _alreadyChosen1 = CHARACTERCOLORS.BOY1;
    public CHARACTERCOLORS _alreadyChosen2 = CHARACTERCOLORS.NUM_COLORS;

    PlayerInfoBox _playerInfoBox;
    protected Player _controllingPlayer;

    protected MenuOption[] _options;

    protected bool _isActive;

    protected GameManager _gameManager;
    StorySelectMenu _storySelectMenu;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _storySelectMenu = FindObjectOfType<StorySelectMenu>();
    }
    // Use this for initialization
    protected virtual void Start () {
        Sprite[] boySprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        Sprite[] girlSprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Girl-Icon");
        _characterSprites[0] = boySprites[0];
        _characterSprites[1] = boySprites[1];
        _characterSprites[2] = boySprites[2];
        _characterSprites[3] = boySprites[3];
        _characterSprites[4] = girlSprites[0];
        _characterSprites[5] = girlSprites[1];

        boySprite.sprite = _characterSprites[(int)_boyColor];
        girlSprite.sprite = _characterSprites[(int)_girlColor];
    }

    // Update is called once per frame
    void Update() {
        if (_isActive) {
            CheckInput();
        }
    }

    protected void CheckInput() {
        if (_controllingPlayer.GetButtonDown("Up")) {
            // Figure out which character is highlighted
            if (IsBoyHighlighted()) {
                ChangeBoy(1);
            } else {
                ChangeGirl(1);
            }
        }
        if (_controllingPlayer.GetButtonDown("Down")) {
            // Figure out which character is highlighted
            if (IsBoyHighlighted()) {
                ChangeBoy(-1);
            } else {
                ChangeGirl(-1);
            }
        }
        if (_controllingPlayer.GetButtonDown("Cancel")) {
            Deactivate();
        }
    }

    protected void ChangeBoy(int dir) {
        do {
            // Move down the list one
            _boyColor = _boyColor + dir;
            if (_boyColor < CHARACTERCOLORS.BOY1) {
                _boyColor = CHARACTERCOLORS.BOY4;
            } else if (_boyColor > CHARACTERCOLORS.BOY4) {
                _boyColor = CHARACTERCOLORS.BOY1;
            }
        } while (_boyColor == _alreadyChosen1 || _boyColor == _alreadyChosen2);

        // Change the icon to the correct image
        boySprite.sprite = _characterSprites[(int)_boyColor];
    }
    protected void ChangeGirl(int dir) {
        do {
            // Move down the list one
            _girlColor = _girlColor + dir;
            if (_girlColor < CHARACTERCOLORS.GIRL1) {
                _girlColor = CHARACTERCOLORS.GIRL2;
            } else if (_girlColor > CHARACTERCOLORS.GIRL2) {
                _girlColor = CHARACTERCOLORS.GIRL1;
            }
        } while (_girlColor == _alreadyChosen1 || _girlColor == _alreadyChosen2);

        // Change the icon to the correct image
        girlSprite.sprite = _characterSprites[(int)_girlColor];
    }

    public void Activate(PlayerInfoBox pib) {
        _playerInfoBox = pib;
        _controllingPlayer = ReInput.players.GetPlayer(pib.playerID);

        // Don't block the character that this player has chosen
        AllowChosenCharacter(_playerInfoBox.charaColor);

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pib.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pib.charaColor);

        gameObject.SetActive(true);
        _isActive = true;
        _storySelectMenu.DisableUI();
    }

    // Makes sure that the character already chosen by the player will be displayed
    // (so they can rechoose the same character if desired)
    protected void AllowChosenCharacter(CHARACTERCOLORS charaColor) {
        if (charaColor == _alreadyChosen1) {
            _alreadyChosen1 = CHARACTERCOLORS.NUM_COLORS;
        } else if (charaColor == _alreadyChosen2) {
            _alreadyChosen2 = CHARACTERCOLORS.NUM_COLORS;
        }
    }

    protected void SetMenuOptionInputs(int playerID) {
        _options = GetComponentsInChildren<MenuOption>(true);
        foreach (MenuOption option in _options) {
            option.SetPlayer(playerID);
        }
    }

    protected void SetSelectionToCharacter(CHARACTERCOLORS charaColor) {
        if (charaColor <= CHARACTERCOLORS.BOY4) {
            // Highlight the boy
            _options[0].Highlight();

            // Set the boy to the correct name and sprite
            _boyColor = charaColor;
            boySprite.sprite = _characterSprites[(int)_boyColor];
        } else if (charaColor >= CHARACTERCOLORS.GIRL1 && charaColor <= CHARACTERCOLORS.GIRL2) {
            // Highlight the girl
            _options[1].Highlight();

            // Set the girl to the correct name and sprite
            _girlColor = charaColor;
            girlSprite.sprite = _characterSprites[(int)_girlColor];
        }
    }

    public virtual void Deactivate() {
        gameObject.SetActive(false);
        _isActive = false;

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
        charaInfo.color = (int)_boyColor;
        _playerInfoBox.SetCharacter(_boyColor);

        CHARACTERCOLORS tempColor = _boyColor;
        ChangeBoy(1);
        SaveChosen(tempColor);

        Deactivate();
    }

    public virtual void ChooseGirl() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.BOY;
        charaInfo.color = (int)_girlColor;
        _playerInfoBox.SetCharacter(_girlColor);

        CHARACTERCOLORS tempColor = _girlColor;
        ChangeGirl(1);
        SaveChosen(tempColor);

        Deactivate();
    }

    protected void SaveChosen(CHARACTERCOLORS chosen) {
        // Hold onto the chosen character so it can be excluded from later choices
        if (_alreadyChosen1 == CHARACTERCOLORS.NUM_COLORS) {
            _alreadyChosen1 = chosen;
        } else if (_alreadyChosen2 == CHARACTERCOLORS.NUM_COLORS) {
            _alreadyChosen2 = chosen;
        }
    }
}
