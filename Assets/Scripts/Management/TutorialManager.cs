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

        _tutorialTime = 0.5f;
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
        } else if (_tutorialIndex == 7 && _playerController.transform.position.x < 0 && _playerController.curState != PLAYER_STATE.SHIFT) {
            ShowNextTutorial();
        }
    }

    void ShowNextTutorial() {
        switch(_tutorialIndex) {
            case 0:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/MovementTutorial"));
                _tutorialIndex++; // 1
                _tutorialTime = 5f;
                break;
            case 1:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/BubbleTutorial"));
                _tutorialIndex++; // 2
                _tutorialTime = -1;
                break;
            case 2:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/AimTutorial"));
                _tutorialIndex++; // 3
                _tutorialTime = -1;
                break;
            case 3:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/ThrowTutorial"));
                _tutorialIndex++; // 4
                _tutorialTime = -1;
                break;
            case 4:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/MatchingTutorial"));
                _tutorialIndex++; // 5
                _tutorialTime = 15f;
                break;
            case 5:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/DroppingSwappingTutorial"));
                _tutorialIndex++; // 6
                _tutorialTime = -1;
                break;
            case 6:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/SwappedTutorial"));
                _tutorialIndex++; // 7
                _tutorialTime = -1;
                break;
            case 7:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/EndTutorial"));
                _tutorialIndex++; // 8
                _tutorialTime = 0.1f;
                break;
            case 8:
                // End level
                GameObject.FindObjectOfType<BubbleManager>().EndGame(1);
                break;
        }
    }

    string GetCorrectTutorial(string baseTutorial) {
        if(Input.GetJoystickNames().Length > 0) {
            baseTutorial += "Controller";
        }

        return baseTutorial;
    }
}
