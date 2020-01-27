using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public enum SPECIALSPAWNMETHOD { BOTH = 0, BALLS, PIPE, NONE, NUM_METHODS };

public class GameSetupWindow : Menu {

    //public Team teamLeft;
    //public Team teamRight;

    public Text lthNumberText;
    public Text rthNumberText;
    public Text aaText;
    public Text hsmText;
    public Text rainbowText;
    public Text deadText;
    public Text gravityText;
    public Text bombText;
    public Text spawnMethodText;
    public AISetupWindow aiSetupWindow;

    bool _aimAssist;

    SPECIALSPAWNMETHOD _specialSpawnMethod;

    CharacterSelect _characterSelect;

    //string[] hsrTexts = new string[3];
    GameSetupOption[] _options = new GameSetupOption[7];

    public void Initialize() {
        gameObject.SetActive(true);

        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.GetComponent<PlayerManager>().SetAimAssist(_aimAssist);

        Activate();
    }

    protected override void Awake() {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _characterSelect = FindObjectOfType<CharacterSelect>();

        GetOptions();

        OptionsSetup();
    }

    void GetOptions() {
        GameSetupOption tempOption;
        int counter = 0;
        for(int i = 0; i < transform.childCount; ++i) {
            tempOption = transform.GetChild(i).GetComponent<GameSetupOption>();
            if (tempOption != null) {
                _options[counter] = tempOption;
                counter++;
            }
        }
    }

    public void OptionsSetup() {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        _gameManager.leftTeamHandicap = 10;
        _gameManager.rightTeamHandicap = 10;

        _aimAssist = false;
        aaText.text = "Off";

        hsmText.text = _gameManager.HamsterSpawnMax.ToString();

        if (HamsterSpawner.canBeRainbow) {
            rainbowText.text = "On";
        } else {
            rainbowText.text = "Off";
        }
        if (HamsterSpawner.canBeDead) {
            deadText.text = "On";
        } else {
            deadText.text = "Off";
        }
        if (HamsterSpawner.canBeGravity) {
            gravityText.text = "On";
        } else {
            gravityText.text = "Off";
        }
        if (HamsterSpawner.canBeBomb) {
            bombText.text = "On";
        } else {
            bombText.text = "Off";
        }
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        if (InputState.GetButtonOnAnyControllerPressed("Cancel") && !IsAnyOptionSelected()) {
            // If there's AI's
            if (_characterSelect.numAI > 0) {
                // Go back to AI setup
                Deactivate();
                aiSetupWindow.gameObject.SetActive(true);
            } else {
                // Go back to char select
                Deactivate();
                _characterSelect.Activate();
            }

            // Make sure the button press doesn't overflow into the next menu.
            Input.ResetInputAxes();
        }
    }

    public override void Activate() {
        base.Activate();
    }

    public override void Deactivate() {
        base.Deactivate();

        gameObject.SetActive(false);
    }

    bool IsAnyOptionSelected() {
        for(int i = 0; i < _options.Length; ++i) {
            if(_options[i].IsSelected) {
                return true;
            }
        }

        return false;
    }

    public void AimAssistButton() {
        _aimAssist = !_aimAssist;
        if(_aimAssist) {
            aaText.text = "On";
        } else {
            aaText.text = "Off";
        }
        _gameManager.GetComponent<PlayerManager>().SetAimAssist(_aimAssist);
    }

    // Decrease the Hamster Spawn Rate
    public void DecreaseHSR() {
        _gameManager.HamsterSpawnMax -= 2;
        hsmText.text = _gameManager.HamsterSpawnMax.ToString();
    }

    // Increase the Hamster Spawn Rate
    public void IncreaseHSR() {
        _gameManager.HamsterSpawnMax += 2;
        hsmText.text = _gameManager.HamsterSpawnMax.ToString();
    }

    // Turns On/Off Rainbow hamsters
    public void RainbowHamsterButton() {
        HamsterSpawner.canBeRainbow = !HamsterSpawner.canBeRainbow;
        if(HamsterSpawner.canBeRainbow) {
            rainbowText.text = "On";
        } else {
            rainbowText.text = "Off";
        }
    }
    // Turns On/Off Dead hamsters
    public void DeadHamsterButton() {
        HamsterSpawner.canBeDead = !HamsterSpawner.canBeDead;
        if (HamsterSpawner.canBeDead) {
            deadText.text = "On";
        } else {
            deadText.text = "Off";
        }
    }
    // Turns On/Off Gravity hamsters
    public void GravityHamsterButton() {
        HamsterSpawner.canBeGravity = !HamsterSpawner.canBeGravity;
        if (HamsterSpawner.canBeGravity) {
            gravityText.text = "On";
        } else {
            gravityText.text = "Off";
        }
    }
    // Turns On/Off Bomb hamsters
    public void BombHamsterButton() {
        HamsterSpawner.canBeBomb = !HamsterSpawner.canBeBomb;
        if (HamsterSpawner.canBeBomb) {
            bombText.text = "On";
        } else {
            bombText.text = "Off";
        }
    }

    public void ChangeSpawnMethodUp() {
        _specialSpawnMethod++;
        if(_specialSpawnMethod >= SPECIALSPAWNMETHOD.NUM_METHODS) {
            _specialSpawnMethod = SPECIALSPAWNMETHOD.BOTH;
        }

        SetSpawnMethod();
    }

    public void ChanceSpawnMethodDown() {
        _specialSpawnMethod--;
        if (_specialSpawnMethod < SPECIALSPAWNMETHOD.BOTH) {
            _specialSpawnMethod = SPECIALSPAWNMETHOD.NONE;
        }

        SetSpawnMethod();
    }

    void SetSpawnMethod() {
        switch(_specialSpawnMethod) {
            case SPECIALSPAWNMETHOD.BOTH:
                spawnMethodText.text = "Both";
                break;
            case SPECIALSPAWNMETHOD.BALLS:
                spawnMethodText.text = "Balls";
                break;
            case SPECIALSPAWNMETHOD.PIPE:
                spawnMethodText.text = "Pipe";
                break;
            case SPECIALSPAWNMETHOD.NONE:
                spawnMethodText.text = "None";
                break;
        }

        _gameManager.specialSpawnMethod = _specialSpawnMethod;
    }

    public void LoadNextScene() {
        if(_gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            _gameManager.LoadPuzzleChallenge();
            return;
        }

        //if (_gameManager.isOnline) {
            //PhotonNetwork.LoadLevel("NetworkedMapSelect");
        //    PhotonNetwork.LoadLevel("NetworkedMapSelectWheel");
        //} else {
            SceneManager.LoadScene("MapSelectWheel");
        //}
    }

    // Sets the game up for demo mode and skips this menu
    public void DemoSetup() {
        _gameManager = FindObjectOfType<GameManager>();

        _gameManager.GetComponent<PlayerManager>().SetAimAssist(false);
        _gameManager.HamsterSpawnMax = 8;
        HamsterSpawner.canBeRainbow = true;
        HamsterSpawner.canBeDead = true;
        HamsterSpawner.canBeBomb = false;
        HamsterSpawner.canBeGravity = false;

        SceneManager.LoadScene("MapSelectWheel");
    }
}
