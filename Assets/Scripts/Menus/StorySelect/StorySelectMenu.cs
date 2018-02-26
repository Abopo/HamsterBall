using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorySelectMenu : MonoBehaviour {
    public Text location;
    public Image locationImage;
    public Text gameType;
    public Text highscoreHeader;
    public Text highscore;
    public Text winCondition;

    public World[] worlds = new World[2];
    public float worldMoveSpeed;

    int _curWorld;
    bool _movingWorld;
    int _worldDif;

    int _furthestWorld;
    int _furthestLevel;

    Dictionary<string, Sprite> locationImages = new Dictionary<string, Sprite>();

    public int CurWorld {
        get { return _curWorld; }
    }
    public int FurthestWorld {
        get { return _furthestWorld; }
    }
    public int FurthestLevel {
        get { return _furthestLevel; }
    }


    // Use this for initialization
    void Start () {
		locationImages["Forest"] = Resources.Load<Sprite>("Art/UI/OneTube - Forest");
        locationImages["Laboratory"] = Resources.Load<Sprite>("Art/UI/TwoTubes - Laboratory");
        locationImages["Sewers"] = Resources.Load<Sprite>("Art/UI/TwoTubes - Sewers");
        locationImages["City"] = Resources.Load<Sprite>("Art/UI/TwoTubes - City");
        locationImages["Fungals"] = Resources.Load<Sprite>("Art/UI/OneTube - Fungals");
        locationImages["DarkForest"] = Resources.Load<Sprite>("Art/UI/OneTube - DarkForest");

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
            worlds[i-1].Unlock(6);
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
        worlds[world-1].transform.localPosition = new Vector3(0, -110, 0);
        worlds[world-1].Activate(level);

        // Set curWorld to new world
        _curWorld = world-1;
    }

    // Update is called once per frame
    void Update () {
		if(_movingWorld) {
            worlds[_curWorld].transform.Translate(worldMoveSpeed * -_worldDif * Time.deltaTime, 0f, 0f, Space.World);
            worlds[_curWorld+_worldDif].transform.Translate(worldMoveSpeed * -_worldDif * Time.deltaTime, 0f, 0f, Space.World);

            if(_worldDif > 0 && worlds[_curWorld + _worldDif].transform.position.x < 0 ||
               _worldDif < 0 && worlds[_curWorld + _worldDif].transform.position.x > 0) {
                EndMoveWorlds();
            }
        }
	}

    public void UpdateUI(StoryButton storyButton) {
        // Set location name and image
        location.text = storyButton.locationName;
        locationImage.sprite = locationImages[storyButton.locationName];

        int time = 0;
        // Set game type text
        switch(storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
                gameType.text = "Versus";
                highscoreHeader.text = "Highscore";
                highscore.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore").ToString();
                break;
            case GAME_MODE.SP_POINTS:
                gameType.text = "Point Challenge";
                highscoreHeader.text = "Best Time";
                time = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore");
                highscore.text = string.Format("{0}:{1:00}", (int)time / 60, (int)time % 60);
                break;
            case GAME_MODE.SP_MATCH:
                gameType.text = "Match Challenge";
                highscoreHeader.text = "Best Time";
                //highscore.text = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore").ToString("0:00");
                time = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore");
                highscore.text = string.Format("{0}:{1:00}", (int)time / 60, (int)time % 60);
                break;
            case GAME_MODE.SP_CLEAR:
                gameType.text = "Clear Challenge";
                highscoreHeader.text = "Best Time";
                time = PlayerPrefs.GetInt(storyButton.sceneNumber.ToString() + "Highscore");
                highscore.text = string.Format("{0}:{1:00}", (int)time / 60, (int)time % 60);
                break;
        }

        // Set win condition text
        winCondition.text = storyButton.winCondition;
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
                worlds[_curWorld + dir].transform.localPosition = new Vector3(700, -110, 0);
            } else if(dir < 0) {
                worlds[_curWorld + dir].transform.localPosition = new Vector3(-700, -110, 0);
            }
        }
    }

    void EndMoveWorlds() {
        // Move old world off screen
        worlds[_curWorld].transform.localPosition = new Vector3(0, -300, 0);
        worlds[_curWorld].Deactivate();
        
        // Set the new world into the right position
        worlds[_curWorld + _worldDif].transform.localPosition = new Vector3(0, -110, 0);
        if (_worldDif > 0) {
            worlds[_curWorld + _worldDif].Activate(0);
        } else {
            worlds[_curWorld + _worldDif].Activate(5);
        }

        // Set curWorld to new world
        _curWorld = _curWorld + _worldDif;

        // Stop moving
        _movingWorld = false;
    }
}
