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

    float _writeDelay = 0.025f;
    float _writeTimer = 0f;

	// Use this for initialization
	void Start () {
        done = true;
	}
	
	// Update is called once per frame
	void Update () {
        CheckInput();

		if(!done) {
            _writeTimer += Time.deltaTime;
            if(_writeTimer >= _writeDelay) {
                // Display the next character
                _displayString += _textToWrite[_index];
                displayText.text = _displayString;

                // TODO: Maybe play a little typey sound

                _index++;
                _writeTimer = 0f;
                if(_displayString == _textToWrite) {
                    done = true;
                }
            }
        }
	}

    void CheckInput() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            // Skip writing
            _displayString = _textToWrite;
            displayText.text = _displayString;
            done = true;
        }
    }

    public void StartWriting(string text) {
        _textToWrite = text;
        _displayString = "";
        displayText.text = _displayString;

        done = false;
        _index = 0;
        _writeTimer = 0f;
    }
}
