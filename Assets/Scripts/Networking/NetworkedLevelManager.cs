using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class NetworkedLevelManager : Photon.MonoBehaviour {
    int _masterClientID;

	// Use this for initialization
	void Start () {
        FindMasterClient();
	}

    void FindMasterClient() {
        foreach(PhotonPlayer pp in PhotonNetwork.playerList) {
            if(pp.IsMasterClient) {
                _masterClientID = pp.ID;
                Debug.Log("Master Client ID: " + _masterClientID);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // TODO: put this function in a place that makes more sense
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        Debug.Log("Player disconnected, ID: " + otherPlayer.ID);

        // If the disconnected player was the master client
        if (otherPlayer.ID == _masterClientID) {
            // TODO: throw up a message saying the host disconnected

            // Leave the room
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("OnlineLobby");
        }
    }
}
