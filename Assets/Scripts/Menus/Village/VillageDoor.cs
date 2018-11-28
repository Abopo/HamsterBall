using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class VillageDoor : MonoBehaviour {
    public string _sceneToLoad;

    bool _isPlayerHere;

    Player _playerInput;

	// Use this for initialization
	void Start () {
        _isPlayerHere = false;

        _playerInput = ReInput.players.GetPlayer(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (_isPlayerHere) {
            if (_playerInput.GetButtonDown("Up") && _sceneToLoad != "") {
                // Load the proper scene
                SceneManager.LoadScene(_sceneToLoad);
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            _isPlayerHere = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = false;
        }
    }
}
