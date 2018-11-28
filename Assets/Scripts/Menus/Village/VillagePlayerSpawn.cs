using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagePlayerSpawn : MonoBehaviour {
    public GameObject playerObj;

    Vector3 _spawnPosition;

    // Use this for initialization
    void Start () {
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
}
