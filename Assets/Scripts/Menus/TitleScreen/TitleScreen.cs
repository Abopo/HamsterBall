﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class TitleScreen : MonoBehaviour {
    public bool resetData;

    InputState _input;
    GameManager _gameManager;

    private void Awake() {
        _input = new InputState(0);
    }

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();

        if (resetData) {
            // TODO: Remove for final build
            ES3.Save<int>("FirstTimePlaying", 0);

            // Delete the save file
            ES3.DeleteFile();
        }
    }

    // Update is called once per frame
    void Update () {
        _input.GetInput();
        if(_input.AnyButtonPressed()) {
            
            if (_gameManager.demoMode) {
                // Since it's demo mode make sure the correct stages are unlocked
                ES3.Save<int>("Forest", 1);
                ES3.Save<int>("Mountain", 1);
                ES3.Save<int>("Beach", 1);
                ES3.Save<int>("City", 1);
                ES3.Save<int>("Sewers", 0);
                ES3.Save<int>("Corporation", 0);
                ES3.Save<int>("Laboratory", 0);
                ES3.Save<int>("Airship", 0);
            }
            
            // Load village
            LoadingScreen.sceneToLoad = "VillageScene";
            SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
        }
    }
}
