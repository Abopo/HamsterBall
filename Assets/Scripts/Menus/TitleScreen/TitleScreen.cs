﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class TitleScreen : MonoBehaviour {

    InputState _input;

	// Use this for initialization
	void Start () {
        _input = new InputState(0);
    }
	
	// Update is called once per frame
	void Update () {
        _input.GetInput();
        if(_input.select.isJustPressed) {
            // Load in demo mode
            SceneManager.LoadScene("LocalPlay");
        } else if(_input.AnyButtonPressed()) {
            // Load village
            SceneManager.LoadScene("VillageScene");
        }
	}
}
