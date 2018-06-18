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
            if(_slidingIn || _slidingOut) {
                return true;
            } else {
                return false;
            }
        }
    }

    float _slideSpeed = 20.0f;
    bool _slidingIn = false;
    bool _slidingOut = false;

    bool _isSpeaking;

    Image _image;
    SpriteRenderer _speakerArrow;
    RectTransform _rectTransform;
    CutsceneManager _cutsceneManager;

    private void Awake() {
        _image = GetComponent<Image>();
        _speakerArrow = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _rectTransform = GetComponent<RectTransform>();
        _cutsceneManager = FindObjectOfType<CutsceneManager>();

        SetIsSpeaking(false);
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(_slidingIn) {
            _speakerArrow.enabled = false;
            SlidingIn();
        } else if (_slidingOut) {
            _speakerArrow.enabled = false;
            SlidingOut();
        }
    }

    void SlidingIn() {
        if (side < 0) {
            _rectTransform.Translate(_slideSpeed * Time.unscaledDeltaTime, 0f, 0f);
            if (_rectTransform.localPosition.x >= screenPos) {
                SlideInFinish();
            }
        } else if (side > 0) {
            _rectTransform.Translate(-_slideSpeed * Time.unscaledDeltaTime, 0f, 0f);
            if (_rectTransform.localPosition.x <= screenPos) {
                SlideInFinish();
            }
        }
    }

    void SlideInFinish() {
        _rectTransform.localPosition = new Vector3(screenPos, _rectTransform.localPosition.y, _rectTransform.localPosition.z);
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
            _rectTransform.Translate(-_slideSpeed * Time.unscaledDeltaTime, 0f, 0f);
            if (_rectTransform.localPosition.x <= offScreenPos) {
                SlideOutFinish();
            }
        } else if(side > 0) {
            _rectTransform.Translate(_slideSpeed * Time.unscaledDeltaTime, 0f, 0f);
            if (_rectTransform.localPosition.x >= offScreenPos) {
                SlideOutFinish();
            }
        }
    }

    void SlideOutFinish() {
        _rectTransform.localPosition = new Vector3(offScreenPos, _rectTransform.localPosition.y, _rectTransform.localPosition.z);
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
}
