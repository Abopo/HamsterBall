using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {

    public Text winningTeamText;
    public MenuOption[] menuOptions;

    float winTime = 1.0f;
    float winTimer = 0.0f;

    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        // Game is paused here, so just use a fake delta time
        winTimer += 0.03f;
        if(winTimer > winTime) {
            foreach (MenuOption mo in menuOptions) {
                mo.isReady = true;
            }
        }
    }

    public void SetWinningTeamText(int lostTeam) {
        if(lostTeam == 1) {
            winningTeamText.text = "Left Team Wins";
        } else if(lostTeam == 0) {
            winningTeamText.text = "Right Team Wins";
        } else {
            winningTeamText.text = "What happened? No team was given.";
        }
    }

    public void Activate(int team) {
        gameObject.SetActive(true);
        SetWinningTeamText(team);
        menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in menuOptions) {
            mo.isReady = false;
        }
    }

    public void Activate() {
        gameObject.SetActive(true);
        winningTeamText.text = "You did it!";
        menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in menuOptions) {
            mo.isReady = false;
        }
    }
}
