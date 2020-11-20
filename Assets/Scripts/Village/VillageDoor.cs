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

    InteractIcon _interactIcon;

    VillagePlayerSpawn _villagePlayerSpawn;
    GameManager _gameManager;

    bool _isActive = true;

    private void Awake() {
        _playerInput = ReInput.players.GetPlayer(0);

        _interactIcon = transform.Find("Interact Icon").GetComponent<InteractIcon>();

        _villagePlayerSpawn = FindObjectOfType<VillagePlayerSpawn>();
        _gameManager = GameManager.instance;
    }
    // Use this for initialization
    protected virtual void Start () {
        _isPlayerHere = false;

        if(_sceneToLoad == "ShopMenu" || _sceneToLoad == "OptionsMenu") {
            if(_gameManager.demoMode) {
                // Deactivate this door
                _isActive = false;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_isPlayerHere && !_gameManager.isPaused) {
            if (_playerInput.GetButtonDown("Interact")) {
                
                EnterDoor();
            }
        }
	}

    protected virtual void EnterDoor() {
        if(_sceneToLoad == "") {
            return;
        }

        SoundManager.mainAudio.Door();

        // Load the proper scene
        _gameManager.sceneTransition.StartTransition(_sceneToLoad);
        //SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Additive);

        // Set the player's respawn point to this door
        _villagePlayerSpawn.SetSpawnPosition(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player" && _isActive) {
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
