using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelect : Photon.MonoBehaviour {
    public Animator[] charaAnimators;
    public GameObject[] readySprites;

    public SuperTextMesh gameSetupText;

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
        Debug.Log("Initialize Selectors");

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

                    /*
                    // If it hasn't been initialize yet
                    if (charaSelectors[j].playerNum == -1) {
                        Debug.Log("Initizalizing selector");

                        // Initialize it
                        charaSelectors[j].NetworkInitialize();
                       
                        // If we own this selector
                        if (charaSelectors[j].ownerId == PhotonNetwork.player.ID) {
                            // Activate it with a controller
                            charaSelectors[j].Activate(false, true);
                        } else {
                            // Activate it with no controller
                            charaSelectors[j].Activate(false, false);
                        }

                        // Add it to the character select
                        _characterSelect.AddSelector(charaSelectors[j]);
                    }
                    */
                }
            }
        }
    }

    void InitializeSelector(int playerID) {
        Debug.Log("Initialize player " + playerID);

        // Find all the selectors
        CharacterSelector[] charaSelectors = FindObjectsOfType<CharacterSelector>();
    
        foreach(CharacterSelector cs in charaSelectors) {
            if(cs.playerNum == playerID-1) {
                // Take ownership of the selector and the associated player
                cs.GetComponent<PhotonView>().TransferOwnership(playerID);
                cs.charaWindow.PlayerController.PhotonView.TransferOwnership(playerID);

                // Activate it
                cs.Activate(false, true);
            }
        }
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

    [PunRPC]
    public void GameSetupStart() {
        // Master client has started setting up the game, so freeze player and show some text
        CSPlayerController[] allPlayers = FindObjectsOfType<CSPlayerController>();
        foreach(CSPlayerController cspc in allPlayers) {
            cspc.underControl = false;
        }

        _characterSelect.noControl = true;

        gameSetupText.gameObject.SetActive(true);
    }

    [PunRPC]
    public void GameSetupCancel() {
        // Master client backed out of game setup
        CSPlayerController[] allPlayers = FindObjectsOfType<CSPlayerController>();
        foreach (CSPlayerController cspc in allPlayers) {
            cspc.underControl = true;
        }

        _characterSelect.noControl = false;

        gameSetupText.gameObject.SetActive(false);
    }
}
