using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class RandomMatchmaking : Photon.PunBehaviour {

    // Use this for initialization
    void Start() {
        PhotonNetwork.ConnectUsingSettings("0.1");

        // How much photon will log events
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedLobby() {
        PhotonNetwork.JoinRandomRoom();
    }
}
