using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class TextWriter : MonoBehaviour {
    public SuperTextMesh displayText;
    public bool done;
    public bool paused;

    string _textToWrite;
    string _displayString = "";
    int _index;
    bool _done;

    float _writeDelay = 0.02f;
    float _writeTimer = 0f;

    Player _player;

	// Use this for initialization
	void Start () {
        _done = true;
        _player = ReInput.players.GetPlayer(0);
	}
	
	// Update is called once per frame
	void Update () {
		if(!_done && !paused) {
            _writeTimer += Time.unscaledDeltaTime;
            if(_writeTimer >= _writeDelay) {
                // Display the next character
                _displayString += _textToWrite[_index];

                // Display rich text tags all at once
                if(_textToWrite[_index] == '<') {
                    while(_textToWrite[_index] != '>') {
                        _displayString += _textToWrite[++_index];
                    }
                }

                displayText.text = _displayString;

                // TODO: Maybe play a little typey sound

                _index++;
                _writeTimer = 0f;
                if(_displayString == _textToWrite || _index >= _textToWrite.Length) {
                    done = true;
                    _done = true;
                }
            }
        } else {
            done = _done;
        }

        CheckInput();
    }

    void CheckInput() {
        if(_player.GetButtonDown("Submit") && _displayString.Length > 2) {
            // Skip writing
            _displayString = _textToWrite;
            displayText.text = _displayString;
            _done = true;
        }
        if(_player.GetButtonDown("Submit")) {
            if (_displayString == _textToWrite) {
                done = true;
            }
        }
    }

    public void StartWriting(string text) {
        _textToWrite = text;
        ClearText();

        _done = false;
        done = false;
        _index = 0;
        _writeTimer = 0f;
    }

    public void ClearText() {
        _displayString = "";
        displayText.text = _displayString;
    }

    public void GetButton(string eventString, STMTextInfo textInfo) {
        Player player = ReInput.players.GetPlayer(0);
        ActionElementMap aeMap = null;

        switch (eventString) {
            case "Jump":
                Debug.Log("Find Jump key");
                aeMap = player.controllers.maps.GetFirstButtonMapWithAction(2, true);
                break;
            case "Catch":
                aeMap = player.controllers.maps.GetFirstButtonMapWithAction(3, true);
                break;
            case "Attack":
                aeMap = player.controllers.maps.GetFirstButtonMapWithAction(4, true);
                break;
            case "Swap":
                aeMap = player.controllers.maps.GetFirstButtonMapWithAction(5, true);
                break;
        }

        if (aeMap != null) {
            // If we haven't inserted the button yet
            char blah = _displayString[textInfo.rawIndex];
            if (blah == '?') {
                _displayString = _displayString.Remove(textInfo.rawIndex, 1);
                _displayString = _displayString.Insert(textInfo.rawIndex, aeMap.elementIdentifierName);
            }
        }
    }
}