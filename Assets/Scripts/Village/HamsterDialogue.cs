using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

// This controls the text box and dialogue for the villag hamsters
public class HamsterDialogue : MonoBehaviour {
    public string dialogue;

    bool _isPlayerHere;

    GameObject _dialogueCanvas;
    TextWriter _textWriter;

    InteractIcon _interactIcon;

    Player _playerInput;
    GameManager _gameManager;

    private void Awake() {
        _dialogueCanvas = transform.GetChild(0).gameObject;
        _textWriter = GetComponent<TextWriter>();

        _interactIcon = GetComponentInChildren<InteractIcon>();

        _playerInput = ReInput.players.GetPlayer(0);
        _gameManager = FindObjectOfType<GameManager>();
    }
    // Use this for initialization
    void Start () {
        _isPlayerHere = false;

        // Hide the dialogue box just in case
        HideDialogue();
    }

    // Update is called once per frame
    void Update () {
        if (_isPlayerHere && !_gameManager.isPaused) {
            if (_playerInput.GetButtonDown("Interact")) {
                if(!_dialogueCanvas.activeSelf) {
                    DisplayDialogue();
                } else {
                    HideDialogue();
                }
            }
        }
    }

    void DisplayDialogue() {
        if (dialogue != "") {
            _dialogueCanvas.SetActive(true);
            _textWriter.StartWriting(dialogue);
            _interactIcon.Deactivate();
        }
    }

    void HideDialogue() {
        _dialogueCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = true;
            _interactIcon.Activate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = false;
            _interactIcon.Deactivate();
            HideDialogue();
        }
    }
}
