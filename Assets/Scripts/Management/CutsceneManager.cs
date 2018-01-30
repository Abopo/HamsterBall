using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CutsceneManager : MonoBehaviour {
    public Text _titleText;
    public Image _leftCharacterSprite;
    public Image _rightCharacterSprite;
    public Image _backgroundSprite;

    TextWriter _dialogueText;
    AudioSource _audioSource;
    TextAsset _textAsset;

    string[] _linesFromFile;
    int _fileIndex;
    string _escapeChar;
    string _readText;

    bool _ready;
    bool _playedAudio;

    // Use this for initialization
    void Start() {
        _dialogueText = GetComponent<TextWriter>();
        _audioSource = GetComponent<AudioSource>();

        _textAsset = Resources.Load<TextAsset>("Text/OpeningCutscene");
        _linesFromFile = _textAsset.text.Split("\n"[0]);
        int i = 0;
        foreach(string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            i++;
        }
        _fileIndex = 0;

        _ready = true;
        _playedAudio = false;
    }

    // Update is called once per frame
    void Update() {
        CheckInput();

        if (_playedAudio && !_audioSource.isPlaying) {
            _ready = true;
        }
        if (_dialogueText.done) {
            _ready = true;
        }
    }

    void CheckInput() {
        if (Input.GetKeyDown(KeyCode.Space) && _ready) {
            // Move to next thing
            _ready = false;
            ReadEscapeCharacter();
        }
    }

    void ReadEscapeCharacter() {
        do {
            _escapeChar = _linesFromFile[_fileIndex++];
        } while (_escapeChar == "");

        switch (_escapeChar) {
            case "T":
                ReadTitle();
                break;
            case "L":
                ReadLocation();
                break;
            case "C1":
            case "C2":
                ReadCharacter();
                break;
            case "D":
                ReadDialogue();
                break;
            case "S":
                ReadSound();
                break;
            case "B":
                LoadBoard();
                break;
        }
    }

    void ReadTitle() {
        // Read the title text
        _readText = _linesFromFile[_fileIndex++];
        _titleText.text = _readText;

        // Go straight to the next data
        ReadEscapeCharacter();
    }

    void ReadLocation() {
        // Read the image file's path
        _readText = _linesFromFile[_fileIndex++];
        _backgroundSprite.sprite = Resources.Load<Sprite>("Art/UI/" + _readText);

        ReadEscapeCharacter();
    }

    void ReadCharacter() {
        // Read the character's name
        _readText = _linesFromFile[_fileIndex++];
        if (_escapeChar == "C1") {
            _rightCharacterSprite.enabled = false;
            _leftCharacterSprite.enabled = true;
            _leftCharacterSprite.sprite = Resources.Load<Sprite>("Art/Characters/" + _readText);
        } else if(_escapeChar == "C2") {
            _leftCharacterSprite.enabled = false;
            _rightCharacterSprite.enabled = true;
            _rightCharacterSprite.sprite = Resources.Load<Sprite>("Art/Characters/" + _readText);
        }

        ReadEscapeCharacter();
    }

    void ReadDialogue() {
        // Read the dialogue
        _readText = _linesFromFile[_fileIndex++];
        _dialogueText.StartWriting(_readText);
    }

    void ReadSound() {
        // Read the sound file path
        _readText = _linesFromFile[_fileIndex++];
        _audioSource.clip = Resources.Load<AudioClip>("Audio/Cutscenes/" + _readText);
        _audioSource.Play();
        _playedAudio = true;
    }

    void LoadBoard() {
        // Read the board file path
        _readText = _linesFromFile[_fileIndex++];
        GetComponent<BoardLoader>().ReadBoardSetup(_readText);
    }
}
