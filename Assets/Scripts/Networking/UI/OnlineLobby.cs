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

    List<GameObject> rooms = new List<GameObject>();

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        PhotonNetwork.ConnectUsingSettings("0.1");

        roomName = "myRoom";

        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.isOnline = true;
        _gameManager.isSinglePlayer = false;
	}

    void ShowRooms() {
        int i = 0;
        foreach(RoomInfo roomInfo in PhotonNetwork.GetRoomList()) {
            GameObject roomInfoUI = GameObject.Instantiate(roomInfoObj, scrollViewContent);
            roomInfoUI.transform.localPosition = new Vector3(197.3f, -21f - (36.7f * i));
            Text[] roomText = roomInfoUI.GetComponentsInChildren<Text>();
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
    }

    public void CreateRoom() {
        if (PhotonNetwork.connectedAndReady) {
            if (PhotonNetwork.playerName != "") {
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
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
        //SceneManager.LoadScene("MainMenu");
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

    private void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}
