using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CutsceneManager : MonoBehaviour {
    public Text titleText;
    public Image leftCharacterSprite;
    public Image rightCharacterSprite;
    public Image backgroundSprite;
    public Image textBacker;
    public Text dialoguetext;

    static public string fileToLoad;

    TextWriter _textWriter;
    AudioSource _audioSource;
    TextAsset _textAsset;

    string[] _linesFromFile;
    int _fileIndex;
    string _escapeChar;
    string _readText;

    bool _ready;
    bool _playedAudio;
    bool _isPlaying;

    GameManager _gameManager;

    // Use this for initialization
    private void Awake() {
        _textWriter = GetComponent<TextWriter>();
        _audioSource = GetComponent<AudioSource>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        StartCutscene(fileToLoad);

        //_textAsset = Resources.Load<TextAsset>("Text/OpeningCutscene");
        _textAsset = Resources.Load<TextAsset>("Text/" + fileToLoad);
        _linesFromFile = _textAsset.text.Split("\n"[0]);
        int i = 0;
        foreach (string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            i++;
        }
        _fileIndex = 0;

        _ready = true;
        _playedAudio = false;
        _isPlaying = true;
    }

    void Start() {
    }

    public void StartCutscene(string textPath) {
        // Pause the game for the cutscene
        _gameManager.Pause();

        // Make sure scene is visible
        titleText.gameObject.SetActive(true);
        leftCharacterSprite.gameObject.SetActive(true);
        rightCharacterSprite.gameObject.SetActive(true);
        backgroundSprite.gameObject.SetActive(true);
        textBacker.gameObject.SetActive(true);
        dialoguetext.gameObject.SetActive(true);

        _textAsset = Resources.Load<TextAsset>("Text/" + textPath);
        _linesFromFile = _textAsset.text.Split("\n"[0]);
        int i = 0;
        foreach (string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            i++;
        }
        _fileIndex = 0;

        _ready = false;
        _isPlaying = true;

        // Start the cutscene
        ReadEscapeCharacter();
    }

    // Update is called once per frame
    void Update() {
        CheckInput();

        if (_playedAudio && !_audioSource.isPlaying) {
            _ready = true;
        }
        if (_textWriter.done) {
            _ready = true;
        }
    }

    void CheckInput() {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.V) || Input.GetMouseButtonDown(0) || Input.GetButtonDown("Joystick Jump 1")) 
            && _ready && _isPlaying) {
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
            case "E":
                EndScene();
                break;
        }
    }

    void ReadTitle() {
        // Read the title text
        _readText = _linesFromFile[_fileIndex++];
        titleText.text = _readText;

        // Go straight to the next data
        ReadEscapeCharacter();
    }

    void ReadLocation() {
        // Read the image file's path
        _readText = _linesFromFile[_fileIndex++];
        if (_readText == "Blank") {
            backgroundSprite.gameObject.SetActive(false);
        } else {
            backgroundSprite.sprite = Resources.Load<Sprite>("Art/UI/" + _readText);
        }

        ReadEscapeCharacter();
    }

    void ReadCharacter() {
        // Read the character's name
        _readText = _linesFromFile[_fileIndex++];
        if (_escapeChar == "C1") {
            rightCharacterSprite.enabled = false;
            leftCharacterSprite.enabled = true;
            leftCharacterSprite.sprite = Resources.Load<Sprite>("Art/Characters/" + _readText);
        } else if(_escapeChar == "C2") {
            leftCharacterSprite.enabled = false;
            rightCharacterSprite.enabled = true;
            rightCharacterSprite.sprite = Resources.Load<Sprite>("Art/Characters/" + _readText);
        }

        ReadEscapeCharacter();
    }

    void ReadDialogue() {
        // Read the dialogue
        _readText = _linesFromFile[_fileIndex++];
        _textWriter.StartWriting(_readText);
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

    void EndScene() {
        // Unpause the game
        _gameManager.Unpause();

        // Disable scene UI
        titleText.gameObject.SetActive(false);
        leftCharacterSprite.gameObject.SetActive(false);
        rightCharacterSprite.gameObject.SetActive(false);
        backgroundSprite.gameObject.SetActive(false);
        textBacker.gameObject.SetActive(false);
        dialoguetext.gameObject.SetActive(false);

        _ready = false;
        _isPlaying = false;
    }
}
