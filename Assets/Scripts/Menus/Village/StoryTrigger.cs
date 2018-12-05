using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryTrigger : MonoBehaviour {

    VillagePlayerSpawn _villagePlayerSpawn;

    // Use this for initialization
    void Start () {
        _villagePlayerSpawn = FindObjectOfType<VillagePlayerSpawn>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            SceneManager.LoadScene("StorySelect");

            // Set the player's respawn point to this door
            _villagePlayerSpawn.SetSpawnPosition(transform.position);
        }
    }
}
