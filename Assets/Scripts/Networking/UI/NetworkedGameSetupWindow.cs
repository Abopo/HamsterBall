using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedGameSetupWindow : Photon.MonoBehaviour {
    GameSettings _gameSettings;

    private void Awake() {
        _gameSettings = FindObjectOfType<GameManager>().gameSettings;
    }

    // Use this for initialization
    void Start () {
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            int hSpawnMax = _gameSettings.HamsterSpawnMax;
            bool raindbow = _gameSettings.specialHamstersMultiplayer[0];
            bool dead = _gameSettings.specialHamstersMultiplayer[1];
            bool bomb = _gameSettings.specialHamstersMultiplayer[2];
            bool gravity = _gameSettings.specialHamstersMultiplayer[3];

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

            _gameSettings.HamsterSpawnMax = hSpawnMax;
            _gameSettings.specialHamstersMultiplayer[0] = rainbow;
            _gameSettings.specialHamstersMultiplayer[1] = dead;
            _gameSettings.specialHamstersMultiplayer[2] = bomb;
            _gameSettings.specialHamstersMultiplayer[3] = gravity;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
