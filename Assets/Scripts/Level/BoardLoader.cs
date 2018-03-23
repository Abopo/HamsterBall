using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class BoardLoader : MonoBehaviour {
    string _cutscenePath;
    GameManager _gameManager;

    string[] _linesFromFile;
    int _fileIndex;
    char _readChar;
    string _readText;

    // Use this for initialization
    void Start () {
        //_cutscenePath = "Assets/Resources/Text/BoardSetup.txt";
        //_reader = new StreamReader(_cutscenePath);
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ReadBoardSetup(string path) {
        // Save the path to the level data
        _gameManager.LevelDoc = path;
        _gameManager.nextLevel = "";

        TextAsset textAsset = Resources.Load<TextAsset>("Text/" + path);
        _linesFromFile = textAsset.text.Split("\n"[0]);
        int i = 0;
        foreach (string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            i++;
        }
        _fileIndex = 0;

        while (_readText != "Done") {
            ReadLine();
        }
    }

    public void ReadCreatedBoard(string path) {
        // Save the path to the level data
        _gameManager.LevelDoc = path;

#if UNITY_EDITOR
        TextAsset textAsset = Resources.Load<TextAsset>("Text/Created Boards/" + path);
        _linesFromFile = textAsset.text.Split("\n"[0]);
        _fileIndex = 0;
#else
        string allText = "";
        if (File.Exists(Application.dataPath + "/Created Boards/" + path + ".txt")) {
            Debug.Log("File exists!");
            allText = File.ReadAllText(Application.dataPath + "/Created Boards/" + path + ".txt");
        } else {
            Debug.Log("File does not exist!");
            Debug.Log("File name: " + Application.dataPath + "/Created Boards/" + path + ".txt");
        }
        _linesFromFile = allText.Split("\n"[0]);
#endif

        int i = 0;
        foreach (string line in _linesFromFile) {
            _linesFromFile[i] = line.Replace("\r", "");
            i++;
        }

        while (_readText != "Done") {
            ReadLine();
        }
    }

    void ReadLine() {
        do {
            //_readText = _reader.ReadLine();
            _readText = _linesFromFile[_fileIndex++];
        } while (_readText == "");

        switch(_readText) {
            case "Bubble Layout":
                ReadBubbleInfo();
                break;
            case "Handicaps":
                ReadHandicaps();
                break;
            case "HSM":
                ReadHamsterSpawnMax();
                break;
            case "Special Hamsters":
                ReadSpecialHamsters();
                break;
            case "AI":
                SetupPlayers();
                break;
            case "Mode":
                SetMode();
                break;
            case "Next":
                ReadNextLevel();
                break;
            case "Board":
                LoadBoard();
                break;
        }
    }

    void ReadBubbleInfo() {
        BubbleInfo[] bubbles = new BubbleInfo[125];
        int bubIndex = 0;
        int stringIndex = 0;

        //_readChar = (char)_reader.Read();
        _readChar = _linesFromFile[_fileIndex][stringIndex++];
        while (_readChar != 'E') {
            if (_readChar != ',') {
                switch (_readChar) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                        bubbles[bubIndex].type = (HAMSTER_TYPES)char.GetNumericValue(_readChar);
                        break;
                    case 'D': // Dead
                        bubbles[bubIndex].type = HAMSTER_TYPES.DEAD;
                        break;
                    case 'R': // Rainbow
                        bubbles[bubIndex].type = HAMSTER_TYPES.RAINBOW;
                        break;
                    case 'B': // Bomb
                        bubbles[bubIndex].type = HAMSTER_TYPES.BOMB;
                        break;
                    case 'G': // Gravity
                        _readChar = _linesFromFile[_fileIndex][stringIndex++];
                        bubbles[bubIndex].type = (HAMSTER_TYPES)char.GetNumericValue(_readChar);
                        bubbles[bubIndex].isGravity = true;
                        break;
                    case 'I': // Ice
                        // Set the previous bubble to be ice
                        bubbles[--bubIndex].isIce = true;
                        break;
                    case 'N': // None
                        bubbles[bubIndex].type = HAMSTER_TYPES.NO_TYPE;
                        break;
                }
                bubbles[bubIndex].isSet = true;
                bubIndex++;
            } else {
                _fileIndex++;
                stringIndex = 0;
            }
            //_readChar = (char)_reader.Read();
            _readChar = _linesFromFile[_fileIndex][stringIndex++];
        }

        BubbleManager.startingBubbleInfo = bubbles;
    }

    void ReadHandicaps() {
        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++];
        _gameManager.SetTeamHandicap(0, int.Parse(_readText));
        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++];
        _gameManager.SetTeamHandicap(1, int.Parse(_readText));
    }

    void ReadHamsterSpawnMax() {
        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++];
        _gameManager.HamsterSpawnMax = int.Parse(_readText);
    }

    void ReadSpecialHamsters() {
        string hamString;

        do {
            //_readText = _reader.ReadLine();
            _readText = _linesFromFile[_fileIndex++];
            //hamString = _reader.ReadLine();
            hamString = _linesFromFile[_fileIndex++];

            switch(_readText) {
                case "Rainbow":
                    if(hamString == "Y") {
                        HamsterSpawner.canBeRainbow = true;
                    } else {
                        HamsterSpawner.canBeRainbow = false;
                    }
                    break;
                case "Dead":
                    if (hamString == "Y") {
                        HamsterSpawner.canBeDead = true;
                    } else {
                        HamsterSpawner.canBeDead = false;
                    }
                    break;
                case "Gravity":
                    if (hamString == "Y") {
                        HamsterSpawner.canBeGravity = true;
                    } else {
                        HamsterSpawner.canBeGravity = false;
                    }
                    break;
                case "Bomb":
                    if (hamString == "Y") {
                        HamsterSpawner.canBeBomb = true;
                    } else {
                        HamsterSpawner.canBeBomb = false;
                    }
                    break;
            }
        } while (_readText != "End");
    }

    void SetupPlayers() {
        PlayerManager playerManager = _gameManager.GetComponent<PlayerManager>();
        playerManager.ClearAllPlayers();

        PlayerInfo player1 = new PlayerInfo();
        player1.playerNum = 1;
        if (Input.GetJoystickNames().Length > 0) {
            player1.controllerNum = 3;
        } else {
            player1.controllerNum = 1;
        }
        player1.team = 0;
        playerManager.AddPlayer(player1);

        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++]; // read AI difficulty
        if (_readText != "None") {
            PlayerInfo player2 = new PlayerInfo();
            player2.playerNum = 2;
            player2.controllerNum = -1;
            player2.team = 1;
            player2.difficulty = int.Parse(_readText);

            //_readText = _reader.ReadLine();
            _readText = _linesFromFile[_fileIndex++]; // read AI character script
            if (_readText != "Standard") {
                SetCharacterAI(player2, _readText);
            }
            playerManager.AddPlayer(player2);
        }
    }

    void SetCharacterAI(PlayerInfo pInfo, string charAI) {
        GeneralHamAI characterAI = new GeneralHamAI();
        switch(charAI) {
            case "GeneralHam":
                characterAI = new GeneralHamAI();
                break;
        }

        pInfo.characterAI = characterAI;
    }

    void SetMode() {
        _readText = _linesFromFile[_fileIndex++];
        switch(_readText) {
            case "Points":
                _gameManager.SetGameMode(GAME_MODE.SP_POINTS);
                _gameManager.goalCount = int.Parse(_linesFromFile[_fileIndex++]);
                _gameManager.timeLimit = int.Parse(_linesFromFile[_fileIndex++]);
                break;
            case "Matches":
                _gameManager.SetGameMode(GAME_MODE.SP_MATCH);
                _gameManager.goalCount = int.Parse(_linesFromFile[_fileIndex++]);
                _gameManager.timeLimit = int.Parse(_linesFromFile[_fileIndex++]);
                break;
            case "Clear":
                _gameManager.SetGameMode(GAME_MODE.SP_CLEAR);
                //_gameManager.goalCount = int.Parse(_linesFromFile[_fileIndex++]);
                _gameManager.timeLimit = int.Parse(_linesFromFile[_fileIndex++]);
                break;
            case "Versus":
                _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);
                break;
        }
    }

    void ReadNextLevel() {
        _readText = _linesFromFile[_fileIndex++];
        _gameManager.nextLevel = _readText;
    }

    void LoadBoard() {
        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++];
        SceneManager.LoadScene(_readText);
    }
}
