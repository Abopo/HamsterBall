using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryPlayerInfo : MonoBehaviour {

    PlayerInfoBox[] _playerInfoBoxes;

    public static StoryPlayerInfo instance;

    private void Awake() {
        // We only want one of these to exist
        SingletonCheck();

        _playerInfoBoxes = GetComponentsInChildren<PlayerInfoBox>();

        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    // Use this for initialization
    void Start () {
        // Hold on to the info between story stages
        DontDestroyOnLoad(gameObject);
	}

    void SingletonCheck() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode) {
        // If we've loaded in story select
        if (scene.buildIndex == 2) {
            // Turn on the info boxes
            foreach (PlayerInfoBox pib in _playerInfoBoxes) {
                pib.gameObject.SetActive(true);
            }
        } else {
            // Turn off the info boxes
            foreach (PlayerInfoBox pib in _playerInfoBoxes) {
                pib.gameObject.SetActive(false);
            }
        }
    }

    public void TurnOnInput() {
        foreach (PlayerInfoBox pib in _playerInfoBoxes) {
            pib.enabled = true;
        }
    }
    public void TurnOffInput() {
        foreach (PlayerInfoBox pib in _playerInfoBoxes) {
            pib.enabled = false;
        }
    }

    public void LoadPlayers() {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        playerManager.ClearAllPlayers();

        foreach (PlayerInfoBox pib in _playerInfoBoxes) {
            pib.LoadCharacter();
        }
    }
}
