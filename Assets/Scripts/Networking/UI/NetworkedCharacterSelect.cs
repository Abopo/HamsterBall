using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelect :Photon.MonoBehaviour {
    public Animator[] charaAnimators;
    public GameObject[] readySprites;

    NewCharacterSelect _characterSelect;

	// Use this for initialization
	void Start () {
        _characterSelect = GetComponent<NewCharacterSelect>();

        CreateNetworkedCharacterSelector();

        //InitializeSelectors();
        StartCoroutine(TryInitializeSelectors());
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
        InitializeSelectors();
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
                if(charaSelectors[j].ownerId == PhotonNetwork.playerList[i].ID) {

                    // If it hasn't been initialize yet
                    if (charaSelectors[j].playerNum == -1 || charaSelectors[j].characterAnimator == null || charaSelectors[j].readySprite == null) {
                        // Initialize it
                        charaSelectors[j].Initialize();
                       
                        // If we own this selector
                        if (charaSelectors[j].ownerId == PhotonNetwork.player.ID) {
                            // Activate it with a controller
                            charaSelectors[j].Activate(InputState.AssignController(), false, true);
                        } else {
                            // Activate it with no controller
                            charaSelectors[j].Activate(-1, false, false);
                        }

                        // Add it to the character select
                        _characterSelect.AddSelector(charaSelectors[j]);
                    }
                }
            }
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer otherPlayer) {
        // Wait for their selector to spawn, then initialize it
        StartCoroutine(TryInitializeSelectors());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    // Networking
    public void CreateNetworkedCharacterSelector() {
        // Make new selector
        GameObject selectorObj = Resources.Load<GameObject>("Prefabs/UI/Character Select/NetworkedCharacterSelector");
        CharacterSelector newSelector = PhotonNetwork.Instantiate("Prefabs/UI/Character Select/NetworkedCharacterSelector", transform.position, Quaternion.identity, 0).GetComponent<CharacterSelector>();
        //newSelector.Initialize();
        /*
        // TODO: Move selector to emtpy character?
        CharacterIcon[] charaIcons = FindObjectsOfType<CharacterIcon>();
        newSelector.transform.position = new Vector3(charaIcons[_characterSelect.NumPlayers].transform.position.x,
                                                    charaIcons[_characterSelect.NumPlayers].transform.position.y,
                                                    charaIcons[_characterSelect.NumPlayers].transform.position.z - 2f);
        newSelector.curCharacterIcon = charaIcons[_characterSelect.NumPlayers];
        newSelector.characterAnimator = charaAnimators[_characterSelect.NumPlayers];
        newSelector.readySprite = readySprites[_characterSelect.NumPlayers];
        */

    }

    public void RemoveNetworkedCharacter(int controllerNum, int ownerID) {

    }
}
