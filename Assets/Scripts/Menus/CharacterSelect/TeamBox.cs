using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBox : MonoBehaviour {
    public int team; // -1 = no team, 0 = left team, 1 = right team
    public int numPlayers;

    bool _player1;
    bool _player2;

    GameMarker[] playerMarkers;

    PlayerManager _playerManager;
    CharacterSelect _characterSelect;

	// Use this for initialization
	void Start () {
        playerMarkers = GetComponentsInChildren<GameMarker>();

        _playerManager = FindObjectOfType<PlayerManager>();
        _characterSelect = FindObjectOfType<CharacterSelect>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            // Add player to the team
            CSPlayerController player = collision.gameObject.GetComponent<CSPlayerController>();
            TakePlayer(player);
            // if this is the first player (or an AI) and they got on the team
            if((player.playerNum == 0 || player.characterSelector.isAI) && player.team != -1) {
                // If there's an ai player available
                if (player.characterSelector.NextAI != null) {
                    // Show it's com text
                    player.characterSelector.NextAI.ShowCOMText();
                }
            }
        }
    }
    */
    private void OnTriggerStay2D(Collider2D collision) {
        if(_player1 == false || _player2 == false) {
            if(collision.tag == "Player") {
                CSPlayerController player = collision.gameObject.GetComponent<CSPlayerController>();
                if(player.team == -1) {
                    TakePlayer(player);

                    // if this is the first player (or an AI) and they got on the team
                    if ((player.playerNum == 0 || player.characterSelector.isAI) && player.team != -1) {
                        // If there's an ai player available
                        if (player.characterSelector.NextAI != null) {
                            // Show it's com text
                            player.characterSelector.NextAI.ShowCOMText();
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            CSPlayerController player = collision.gameObject.GetComponent<CSPlayerController>();

            // If the leaving player was assigned to this team
            if (player.team == team) {
                LosePlayer(player);

                // if this is the first player (or an AI) and they got off the team
                if ((player.playerNum == 0 || player.characterSelector.isAI) && player.team == -1) {
                    // If there's an ai player available
                    if (player.characterSelector.NextAI != null) {
                        // Show it's com text
                        player.characterSelector.NextAI.HideCOMText();
                    }
                }
            }
        }
    }


    void TakePlayer(PlayerController player) {
        // Set first open character to input character
        if (_player1 == false) {
            _player1 = true;
            player.team = team;
            numPlayers++;
            playerMarkers[0].FillIn();
        } else if (_player2 == false) {
            _player2 = true;
            player.team = team;
            numPlayers++;
            playerMarkers[1].FillIn();
        }
    }

    void LosePlayer(PlayerController player) {
        if (_player2 == true) {
            _player2 = false;
            player.team = -1;
            numPlayers--;
            playerMarkers[1].FillOut();
        } else if (_player1 == true) {
            _player1 = false;
            player.team = -1;
            numPlayers--;
            playerMarkers[0].FillOut();
        }
    }
}
