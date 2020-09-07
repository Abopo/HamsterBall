using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonInspection : MonoBehaviour {

    public string roomName;
    public int playersInRoom;
    public int maxPlayers;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(PhotonNetwork.connectedAndReady) {
            if (PhotonNetwork.room != null) {
                roomName = PhotonNetwork.room.Name;
                playersInRoom = PhotonNetwork.room.PlayerCount;
                maxPlayers = PhotonNetwork.room.MaxPlayers;
            }
        }
    }
}
