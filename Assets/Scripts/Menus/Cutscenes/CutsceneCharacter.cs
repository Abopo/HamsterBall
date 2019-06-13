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

    public bool IsSliding {
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

    Image _image;
    SpriteRenderer _speakerArrow;
    CutsceneManager _cutsceneManager;
    RectTransform _rectTransform;
    public RectTransform RectTransform {
        get { return _rectTransform; }
    }

    // Event scripts
    WalkingScript _walkingScript;

    private void Awake() {
        _image = GetComponent<Image>();
        _speakerArrow = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _cutsceneManager = FindObjectOfType<CutsceneManager>();
        _rectTransform = GetComponent<RectTransform>();

        _walkingScript = GetComponent<WalkingScript>();

        SetIsSpeaking(false);
    }

    // Use this for initialization
    void Start () {
        _rectTransform.Translate(_slideSpeed * Time.unscaledDeltaTime, 0f, 0f);
        _rectTransform.anchoredPosition = new Vector2(offScreenPos, _rectTransform.localPosition.y);
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
            _rectTransform.Translate(_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (_rectTransform.anchoredPosition.x >= screenPos) {
                SlideInFinish();
            }
        } else if (side > 0) {
            _rectTransform.Translate(-_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (_rectTransform.anchoredPosition.x <= screenPos) {
                SlideInFinish();
            }
        }
    }

    void SlideInFinish() {
        _rectTransform.anchoredPosition = new Vector2(screenPos, _rectTransform.localPosition.y);
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
            _rectTransform.Translate(-_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (_rectTransform.anchoredPosition.x <= offScreenPos) {
                SlideOutFinish();
            }
        } else if(side > 0) {
            _rectTransform.Translate(_slideSpeed * _fakeDeltaTime, 0f, 0f);
            if (_rectTransform.anchoredPosition.x >= offScreenPos) {
                SlideOutFinish();
            }
        }
    }

    void SlideOutFinish() {
        _rectTransform.anchoredPosition = new Vector2(offScreenPos, _rectTransform.localPosition.y);
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

    public void GoOffscreen() {
        _rectTransform.anchoredPosition = new Vector2(offScreenPos, _rectTransform.localPosition.y);
        onScreen = false;
    }

    public void SetCharacter(string chara, string expression) {
        curCharacter = chara;

        // Get the characters name'
        /*
        string charaName = "";
        int i = 0;
        while(chara[i] != '_') {
            charaName += chara[i];
            i++;
        }
        */

        // Load the image of the character
        // TODO: This is maybe inefficient? idk
        string fullFilePath = "Art/Characters/" + curCharacter + "/" + curCharacter + "_" + expression;
        _image.sprite = Resources.Load<Sprite>(fullFilePath);
        _image.SetNativeSize();
    }

    public void SetExpression(string expression) {
        string fullFilePath = "Art/Characters/" + curCharacter + "/" + curCharacter + "_" + expression;
        _image.sprite = Resources.Load<Sprite>(fullFilePath);
        _image.SetNativeSize();
    }

    public void SetFacing(int facing) {
        _rectTransform.localScale = new Vector3(facing, _rectTransform.localScale.y, _rectTransform.localScale.z);
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
        _rectTransform.Translate(deltaX, deltaY, 0f);
    }
}
