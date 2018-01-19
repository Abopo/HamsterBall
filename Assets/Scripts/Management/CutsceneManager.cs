using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class CutsceneManager : MonoBehaviour {
    public Text _titleText;
    public Image _leftCharacterSprite;
    public Image _rightCharacterSprite;
    public Image _backgroundSprite;

    TextWriter _dialogueText;
    AudioSource _audioSource;
    string _cutscenePath;
    StreamReader _reader;

    char _escapeChar;
    string _readText;

    bool _ready;
    bool _playedAudio;

	// Use this for initialization
	void Start () {
        _dialogueText = GetComponent<TextWriter>();
        _audioSource = GetComponent<AudioSource>();
        _cutscenePath = "Assets/Resources/Text/CutsceneTest.txt";
        _reader = new StreamReader(_cutscenePath);
        _ready = true;
        _playedAudio = false;
	}
	
	// Update is called once per frame
	void Update () {
        CheckInput();

		if(_playedAudio && !_audioSource.isPlaying) {
            _ready = true;
        }
        if(_dialogueText.done) {
            _ready = true;
        }
    }

    void CheckInput() {
        if(Input.GetKeyUp(KeyCode.Space) && _ready) {
            // Move to next thing
            _ready = false;
            ReadEscapeCharacter();
        }
    }

    void ReadEscapeCharacter() {
        do {
            _escapeChar = (char)_reader.Read();
        } while (_escapeChar == '\r' || _escapeChar == '\n');

        switch (_escapeChar) {
            case 'T':
                ReadTitle();
                break;
            case 'L':
                ReadLocation();
                break;
            case 'C':
                ReadCharacter();
                break;
            case 'D':
                ReadDialogue();
                break;
            case 'S':
                ReadSound();
                break;
            case 'B':
                LoadBoard();
                break;
        }
    }

    void ReadTitle() {
        // Skip over a space
        _reader.Read();

        // Read the title text
        _readText = _reader.ReadLine();
        _titleText.text = _readText;
        
        // Go straight to the next data
        ReadEscapeCharacter();
    }

    void ReadLocation() {
        // Skip over a space
        _reader.Read();

        _readText = _reader.ReadLine();
        _backgroundSprite.sprite = Resources.Load<Sprite>("Art/UI/" + _readText);

        ReadEscapeCharacter();
    }

    void ReadCharacter() {
        _escapeChar = (char)_reader.Read();
        
        // Skip over a space
        _reader.Read();

        _readText = _reader.ReadLine();
        if (_escapeChar == '1') {
            _rightCharacterSprite.enabled = false;
            _leftCharacterSprite.enabled = true;
            _leftCharacterSprite.sprite = Resources.Load<Sprite>("Art/Characters/" + _readText);
        } else if(_escapeChar == '2') {
            _leftCharacterSprite.enabled = false;
            _rightCharacterSprite.enabled = true;
            _rightCharacterSprite.sprite = Resources.Load<Sprite>("Art/Characters/" + _readText);
        }

        ReadEscapeCharacter();
    }

    void ReadDialogue() {
        // Skip over a space
        _reader.Read();

        _readText = _reader.ReadLine();
        _dialogueText.StartWriting(_readText);
    }

    void ReadSound() {
        // Skip over a space
        _reader.Read();

        _readText = _reader.ReadLine();
        _audioSource.clip = Resources.Load<AudioClip>("Audio/Cutscenes/" + _readText);
        _audioSource.Play();
        _playedAudio = true;
    }

    void LoadBoard() {
        // Skip over a space
        _reader.Read();

        _readText = _reader.ReadLine();
        SceneManager.LoadScene(_readText);
    }
}
