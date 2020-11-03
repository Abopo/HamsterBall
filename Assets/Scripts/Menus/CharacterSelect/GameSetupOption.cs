using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using System.Collections;

public class GameSetupOption : MenuOption {

    public GameObject infoBox;
    public Text infoBoxText;
    public string infoText;
    public Transform textPosition;
    public Transform valuePosition;
    public Button leftButton;
    public Button rightButton;

    bool _justMoved;
    bool _isSelected;
    bool _justSelected; // use this to stop inputs from flowing over into multiple options.

    public bool IsSelected {
        get { return _isSelected; }        
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _justMoved = false;

        if(isFirstSelection) {
            Highlight();
            //_isHighlighted = true;
        } else {
            isHighlighted = false;
        }
    }

    // Update is called once per frame
    protected override void Update () {
        CheckInput();
	}

    new void CheckInput() {
        if (isHighlighted && !_isSelected) {
            base.Update();
        } 

        if(_isSelected && !_justSelected) {
            // Right
            if (!_justMoved && InputState.GetButtonOnAnyControllerPressed("Right")) {
                rightButton.onClick.Invoke();
                _justMoved = true;
            }
            // Left
            if (!_justMoved && InputState.GetButtonOnAnyControllerPressed("Left")) {
                leftButton.onClick.Invoke();
                _justMoved = true;
            }
            // B
            if (InputState.GetButtonOnAnyControllerPressed("Cancel") || InputState.GetButtonOnAnyControllerPressed("Submit")) {
                Highlight();
            }
        }

        if (InputReset()) {
            _justMoved = false;
            _justHighlighted = false;
        }

        _justSelected = false;
    }

    public override void Highlight() {
        if (selector != null) {
            selector.transform.position = new Vector3(transform.position.x,
                                                       transform.position.y,
                                                       selector.transform.position.z);

            if (!displaySelector) {
                selector.GetComponent<Image>().enabled = false;
            } else {
                selector.GetComponent<Image>().enabled = true;
            }
        }

        if (!infoBox.activeSelf) {
            infoBox.SetActive(true);
        }
        infoBox.transform.position = new Vector3(textPosition.position.x - 75f,
                                                  textPosition.position.y,
                                                  infoBox.transform.position.z);
        infoBoxText.text = infoText;

        // Play a little sound
        PlayHighlightSound();

        _justMoved = true;
        isHighlighted = true;
        _justHighlighted = true;
        _isSelected = false;

        _parentMenu.selectedOption = this;

        // Make sure it's adjacent options are NOT highlighted
        DeHighlightOtherOptions();
    }

    protected override void Select() {
        base.Select();

        selector.transform.position = new Vector3(valuePosition.position.x,
                                                   valuePosition.position.y,
                                                   selector.transform.position.z);

        // Play a little sound
        PlaySelectSound();

        _isSelected = true;
        _justSelected = true;
        isHighlighted = false;
    }

    void OnMouseEnter() {
        Highlight();
    }
}
