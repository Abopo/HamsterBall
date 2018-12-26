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
	void Update () {
        if (_isActive) {
            CheckInput();
        }
    }

    public void Activate(PlayerController pCon) {
        _playerController = pCon;
        _controllingPlayer = ReInput.players.GetPlayer(pCon.inputState.playerID);

        // Don't block the character that this player has chosen
        AllowChosenCharacter(pCon.CharacterName);

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pCon.inputState.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pCon.CharacterName);

        _childObject.SetActive(true);
        _isActive = true;
        // Turn off input for the player
        _gameManager.FullPause();
    }

    public override void Deactivate() {
        _childObject.SetActive(false);
        _isActive = false;

        _gameManager.Unpause();
    }

    public override void ChooseBoy() {
        _playerController.SetCharacterName(_boyName);

        CHARACTERNAMES tempName = _boyName;
        ChangeBoy(1);
        SaveChosen(tempName);

        Deactivate();
    }
    public override void ChooseGirl() {
        _playerController.SetCharacterName(_girlName);

        CHARACTERNAMES tempName = _girlName;
        ChangeGirl(1);
        SaveChosen(tempName);

        Deactivate();
    }
}
