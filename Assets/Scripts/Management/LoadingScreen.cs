﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class LoadingScreen : MonoBehaviour {
    public static string sceneToLoad;

    public Text loadingText;

    bool _started;

    bool _sceneLoaded;
    Player _player;

    // Start is called before the first frame update
    void Start() {
        _sceneLoaded = false;

        _player = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update() {
        if(_sceneLoaded) {
            if (_player.GetButtonDown("Submit")) {
                Debug.Log("Submit load");

                // Start the game countdown
                FindObjectOfType<GameCountdown>().started = true;

                // Destroy the loading screen
                Destroy(gameObject);
            }
        }
    }

    private void LateUpdate() {
        // Do this late so the loading screen assets have time to start
        if (!_started) {
            StartCoroutine(LoadTheScene());
            _started = true;
        }
    }

    IEnumerator LoadTheScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while(!asyncLoad.isDone) {
            yield return null;
        }

        _sceneLoaded = true;
        loadingText.text = "Press A";
    }
}