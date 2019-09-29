using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSetupWindow : MonoBehaviour {

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
    public AISetupWindow aiSetupWindow;

    bool _aimAssist;

    GameManager _gameManager;
    CharacterSelect _characterSelect;

    //string[] hsrTexts = new string[3];
    GameSetupOption[] _options = new GameSetupOption[8];

    public void Initialize() {
        gameObject.SetActive(true);

        _gameManager.GetComponent<PlayerManager>().SetAimAssist(_aimAssist);
    }

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _characterSelect = FindObjectOfType<CharacterSelect>();
    }

    // Use this for initialization
    void Start () {
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
    void Update () {
        if (_gameManager.playerInput.GetButtonDown("Cancel") && !IsAnyOptionSelected()) {
            // If there's AI's
            if (_characterSelect.numAI > 0) {
                // Go back to AI setup
                aiSetupWindow.gameObject.SetActive(true);
                gameObject.SetActive(false);
            } else {
                // Go back to char select
                _characterSelect.Reactivate();
                gameObject.SetActive(false);
            }

            // Make sure the button press doesn't overflow into the next menu.
            Input.ResetInputAxes();
        }
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
        HamsterSpawner.canBeDead = false;
        HamsterSpawner.canBeBomb = false;
        HamsterSpawner.canBeGravity = false;

        SceneManager.LoadScene("MapSelectWheel");
    }
}
