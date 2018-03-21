using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSetupWindow : MonoBehaviour {

    public Team teamLeft;
    public Team teamRight;

    public Text lthNumberText;
    public Text rthNumberText;
    public Text aaText;
    public Text hsrText;
    public Text rainbowText;
    public Text deadText;
    public Text gravityText;
    public Text bombText;
    public CharacterSelect charSelect;
    public AISetupWindow aiSetupWindow;

    bool _aimAssist;

    GameManager _gameManager;

    //string[] hsrTexts = new string[3];
    GameSetupOption[] _options = new GameSetupOption[8];

    public void Initialize() {
        gameObject.SetActive(true);
    }

    // Use this for initialization
    void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //hsrTexts[0] = "Slow";
        //hsrTexts[1] = "Medium";
        //hsrTexts[2] = "Fast";

        GetOptions();
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

        _gameManager.leftTeamHandicap = 9;
        _gameManager.rightTeamHandicap = 9;
        int leftHandicap = 12 - _gameManager.leftTeamHandicap;
        int rightHandicap = 12 - _gameManager.rightTeamHandicap;
        _gameManager.leftTeamHandicap = 12;
        _gameManager.rightTeamHandicap = 12;
        for (int i = 0; i < leftHandicap; ++i) {
            teamLeft.DecreaseHandicap();
        }
        for (int i = 0; i < rightHandicap; ++i) {
            teamRight.DecreaseHandicap();
        }
        lthNumberText.text = teamLeft.handicap.ToString();
        rthNumberText.text = teamRight.handicap.ToString();

        _aimAssist = false;
        aaText.text = "Off";

        hsrText.text = _gameManager.HamsterSpawnMax.ToString();

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
        if (Input.GetButtonDown("Cancel") && !IsAnyOptionSelected()) {
            // Back out to character select
            if (charSelect.AnyAICharacters()) {
                // Go back to AI setup
                aiSetupWindow.gameObject.SetActive(true);
                gameObject.SetActive(false);
            } else {
                // Go straight to char select
                charSelect.Reactivate();
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

    // Increase the Left team's handicap.
    public void IncreaseLTH() {
        teamLeft.IncreaseHandicap();
        teamLeft.FailCheckHandicap();
        lthNumberText.text = teamLeft.handicap.ToString();
    }
    // Decrease the Left team's handicap.
    public void DecreaseLTH() {
        teamLeft.DecreaseHandicap();
        teamLeft.FailCheckHandicap();
        lthNumberText.text = teamLeft.handicap.ToString();
    }

    // Increase the Right team's handicap.
    public void IncreaseRTH() {
        teamRight.IncreaseHandicap();
        teamRight.FailCheckHandicap();
        rthNumberText.text = teamRight.handicap.ToString();
    }
    // Decrease the Right team's handicap.
    public void DecreaseRTH() {
        teamRight.DecreaseHandicap();
        teamRight.FailCheckHandicap();
        rthNumberText.text = teamRight.handicap.ToString();
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
        hsrText.text = _gameManager.HamsterSpawnMax.ToString();
    }

    // Increase the Hamster Spawn Rate
    public void IncreaseHSR() {
        _gameManager.HamsterSpawnMax += 2;
        hsrText.text = _gameManager.HamsterSpawnMax.ToString();
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
        if (_gameManager.isOnline) {
            PhotonNetwork.LoadLevel("NetworkedMapSelect");
        } else {
            SceneManager.LoadScene("MapSelect2");
        }
    }
}
