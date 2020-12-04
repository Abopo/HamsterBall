using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Rewired;

public class VillageManager : MonoBehaviour {

    PauseMenu _pauseMenu;
    GameManager _gameManager;

    Player _player;

    public int villageIndex; // This represents the status of the village as the player progresses through the story.
                             // Currently there are 13 stages of village.

    bool _started;
    public UnityEvent villageStart;

    private void Awake() {
        _pauseMenu = FindObjectOfType<PauseMenu>();

        //InitPlayerPrefs();

        GetVillageIndex();
    }

    // Use this for initialization
    void Start () {
        _gameManager = GameManager.instance;
        _gameManager.isSinglePlayer = true;
        _gameManager.isOnline = false;

        _player = ReInput.players.GetPlayer(0);

        SceneManager.activeSceneChanged += OnSceneChanged;
	}

    void GetVillageIndex() {
        int[] storyProgress = ES3.Load<int[]>("StoryProgress", new int[] { 1, 1 });
        int chapter = storyProgress[0];
        int stage = storyProgress[1];

        switch (chapter) {
            case 1: // Forest
                if(stage == 1) { // Start of the game
                    villageIndex = 0;
                } else if(stage >= 2 && stage < 6) { // First stage finished
                    villageIndex = 1;
                } else if(stage >= 6) { // Rainbow hamster
                    villageIndex = 2;
                }
                break;
            case 2: // Mountain
                if(stage < 7) { // Start of Mountain (forest finished)
                    villageIndex = 3;
                } else if(stage >= 7) { // Middle of mountain (fought goat)
                    villageIndex = 4;
                }
                break;
            case 3: // Beach
                if(stage < 3) { // Mountain finished
                    villageIndex = 5;
                } else if (stage >= 3 && stage < 6) { // Skull hamster
                    villageIndex = 6;
                } else if (stage >= 6) { // Beach half
                    villageIndex = 7;
                }
                break;
            case 4: // City
                if(stage < 2) { // Beach finished
                    villageIndex = 8;
                } else if(stage >= 2 && stage < 5) { // Bomb Hamster
                    villageIndex = 9;
                } else if(stage >= 5) { // City half
                    villageIndex = 10;
                }
                break;
            case 5: // Corporation
                if(stage < 2) { // City finished
                    villageIndex = 11;
                } else if (stage >= 2 && stage < 5) { // Plasma Hamster
                    villageIndex = 12;
                } else if (stage >= 5) { // Corporation half
                    villageIndex = 13;
                }
                break;
            case 6: // Airship
                if (stage < 5) { // Corporation finished
                    villageIndex = 13;
                } else if(stage >= 5 && stage < 10) { // Airship half
                    villageIndex = 14;
                } else { // Game finished
                    villageIndex = 15;
                }
                break;
        }
    }

    void InitPlayerPrefs() {
        // These prefs are only reset on the first launch of the game.
        if (ES3.Load<int>("FirstTimePlaying", 0) == 0) {
            int[] storyProgress = new int[2] { 4, 10 };
            ES3.Save<int[]>("StoryProgress", storyProgress); // How far into the story the player is (used to lock/unlock story levels and determine the village index)
            int[] storyPos = new int[2] { 4, 1 };
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
            //ShopItems();
            LoadInitialShopData();

            // Character palettes
            CharacterPalettes();

            // Music Tracks
            MusicTracks();

            ES3.Save<int>("FirstTimePlaying", 1);
        }

        // Make sure keys exist (from updates)
        if(!ES3.KeyExists("Currency")) {
            ES3.Save<int>("Currency", 200);
        }
        if (!ES3.KeyExists("SoloFlowers")) {
            HighscorePrefs();
        }
        if(!ES3.KeyExists("Kaden Palette 5")) {
            LoadInitialShopData();
        }
        if(!ES3.KeyExists("BoyPalettes")) {
            // Palettes aren't made yet so make em
            CharacterPalettes();
        }
        if(!ES3.KeyExists("Seren Woods 1")) {
            // No music track data so make em
            MusicTracks();
        }
    }

    void HighscorePrefs() {
        int[,] soloHighScores = new int[6, 10];
        int[,] soloFlowers = new int[6, 10];
        int[,] coopHighScores = new int[6, 10];
        int[,] coopFlowers = new int[6, 10];

        ES3.Save<int[,]>("SoloHighScores", soloHighScores);
        ES3.Save<int[,]>("SoloFlowers", soloFlowers);
        ES3.Save<int[,]>("CoopHighScores", coopHighScores);
        ES3.Save<int[,]>("CoopFlowers", coopFlowers);
    }

    // This data is for whether or not shop items are visible in the shop or not
    void LoadInitialShopData() {
        ShopData _shopData;
#if UNITY_EDITOR
        _shopData = ShopData.Load(Path.Combine(Application.dataPath, "Resources/Text/Shop/ShopItemData.xml"));
#else
        _shopData = ShopData.Load(Path.Combine(Application.dataPath, "ShopItemData.xml"));
#endif
        bool[] values = new bool[2]; // 0 - unlocked, 1 - purchased

        // Palettes
        foreach (ItemInfo iInfo in _shopData.paletteData) {
            values[0] = iInfo.unlocked;
            ES3.Save<bool[]>(iInfo.itemName, values);
        }

        // Music
        foreach (ItemInfo iInfo in _shopData.musicData) {
            values[0] = iInfo.unlocked;
            ES3.Save<bool[]>(iInfo.itemName, values);
        }
    }

    // v These functions are for if the item has been purchased/unlocked v

    void CharacterPalettes() {
        // First four palettes for all characters are unlocked by default
        bool[] initialPaletteData = new bool[6] { true, true, true, true, false, false };

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
    void MusicTracks() {
        ES3.Save<bool>("Seren Woods 1 Track", false);
        ES3.Save<bool>("Seren Woods 2 Track", false);
        ES3.Save<bool>("Mount Bolor 1 Track", false);
        ES3.Save<bool>("Mount Bolor 2 Track", false);
        ES3.Save<bool>("Conch Cove 1 Track", false);
        ES3.Save<bool>("Conch Cove 2 Track", false);
        ES3.Save<bool>("Big City 1 Track", false);
        ES3.Save<bool>("Big City 2 Track", false);
        ES3.Save<bool>("Corporation 1 Track", false);
        ES3.Save<bool>("Corporation 2 Track", false);
        ES3.Save<bool>("Laboratoy 1 Track", false);
        ES3.Save<bool>("Laboratoy 2 Track", false);
        ES3.Save<bool>("Airship 1 Track", false);
        ES3.Save<bool>("Airship 2 Track", false);
    }


    void GameStatPrefs() {

    }

    // Update is called once per frame
    void Update () {
		//if(_player.GetButtonDown("Pause")) {
        //    _pauseMenu.Activate(0);
        //}
        
        // If the village scene is active
        if(!_started && SceneManager.GetActiveScene().name == "VillageScene") {
            _started = true;
            villageStart.Invoke();
        }
	}

    void OnSceneChanged(Scene oldScene, Scene newScene) {
        //// If we load into a gameplay scene
        //if (newScene.buildIndex >= 15) {
        if (oldScene.name == "VillageScene") {
            // Unload the village
            SceneManager.UnloadSceneAsync("VillageScene");
        }
        // If we load into the village
        //} else if (newScene.name == "VillageScene") {
            // Make sure the village is active
        //    gameObject.SetActive(true);
        // If we load into any other scene
        //} else {
            // Just deactivate the village for quick access later
        //    gameObject.SetActive(false);
        //}
    }
}
