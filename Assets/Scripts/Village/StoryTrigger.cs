using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryTrigger : MonoBehaviour {

    VillagePlayerSpawn _villagePlayerSpawn;
    GameManager _gameManager;

    private void Awake() {
        _villagePlayerSpawn = FindObjectOfType<VillagePlayerSpawn>();
        _gameManager = FindObjectOfType<GameManager>();
    }
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            if (_gameManager.demoMode) {
                SceneManager.LoadScene("StoryMode-Demo");
            } else {
                SceneManager.LoadScene("StorySelect");
            }

            // Set the player's respawn point to this door
            _villagePlayerSpawn.SetSpawnPosition(transform.position);
        }
    }
}
