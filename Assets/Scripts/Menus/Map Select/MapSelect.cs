using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapSelect : MonoBehaviour {
    public MapIcon[] _mapIcons = new MapIcon[3];
    public Button selectButton;

    GameManager _gameManager;
    AudioSource _audioSource;

    bool _justMoved;

    // Networking
    int _masterClientID;

    // Use this for initialization
    void Start() {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();

        FindMasterClient();
    }

    void FindMasterClient() {
        foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
            if (pp.IsMasterClient) {
                _masterClientID = pp.ID;
                Debug.Log("Master Client ID: " + _masterClientID);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || 
            (Input.GetAxis("Horizontal") < -0.3f && !_justMoved)) {
            MoveMapsRight();
            _audioSource.Play();
            _justMoved = true;
        } else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || 
            (Input.GetAxis("Horizontal") > 0.3f && !_justMoved)) {
            MoveMapsLeft();
            _audioSource.Play();
            _justMoved = true;
        }
        if (Input.GetAxis("Horizontal") < 0.3f && Input.GetAxis("Horizontal") > -0.3f) {
            _justMoved = false;
        }

        if (Input.GetButtonDown("Cancel")) {
            _audioSource.Play();
            LoadCharacterSelect();
        }
    }

    public void MoveMapsLeft() {
        for(int i = 0; i < 3; ++i) {
            _mapIcons[i].Move(true);
        }
    }

    public void MoveMapsRight() {
        for (int i = 0; i < 3; ++i) {
            _mapIcons[i].Move(false);
        }
    }

    public void LoadSelectedMap() {
        for (int i = 0; i < 3; ++i) {
            if (_mapIcons[i].index == 1) {
                _gameManager.stage = "";
                string levelName = "";
                if(_gameManager.isOnline) {
                    levelName = "Networked ";
                }
                levelName += _mapIcons[i].mapName;

                if(_gameManager.isSinglePlayer) {
                    levelName += " - SinglePlayer";
                }

                if (_gameManager.isOnline && PhotonNetwork.connectedAndReady) {
                    PhotonNetwork.LoadLevel(levelName);
                } else {
                    SceneManager.LoadScene(levelName);
                }
            }
        }
    }

    public void LoadCharacterSelect() {
        SceneManager.LoadScene("CharacterSelect");
    }

    public bool IsRotating() {
        if(_mapIcons[0].Moving || _mapIcons[1].Moving || _mapIcons[2].Moving) {
            return true;
        } else {
            return false;
        }
    }

    // Networking 
    public void LoadNetworkedCharacterSelect() {
        PhotonNetwork.LoadLevel("NetworkedCharacterSelect");
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
        } else if(PhotonNetwork.playerList.Length < 2) {
            // TODO: this will cause issues if there's a team of 2 and the other team disconnects

            LoadNetworkedCharacterSelect();
        }
    }
}
