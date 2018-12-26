using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class VillageDoor : MonoBehaviour {
    public string _sceneToLoad;

    bool _isPlayerHere;

    Player _playerInput;
    protected PlayerController _playerController;

    VillagePlayerSpawn _villagePlayerSpawn;
    GameManager _gameManager;

	// Use this for initialization
	protected virtual void Start () {
        _isPlayerHere = false;

        _playerInput = ReInput.players.GetPlayer(0);

        _villagePlayerSpawn = FindObjectOfType<VillagePlayerSpawn>();
        _gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_isPlayerHere && !_gameManager.isPaused) {
            if (_playerInput.GetButtonDown("Up")) {
                EnterDoor();
            }
        }
	}

    protected virtual void EnterDoor() {
        if(_sceneToLoad == "") {
            return;
        }

        // Load the proper scene
        SceneManager.LoadScene(_sceneToLoad);

        // Set the player's respawn point to this door
        _villagePlayerSpawn.SetSpawnPosition(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            _isPlayerHere = true;
            _playerController = collision.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = false;
            _playerController = null;
        }
    }
}
