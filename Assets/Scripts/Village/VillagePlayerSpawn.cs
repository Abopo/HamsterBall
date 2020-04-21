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
        newPlayer.SetInputID(0);
        newPlayer.team = 0;
        newPlayer.transform.position = transform.position;
        newPlayer.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

        CharaInfo charaInfo = new CharaInfo();
        charaInfo.name = (CHARACTERS)ES3.Load<int>("Player1Character", 0);
        charaInfo.color = ES3.Load<int>("Player1Color", 0);
        newPlayer.SetCharacterInfo(charaInfo);
        
        newPlayer.aimAssist = false;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SetSpawnPosition(Vector3 pos) {
        _spawnPosition = new Vector3(pos.x, pos.y+0.15f, transform.position.z);
    }
}
