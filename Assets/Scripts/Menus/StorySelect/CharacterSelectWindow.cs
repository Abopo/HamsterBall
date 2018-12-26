using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class CharacterSelectWindow : MonoBehaviour {
    protected CHARACTERNAMES _boyName = CHARACTERNAMES.BOY2;
    protected CHARACTERNAMES _girlName = CHARACTERNAMES.GIRL1;
    public Image boySprite;
    public Image girlSprite;
    protected Sprite[] characterSprites = new Sprite[6];

    public CHARACTERNAMES _alreadyChosen1 = CHARACTERNAMES.BOY1;
    public CHARACTERNAMES _alreadyChosen2 = CHARACTERNAMES.NUM_CHARACTERS;

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
        Sprite[] girlSprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Girl-Icon");
        characterSprites[0] = boySprites[0];
        characterSprites[1] = boySprites[1];
        characterSprites[2] = boySprites[2];
        characterSprites[3] = boySprites[3];
        characterSprites[4] = girlSprites[0];
        characterSprites[5] = girlSprites[1];

        boySprite.sprite = characterSprites[(int)_boyName];
        girlSprite.sprite = characterSprites[(int)_girlName];
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
            _boyName = _boyName + dir;
            if (_boyName < CHARACTERNAMES.BOY1) {
                _boyName = CHARACTERNAMES.BOY4;
            } else if (_boyName > CHARACTERNAMES.BOY4) {
                _boyName = CHARACTERNAMES.BOY1;
            }
        } while (_boyName == _alreadyChosen1 || _boyName == _alreadyChosen2);

        // Change the icon to the correct image
        boySprite.sprite = characterSprites[(int)_boyName];
    }
    protected void ChangeGirl(int dir) {
        do {
            // Move down the list one
            _girlName = _girlName + dir;
            if (_girlName < CHARACTERNAMES.GIRL1) {
                _girlName = CHARACTERNAMES.GIRL2;
            } else if (_girlName > CHARACTERNAMES.GIRL2) {
                _girlName = CHARACTERNAMES.GIRL1;
            }
        } while (_girlName == _alreadyChosen1 || _girlName == _alreadyChosen2);

        // Change the icon to the correct image
        girlSprite.sprite = characterSprites[(int)_girlName];
    }

    public void Activate(PlayerInfoBox pib) {
        _playerInfoBox = pib;
        _controllingPlayer = ReInput.players.GetPlayer(pib.playerID);

        // Don't block the character that this player has chosen
        AllowChosenCharacter(_playerInfoBox.characterName);

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pib.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pib.characterName);

        gameObject.SetActive(true);
        _isActive = true;
        _storySelectMenu.DisableUI();
    }

    // Makes sure that the character already chosen by the player will be displayed
    // (so they can rechoose the same character if desired)
    protected void AllowChosenCharacter(CHARACTERNAMES charaName) {
        if (charaName == _alreadyChosen1) {
            _alreadyChosen1 = CHARACTERNAMES.NUM_CHARACTERS;
        } else if (charaName == _alreadyChosen2) {
            _alreadyChosen2 = CHARACTERNAMES.NUM_CHARACTERS;
        }
    }

    protected void SetMenuOptionInputs(int playerID) {
        _options = GetComponentsInChildren<MenuOption>(true);
        foreach (MenuOption option in _options) {
            option.SetPlayer(playerID);
        }
    }

    protected void SetSelectionToCharacter(CHARACTERNAMES charaName) {
        if (charaName <= CHARACTERNAMES.BOY4) {
            // Highlight the boy
            _options[0].Highlight();

            // Set the boy to the correct name and sprite
            _boyName = charaName;
            boySprite.sprite = characterSprites[(int)_boyName];
        } else if (charaName >= CHARACTERNAMES.GIRL1 && charaName <= CHARACTERNAMES.GIRL2) {
            // Highlight the girl
            _options[1].Highlight();

            // Set the girl to the correct name and sprite
            _girlName = charaName;
            girlSprite.sprite = characterSprites[(int)_girlName];
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
        _playerInfoBox.SetCharacter(_boyName);

        CHARACTERNAMES tempName = _boyName;
        ChangeBoy(1);
        SaveChosen(tempName);

        Deactivate();
    }

    public virtual void ChooseGirl() {
        _playerInfoBox.SetCharacter(_girlName);

        CHARACTERNAMES tempName = _girlName;
        ChangeGirl(1);
        SaveChosen(tempName);

        Deactivate();
    }

    protected void SaveChosen(CHARACTERNAMES chosen) {
        // Hold onto the chosen character so it can be excluded from later choices
        if (_alreadyChosen1 == CHARACTERNAMES.NUM_CHARACTERS) {
            _alreadyChosen1 = chosen;
        } else if (_alreadyChosen2 == CHARACTERNAMES.NUM_CHARACTERS) {
            _alreadyChosen2 = chosen;
        }
    }
}
