using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;


public enum AIMASSIST { ALWAYS = 0, AFTERLOSS, NEVER }
public class AimAssistSetting : MenuOption {

    public Text text;

    GameManager _gameManager;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        _gameManager = FindObjectOfType<GameManager>();

        SetText();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(isHighlighted) {
            CheckInput();
        }
    }

    new void CheckInput() {
        if(_player.GetButtonDown("Left")) {
            // Adjust setting to the left
            _gameManager.aimAssistSetting -= 1;
            if(_gameManager.aimAssistSetting < AIMASSIST.ALWAYS) {
                _gameManager.aimAssistSetting = AIMASSIST.NEVER;
            }
            ES3.Save<int>("AimAssist", (int)_gameManager.aimAssistSetting);
            SetText();
        } else if(_player.GetButtonDown("Right")) {
            // Adjust setting to the right
            _gameManager.aimAssistSetting += 1;
            if (_gameManager.aimAssistSetting > AIMASSIST.NEVER) {
                _gameManager.aimAssistSetting = AIMASSIST.ALWAYS;
            }
            ES3.Save<int>("AimAssist", (int)_gameManager.aimAssistSetting);
            SetText();
        }
    }

    void SetText() {
        switch(_gameManager.aimAssistSetting) {
            case AIMASSIST.ALWAYS:
                text.text = "Always";
                break;
            case AIMASSIST.AFTERLOSS:
                text.text = "After loss";
                break;
            case AIMASSIST.NEVER:
                text.text = "Never";
                break;
        }
    }

    public override void Highlight() {
        base.Highlight();
    }

    protected override void Select() {
    }
}
