using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlineLobby : MonoBehaviour {

    public string roomName;
    public Transform scrollViewContent;
    public GameObject roomInfoObj;
    public GameObject nameWarningObj;
    public SuperTextMesh roomWarningObj;

    public InputField playerNameInput;

    List<GameObject> rooms = new List<GameObject>();

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        // If we're not connected to photon yet
        if (!PhotonNetwork.connectedAndReady) {
            PhotonNetwork.ConnectUsingSettings("0.1");
            PhotonNetwork.networkingPeer.DisconnectTimeout = 900000000;

        // If we are still in a room
        } else if (PhotonNetwork.room != null) {
            PhotonNetwork.LeaveRoom();
        }

        roomName = "myRoom";

        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.isOnline = true;
        _gameManager.isSinglePlayer = false;

        // Start the player out on the name input field
        playerNameInput.Select();
	}

    void ShowRooms() {
        int i = 0;
        foreach(RoomInfo roomInfo in PhotonNetwork.GetRoomList()) {
            GameObject roomInfoUI = GameObject.Instantiate(roomInfoObj, scrollViewContent);
            roomInfoUI.transform.localPosition = new Vector3(193.6f, -21f - (40f * i));
            SuperTextMesh[] roomText = roomInfoUI.GetComponentsInChildren<SuperTextMesh>();
            roomText[0].text = roomInfo.Name;
            roomText[1].text = roomInfo.PlayerCount.ToString() + "/4";
            roomInfoUI.GetComponentInChildren<JoinRoomButton>().onlineLobby = this;
            rooms.Add(roomInfoUI);
            ++i;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SetPlayerName(string name) {
        PhotonNetwork.playerName = name;
        ToggleNameWarning(false);
    }

    public void SetRoomName(string name) {
        roomName = name;
        ToggleRoomWarning(false);
    }

    public void CreateRoom() {
        if (PhotonNetwork.connectedAndReady) {
            if (PhotonNetwork.playerName != "") {
                if (roomName != "" && IsRoomAvailable()) {
                    PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
                } else {
                    ToggleRoomWarning(true);
                }
            } else {
                ToggleNameWarning(true);
            }
        }
    }

    public void JoinRandomRoom() {
        if (PhotonNetwork.connectedAndReady) {
            if (PhotonNetwork.playerName != "") {
                PhotonNetwork.JoinRandomRoom();
            } else {
                ToggleNameWarning(true);
            }
        }
    }

    public void JoinRoom(string rName) {
        if (PhotonNetwork.playerName != "") {
            PhotonNetwork.JoinRoom(rName);
        } else {
            ToggleNameWarning(true);
        }
    }

    public void Quit() {
        PhotonNetwork.Disconnect();
        FindObjectOfType<GameManager>().VillageButton();
    }

    bool IsRoomAvailable() {
        bool roomAvailable = true;

        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach(RoomInfo r in rooms) {
            if(r.Name == roomName) {
                roomAvailable = false;
            }
        }

        return roomAvailable;
    }

    public void OnCreatedRoom() {
        PhotonNetwork.LoadLevel("NetworkedCharacterSelect"); 
    }

    public void OnReceivedRoomListUpdate() {
        foreach(GameObject gO in rooms) {
            Destroy(gO);
        }
        rooms.Clear();

        ShowRooms();
    }

    public void ToggleNameWarning(bool on) {
        nameWarningObj.SetActive(on);
    }

    public void ToggleRoomWarning(bool on) {
        if (on) {
            if (roomName == "") {
                roomWarningObj.text = "Please enter a room name";
            } else {
                roomWarningObj.text = "Room name already taken";
            }
        }

        roomWarningObj.gameObject.SetActive(on);
    }

    private void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}
