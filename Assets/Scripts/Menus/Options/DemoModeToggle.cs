using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoModeToggle : MonoBehaviour {

    GameManager _gameManager;
    Toggle theToggle;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        theToggle = GetComponent<Toggle>();

        theToggle.isOn = _gameManager.demoMode;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleDemoMode() {
        _gameManager.SetDemoMode(theToggle.isOn);
        //_gameManager.demoMode = theToggle.isOn;
    }
}
