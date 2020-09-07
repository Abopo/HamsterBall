using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelect : Photon.MonoBehaviour {
    public Animator[] charaAnimators;
    public GameObject[] readySprites;

    public SuperTextMesh gameSetupText;

    int _playersReady = 0;
    bool _tryingGameSetup;

    CharacterSelect _characterSelect;

    private void Awake() {
        FindObjectOfType<GameManager>().gameMode = GAME_MODE.MP_VERSUS;
    }
    // Use this for initialization
    void Start () {
        _characterSelect = GetComponent<CharacterSelect>();

        InitializeSelector(PhotonNetwork.player.ID);
        //InitializeSelectors();
        //StartCoroutine(TryInitializeSelectors());

        gameSetupText.gameObject.SetActive(false);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager.isPaused) {
            gameManager.Unpause();
        }
    }

    IEnumerator TryInitializeSelectors() {
        // Find all the selectors
        CharacterSelector[] charaSelectors = FindObjectsOfType<CharacterSelector>();

        // Wait until the number of selectors matches the number of players
        while(charaSelectors.Length != PhotonNetwork.playerList.Length) {
            charaSelectors = FindObjectsOfType<CharacterSelector>();
            yield return null;
        }

        // Once all the selectors have been created by the server
        // Initialize them
        //InitializeSelectors();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitializeSelectors() {
        // Find all the selectors
        CharacterSelector[] charaSelectors = FindObjectsOfType<CharacterSelector>();

        // Initialize them in order of owner
        // For each player in the room
        for (int i = 0; i < PhotonNetwork.playerList.Length; ++i) {
            // Find the matching selector
            for(int j = 0; j < charaSelectors.Length; ++j) {
                if(charaSelectors[j].playerNum == PhotonNetwork.playerList[i].ID) {

                    charaSelectors[j].NetworkInitialize();
                    charaSelectors[j].Activate(false, true);

                }
            }
        }
    }

    void InitializeSelector(int playerID) {
        Debug.Log("Initialize player " + playerID);

        // Figure out what player we are based on everyone's player numbers
        int playerNum = FindPlayerNumber();

        // Find the next selector
        CharacterSelectResources charaResources = FindObjectOfType<CharacterSelectResources>();
        CharacterSelector cs = charaResources.charaSelectors[playerNum];

        if (!cs.isActive) {
            // Take ownership of the selector and the associated player
            cs.GetComponent<PhotonView>().TransferOwnership(playerID);
            cs.charaWindow.PlayerController.PhotonView.TransferOwnership(playerID);

            // Send out owner change to other players
            cs.GetComponent<NetworkedCharacterSelector>().photonView.RPC("OwnerChanged", PhotonTargets.Others, playerID);

            // Activate it
            cs.Activate(false, true);
        }
    }

    int FindPlayerNumber() {
        int playerNum = 0;

        // We will determine our player number based on our network id

        // Get and order the players in ascending id order
        PhotonPlayer[] allPlayers = PhotonNetwork.playerList;
        int n = allPlayers.Length;
        for (int i = 0; i < n - 1; i++) {
            for (int j = 0; j < n - i - 1; j++) {
                if (allPlayers[j].ID > allPlayers[j + 1].ID) {
                    // swap temp and arr[i] 
                    PhotonPlayer temp = allPlayers[j];
                    allPlayers[j] = allPlayers[j + 1];
                    allPlayers[j + 1] = temp;
                }
            }
        }   

        // Then go through until we find our id, marking up our num as we go
        foreach (PhotonPlayer pPlayer in allPlayers) {
            if (pPlayer.ID == PhotonNetwork.player.ID) {
                break;
            }

            playerNum++;
        }

        return playerNum;
    }

    public void OnPhotonPlayerConnected(PhotonPlayer otherPlayer) {
        // Wait for their selector to spawn, then initialize it
        //StartCoroutine(TryInitializeSelectors());
        //InitializeSelector(otherPlayer.ID);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    public void RemoveNetworkedCharacter(int ownerID) {

    }

    public void StartTryGameSetup() {
        if (!_tryingGameSetup) {
            _playersReady = 0;

            photonView.RPC("TryGameSetup", PhotonTargets.All);

            _tryingGameSetup = true;
        }
    }

    [PunRPC]
    void TryGameSetup() {
        // Master client wants to start setting up the game

        // Make sure everyone is on a team
        if(_characterSelect.AllPlayersOnBothTeams()) {
            // If we're all good, stop player movement
            StopPlayerMovement();

            // Send response that alls good
            photonView.RPC("SetupOK", PhotonTargets.MasterClient, true);
        } else {
            // Someone moved off the platform, so we are not ok to set up
            photonView.RPC("SetupOK", PhotonTargets.MasterClient, false);
        }
    }

    [PunRPC]
    void SetupOK(bool ok) {
        if (ok) {
            _playersReady++;

            // If everyone responded that they're ready
            if(_playersReady >= PhotonNetwork.playerList.Length) {
                // Start game setup
                _characterSelect.OpenSetupMenu();

                // Tell everyone else we're setting up
                photonView.RPC("GameSetupStart", PhotonTargets.Others);

                _tryingGameSetup = false;
            }
        } else {
            // Somebody failed to ready, cancel
            photonView.RPC("GameSetupCancel", PhotonTargets.Others);

            _tryingGameSetup = false;
        }
    }

    [PunRPC]
    public void GameSetupStart() {
        // Master client has started setting up the game, so freeze player and show some text
        StopPlayerMovement();

        _characterSelect.Deactivate();

        gameSetupText.gameObject.SetActive(true);
    }

    [PunRPC]
    public void GameSetupCancel() {
        // Master client backed out of game setup
        ResumePlayerMovement();

        _characterSelect.Activate();

        gameSetupText.gameObject.SetActive(false);
    }

    void StopPlayerMovement() {
        CSPlayerController[] allPlayers = FindObjectsOfType<CSPlayerController>();
        foreach (CSPlayerController cspc in allPlayers) {
            cspc.LoseControl();
            //cspc.underControl = false;
        }

        _characterSelect.noControl = true;
    }

    void ResumePlayerMovement() {
        CSPlayerController[] allPlayers = FindObjectsOfType<CSPlayerController>();
        foreach (CSPlayerController cspc in allPlayers) {
            if (cspc.inPlayArea) {
                cspc.underControl = true;
            }
        }

        _characterSelect.noControl = false;
    }
}
