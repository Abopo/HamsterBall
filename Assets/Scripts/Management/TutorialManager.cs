using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TutorialManager : MonoBehaviour {
    public CutsceneManager cutsceneManager;
    public SuperTextMesh tutorialInfoText;

    int _tutorialIndex = 0; // keeps track of what tutorial should be shown next.
    float _tutorialTime;
    float _tutorialTimer = 0;

    bool _tutorialFinished = false;

    bool _controller = false;

    PlayerController _playerController;
    PlayerController _aiController;

    GameManager _gameManager;
    LevelManager _levelManager;

    private void Awake() {
        _gameManager = GameManager.instance;
        _levelManager = FindObjectOfType<LevelManager>();
    }
    // Use this for initialization
    void Start () {
        GetPlayer();

        cutsceneManager.cutsceneEnd.AddListener(CutsceneEnded);

        if (_aiController != null) {
            // Turn off ai for now
            _aiController.GetComponent<AIController>().enabled = false;
        }

        tutorialInfoText.transform.parent.gameObject.SetActive(false);

        _tutorialTime = 0.5f;

        if(ReInput.controllers.joystickCount > 0) {
            _controller = true;
        }
	}

    void GetPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players) {
            if (!p.GetComponent<PlayerController>().aiControlled) {
                _playerController = p.GetComponent<PlayerController>();
            } else {
                _aiController = p.GetComponent<PlayerController>();
            }
        }

        if(_playerController != null) {
            // Lock player actions until unlocked
            _playerController.LockState(PLAYER_STATE.CATCH);
            _playerController.LockState(PLAYER_STATE.THROW);
            _playerController.LockState(PLAYER_STATE.SHIFT);
        }
    }

    // Update is called once per frame
    void Update () {
        if(_tutorialFinished) {
            return;
        }

        if (_tutorialTime != -1) {
            _tutorialTimer += Time.deltaTime;
            if (_tutorialTimer >= _tutorialTime) {
                ShowNextTutorial();
                _tutorialTimer = 0f;
            }
        }

        if(_playerController == null) {
            GetPlayer();
        }

        if(_tutorialIndex == 1 && _playerController.CurState == PLAYER_STATE.IDLE && _playerController.transform.position.y > -2.6f) {
            _tutorialTime = 1f;
        } else if (_tutorialIndex == 2 && _playerController.CurState == PLAYER_STATE.IDLE && _playerController.heldBall != null) {
            ShowNextTutorial();
        } else if (_tutorialIndex == 3 && _playerController.CurState == PLAYER_STATE.THROW) {
            _tutorialTime = 0.2f;
        } else if (_tutorialIndex == 4 && _playerController.CurState != PLAYER_STATE.THROW && _playerController.heldBall == null) {
            ShowNextTutorial();
        } else if (_tutorialIndex == 6 && _playerController.transform.position.x > 0 && _playerController.CurState != PLAYER_STATE.SHIFT) {
            ShowNextTutorial();
        } else if (_tutorialIndex == 7 && _playerController.transform.position.x < 0 && _playerController.CurState != PLAYER_STATE.SHIFT) {
            ShowNextTutorial();
        }
    }

    void ShowNextTutorial() {
        switch(_tutorialIndex) {
            case 0:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/MovementTutorial"));
                if (_controller) {
                    tutorialInfoText.text = "Move with Left Joystick, Jump with <c=green>A<c=white>. Get up high!";
                } else {
                    tutorialInfoText.text = "Move with <c=green>A/D<c=white>, Jump with <c=green>W<c=white>. Get up high!";
                }
                _tutorialIndex++; // 1
                _tutorialTime = -1;
                break;
            case 1:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/BubbleTutorial"));
                if (_controller) {
                    tutorialInfoText.text = "Catch a hamster with <c=4356FF>X<c=white>!";
                } else {
                    tutorialInfoText.text = "Catch a hamster with <c=4356FF>J<c=white>!";
                }
                _tutorialIndex++; // 2
                _tutorialTime = -1;
                // Unlock the catch state
                _playerController.UnlockState(PLAYER_STATE.CATCH);
                break;
            case 2:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/AimTutorial"));
                if (_controller) {
                    tutorialInfoText.text = "Pick a good spot and start aiming with <c=4356FF>X<c=white>!";
                } else {
                    tutorialInfoText.text = "Pick a good spot and start aiming with <c=4356FF>J<c=white>!";
                }
                _tutorialIndex++; // 3
                _tutorialTime = -1;
                // Unlock the throw state
                _playerController.UnlockState(PLAYER_STATE.THROW);
                break;
            case 3:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/ThrowTutorial"));
                if (_controller) {
                    tutorialInfoText.text = "Aim at matching colored balls, and throw with <c=4356FF>X<c=white>!";
                } else {
                    tutorialInfoText.text = "Aim at matching colored balls, and throw with <c=4356FF>J<c=white>!";
                }
                _tutorialIndex++; // 4
                _tutorialTime = -1;
                break;
            case 4:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/MatchingTutorial"));
                if (_controller) {
                    tutorialInfoText.text = "Try to make some matches! Catch, aim, throw all with <c=4356FF>X<c=white>!";
                } else {
                    tutorialInfoText.text = "Try to make some matches! Catch, aim, throw all with <c=4356FF>J<c=white>!";
                }
                _tutorialIndex++; // 5
                _tutorialTime = 15f;
                break;
            case 5:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/DroppingSwappingTutorial"));
                if (_controller) {
                    tutorialInfoText.text = "Press the <c=yellow>Y<c=white> key to do...something?";
                } else {
                    tutorialInfoText.text = "Press the <c=yellow>L<c=white> key to do...something?";
                }
                _tutorialIndex++; // 6
                _tutorialTime = -1;
                // Re-lock the catch state so player can't win the match
                _playerController.LockState(PLAYER_STATE.CATCH);
                // Unlock the shift state
                _playerController.UnlockState(PLAYER_STATE.SHIFT);
                break;
            case 6:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/SwappedTutorial"));
                _tutorialIndex++; // 7
                _tutorialTime = -1;
                // Unlock the catch state i guess
                _playerController.UnlockState(PLAYER_STATE.CATCH);
                break;
            case 7:
                cutsceneManager.StartCutscene(GetCorrectTutorial("Tutorials/EndTutorial"));
                _tutorialIndex++; // 8
                _tutorialTime = 0.1f;
                break;
            case 8:
                // End level
                EndGame();
                break;
        }

        // Hide the info text during cutscenes
        tutorialInfoText.transform.parent.gameObject.SetActive(false);

        _gameManager.FullPause();
    }

    string GetCorrectTutorial(string baseTutorial) {
        if(ReInput.controllers.joystickCount > 0) {
            baseTutorial += "Controller";
        }

        return baseTutorial;
    }

    void CutsceneEnded() {
        tutorialInfoText.transform.parent.gameObject.SetActive(true);

        _levelManager.GameStart();
    }

    // We kind of have to fake a game end scenario
    void EndGame() {
        _gameManager.Unpause();

        _tutorialFinished = true;

        FindObjectOfType<BubbleManager>().wonGame = true;

        _levelManager.GameEnd();

        // Pretend we are in a single player mode
        _gameManager.isSinglePlayer = true;
        _gameManager.gameMode = GAME_MODE.SP_CLEAR;

        _levelManager.ActivateResultsScreen(0, 1);

        _gameManager.EndGame(0, 100);
    }
}
