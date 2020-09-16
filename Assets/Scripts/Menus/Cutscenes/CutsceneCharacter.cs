using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum CHARACTERS { BOY = 0, GIRL, OVERSEER, OWL, GOAT, SNAIL, ROOSTER, BAT, NUM_CHARACTERS };

public class CutsceneCharacter : MonoBehaviour {
    public string curCharacter;
    public string curExpression;
    public bool onScreen;
    public int side; // Which side of the screen the character should be on: -1 == left; 1 == right

    public string charaToChangeTo = "Clear";
    public string expressionToChangeTo = "Clear";

    public float screenPos;
    public float offScreenPos;

    public bool IsMoving {
        get {
            if(_slidingIn || _slidingOut || _walkingScript.isWalking) {
                return true;
            } else {
                return false;
            }
        }
    }


    float _slideSpeed = 15.0f;
    bool _slidingIn = false;
    bool _slidingOut = false;

    bool _isSpeaking;

    float _fakeDeltaTime = 0.02f;

    SpriteRenderer _sprite;
    SpriteRenderer _speakerArrow;
    CutsceneManager _cutsceneManager;

    // Event scripts
    WalkingScript _walkingScript;

    private void Awake() {
        _sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _speakerArrow = transform.GetChild(1).GetComponent<SpriteRenderer>();
        _cutsceneManager = FindObjectOfType<CutsceneManager>();

        _walkingScript = GetComponent<WalkingScript>();

        SetIsSpeaking(false);
    }

    // Use this for initialization
    void Start () {
        //transform.Translate(_slideSpeed * Time.unscaledDeltaTime, 0f, 0f);
        //_rectTransform.anchoredPosition = new Vector2(offScreenPos, _rectTransform.localPosition.y);
    }

    // Update is called once per frame
    void Update () {
        if (_slidingIn) {
            _speakerArrow.enabled = false;
            SlidingIn();
        } else if (_slidingOut) {
            _speakerArrow.enabled = false;
            SlidingOut();
        }
    }
    
    void SlidingIn() {
        if (side < 0) {
            transform.Translate(_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (transform.position.x >= screenPos) {
                EnterFinish();
            }
        } else if (side > 0) {
            transform.Translate(-_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (transform.position.x <= screenPos) {
                EnterFinish();
            }
        }
    }

    public void EnterFinish() {
        transform.position = new Vector2(screenPos, transform.position.y);
        _slidingIn = false;
        onScreen = true;

        if(_isSpeaking) {
            // Show the speaker arrow
            _speakerArrow.enabled = true;
        }

        // Continue the cutscene
        _cutsceneManager.ReadEscapeCharacter();
    }

    void SlidingOut() {
        if (side < 0) {
            transform.Translate(-_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (transform.position.x <= offScreenPos) {
                SlideOutFinish();
            }
        } else if(side > 0) {
            transform.Translate(_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (transform.position.x >= offScreenPos) {
                SlideOutFinish();
            }
        }
    }

    void SlideOutFinish() {
        transform.position = new Vector2(offScreenPos, transform.position.y);
        _slidingOut = false;
        onScreen = false;

        // If we need to change to a new character
        if (charaToChangeTo != "Clear") {
            // Change to that character
            SetCharacter(charaToChangeTo, expressionToChangeTo);
            charaToChangeTo = "Clear";
            // Slide back in
            SlideIn();
        } else if(charaToChangeTo == "Clear") {
            // Continue the cutscene
            _cutsceneManager.ReadEscapeCharacter();
        }
    }

    public void SlideIn() {
        _slidingIn = true;
        _slidingOut = false;
    }

    public void SlideOut() {
        _slidingOut = true;
        _slidingIn = false;
    }

    public void StopSliding() {
        _slidingIn = false;
        _slidingOut = false;
    }

    public void WalkIn() {
        _speakerArrow.enabled = false;
        _walkingScript.StartWalking();
    }

    public void GoOffscreen() {
        transform.position = new Vector2(offScreenPos, transform.position.y);
        onScreen = false;
    }

    public void SetCharacter(string chara, string expression) {
        curCharacter = chara;

        // Load the image of the character
        SetExpression(expression);
    }

    public void SetExpression(string expression) {
        // TODO: This is maybe inefficient? idk
        string fullFilePath = "Art/Characters/" + curCharacter + "/" + curCharacter + "_" + expression;
        _sprite.sprite = Resources.Load<Sprite>(fullFilePath);
        // If we tried to load an expression that doesn't exist
        if (_sprite.sprite == null) {
            // Just load the neutral expression
            fullFilePath = "Art/Characters/" + curCharacter + "/" + curCharacter + "_Neutral";
            _sprite.sprite = Resources.Load<Sprite>(fullFilePath);
        }
    }

    public void SetFacing(int facing) {
        transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
    }

    public void SetIsSpeaking(bool speaking) {
        _isSpeaking = speaking;

        if(_isSpeaking) {
            _speakerArrow.enabled = true;
        } else {
            _speakerArrow.enabled = false;
        }
    }

    public void Translate(float deltaX, float deltaY) {
        transform.Translate(deltaX, deltaY, 0f);
    }
}
