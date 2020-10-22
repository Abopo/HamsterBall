using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AISetupOption : MenuOption {
    public PlayerInfo aiInfo;
    public SuperTextMesh aiDifficultyText;

    Selectable _selectable;
    bool _justMoved;

    protected override void Awake() {
        base.Awake();

        _selectable = GetComponent<Selectable>();
    }
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
            if (InputState.GetButtonOnAnyControllerPressed("Right")) {
                IncreaseAIDifficulty();
                _justMoved = true;
            }
            // Left
            if (InputState.GetButtonOnAnyControllerPressed("Left")) {
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

    public override void Highlight() {
        base.Highlight();

        _selectable.Select();
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

        aiDifficultyText.text = aiInfo.difficulty.ToString();
    }

}
