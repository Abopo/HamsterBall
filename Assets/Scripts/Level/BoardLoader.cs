using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class BoardLoader : MonoBehaviour {
    string _cutscenePath;
    StreamReader _reader;
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
        //_reader = new StreamReader("Assets/Resources/Text/" + path);
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

    void ReadLine() {
        do {
            //_readText = _reader.ReadLine();
            _readText = _linesFromFile[_fileIndex++];
        } while (_readText == "");

        switch(_readText) {
            case "Bubble Layout":
                ReadBubbleLayout();
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
            case "Board":
                LoadBoard();
                break;
        }
    }

    void ReadBubbleLayout() {
        int[] bubbles = new int[50];
        int bubIndex = 0;
        int stringIndex = 0;

        //_readChar = (char)_reader.Read();
        _readChar = _linesFromFile[_fileIndex][stringIndex++];
        while (_readChar != 'E') {
            if (_readChar != ',') {
                bubbles[bubIndex] = (int)char.GetNumericValue(_readChar);
                bubIndex++;
            } else {
                _fileIndex++;
                stringIndex = 0;
            }
            //_readChar = (char)_reader.Read();
            _readChar = _linesFromFile[_fileIndex][stringIndex++];
        }

        BubbleManager.startingBubbleTypes = bubbles;
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

        PlayerInfo player1 = new PlayerInfo();
        player1.playerNum = 1;
        player1.controllerNum = 1;
        player1.team = 0;
        playerManager.AddPlayer(player1);

        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++]; // read AI difficulty

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

    void SetCharacterAI(PlayerInfo pInfo, string charAI) {
        GeneralHamAI characterAI = new GeneralHamAI();
        switch(charAI) {
            case "GeneralHam":
                characterAI = new GeneralHamAI();
                break;
        }

        pInfo.characterAI = characterAI;
    }

    void LoadBoard() {
        //_readText = _reader.ReadLine();
        _readText = _linesFromFile[_fileIndex++];
        SceneManager.LoadScene(_readText);
    }
}
