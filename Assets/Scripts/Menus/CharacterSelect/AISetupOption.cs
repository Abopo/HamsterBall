using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AISetupOption : MenuOption {
    public PlayerInfo aiInfo;
    public SuperTextMesh aiDifficultyText;

    bool _justMoved;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _justMoved = false;
	}

    public void Initialize(PlayerInfo pI) {
        aiInfo = pI;
        aiInfo.difficulty = 1;
        UpdateText();
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();

        if (isHighlighted && !_justMoved) {
            // Right
            if (InputState.GetButtonOnAnyControllerPressed("MoveRight")) {
                IncreaseAIDifficulty();
                _justMoved = true;
            }
            // Left
            if (InputState.GetButtonOnAnyControllerPressed("MoveLeft")) {
                DecreaseAIDifficulty();
                _justMoved = true;
            }
        }

        if (InputReset()) {
            _justMoved = false;
        }

    }

    protected override void Select() {
        //base.Select();
    }

    public void IncreaseAIDifficulty() {
        aiInfo.difficulty++;
        if (aiInfo.difficulty > 10) {
            aiInfo.difficulty = 1;
        }

        PlaySelectSound();

        UpdateText();
    }

    public void DecreaseAIDifficulty() {
        aiInfo.difficulty--;
        if (aiInfo.difficulty < 1) {
            aiInfo.difficulty = 10;
        }

        PlaySelectSound();

        UpdateText();
    }

    void UpdateText() {
        aiDifficultyText.text = aiInfo.difficulty.ToString();
        switch (aiInfo.difficulty) {
            case 1:
            case 2:
            case 3:
                aiDifficultyText.color = Color.cyan;
                break;
            case 4:
            case 5:
            case 6:
                aiDifficultyText.color = Color.green;
                break;
            case 7:
            case 8:
            case 9:
                aiDifficultyText.color = Color.yellow;
                break;
            case 10:
                aiDifficultyText.color = Color.red;
                break;
        }
    }

}
