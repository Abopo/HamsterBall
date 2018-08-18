﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AISetupWindow : MonoBehaviour {
    public AISetupOption ai1Setup;
    public AISetupOption ai2Setup;
    public AISetupOption ai3Setup;
    public SetupReadyButton aiReadyButton;

    public SpriteRenderer ai1Sprite;
    public SpriteRenderer ai2Sprite;
    public SpriteRenderer ai3Sprite;

    public GameSetupWindow gameSetupWindow;
    public CharacterSelect characterSelect;

    int _numAIs;

    Sprite[] characterSprites = new Sprite[4];

    PlayerManager _playerManager;

    // Use this for initialization
    void Start () {
    }

    public void Initialize() {
        gameObject.SetActive(true);
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        Sprite[] charaSprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");

        characterSprites[0] = charaSprites[0];
        characterSprites[1] = charaSprites[1];
        characterSprites[2] = charaSprites[2];
        characterSprites[3] = charaSprites[3];

        GetAIPlayerInfo();
        MenuMovementSetup();

        _numAIs = 0;
        if (ai1Setup.gameObject.activeSelf) {
            SetPlayerSprites(ai1Setup.aiInfo, ai1Sprite);
            _numAIs++;
        }
        if (ai2Setup.gameObject.activeSelf) {
            SetPlayerSprites(ai2Setup.aiInfo, ai2Sprite);
            _numAIs++;
        }
        if (ai3Setup.gameObject.activeSelf) {
            SetPlayerSprites(ai3Setup.aiInfo, ai3Sprite);
            _numAIs++;
        }
    }

    void GetAIPlayerInfo() {
        for(int i = 0; i < _playerManager.NumPlayers; ++i) {
            // If this player is an AI
            PlayerInfo pi = _playerManager.GetPlayerByIndex(i);
            if (pi != null && pi.controllerNum < 0) {
                if(!ai1Setup.gameObject.activeSelf) {
                    ai1Setup.Initialize(pi);
                } else if(!ai2Setup.gameObject.activeSelf) {
                    ai2Setup.Initialize(pi);
                } else if(!ai3Setup.gameObject.activeSelf) {
                    ai3Setup.Initialize(pi);
                }
            }
        }
    }

    void MenuMovementSetup() {
        // If the 3rd AI is active
        if(ai3Setup.gameObject.activeSelf) {
            // That means both AI's above it are also active.
            ai1Setup.adjOptions[1] = ai2Setup;
            ai1Setup.adjOptions[3] = aiReadyButton;
            ai2Setup.adjOptions[1] = ai3Setup;
            ai2Setup.adjOptions[3] = ai1Setup;
            ai3Setup.adjOptions[1] = aiReadyButton;
            ai3Setup.adjOptions[3] = ai2Setup;
            aiReadyButton.adjOptions[1] = ai1Setup;
            aiReadyButton.adjOptions[3] = ai3Setup;
            // If the 2nd AI is active
        } else if(ai2Setup.gameObject.activeSelf) {
            ai1Setup.adjOptions[1] = ai2Setup;
            ai1Setup.adjOptions[3] = aiReadyButton;
            ai2Setup.adjOptions[1] = aiReadyButton;
            ai2Setup.adjOptions[3] = ai1Setup;
            aiReadyButton.adjOptions[1] = ai1Setup;
            aiReadyButton.adjOptions[3] = ai2Setup;
            // If only the first AI is active
        } else {
            ai1Setup.adjOptions[1] = aiReadyButton;
            ai1Setup.adjOptions[3] = aiReadyButton;
            aiReadyButton.adjOptions[1] = ai1Setup;
            aiReadyButton.adjOptions[3] = ai1Setup;
        }
    }

    void SetPlayerSprites(PlayerInfo pi, SpriteRenderer sr) {
        sr.sprite = characterSprites[(int)pi.characterName];
        /*
        switch(pi.characterName) {
            case CHARACTERNAMES.BOB:
                sr.sprite = characterSprites[0];
                break;
            case CHARACTERNAMES.BUB:
                sr.sprite = characterSprites[1];
                break;
            case CHARACTERNAMES.NEGABOB:
                sr.sprite = characterSprites[2];
                break;
            case CHARACTERNAMES.NEGABUB:
                sr.sprite = characterSprites[3];
                break;
            case CHARACTERNAMES.PEPSIMAN:
                sr.sprite = characterSprites[4];
                break;
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Cancel")) {
            // Turn off menu and turn back on character select stuff
            characterSelect.Reactivate();
            Deactivate();
        }
    }

    void Deactivate() {
        ai1Setup.gameObject.SetActive(false);
        ai2Setup.gameObject.SetActive(false);
        ai3Setup.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OpenGameSetup() {
        //SceneManager.LoadScene("MapSelect2");
        if (FindObjectOfType<GameManager>().demoMode) {
            gameSetupWindow.DemoSetup();
        } else {
            // Turn on GameSetupWindow
            gameSetupWindow.Initialize();
            gameObject.SetActive(false);
        }
    }

    void UpdateText(PlayerInfo pInfo, Text text) {
        switch(pInfo.difficulty) {
            case 0:
                text.text = "Easy";
                text.color = Color.blue;
                break;
            case 1:
                text.text = "Medium";
                text.color = Color.green;
                break;
            case 2:
                text.text = "Hard";
                text.color = Color.yellow;
                break;
            case 3:
                text.text = "Expert";
                text.color = Color.red;
                break;
        }
    }
}
