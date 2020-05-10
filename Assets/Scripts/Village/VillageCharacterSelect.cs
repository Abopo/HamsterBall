using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
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

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pCon.inputState.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pCon.CharaInfo);

        _childObject.SetActive(true);
        _isActive = true;

        // Wait a frame to avoid input overflow
        _waitFrame = true;

        TakeFocus();

        // Turn off input for the player
        _gameManager.FullPause();
    }

    public override void Deactivate() {
        _childObject.SetActive(false);
        _isActive = false;

        _chosenBoyPalette = null;
        _chosenGirlPalette = null;

        LoseFocus();

        _gameManager.Unpause();
    }

    public override void ChooseBoy() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.BOY;

        // Get the color out of the material name
        string paletteString = new String(_boyPalettes[_boyPaletteIndex].name.Where(Char.IsDigit).ToArray());
        charaInfo.color = int.Parse(paletteString);

        _playerController.SetCharacterInfo(charaInfo);
        ES3.Save<int>("Player1Character", charaInfo.name);
        ES3.Save<int>("Player1Color", charaInfo.color);

        Deactivate();
    }
    public override void ChooseGirl() {
        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = CHARACTERS.GIRL;

        // Get the color out of the material name
        string paletteString = new String(_girlPalettes[_girlPaletteIndex].name.Where(Char.IsDigit).ToArray());
        charaInfo.color = int.Parse(paletteString);

        _playerController.SetCharacterInfo(charaInfo);
        ES3.Save<int>("Player1Character", charaInfo.name);
        ES3.Save<int>("Player1Color", charaInfo.color);

        Deactivate();
    }
}
