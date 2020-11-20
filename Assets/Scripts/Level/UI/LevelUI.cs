using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {
    public Text levelTimer;

    public SuperTextMesh marginMultiplierText;
    float _marginTimer = 0;
    float _marginTime = 120f;

    GameMarker[] _gameMarkers;

    GameManager _gameManager;
    LevelManager _levelManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameManager.instance;
        _levelManager = FindObjectOfType<LevelManager>();

        SetupGameMarkers();
	}

    public void SetupGameMarkers() {
        _gameMarkers = GetComponentsInChildren<GameMarker>();

        // Clear all the game markers first
        foreach (GameMarker gM in _gameMarkers) {
            gM.FillOut();
        }

        // If we're playing team survival mode
        if (_gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            // Gotta turn off all the stuff in the upper center wall area
            foreach(GameMarker gM in _gameMarkers) {
                gM.gameObject.SetActive(false);
            }
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        } else {
            if (_gameManager.leftTeamGames > 0) {
                FillInGameMarker(0);
            }
            if (_gameManager.rightTeamGames > 0) {
                FillInGameMarker(1);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (levelTimer != null && _levelManager != null) {
            int seconds = Mathf.FloorToInt(_levelManager.LevelTimer % 60);
            int minutes = Mathf.FloorToInt(_levelManager.LevelTimer / 60);
            levelTimer.text = string.Format("{0}:{1:00}", minutes, seconds);
        }

        if (_gameManager.gameMode == GAME_MODE.MP_VERSUS || _gameManager.gameMode == GAME_MODE.MP_PARTY) {
            // Update margin stuff
            _marginTimer += Time.deltaTime;
            if (_marginTimer >= _marginTime) {
                IncreaseMarginMultiplier();
                _marginTime = 30f;
                _marginTimer = 0f;
            }
        }

        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.N)) {
            IncreaseMarginMultiplier();
        }
    }

    public void FillInGameMarker(int team) {
        //Debug.Log("Should fill in");
        foreach(GameMarker gM in _gameMarkers) {
            if(gM.team == team && !gM.isFilledIn) {
                gM.FillIn();
                break;
            }
        }
    }

    void IncreaseMarginMultiplier() {
        if (marginMultiplierText == null) {
            return;
        }

        float marginMultiplier = Bubble.marginMultiplier;

        marginMultiplier += 0.5f;
        if (marginMultiplier > 5) {
            marginMultiplier = 5;
        }

        Bubble.marginMultiplier = marginMultiplier;

        Color outColor = Color.black;

        if (marginMultiplier == 1f) {
            marginMultiplierText.color = Color.black;
        } else if (marginMultiplier == 1.5f) {
            ColorUtility.TryParseHtmlString("#3131FF", out outColor);
            marginMultiplierText.size = 22;
        } else if (marginMultiplier == 2f) {
            ColorUtility.TryParseHtmlString("#36FDFD", out outColor);
            //outColor = Color.cyan;
            marginMultiplierText.size = 24;
        } else if (marginMultiplier == 2.5f) {
            ColorUtility.TryParseHtmlString("#4CFF4C", out outColor);
            //outColor = Color.green;
            marginMultiplierText.size = 22;
        } else if (marginMultiplier == 3f) {
            ColorUtility.TryParseHtmlString("#E000B0", out outColor);
            //outColor = Color.magenta;
            marginMultiplierText.size = 24;
        } else if (marginMultiplier == 3.5f) {
            ColorUtility.TryParseHtmlString("#FDEE3A", out outColor);
            //outColor = Color.yellow;
            marginMultiplierText.size = 22;
        } else if (marginMultiplier == 4f) {
            ColorUtility.TryParseHtmlString("#E91919", out outColor);
            //outColor = Color.red;
            marginMultiplierText.size = 24;
        } else if (marginMultiplier == 4.5f) {
            ColorUtility.TryParseHtmlString("#E91919", out outColor);
            //outColor = Color.red;
            marginMultiplierText.size = 21;
        } else if (marginMultiplier >= 5f) {
            ColorUtility.TryParseHtmlString("#E91919", out outColor);
            marginMultiplierText.size = 30;
        }

        marginMultiplierText.color = outColor;

        marginMultiplierText.text = "<w=expand>x" + marginMultiplier.ToString();
    }
}
