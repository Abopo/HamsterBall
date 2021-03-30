using UnityEngine;
using UnityEngine.UI;

public class StorySelectMenu : MonoBehaviour {
    public SuperTextMesh chapter;
    public Image gameType;
    public SuperTextMesh highscoreSolo;
    public SuperTextMesh highscoreCoop;
    public Image soloFlower;
    public Image coopFlower;
    public SuperTextMesh flowerRequirement1;
    public SuperTextMesh flowerRequirement2;
    public SuperTextMesh winCondition;
    public CharacterSelectWindow characterSelectWindow;

    public World[] worlds = new World[2];
    public float worldMoveSpeed;

    StagePicture _stagePicture;

    int _curWorld;
    bool _movingWorld;
    int _worldDif;

    int _furthestWorld;
    int _furthestLevel;

    float _worldYPos = 0f;

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
    public StoryPlayerInfo storyPlayerInfo;
    public StorySelectResources resources;

    private void Awake() {
        _gameManager = GameManager.instance;
        storyPlayerInfo = FindObjectOfType<StoryPlayerInfo>();
        _stagePicture = FindObjectOfType<StagePicture>();
        resources = GetComponent<StorySelectResources>();
    }
    // Use this for initialization
    void Start () {
        _gameManager.prevMenu = MENU.STORY;

        _worldYPos = worlds[0].transform.localPosition.y;

        // Set the move world speed
        // Should take 1/2 a second to complete
        worldMoveSpeed = Screen.currentResolution.width;

        if (!_gameManager.demoMode) {
            // Load saved world
            LoadSaveData();
        } else {
            LoadDemoData();
        }
    }

    void LoadSaveData() {
        // Load how far the player has gotten
        int[] storyProgress = ES3.Load<int[]>("StoryProgress");
        _furthestWorld = storyProgress[0];
        _furthestLevel = storyProgress[1];

        // Unlock all the fully unlocked worlds
        for (int i = 1; i < _furthestWorld; ++i) {
            worlds[i-1].Unlock(9);
        }
        // Unlock the partially completed world
        worlds[_furthestWorld-1].Unlock(_furthestLevel-1);

        // Load where the player last left off
        int[] storyPos = ES3.Load<int[]>("StoryPos");
        int world = storyPos[0];
        int level = storyPos[1];

        // Move old world off screen
        worlds[0].transform.localPosition = new Vector3(0, -300, 0);
        worlds[0].Deactivate();

        // Set the new world into the right position
        worlds[world-1].transform.localPosition = new Vector3(0, _worldYPos, 0);
        worlds[world-1].Activate(level-1);

        // Set curWorld to new world
        _curWorld = world-1;
    }

    void LoadDemoData() {
        // Load where the player last left off
        int storyPos = ES3.Load<int>("DemoPos", 1);

        worlds[0].Activate(storyPos - 1);
    }

    // Update is called once per frame
    void Update () {
        CheckInput();

		if(_movingWorld) {
            worlds[_curWorld].transform.Translate(worldMoveSpeed * -_worldDif * Time.deltaTime, 0f, 0f);
            worlds[_curWorld+_worldDif].transform.Translate(worldMoveSpeed * -_worldDif * Time.deltaTime, 0f, 0f);

            if(_worldDif > 0 && worlds[_curWorld + _worldDif].transform.localPosition.x < 0 ||
               _worldDif < 0 && worlds[_curWorld + _worldDif].transform.localPosition.x > 0) {
                EndMoveWorlds();
            }
        }
	}

    void CheckInput() {
        if(_gameManager.playerInput.GetButtonDown("Cancel")) {
            if (characterSelectWindow.hasFocus) {
                characterSelectWindow.Deactivate();
            } else {
                GameManager.instance.VillageButton();
            }
        }
    }

