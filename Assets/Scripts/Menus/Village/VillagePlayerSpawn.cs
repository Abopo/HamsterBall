using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagePlayerSpawn : MonoBehaviour {
    public GameObject playerObj;

    static Vector3 _spawnPosition = Vector3.zero;

    // Use this for initialization
    void Start () {
        if(_spawnPosition == Vector3.zero) {
            _spawnPosition = transform.position;
        }

        transform.position = _spawnPosition;

        SpawnPlayer();
	}

    void SpawnPlayer() {
        PlayerController newPlayer;

        newPlayer = Instantiate(playerObj).GetComponentInChildren<PlayerController>();
        newPlayer.SetPlayerNum(0);
        newPlayer.team = 0;
        newPlayer.transform.position = transform.position;
        newPlayer.SetCharacterName(CHARACTERNAMES.BOY1);
        newPlayer.aimAssist = false;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SetSpawnPosition(Vector3 pos) {
        _spawnPosition = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
