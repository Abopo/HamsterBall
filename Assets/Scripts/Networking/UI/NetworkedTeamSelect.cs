using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedTeamSelect : MonoBehaviour {
    public GameObject gameSetupText;

    TeamSelect _teamSelect;

	// Use this for initialization
	void Start () {
        _teamSelect = GetComponent<TeamSelect>();

        AssignCharacterOwnership();
	}

    void AssignCharacterOwnership() {
        // TODO: only do this for players with associated characters (maybe one joined late and shouldn't have one?)
        for (int i = 0; i < PhotonNetwork.playerList.Length; ++i) {
            _teamSelect.characters[i].PhotonView.TransferOwnership(PhotonNetwork.playerList[i]);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void RemoveNetworkedCharacter(int Id) {

    }

    [PunRPC]
    void GameSetup(bool doingIt) {
        _teamSelect.TurnOffCharacters();

        if (doingIt) {
            gameSetupText.SetActive(true);
        } else {
            gameSetupText.SetActive(false);
        }
    }
}