    public void UpdateUI(StoryButton storyButton) {
        // Set chapter number
        int world = storyButton.GetComponentInParent<World>().worldNum;
        chapter.text = "Chapter " + world;

        // Set stage info and highscore text
        int[,] soloHighScores = ES3.Load<int[,]>("SoloHighScores");
        int[,] coopHighScores = ES3.Load<int[,]>("CoopHighScores");

        switch (storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
                gameType.sprite = resources.gameModes[2];

                highscoreSolo.text = soloHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();
                highscoreCoop.text = coopHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();

                break;
            case GAME_MODE.SP_POINTS:
                gameType.sprite = resources.gameModes[1];

                highscoreSolo.text = soloHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();
                highscoreCoop.text = coopHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();

                break;
            case GAME_MODE.SP_CLEAR:
                gameType.sprite = resources.gameModes[0];

                int seconds = Mathf.FloorToInt(soloHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1] % 60);
                int minutes = Mathf.FloorToInt(soloHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1] / 60);
                highscoreSolo.text = string.Format("{0}:{1:00}", minutes, seconds);
                seconds = Mathf.FloorToInt(coopHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1] % 60);
                minutes = Mathf.FloorToInt(coopHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1] / 60);
                highscoreCoop.text = string.Format("{0}:{1:00}", minutes, seconds);

                break;
        }

        // Set flower stuff
        SetFlowerData(storyButton);
        if (storyPlayerInfo.IsCoop) {
            SetFlowerRequirementTexts(storyButton, storyButton.cpFlower2Requirement, storyButton.cpFlower3Requirement);
        } else {
            SetFlowerRequirementTexts(storyButton, storyButton.spFlower2Requirement, storyButton.spFlower3Requirement);
        }

        // Update the stage picture
        _stagePicture.UpdateImages(storyButton);

        // Set win condition text
        winCondition.text = storyButton.winCondition;
    }

    void SetFlowerData(StoryButton storyButton) {
        int[,] soloFlowerData = ES3.Load<int[,]>("SoloFlowers");
        int soloFlowerCount = soloFlowerData[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1];
        if (soloFlowerCount > 0) {
            soloFlower.gameObject.SetActive(true);
            soloFlower.sprite = resources.flowerSprites[soloFlowerCount - 1];
            soloFlower.SetNativeSize();
        } else {
            soloFlower.gameObject.SetActive(false);
        }

        int[,] coopFlowerData = ES3.Load<int[,]>("CoopFlowers");
        int coopFlowerCount = coopFlowerData[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1];
        if (coopFlowerCount > 0) {
            coopFlower.gameObject.SetActive(true);
            coopFlower.sprite = resources.flowerSprites[coopFlowerCount - 1];
            coopFlower.SetNativeSize();
        } else {
            coopFlower.gameObject.SetActive(false);
        }
    }

    void SetFlowerRequirementTexts(StoryButton storyButton, int fr1, int fr2) {
        switch(storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
            case GAME_MODE.SP_POINTS:
                    flowerRequirement1.text = fr1.ToString();
                    flowerRequirement2.text = fr2.ToString();
                break;
            case GAME_MODE.SP_CLEAR:
                    int seconds = fr1 % 60;
                    int minutes = fr1 / 60;
                    flowerRequirement1.text = string.Format("{0}:{1:00}", minutes, seconds);
                    seconds = fr2 % 60;
                    minutes = fr2 / 60;
                    flowerRequirement2.text = string.Format("{0}:{1:00}", minutes, seconds);
                break;
        }
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
            FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/Page Turn");
            // Place the next world in correct location
            if (dir > 0) {
                worlds[_curWorld + dir].transform.localPosition = new Vector3(750, _worldYPos, 0);
            } else if(dir < 0) {
                worlds[_curWorld + dir].transform.localPosition = new Vector3(-750, _worldYPos, 0);
            }
        }

        // Set the move world speed
        // Should take 1/2 a second to complete
        worldMoveSpeed = worlds[0].GetComponentInParent<Canvas>().pixelRect.width;
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
