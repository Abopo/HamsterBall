using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class VillageCharacterSelect : CharacterSelectWindow {

    PlayerController _playerController;
    GameObject _childObject;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        _playerController = FindObjectOfType<PlayerController>();

        _childObject = transform.GetChild(0).gameObject;
	}

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public void Activate(PlayerController pCon) {
        _playerController = pCon;
        _controllingPlayer = ReInput.players.GetPlayer(pCon.inputState.playerID);

        // Don't block the character that this player has chosen
        AllowChosenCharacter(pCon.CharaInfo);

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pCon.inputState.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pCon.CharaInfo);

        _childObject.SetActive(true);
        _isActive = true;

        TakeFocus();

        // Turn off input for the player
        _gameManager.FullPause();
    }

    // Makes sure that the character already chosen by the player will be displayed
    // (so they can rechoose the same character if desired)
    protected void AllowChosenCharacter(CharaInfo charaInfo) {
        // Convert CharaInfo to CHARACTERCOLORS
        int charaColor = 0;
        if(charaInfo.name == CHARACTERS.BOY) {
            charaColor = charaInfo.color - 1;
        } else if(charaInfo.name == CHARACTERS.GIRL) {
            charaColor = 3 + charaInfo.color;
        }

        AllowChosenCharacter((CHARACTERCOLORS)charaColor);
    }

    protected void SetSelectionToCharacter(CharaInfo charaInfo) {
        // Convert CharaInfo to CHARACTERCOLORS
        int charaColor = 0;
        if (charaInfo.name == CHARACTERS.BOY) {
            charaColor = charaInfo.color - 1;
        } else if (charaInfo.name == CHARACTERS.GIRL) {
            charaColor = 3 + charaInfo.color;
        }

        SetSelectionToCharacter((CHARACTERCOLORS)charaColor);
    }

    public override void Deactivate() {
        _childObject.SetActive(false);
        _isActive = false;

        _gameManager.Unpause();
    }

    public override void ChooseBoy() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.BOY;
        charaInfo.color = (int)_boyColor + 1;
        _playerController.SetCharacterInfo(charaInfo);
        PlayerPrefs.SetInt("Player1Character", (int)_boyColor);

        CHARACTERCOLORS tempColor = _boyColor;
        ChangeBoy(1);
        SaveChosen(tempColor);

        Deactivate();
    }
    public override void ChooseGirl() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.GIRL;
        charaInfo.color = (int)_girlColor - 3;
        _playerController.SetCharacterInfo(charaInfo);
        PlayerPrefs.SetInt("Player1Character", (int)_girlColor);

        CHARACTERCOLORS tempColor = _girlColor;
        ChangeGirl(1);
        SaveChosen(tempColor);

        Deactivate();
    }
}
