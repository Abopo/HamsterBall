using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Team : MonoBehaviour {

    public GameObject hamsterMeter;
    public GameObject hamsterTallyObj;

    public int numPlayers;
    public int team; // -1 = no team, 0 = left team, 1 = right team
    public int handicap;

    Character _character1;
    Character _character2;

    Vector2 _char1Pos;
    Vector2 _char2Pos;

    GameManager _gameManager;

    private void Awake() {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        numPlayers = 0;

        _char1Pos = transform.GetChild(0).position;
        _char2Pos = transform.GetChild(1).position;
    }

    // Use this for initialization
    void Start () {
        //readySprite1.gameObject.SetActive(false);
        //readySprite2.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public bool HasSpace() {
        if (_character1 == null || _character2 == null) {
            return true;
        } else {
            return false;
        }
    }

    public void TakeCharacter(Character character) {
        // Set first open character to input character
        if(_character1 == null) {
            _character1 = character;
            // Move character sprite to corresponding position
            _character1.transform.position = _char1Pos;
            //SetReadyText(readySprite1, character);
            numPlayers++;
        } else if(_character2 == null) {
            _character2 = character;
            // Move character sprite to corresponding position
            _character2.transform.position = _char2Pos;
            //SetReadyText(readySprite2, character);
            numPlayers++;
        }
    }

    void SetReadyText(SpriteRenderer rSprite, Character chara) {
        // Show Ready Text

        rSprite.gameObject.SetActive(true);
        if(chara.isAI) {
            rSprite.transform.GetChild(0).gameObject.SetActive(false);
        } else {
            rSprite.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void LoseCharacter(Character character) {
        if (_character1 == character) {
            _character1 = null;
            //readySprite1.gameObject.SetActive(false);
            numPlayers--;
        } else if(_character2 == character) {
            _character2 = null;
            //readySprite2.gameObject.SetActive(false);
            numPlayers--;
        }
    }

    public void IncreaseHandicap() {
        // Check current handicap
        // If below max
        if (handicap < 12) {
            // Adjust Hamster Meter accordingly
            // - Add a sprite
            GameObject hamsterTally = GameObject.Instantiate(hamsterTallyObj, hamsterMeter.transform) as GameObject;
            hamsterTally.transform.position = new Vector3(hamsterMeter.transform.position.x,
                                                            hamsterMeter.transform.GetChild(hamsterMeter.transform.childCount - 2).transform.position.y + 0.95f,
                                                            hamsterMeter.transform.position.z);
            // - Adjust position of meter to be centered.
            float y = 1.9f - ((hamsterMeter.transform.childCount - 6) * 0.5f);
            hamsterMeter.transform.position = new Vector3(hamsterMeter.transform.position.x,
                                                            y,
                                                            hamsterMeter.transform.position.z);
            handicap = hamsterMeter.transform.childCount;
            _gameManager.SetTeamHandicap(team, handicap);
            //HandicapCountText.text = handicap.ToString();
        }
    }

    public void DecreaseHandicap() {
        // Check current handicap
        // If above minimum
        if (handicap > 6) {
            // Adjust Hamster Meter accordingly
            // - Remove a sprite
            DestroyImmediate(hamsterMeter.transform.GetChild(hamsterMeter.transform.childCount - 1).gameObject);
            // - Adjust position of meter to be centered.
            float y = 1.9f - ((hamsterMeter.transform.childCount - 6) * 0.5f);
            hamsterMeter.transform.position = new Vector3(hamsterMeter.transform.position.x,
                                                            y,
                                                            hamsterMeter.transform.position.z);
            // for some reason, the hamsterMeter doesn't update it's childCount immediately?s
            handicap = hamsterMeter.transform.childCount;
            _gameManager.SetTeamHandicap(team, handicap);
            //HandicapCountText.text = handicap.ToString();
        }
    }

    public void FailCheckHandicap() {
        if (handicap >= 13) {
            while (handicap > 13) {
                DecreaseHandicap();
            }
        }
        if (handicap <= 6) {
            while (handicap < 6) {
                IncreaseHandicap();
            }
        }
    }

    public string GetCharacterName(Character chara) {
        string nickName = "";
        if(_character1 = chara) {
            //nickName =  readyText1.text;
        } else if(_character2 == chara) {
            //return readyText2.text;
        }

        return nickName; 
    }
}
