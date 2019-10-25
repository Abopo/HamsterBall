using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Rewired;

public enum MENUTYPE { MAIN = 0, SUB, NUM_TYPES };

// A base class for all menu options, i.e. buttons, sliders, etc.
// Mostly allows for any menu option to move the selector to any other kind of menu option.
public class MenuOption : MonoBehaviour {
    public GameObject selector;
    public MenuOption[] adjOptions = new MenuOption[4]; // 0 - right, 1 - down, 2 - left, 3 - up
    public MENUTYPE menuType;
    public bool isFirstSelection;
    public bool isReady = true;

    //protected Vector2 _selectedPos;
    public bool isHighlighted;
    protected bool _justHighlighted; // use this to stop inputs from flowing over into multiple options.
    protected bool _moved;

    protected Player _player;

    MenuOption[] _allOtherOptions;

    protected virtual void Awake() {
        if (isFirstSelection) {
            isHighlighted = true;
            _justHighlighted = true;
        } else {
            isHighlighted = false;
            _justHighlighted = false;
        }
    }

    // Use this for initialization
    protected virtual void Start () {
        //_selectedPos = transform.position;
        _moved = true;

        if (_player == null) {
            _player = ReInput.players.GetPlayer(0);
        }

        _allOtherOptions = FindObjectsOfType<MenuOption>();
    }

    public void SetPlayer(int playerID) {
        _player = ReInput.players.GetPlayer(playerID);
    }

    // Update is called once per frame
    protected virtual void Update () {
        if(!isReady) {
            return;
        }

        if (isHighlighted && !_justHighlighted) {
            if (InputState.GetButtonOnAnyControllerPressed("Submit")) {
                Select();
            }
        }

        if (!_moved && isHighlighted && !_justHighlighted) {
            // Right
            if (InputState.GetButtonOnAnyControllerPressed("MoveRight")) {
                TryHighlight(0);
            }
            // Down
            if (InputState.GetButtonOnAnyControllerPressed("MoveDown")) {
                TryHighlight(1);
            }
            // Left
            if (InputState.GetButtonOnAnyControllerPressed("MoveLeft")) {
                TryHighlight(2);
            }
            // Up
            if (InputState.GetButtonOnAnyControllerPressed("MoveUp")) {
                TryHighlight(3);
            }
        } else {
            if (InputReset()) {
                _moved = false;
                _justHighlighted = false;
            }
        }
    }

    void TryHighlight(int index) {
        if (adjOptions[index] != null) {
            if (adjOptions[index].isReady) {
                _moved = true;
                adjOptions[index].Highlight();
            } else {
                // Search for a valid option in the same direction
                MenuOption validOption = FindValidOption(index);
                if (validOption != null) {
                    validOption.Highlight();
                }
            }
        }
    }

    public virtual void Highlight() {
        if(!isReady) {
            return;
        }

        if (selector != null) {
            selector.transform.position = new Vector3(transform.position.x,
                                                       transform.position.y,
                                                       selector.transform.position.z);
        }

        // Play a little sound
        PlayHighlightSound();

        _moved = true;
        isHighlighted = true;
        _justHighlighted = true;

        // Make sure it's adjacent options are NOT highlighted
        DeHighlightOtherOptions();
    }

    protected virtual void Select() {
        //PlaySelectSound();
    }

    protected void DeHighlightAdjOptions() {
        for (int i = 0; i < 4; ++i) {
            if (adjOptions[i] != null) {
                adjOptions[i].isHighlighted = false;
            }
        }
    }

    MenuOption FindValidOption(int index) {
        MenuOption validOption = adjOptions[index];

        while(validOption != null) {
            if(validOption.isReady) {
                break;
            }

            validOption = validOption.adjOptions[index];
        }

        return validOption;
    }

    protected void DeHighlightOtherOptions() {
        if (_allOtherOptions != null) {
            foreach (MenuOption mO in _allOtherOptions) {
                if (mO != this && mO.isReady) {
                    mO.Unhighlight();
                }
            }
        }
    }

    public virtual void Unhighlight() {
        isHighlighted = false;
    }

    protected bool AnyInput() {
        if (InputState.GetButtonOnAnyControllerPressed("MoveRight") ||
            InputState.GetButtonOnAnyControllerPressed("MoveLeft") ||
            InputState.GetButtonOnAnyControllerPressed("MoveUp") ||
            InputState.GetButtonOnAnyControllerPressed("MoveDown")) {
            return true;
        }

        return false;
    }
    protected bool InputReset() {
        if(!InputState.GetButtonOnAnyControllerPressed("MoveRight") && 
            !InputState.GetButtonOnAnyControllerPressed("MoveLeft") &&
            !InputState.GetButtonOnAnyControllerPressed("MoveUp") && 
            !InputState.GetButtonOnAnyControllerPressed("MoveDown")) {
            return true;
        }

        return false;
    }

    public void PlayHighlightSound() {
        //if (_audioSource != null) {
        //_audioSource.clip = _highlightClip;
        //_audioSource.Play();
        //}
        switch (menuType) {
            case MENUTYPE.MAIN:
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuHighlight);
                break;
            case MENUTYPE.SUB:
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuHighlight);
                break;
        }
    }

    public void PlaySelectSound() {
        switch (menuType) {
            case MENUTYPE.MAIN:
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuSelect);
                break;
            case MENUTYPE.SUB:
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuSelect);
                break;
        }
    }

    void OnMouseEnter() {
        if (isReady) {
            Highlight();
        }
    }

    private void OnEnable() {
        if(isHighlighted) {
            _justHighlighted = true;
        }
    }
}
