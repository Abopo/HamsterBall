using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedGameSetupWindow : Photon.MonoBehaviour {
    GameManager _gameManager;

    private void Awake() {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Use this for initialization
    void Start () {
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            int hSpawnMax = _gameManager.HamsterSpawnMax;
            bool raindbow = HamsterSpawner.canBeRainbow;
            bool dead = HamsterSpawner.canBeDead;
            bool gravity = HamsterSpawner.canBeGravity;
            bool bomb = HamsterSpawner.canBeBomb;

            stream.Serialize(ref _gameManager.leftTeamHandicap);
            stream.Serialize(ref _gameManager.rightTeamHandicap);
            stream.Serialize(ref hSpawnMax);
            stream.Serialize(ref raindbow);
            stream.Serialize(ref dead);
            stream.Serialize(ref gravity);
            stream.Serialize(ref bomb);
        } else {
            int lHandi = 0;
            int rHandi = 0;
            int hSpawnMax = 0;
            bool rainbow = false;
            bool dead = false;
            bool gravity = false;
            bool bomb = false;

            stream.Serialize(ref lHandi);
            stream.Serialize(ref rHandi);
            stream.Serialize(ref hSpawnMax);
            stream.Serialize(ref rainbow);
            stream.Serialize(ref dead);
            stream.Serialize(ref gravity);
            stream.Serialize(ref bomb);

            _gameManager.SetTeamHandicap(0, lHandi);
            _gameManager.SetTeamHandicap(1, rHandi);
            _gameManager.HamsterSpawnMax = hSpawnMax;
            HamsterSpawner.canBeRainbow = rainbow;
            HamsterSpawner.canBeDead = dead;
            HamsterSpawner.canBeGravity = gravity;
            HamsterSpawner.canBeBomb = bomb;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
