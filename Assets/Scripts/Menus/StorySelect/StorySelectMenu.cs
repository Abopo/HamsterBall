using UnityEngine;
using UnityEngine.UI;

public class StorySelectMenu : MonoBehaviour {
    public Text chapter;
    public Text location;
    public Text gameType;
    public Text highscoreSolo;
    public Text highscoreCoop;
    public Image soloFlower;
    public Image coopFlower;
    public Text flowerRequirement1;
    public Text flowerRequirement2;
    public Text winCondition;
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

    Sprite[] _flowerSprites = new Sprite[3];
    Color[] _flowerColors = new Color[3];

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

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _stagePicture = FindObjectOfType<StagePicture>();
        LoadFlowerSprites();
    }
    // Use this for initialization
    void Start () {
        _gameManager.prevMenu = MENU.STORY;

        _worldYPos = worlds[0].transform.localPosition.y;

        // Load saved world
        LoadSaveData();
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

    void LoadFlowerSprites() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/Effects/PointShapes");
        _flowerSprites[0] = sprites[23];
        _flowerSprites[1] = sprites[24];
        _flowerSprites[2] = sprites[25];

        _flowerColors[0] = new Color(0.79f, 0.42f, 0.04f);
        _flowerColors[1] = new Color(0.79f, 0.79f, 0.79f);
        _flowerColors[2] = new Color(1f, 0.73f, 0.25f);
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
            if (characterSelectWindow.hasFocus) {
                characterSelectWindow.Deactivate();
            } else {
                FindObjectOfType<GameManager>().VillageButton();
            }
        }
    }

    public void UpdateUI(StoryButton storyButton) {
        // Set chapter number
        int world = storyButton.GetComponentInParent<World>().worldNum;
        chapter.text = "Chapter " + world;

        // Set the location name
        location.text = storyButton.locationName;

        // Set stage info and highscore text
        int[,] soloHighScores = ES3.Load<int[,]>("SoloHighScores");
        int[,] coopHighScores = ES3.Load<int[,]>("CoopHighScores");

        switch (storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
                gameType.text = "Versus Stage";

                highscoreSolo.text = soloHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();
                highscoreCoop.text = coopHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();

                break;
            case GAME_MODE.SP_POINTS:
                gameType.text = "Point Challenge";

                highscoreSolo.text = soloHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();
                highscoreCoop.text = coopHighScores[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1].ToString();

                break;
            case GAME_MODE.SP_CLEAR:
                gameType.text = "Puzzle Stage";

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
        SetFlowerRequirementTexts(storyButton);

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
            soloFlower.sprite = _flowerSprites[soloFlowerCount - 1];
            soloFlower.color = _flowerColors[soloFlowerCount - 1];
        } else {
            soloFlower.gameObject.SetActive(false);
        }

        int[,] coopFlowerData = ES3.Load<int[,]>("CoopFlowers");
        int coopFlowerCount = coopFlowerData[storyButton.stageNumber[0] - 1, storyButton.stageNumber[1] - 1];
        if (coopFlowerCount > 0) {
            coopFlower.gameObject.SetActive(true);
            coopFlower.sprite = _flowerSprites[coopFlowerCount - 1];
            coopFlower.color = _flowerColors[coopFlowerCount - 1];
        } else {
            coopFlower.gameObject.SetActive(false);
        }
    }

    void SetFlowerRequirementTexts(StoryButton storyButton) {
        switch(storyButton.gameType) {
            case GAME_MODE.MP_VERSUS:
            case GAME_MODE.SP_POINTS:
                flowerRequirement1.text = ": " + storyButton.flower2Requirement.ToString();
                flowerRequirement2.text = ": " + storyButton.flower3Requirement.ToString();
                break;
            case GAME_MODE.SP_CLEAR:
                int seconds = storyButton.flower2Requirement % 60;
                int minutes = storyButton.flower2Requirement / 60;
                flowerRequirement1.text = ": " + string.Format("{0}:{1:00}", minutes, seconds);
                seconds = storyButton.flower3Requirement % 60;
                minutes = storyButton.flower3Requirement / 60;
                flowerRequirement2.text = ": " + string.Format("{0}:{1:00}", minutes, seconds);

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
