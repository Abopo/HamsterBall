using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Rewired;

public class VillageCharacterSelect : CharacterSelectWindow {

    PlayerController _playerController;

    protected override void Awake() {
        base.Awake();

        _playerController = FindObjectOfType<PlayerController>();
    }
    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public void Activate(PlayerController pCon) {
        if (menuObj != null) {
            menuObj.SetActive(true);
        }

        _playerController = pCon;
        _controllingPlayer = ReInput.players.GetPlayer(pCon.inputState.playerID);

        // Use that players inputs to control the menu options
        SetMenuOptionInputs(pCon.inputState.playerID);

        // Set the selection to the players character
        SetSelectionToCharacter(pCon.CharaInfo);

        _isActive = true;

        // Wait a frame to avoid input overflow
        _waitFrame = true;

        TakeFocus();

        // Turn off input for the player
        _gameManager.FullPause();
    }

    public override void Deactivate() {
        if (menuObj != null) {
            menuObj.SetActive(false);
        }

        _isActive = false;

        _chosenBoyPalette = null;
        _chosenGirlPalette = null;

        _gameManager.Unpause();

        LoseFocus();
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
