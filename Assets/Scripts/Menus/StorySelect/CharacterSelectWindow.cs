using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectWindow : MonoBehaviour {

    CHARACTERNAMES _chosenCharacter;

    StoryButton _storyButton;

    GameManager _gameManager;
    BoardLoader _boardLoader;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _boardLoader = FindObjectOfType<BoardLoader>();
    }
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	}

    public void Activate(StoryButton storyButton) {
        _storyButton = storyButton;
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        _storyButton.isReady = true;
        gameObject.SetActive(false);
    }

    public void ChooseBoy() {
        _chosenCharacter = CHARACTERNAMES.BOY1;
        LoadStage();
    }

    public void ChooseGirl() {
        _chosenCharacter = CHARACTERNAMES.GIRL1;
        LoadStage();
    }

    void LoadStage() {
        AddPlayers();

        _gameManager.stage = _storyButton.sceneNumber;

        // Set the new story position to here
        PlayerPrefs.SetString("StoryPos", _storyButton.sceneNumber);

        // Set the character in the board loader
        _boardLoader.SetCharacter(_chosenCharacter);

        if (_storyButton.hasCutscene) {
            // Load a cutscene
            CutsceneManager.fileToLoad = _storyButton.fileToLoad;
            SceneManager.LoadScene("Cutscene");
        } else {
            // Load a board
            _boardLoader.ReadBoardSetup(_storyButton.fileToLoad);
        }
    }

    void AddPlayers() {
        PlayerManager playerManager = _gameManager.GetComponent<PlayerManager>();
        playerManager.ClearAllPlayers();

        PlayerInfo player1 = new PlayerInfo();
        player1.playerNum = 1;
        int joystick = InputState.GetValidJoystick();
        if (joystick > 0) {
            player1.controllerNum = joystick;
        } else {
            player1.controllerNum = 1;
        }
        player1.characterName = _chosenCharacter;
        player1.team = 0;
        playerManager.AddPlayer(player1);

        // TODO: Add a seond player if playing coop
    }
}
