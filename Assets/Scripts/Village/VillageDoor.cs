﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class VillageDoor : MonoBehaviour {
    public string _sceneToLoad;

    bool _isPlayerHere;

    Player _playerInput;
    protected PlayerController _playerController;

    InteractIcon _interactIcon;

    VillagePlayerSpawn _villagePlayerSpawn;
    GameManager _gameManager;

    private void Awake() {
        _playerInput = ReInput.players.GetPlayer(0);

        _interactIcon = GetComponentInChildren<InteractIcon>();

        _villagePlayerSpawn = FindObjectOfType<VillagePlayerSpawn>();
        _gameManager = FindObjectOfType<GameManager>();
    }
    // Use this for initialization
    protected virtual void Start () {
        _isPlayerHere = false;

	}
	
	// Update is called once per frame
	void Update () {
        if (_isPlayerHere && !_gameManager.isPaused) {
            if (_playerController.CanJump && _playerInput.GetButtonDown("Interact")) {
                EnterDoor();
            }
        }
	}

    protected virtual void EnterDoor() {
        if(_sceneToLoad == "") {
            return;
        }

        // Load the proper scene
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Additive);

        // Set the player's respawn point to this door
        _villagePlayerSpawn.SetSpawnPosition(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            _isPlayerHere = true;
            _playerController = collision.GetComponent<PlayerController>();
            _interactIcon.Activate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = false;
            _playerController = null;
            _interactIcon.Deactivate();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive) {
            // Set the new scene to active
            SceneManager.SetActiveScene(scene);
        }
    }
}
