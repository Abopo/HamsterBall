using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorySelectMenu : MonoBehaviour {
    public Text chapter;
    public Text location;
    public Image locationImage;
    public Image locationImageSP;
    public Text gameType;
    public Text highscoreSolo;
    public Text highscoreCoop;
    public Text winCondition;
    public CharacterSelectWindow characterSelectWindow;

    public World[] worlds = new World[2];
    public float worldMoveSpeed;


    int _curWorld;
    bool _movingWorld;
    int _worldDif;

    int _furthestWorld;
    int _furthestLevel;

    float _worldYPos = 0f;

    Dictionary<string, Sprite> locationImages = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> locationImagesSP = new Dictionary<string, Sprite>();
    BoardDisplay _boardDisplay;

    public int CurWorld {
        get { return _curWorld; }
    }
    public int FurthestWorld {
        get { return _furthestWorld; }
    }
    public int FurthestLevel {
        get { return _furthestLevel; }
    }
    public bool MovingWorld {
        get { return _movingWorld; }
    }

    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.prevMenu = MENU.STORY;

        // Multiplayer images
		locationImages["Forest"] = Resources.Load<Sprite>("Art/UI/Map Select/BasicForest");
        locationImages["Mountain"] = Resources.Load<Sprite>("Art/UI/Map Select/BasicMountain");
        locationImages["Beach"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Beach");
        locationImages["City"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - City");
        locationImages["Sewers"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Sewers");
        locationImages["Laboratory"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Laboratory");
        locationImages["Fungals"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - Fungals");
        locationImages["DarkForest"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - DarkForest");
        locationImages["Space"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Space");

        // Single player images
        locationImagesSP["Forest"] = Resources.Load<Sprite>("Art/UI/Map Select/ForestBoard");
        locationImagesSP["Mountain"] = Resources.Load<Sprite>("Art/UI/Map Select/MountainBoard");
        locationImagesSP["Beach"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Beach");
        locationImagesSP["City"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - City");
        locationImagesSP["Sewers"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Sewers");
        locationImagesSP["Laboratory"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Laboratory");
        locationImagesSP["Fungals"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - Fungals");
        locationImagesSP["DarkForest"] = Resources.Load<Sprite>("Art/UI/Map Select/OneTube - DarkForest");
        locationImagesSP["Space"] = Resources.Load<Sprite>("Art/UI/Map Select/TwoTubes - Space");

        _boardDisplay = FindObjectOfType<BoardDisplay>();

        _worldYPos = worlds[0].transform.localPosition.y;

        // Load saved world
        LoadSaveData();
    }

    void LoadSaveData() {
        // Load how far the player has gotten
        string storyProgress = PlayerPrefs.GetString("StoryProgress");
        _furthestWorld = int.Parse(storyProgress[0].ToString());
        _furthestLevel = int.Parse(storyProgress[2].ToString());

        // Unlock all the fully unlocked worlds
        for (int i = 1; i < _furthestWorld; ++i) {
            worlds[i-1].Unlock(9);
        }
        // Unlock the partially completed world
        worlds[_furthestWorld-1].Unlock(_furthestLevel);

        // Load where the player last left off
        string storyPos = PlayerPrefs.GetString("StoryPos");
        int world = int.Parse(storyPos[0].ToString());
        int level = int.Parse(storyPos[2].ToString());

        // Move old world off screen
        worlds[0].transform.localPosition = new Vector3(0, -300, 0);
        worlds[0].Deactivate();

        // Set the new world into the right position
        worlds[world-1].transform.localPosition = new Vector3(0, _worldYPos, 0);
        worlds[world-1].Activate(level);

        // Set curWorld to new world
        _curWorld = world-1;
    }

    // Update is called once per frame
    void Update () {
        CheckInput();

		if(_movingWorld) {
            worlds[_curWorld].transform.Translate(worldMoveSpeed * -_worldDif * Time.deltaTime, 0f, 0f, Space.World);
            worlds[_curWorld+_worldDif].transform.Translate(worldMoveSpeed * -_worldDif * Time.deltaTime, 0f, 0f, Space.World);

            if(_worldDif > 0 && worlds[_curWorld + _worldDif].transform.position.x < 0 ||
               _worldDif < 0 && worlds[_curWorld + _worldDif].transform.position.x > 0) {
                EndMoveWorlds();
            }
        }
	}

    void CheckInput() {
        if(_gameManager.playerInput.GetButtonDown("Cancel")) {
            if (characterSelectWindow.gameObject.activeSelf) {
                characterSelectWindow.Deactivate();
            } else {
                FindObjectOfType<GameManager>().MainMenuButton();
            }
        }
    }

    public void UpdateUI(StoryButton storyButton) {
        // Set chapter number
        int world = storyButton.GetComponentInParent<World>().worldNum;
        chapter.text = "Chapter " + world;

        // Set the location name
        location.text = storyButton.locationName;

        // Set game type text
        switch(storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
                gameType.text = "Versus Stage";
                highscoreSolo.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "HighscoreSolo").ToString();
                highscoreCoop.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "HighscoreCoop").ToString();

                VersusSetup(storyButton);
                break;
            case GAME_MODE.SP_POINTS:
                gameType.text = "Point Challenge";
                highscoreSolo.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "HighscoreSolo").ToString();
                highscoreCoop.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "HighscoreCoop").ToString();

                SinglePlayerSetup(storyButton);
                break;
            case GAME_MODE.SP_CLEAR:
                gameType.text = "Puzzle Stage";
                highscoreSolo.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "HighscoreSolo").ToString();
                highscoreCoop.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "HighscoreCoop").ToString();

                SinglePlayerSetup(storyButton);
                break;
        }

        // Set win condition text
        winCondition.text = storyButton.winCondition;
    }

    void VersusSetup(StoryButton storyButton) {
        // Show the versus location image
        locationImage.transform.parent.gameObject.SetActive(true);
        // Hide the single player location image
        locationImageSP.transform.parent.gameObject.SetActive(false);
        
        // Set location image
        locationImage.sprite = locationImages[storyButton.locationName];

        // Clear any previews that may be showing
        _boardDisplay.ClearBoard();
    }
    void SinglePlayerSetup(StoryButton storyButton) {
        // Show the single player location image
        locationImageSP.transform.parent.gameObject.SetActive(true);
        // Hide the versus location image
        locationImage.transform.parent.gameObject.SetActive(false);

        // Set location image
        locationImageSP.sprite = locationImagesSP[storyButton.locationName];

        // Display a preview of the stage
        _boardDisplay.LoadBoard(storyButton.fileToLoad);
    }

    // Change the UI to a different world
    // dir : 1 = to the right, -1 = to the left (can increase number to move more worlds over)
    public void StartMoveWorlds(int dir) {
        // If this move would go out of bounds
        if (_curWorld + dir >= worlds.Length || _curWorld + dir < 0 || dir == 0) {
            // Don't move (Or loop around?)

        } else {
            // Setup move variables
            _worldDif = dir;
            _movingWorld = true;

            // Place the next world in correct location
            if (dir > 0) {
                worlds[_curWorld + dir].transform.localPosition = new Vector3(700, _worldYPos, 0);
            } else if(dir < 0) {
                worlds[_curWorld + dir].transform.localPosition = new Vector3(-700, _worldYPos, 0);
            }
        }
    }

    void EndMoveWorlds() {
        // Move old world off screen
        worlds[_curWorld].transform.localPosition = new Vector3(0, -300, 0);
        worlds[_curWorld].Deactivate();
        
        // Set the new world into the right position
        worlds[_curWorld + _worldDif].transform.localPosition = new Vector3(0, _worldYPos, 0);
        if (_worldDif > 0) {
            worlds[_curWorld + _worldDif].Activate(0);
        } else {
            worlds[_curWorld + _worldDif].Activate(9);
        }

        // Set curWorld to new world
        _curWorld = _curWorld + _worldDif;

        // Stop moving
        _movingWorld = false;
    }

    // Enable all the UI functionality
    public void EnableUI() {
        // Find the current highlighted stage and enable it
        foreach(StoryButton sButton in worlds[_curWorld].StoryButtons) {
            if(sButton.isHighlighted) {
                sButton.isReady = true;
            }
        }
        
        // Enable the player info
        FindObjectOfType<StoryPlayerInfo>().TurnOnInput();
    }
    // Disables all the UI functionality
    public void DisableUI() {
        // Find the current highlighted stage and disable it
        foreach (StoryButton sButton in worlds[_curWorld].StoryButtons) {
            if (sButton.isHighlighted) {
                sButton.isReady = false;
            }
        }

        // Disable the player info
        FindObjectOfType<StoryPlayerInfo>().TurnOffInput();
    }
}
