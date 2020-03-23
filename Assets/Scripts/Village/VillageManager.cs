using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class VillageManager : MonoBehaviour {
    public bool resetData;

    PauseMenu _pauseMenu;
    GameManager _gameManager;

    Player _player;

    public int villageIndex; // This represents the status of the village as the player progresses through the story.
                             // Currently there are 13 stages of village.

    private void Awake() {
        _pauseMenu = FindObjectOfType<PauseMenu>();

        InitPlayerPrefs();

        GetVillageIndex();
    }

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.isSinglePlayer = true;
        _gameManager.isOnline = false;

        _player = ReInput.players.GetPlayer(0);
	}

    void GetVillageIndex() {
        string storyProgress = PlayerPrefs.GetString("StoryProgress");
        int chapter = int.Parse(storyProgress[0].ToString());
        int stage = int.Parse(storyProgress[2].ToString());

        switch (chapter) {
            case 1:
                if(stage == 1) { // Start of the game
                    villageIndex = 0;
                } else if(stage >= 2 && stage < 6) { // First stage finished
                    villageIndex = 1;
                } else if(stage >= 6) { // Rainbow hamster
                    villageIndex = 2;
                }
                break;
            case 2:
                if(stage < 7) { // Start of Mountain (forest finished)
                    villageIndex = 3;
                } else if(stage >= 7) { // Middle of mountain (fought goat)
                    villageIndex = 4;
                }
                break;
            case 3:
                if (stage >= 3) { // Skull hamster
                    villageIndex = 6;
                } else if (stage >= 1) { // Mountain finished 
                    villageIndex = 5;
                }
                break;
            case 4:
                if(stage >= 1) { // Beach finished
                    villageIndex = 7;
                }
                break;
            // TODO: Finish this switch statement
        }
    }

    void InitPlayerPrefs() {
        if (resetData) {
            // TODO: Remove for final build
            PlayerPrefs.SetInt("FirstTimePlaying", 0);

            // Delete the save file
            ES3.DeleteFile();
        }

        // These prefs reset on every game close/launch.
        PlayerPrefs.SetInt("Player1Character", (int)CHARACTERCOLORS.BOY1);
        PlayerPrefs.SetInt("Player2Character", (int)CHARACTERCOLORS.GIRL1);

        // These prefs are only reset on the first launch of the game.
        if (PlayerPrefs.GetInt("FirstTimePlaying", 0) == 0) {
            PlayerPrefs.SetString("StoryProgress", "3-10"); // How far into the story the player is (used to lock/unlock story levels and determine the village index)
            PlayerPrefs.SetString("StoryPos", "1-1"); // Last place in the story the player was on (used to position the selector in the story select scene)

            // Stages
            PlayerPrefs.SetInt("Forest", 1);
            PlayerPrefs.SetInt("Mountain", 1);
            PlayerPrefs.SetInt("Beach", 1);
            PlayerPrefs.SetInt("City", 1);
            PlayerPrefs.SetInt("Sewers", 0);
            PlayerPrefs.SetInt("Corporation", 1);
            PlayerPrefs.SetInt("Laboratory", 1);
            PlayerPrefs.SetInt("Airship", 1);

            // Highscores
            HighscorePrefs();

            // Shop items
            ShopItems();

            PlayerPrefs.SetInt("FirstTimePlaying", 1);
        }
    }

    void HighscorePrefs() {
        // World 1
        PlayerPrefs.SetInt("1-1HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-1HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-2HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-2HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-3HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-3HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-4HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-4HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-5HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-5HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-6HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-6HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-7HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-7HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-8HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-8HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-9HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-9HighscoreCoop", 0);
        PlayerPrefs.SetInt("1-10HighscoreSolo", 0);
        PlayerPrefs.SetInt("1-10HighscoreCoop", 0);
        // World 2
        PlayerPrefs.SetInt("2-1HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-1HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-2HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-2HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-3HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-3HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-4HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-4HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-5HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-5HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-6HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-6HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-7HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-7HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-8HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-8HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-9HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-9HighscoreCoop", 0);
        PlayerPrefs.SetInt("2-10HighscoreSolo", 0);
        PlayerPrefs.SetInt("2-10HighscoreCoop", 0);
        // World 3
        PlayerPrefs.SetInt("3-1HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-1HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-2HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-2HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-3HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-3HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-4HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-4HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-5HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-5HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-6HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-6HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-7HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-7HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-8HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-8HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-9HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-9HighscoreCoop", 0);
        PlayerPrefs.SetInt("3-10HighscoreSolo", 0);
        PlayerPrefs.SetInt("3-10HighscoreCoop", 0);
    }

    void ShopItems() {
        TextAsset palettes = Resources.Load<TextAsset>("Text/Shop/ShopItemInitialData");
        string[] linesFromFile = palettes.text.Split("\n"[0]);
        int i = 0;
        foreach (string line in linesFromFile) {
            linesFromFile[i] = line.Replace("\r", "");
            i++;
        }

        int index = 0;
        string readLine = linesFromFile[index++];
        string tempString;
        bool[] values = new bool[2];

        while (readLine != "End") {
            if (readLine == "") {
                readLine = linesFromFile[index++];
                continue;
            }

            // Create data for the items
            tempString = readLine;
            readLine = linesFromFile[index++];
            if(readLine == "0") {
                // This item becomes available in the shop later
                values[0] = false;
            } else if(readLine == "1") {
                // This item starts available in the shop
                values[0] = true;
            }

            ES3.Save<bool[]>(tempString, values);
        }
    }

    void GameStatPrefs() {

    }

    // Update is called once per frame
    void Update () {
		if(_player.GetButtonDown("Pause")) {
            _pauseMenu.Activate(0);
        }
	}
}
