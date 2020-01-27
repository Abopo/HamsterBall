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

    private void Awake() {
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
