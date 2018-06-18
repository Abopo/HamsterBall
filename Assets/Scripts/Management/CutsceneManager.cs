using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class CutsceneManager : MonoBehaviour {
    public Text titleText;

    public CutsceneCharacter leftChara1;
    public CutsceneCharacter leftChara2;
    public CutsceneCharacter rightChara1;
    public CutsceneCharacter rightChara2;

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

        _fileIndex = 0;

        _ready = true;
        _playedAudio = false;
        _isPlaying = true;

        CharaSetup();
    }

    void CharaSetup() {
        leftChara1.screenPos = -105f;
        leftChara1.offScreenPos = -500f;
        leftChara1.side = -1;
        leftChara2.screenPos = -290f;
        leftChara2.offScreenPos = -500f;
        leftChara2.side = -1;
        rightChara1.screenPos = 105f;
        rightChara1.offScreenPos = 500;
        rightChara1.side = 1;
        rightChara2.screenPos = 290f;
        rightChara2.offScreenPos = 500;
        rightChara2.side = 1;
    }

    void Start() {
        if (fileToLoad != "") {
            StartCutscene(fileToLoad);
        }
    }

    public void StartCutscene(string textPath) {
        // Pause the game for the cutscene
        _gameManager.FullPause();

        // Make sure scene is visible
        titleText.gameObject.SetActive(true);
        leftChara1.gameObject.SetActive(true);
        leftChara2.gameObject.SetActive(true);
        rightChara1.gameObject.SetActive(true);
        rightChara2.gameObject.SetActive(true);
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
        // If a character is sliding, wait for them to finish
        if (leftChara1.IsSliding || leftChara2.IsSliding || rightChara1.IsSliding || rightChara2.IsSliding) {
            return;
        }

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
            //_ready = false;
            //Reset();
            ReadEscapeCharacter();
        }
    }

    private void Reset() {
        _ready = false;
        leftChara1.SetIsSpeaking(false);
        leftChara2.SetIsSpeaking(false);
        rightChara1.SetIsSpeaking(false);
        rightChara2.SetIsSpeaking(false);
    }

    public void ReadEscapeCharacter() {
        //do {
        //    _escapeChar = _linesFromFile[_fileIndex++];
        //} while (_escapeChar == "");

        _escapeChar = _linesFromFile[_fileIndex++];
        while(_escapeChar == "") {
            Reset();
            _escapeChar = _linesFromFile[_fileIndex++];
        }

        switch (_escapeChar) {
            case "T":
                ReadTitle();
                break;
            case "L":
                ReadLocation();
                break;
            case "CL1":
            case "CL2":
            case "CR1":
            case "CR2":
                ReadCharacters();
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
            case "Done":
                ReturnToStorySelect();
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
            backgroundSprite.sprite = Resources.Load<Sprite>("Art/Levels/" + _readText);
        }

        ReadEscapeCharacter();
    }

    void ReadCharacters() {
        // Read the character's name
        _readText = _linesFromFile[_fileIndex++];
        switch(_escapeChar) {
            case "CL1":
                SetCharacter(leftChara1);
                break;
            case "CL2":
                SetCharacter(leftChara2);
                break;
            case "CR1":
                SetCharacter(rightChara1);
                break;
            case "CR2":
                SetCharacter(rightChara2);
                break;
        }
    }

    void SetCharacter(CutsceneCharacter character) {
        // Read in the character's expression
        string expressionText = _linesFromFile[_fileIndex++];

        if (_readText == "Clear") {
            // Move off screen
            character.SlideOut();
        } else {
            character.SetIsSpeaking(true);

            // If already in place
            if (character.onScreen) {
                // and we need to change character
                if (character.curCharacter != _readText) {
                    // Set the new character
                    character.charaToChangeTo = _readText;
                    character.expressionToChangeTo = expressionText;

                    // And slide out to change off screen
                    character.SlideOut();
                    // If the expression needs to change
                } else if (character.curExpression != expressionText) {
                    // Change expressions
                    character.SetExpression(expressionText);

                    ReadFacing(character);

                    // Go ahead and continue the cutscene
                    ReadEscapeCharacter();
                }

                ReadFacing(character);

                // else, slide into place
            } else {
                // Set the character
                character.SetCharacter(_readText, expressionText);
                // Slide in
                character.SlideIn();
            }
        }
    }

    void ReadFacing(CutsceneCharacter character) {
        // Read and set facing
        string facingText = _linesFromFile[_fileIndex];
        if (facingText == "Left") {
            character.SetFacing(-1);
            _fileIndex++;
        } else if (facingText == "Right") {
            character.SetFacing(1);
            _fileIndex++;
        }
    }

    void ReadDialogue() {
        _ready = false;

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
        // Since we are about to leave, clean up
        CleanUp();

        // Read the board file path
        _readText = _linesFromFile[_fileIndex++];
        GetComponent<BoardLoader>().ReadBoardSetup(_readText);
    }

    void CleanUp() {
        fileToLoad = "";
    }

    void EndScene() {
        // Unpause the game
        _gameManager.Unpause();

        // Disable scene UI
        titleText.gameObject.SetActive(false);
        leftChara1.gameObject.SetActive(false);
        leftChara2.gameObject.SetActive(false);
        rightChara1.gameObject.SetActive(false);
        rightChara2.gameObject.SetActive(false);
        backgroundSprite.gameObject.SetActive(false);
        textBacker.gameObject.SetActive(false);
        dialoguetext.gameObject.SetActive(false);

        _ready = false;
        _isPlaying = false;
    }

    void ReturnToStorySelect() {
        SceneManager.LoadScene("StorySelect");
    }
}
