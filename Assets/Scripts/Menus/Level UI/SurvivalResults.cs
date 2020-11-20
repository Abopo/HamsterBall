using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalResults : MonoBehaviour {
    public SuperTextMesh timeText1;
    public SuperTextMesh timeText2;
    public NumberTick currencyText;

    int _gameTime;

    MenuOption[] _menuOptions;

    float _winTime = 1f;
    float _winTimer = 0.8f;
    bool _canInteract = false;

    GameManager _gameManager;
    LevelManager _levelManager;

    private void Awake() {
        _gameManager = GameManager.instance;
        _levelManager = FindObjectOfType<LevelManager>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        _winTimer += Time.unscaledDeltaTime;
        if (_winTimer > _winTime) {
            _canInteract = true;

            if (_menuOptions == null || _menuOptions.Length == 0) {
                _menuOptions = transform.GetComponentsInChildren<MenuOption>();
            } else {
                foreach (MenuOption mo in _menuOptions) {
                    if (mo != null) {
                        mo.isReady = true;
                    }
                }
            }
        }
    }

    public void Activate() {
        gameObject.SetActive(true);

        _menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in _menuOptions) {
            mo.isReady = false;
        }

        _gameTime = (int)_levelManager.LevelTimer;

        SetTimeTexts();
        SetCurrency();

        _winTimer = 0.8f;
        _canInteract = false;
    }

    void SetTimeTexts() {

        int seconds = Mathf.FloorToInt(_gameTime % 60);
        int minutes = Mathf.FloorToInt(_gameTime / 60);
        timeText1.text = "This Time: " + string.Format("{0}:{1:00}", minutes, seconds);

        // TODO: Save and load best times
        timeText2.text = "Best Time: " + string.Format("{0}:{1:00}", minutes, seconds);
    }

    void SetCurrency() {
        int gainedCurrency = 0;

        // The currency gained is based on the time and # of players
        gainedCurrency = _gameTime * 2;

        currencyText.StartTick(0, gainedCurrency);

        // Add and save currency based on score
        int totalCurrency = ES3.Load<int>("Currency", 0);
        totalCurrency += gainedCurrency;
        ES3.Save<int>("Currency", totalCurrency);
    }

    public void PlayAgain() {
        if (!_canInteract) {
            return;
        }

        _gameManager.PlayAgainButton();
    }

    public void CharacterSelect() {
        if (!_canInteract) {
            return;
        }

        _gameManager.CharacterSelectButton();
    }
}
