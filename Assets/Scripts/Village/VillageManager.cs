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
        int[] storyProgress = ES3.Load<int[]>("StoryProgress");
        int chapter = storyProgress[0];
        int stage = storyProgress[1];

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
            ES3.Save<int>("FirstTimePlaying", 0);

            // Delete the save file
            ES3.DeleteFile();
        }

        // These prefs are only reset on the first launch of the game.
        if (ES3.Load<int>("FirstTimePlaying", 0) == 0) {
            int[] storyProgress = new int[2] { 3, 10 };
            ES3.Save<int[]>("StoryProgress", storyProgress); // How far into the story the player is (used to lock/unlock story levels and determine the village index)
            int[] storyPos = new int[2] { 1, 1 };
            ES3.Save<int[]>("StoryPos", storyPos); // Last place in the story the player was on (used to position the selector in the story select scene)

            // Stages
            ES3.Save<int>("Forest", 1);
            ES3.Save<int>("Mountain", 1);
            ES3.Save<int>("Beach", 1);
            ES3.Save<int>("City", 1);
            ES3.Save<int>("Sewers", 1);
            ES3.Save<int>("Corporation", 1);
            ES3.Save<int>("Laboratory", 1);
            ES3.Save<int>("Airship", 1);

            // Chosen players
            ES3.Save<int>("Player1Character", CHARACTERS.BOY);
            ES3.Save<int>("Player1Color", 1);
            ES3.Save<int>("Player2Character", CHARACTERS.GIRL);
            ES3.Save<int>("Player2Color", 1);

            // Currency
            ES3.Save<int>("Currency", 200);

            // Highscores
            HighscorePrefs();

            // Shop items
            ShopItems();

            // Character palettes
            CharacterPalettes();

            ES3.Save<int>("FirstTimePlaying", 1);
        }
    }

    void HighscorePrefs() {
        int[,] soloHighScores = new int[6, 10];
        int[,] coopHighScores = new int[6, 10];

        ES3.Save<int[,]>("SoloHighScores", soloHighScores);
        ES3.Save<int[,]>("CoopHighScores", coopHighScores);
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

    void CharacterPalettes() {
        // First four palettes for all characters are unlocked by default
        bool[] initialPaletteData = new bool[5] { true, true, true, false, false};

        ES3.Save<bool[]>("BoyPalettes", initialPaletteData);
        ES3.Save<bool[]>("GirlPalettes", initialPaletteData);
        ES3.Save<bool[]>("OwlPalettes", initialPaletteData);
        ES3.Save<bool[]>("GoatPalettes", initialPaletteData);
        ES3.Save<bool[]>("SnailPalettes", initialPaletteData);
        ES3.Save<bool[]>("LizardPalettes", initialPaletteData);
        ES3.Save<bool[]>("RoosterPalettes", initialPaletteData);
        ES3.Save<bool[]>("BatPalettes", initialPaletteData);
        ES3.Save<bool[]>("LackeyPalettes", initialPaletteData);
        ES3.Save<bool[]>("CrocPalettes", initialPaletteData);
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
