using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour {
    public Text displayText;
    public bool done;

    string _textToWrite;
    string _displayString;
    int _index;
    bool _done;

    float _writeDelay = 0.02f;
    float _writeTimer = 0f;

	// Use this for initialization
	void Start () {
        _done = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(!_done) {
            _writeTimer += Time.unscaledDeltaTime;
            if(_writeTimer >= _writeDelay) {
                // Display the next character
                _displayString += _textToWrite[_index];
                displayText.text = _displayString;

                // TODO: Maybe play a little typey sound

                _index++;
                _writeTimer = 0f;
                if(_displayString == _textToWrite) {
                    done = true;
                    _done = true;
                }
            }
        }

        CheckInput();
    }

    void CheckInput() {
        if(Input.GetKeyDown(KeyCode.Space) && _displayString.Length > 2) {
            // Skip writing
            _displayString = _textToWrite;
            displayText.text = _displayString;
            _done = true;
        }
        if(Input.GetKeyUp(KeyCode.Space)) {
            if (_displayString == _textToWrite) {
                done = true;
            }
        }
    }

    public void StartWriting(string text) {
        _textToWrite = text;
        _displayString = "";
        displayText.text = _displayString;

        _done = false;
        done = false;
        _index = 0;
        _writeTimer = 0f;
    }
}