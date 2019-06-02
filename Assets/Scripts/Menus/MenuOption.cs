using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Rewired;

// A base class for all menu options, i.e. buttons, sliders, etc.
// Mostly allows for any menu option to move the selector to any other kind of menu option.
[RequireComponent (typeof(AudioSource))]
public class MenuOption : MonoBehaviour {
    public GameObject selector;
    public MenuOption[] adjOptions = new MenuOption[4]; // 0 - right, 1 - down, 2 - left, 3 - up
    public bool isFirstSelection;
    public bool isReady = true;

    //protected Vector2 _selectedPos;
    public bool isHighlighted;
    protected bool _justHighlighted; // use this to stop inputs from flowing over into multiple options.
    protected bool _moved;

    protected Player _player;

    protected AudioSource _audioSource;
    AudioClip _highlightClip;
    AudioClip _selectClip;

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

        _audioSource = GetComponent<AudioSource>();
        _highlightClip = Resources.Load<AudioClip>("Audio/SFX/Highlight");
        _selectClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");

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
            if (_player.GetButtonDown("Submit")) {
                Select();
            }
        }

        if (!_moved && isHighlighted && !_justHighlighted) {
            // Right
            if (InputRight()) {
                TryHighlight(0);
            }
            // Down
            if (InputDown()) {
                TryHighlight(1);
            }
            // Left
            if (InputLeft()) {
                TryHighlight(2);
            }
            // Up
            if (InputUp()) {
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
        if (isReady) {
            GetComponent<Button>().onClick.Invoke();
        }
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

    protected bool InputRight() {
        if (_player.GetAxis("Horizontal0") > 0.3f) {
            return true;
        }

        return false;
    }

    protected bool InputLeft() {
        if (_player.GetAxis("Horizontal0") < -0.3f) {
            return true;
        }

        return false;
    }

    protected bool InputUp() {
        if(_player.GetAxis("Vertical") > 0.3f) {
            return true;
        }

        return false;
    }

    protected bool InputDown() {
        if (_player.GetAxis("Vertical") < -0.3f) {
            return true;
        }

        return false;
    }

    protected bool InputReset() {
        if(_player.GetAxis("Horizontal0") < 0.3f && _player.GetAxis("Horizontal0") > -0.3f &&
           _player.GetAxis("Vertical") < 0.3f && _player.GetAxis("Vertical") > -0.3f) {
            return true;
        }

        return false;
    }

    public void PlayHighlightSound() {
        //if (_audioSource != null) {
            //_audioSource.clip = _highlightClip;
            //_audioSource.Play();
        //}
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuHighlight);
    }

    public void PlaySelectSound() {
        //if (_audioSource != null) {
            //_audioSource.clip = _selectClip;
            //_audioSource.Play();
        //}
		FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuSelect);
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
