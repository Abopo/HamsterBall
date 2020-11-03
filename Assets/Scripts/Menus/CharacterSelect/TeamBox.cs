using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBox : MonoBehaviour {
    public int team; // -1 = no team, 0 = left team, 1 = right team
    public int numPlayers;

    bool _player1;
    bool _player2;

    GameMarker[] playerMarkers;

	// Use this for initialization
	void Start () {
        playerMarkers = GetComponentsInChildren<GameMarker>();

        if(GameManager.instance.gameMode == GAME_MODE.SURVIVAL) {
            // There are no teams for base survival, so turn off the team signs
            GetComponent<SpriteRenderer>().enabled = false;
            playerMarkers[0].GetComponent<SpriteRenderer>().enabled = false;
            playerMarkers[1].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay2D(Collider2D collision) {
        if(_player1 == false || _player2 == false) {
            if(collision.tag == "Player") {
                CSPlayerController player = collision.gameObject.GetComponent<CSPlayerController>();

                if(player.team == -1) {
                    TakePlayer(player);
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
