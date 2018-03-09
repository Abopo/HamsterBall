using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Makes it so you can only input numbers to the time limit
public class TimeLimitInput : MonoBehaviour {

    InputField timeInputField;

	// Use this for initialization
	void Start () {
        timeInputField = GetComponent<InputField>();

        // Sets the MyValidate method to invoke after the input field's default input validation invoke (default validation happens every time a character is entered into the text field.)
        timeInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };
    }

    // Update is called once per frame
    void Update () {
		
	}

    private char MyValidate(char charToValidate) {
        if(charToValidate >= '0' && charToValidate <= '9') {
            return charToValidate;
        } else {
            return '\0';
        }
    }
}
