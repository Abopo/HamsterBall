﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves the water up and down in the beach level
public class WaterController : MonoBehaviour {

    public float WaterHeight {
        get {
            return transform.position.y + 1.9f;
        }
    }

    EdgeCollider2D[] _floatingObjects;
    GameManager _gameManager;

    PlayerController[] _players;

    // Use this for initialization
    void Start () {
        _floatingObjects = GetComponentsInChildren<EdgeCollider2D>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        TurnOffFloatingObjects();

        _players = FindObjectsOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        // Don't update if the game is over
        if (_gameManager.gameIsOver) {
            return;
        }

        // Handle players water effects
        if(_players.Length == 0) {
            _players = FindObjectsOfType<PlayerController>();
        }
        foreach (PlayerController player in _players) {
            if(player.transform.position.y < WaterHeight) {
                player.platformIndex = 2;
            } else if(player.platformIndex == 2) {
                player.platformIndex = 0;
            }
        }
	}

    void TurnOnFloatingObjects() {
        foreach (EdgeCollider2D obj in _floatingObjects) {
            obj.enabled = true;
        }
    }

    void TurnOffFloatingObjects() {
        foreach (EdgeCollider2D obj in _floatingObjects) {
            obj.enabled = false;
        }
    }
}
