using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

// This controls the text box and dialogue for the villag hamsters
public class HamsterDialogue : MonoBehaviour {
    public string dialogue;

    bool _isPlayerHere;

    GameObject _dialogueBox;
    TextWriter _textWriter;

    Player _playerInput;
    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        _isPlayerHere = false;

        _dialogueBox = transform.GetChild(0).gameObject;
        _textWriter = GetComponent<TextWriter>();

        _playerInput = ReInput.players.GetPlayer(0);
        _gameManager = FindObjectOfType<GameManager>();

        // Hide the dialogue box just in case
        HideDialogue();
    }

    // Update is called once per frame
    void Update () {
        if (_isPlayerHere && !_gameManager.isPaused) {
            if (_playerInput.GetButtonDown("MoveUp")) {
                if(!_dialogueBox.activeSelf) {
                    DisplayDialogue();
                } else {
                    HideDialogue();
                }
            }
        }
    }

    void DisplayDialogue() {
        if (dialogue != "") {
            _dialogueBox.SetActive(true);
            _textWriter.StartWriting(dialogue);
        }
    }

    void HideDialogue() {
        _dialogueBox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            _isPlayerHere = false;
            HideDialogue();
        }
    }
}
