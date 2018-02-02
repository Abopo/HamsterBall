using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    public CutsceneManager cutsceneManager;

    int _tutorialIndex = 0; // keeps track of what tutorial should be shown next.
    float _tutorialTime;
    float _tutorialTimer = 0;

    PlayerController _playerController;
    PlayerController _aiController;

	// Use this for initialization
	void Start () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in players) {
            if(!p.GetComponent<PlayerController>().aiControlled) {
                _playerController = p.GetComponent<PlayerController>();
            } else {
                _aiController = p.GetComponent<PlayerController>();
            }
        }

        // Turn off ai for now
        _aiController.GetComponent<AIController>().enabled = false;

        cutsceneManager.StartCutscene("Tutorials/MovementTutorial");
        _tutorialIndex++; // 1
        _tutorialTime = 5f;
	}
	
	// Update is called once per frame
	void Update () {
        if (_tutorialTime != -1) {
            _tutorialTimer += Time.deltaTime;
            if (_tutorialTimer >= _tutorialTime) {
                ShowNextTutorial();
                _tutorialTimer = 0f;
            }
        }

        if (_tutorialIndex == 2 && _playerController.heldBubble != null) {
            ShowNextTutorial();
        } else if (_tutorialIndex == 3 && _playerController.curState == PLAYER_STATE.THROW) {
            ShowNextTutorial();
        } else if (_tutorialIndex == 4 && _playerController.curState != PLAYER_STATE.THROW) {
            ShowNextTutorial();
        } else if (_tutorialIndex == 6 && _playerController.transform.position.x > 0 && _playerController.curState != PLAYER_STATE.SHIFT) {
            ShowNextTutorial();
        }
	}

    void ShowNextTutorial() {
        switch(_tutorialIndex) {
            case 1:
                cutsceneManager.StartCutscene("Tutorials/BubbleTutorial");
                _tutorialIndex++; // 2
                _tutorialTime = -1;
                break;
            case 2:
                cutsceneManager.StartCutscene("Tutorials/AimTutorial");
                _tutorialIndex++; // 3
                break;
            case 3:
                cutsceneManager.StartCutscene("Tutorials/ThrowTutorial");
                _tutorialIndex++; // 4
                break;
            case 4:
                cutsceneManager.StartCutscene("Tutorials/MatchingTutorial");
                _tutorialIndex++; // 5
                _tutorialTime = 15f;
                break;
            case 5:
                cutsceneManager.StartCutscene("Tutorials/DroppingSwappingTutorial");
                _tutorialIndex++; // 6
                break;
            case 6:
                cutsceneManager.StartCutscene("Tutorials/SwappedTutorial");
                _tutorialIndex++; // 7
                break;
        }
    }
}
