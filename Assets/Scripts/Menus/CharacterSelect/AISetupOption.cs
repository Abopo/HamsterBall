using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AISetupOption : MenuOption {
    public PlayerInfo aiInfo;
    public Text aiDifficultyText;

    bool _justMoved;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _justMoved = false;
	}

    public void Initialize(PlayerInfo pI) {
        aiInfo = pI;
        UpdateText();
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        if (_isHighlighted && !_justMoved) {
            // Right
            if (InputRight()) {
                IncreaseAIDifficulty();
                _justMoved = true;
            }
            // Left
            if (InputLeft()) {
                DecreaseAIDifficulty();
                _justMoved = true;
            }
        }

        if (Input.GetAxis("Horizontal") < 0.3f && Input.GetAxis("Horizontal") > -0.3f) {
            _justMoved = false;
        }

    }

    protected override void Select() {
        base.Select();
    }

    public void IncreaseAIDifficulty() {
        aiInfo.difficulty++;
        if (aiInfo.difficulty == 4) {
            aiInfo.difficulty = 0;
        }

        PlaySelectSound();

        UpdateText();
    }

    public void DecreaseAIDifficulty() {
        aiInfo.difficulty--;
        if (aiInfo.difficulty == -1) {
            aiInfo.difficulty = 3;
        }

        PlaySelectSound();

        UpdateText();
    }

    void UpdateText() {
        switch (aiInfo.difficulty) {
            case 0:
                aiDifficultyText.text = "Easy";
                aiDifficultyText.color = Color.blue;
                break;
            case 1:
                aiDifficultyText.text = "Medium";
                aiDifficultyText.color = Color.green;
                break;
            case 2:
                aiDifficultyText.text = "Hard";
                aiDifficultyText.color = Color.yellow;
                break;
            case 3:
                aiDifficultyText.text = "Expert";
                aiDifficultyText.color = Color.red;
                break;
        }
    }

}
