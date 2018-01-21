using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class BoardLoader : MonoBehaviour {
    string _cutscenePath;
    StreamReader _reader;
    GameManager _gameManager;

    char _readChar;
    string _readText;

    // Use this for initialization
    void Start () {
        _cutscenePath = "Assets/Resources/Text/BoardSetup.txt";
        _reader = new StreamReader(_cutscenePath);
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ReadBoardSetup(string path) {
        _reader = new StreamReader("Assets/Resources/Text/" + path);
        while(_readText != "Done") {
            ReadLine();
        }
    }

    void ReadLine() {
        do {
            _readText = _reader.ReadLine();
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
        int index = 0;

        _readChar = (char)_reader.Read();
        while (_readChar != 'E') {
            if (_readChar != '\r' && _readChar != '\n') {
                bubbles[index] = (int)char.GetNumericValue(_readChar);
                index++;
            }
            _readChar = (char)_reader.Read();
        }

        BubbleManager.startingBubbleTypes = bubbles;
    }

    void ReadHandicaps() {
        _readText = _reader.ReadLine();
        _gameManager.SetTeamHandicap(0, int.Parse(_readText));
        _readText = _reader.ReadLine();
        _gameManager.SetTeamHandicap(1, int.Parse(_readText));
    }

    void ReadHamsterSpawnMax() {
        _readText = _reader.ReadLine();
        _gameManager.HamsterSpawnMax = int.Parse(_readText);
    }

    void ReadSpecialHamsters() {
        string hamString;

        do {
            _readText = _reader.ReadLine();
            hamString = _reader.ReadLine();

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

        _readText = _reader.ReadLine(); // read AI difficulty
        PlayerInfo player2 = new PlayerInfo();
        player2.playerNum = 2;
        player2.controllerNum = -1;
        player2.team = 1;
        player2.difficulty = int.Parse(_readText);

        _readText = _reader.ReadLine(); // read AI character script
        if(_readText != "Standard") {
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
        _readText = _reader.ReadLine();
        SceneManager.LoadScene(_readText);
    }
}
