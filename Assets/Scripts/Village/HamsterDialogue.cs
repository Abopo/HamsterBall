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

    HamsterDialogueBox _dialogueBox;

    Player _playerInput;
    GameManager _gameManager;

    public FMOD.Studio.EventInstance HamsterTalkEvent;

    protected virtual void Awake() {
        _dialogueCanvas = transform.GetChild(0).gameObject;
        _textWriter = GetComponent<TextWriter>();

        _interactIcon = GetComponentInChildren<InteractIcon>();

        _dialogueBox = GetComponentInChildren<HamsterDialogueBox>(true);

        _playerInput = ReInput.players.GetPlayer(0);
        _gameManager = GameManager.instance;
    }
    // Use this for initialization
    protected virtual void Start () {
        _isPlayerHere = false;

        // Load the right talking sound
        switch (_dialogueBox.color) {
            case HAMSTER_TYPES.RED:
            case HAMSTER_TYPES.GRAY:
            case HAMSTER_TYPES.PURPLE:
                HamsterTalkEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.HamsterTalk);
                break;
            case HAMSTER_TYPES.PINK:
            case HAMSTER_TYPES.YELLOW:
                HamsterTalkEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.HamsterTalkHigh);
                break;
            case HAMSTER_TYPES.BLUE:
            case HAMSTER_TYPES.GREEN:
            case HAMSTER_TYPES.RAINBOW:
                HamsterTalkEvent = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.HamsterTalkLow);
                break;
        }

        // Hide the dialogue box just in case
        HideDialogue();
    }

    // Update is called once per frame
    protected virtual void Update () {
        if (_isPlayerHere && !_gameManager.isPaused) {
            if (_playerInput.GetButtonDown("Interact")) {
                if(!_dialogueCanvas.activeSelf) {
                    DisplayDialogue();
                } else {
                    HideDialogue();
                }
            }
        }

        if(_dialogueCanvas.activeSelf && _textWriter.done) {
            // Stop the hamster talking sound
            HamsterTalkEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    protected virtual void DisplayDialogue() {
        if (dialogue != "") {
            _dialogueCanvas.SetActive(true);
            _textWriter.StartWriting(dialogue);
            _interactIcon.Deactivate();

            // Start the hamster talking sound
            HamsterTalkEvent.start();
        }
    }

    void HideDialogue() {
        _dialogueCanvas.SetActive(false);

        // Make sure we stop the talking sounds
        HamsterTalkEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
