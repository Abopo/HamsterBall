using UnityEngine;
using System.Collections;

// A base class for all menu options, i.e. buttons, sliders, etc.
// Mostly allows for any menu option to move the selector to any other kind of menu option.
[RequireComponent (typeof(AudioSource))]
public class MenuOption : MonoBehaviour {
    public GameObject selector;
    public MenuOption[] adjOptions = new MenuOption[4];
    public bool isFirstSelection;
    public bool isReady = true;

    //protected Vector2 _selectedPos;
    protected bool _isHighlighted;
    protected bool _justHighlighted; // use this to stop inputs from flowing over into multiple options.
    bool _moved;

    protected AudioSource _audioSource;
    AudioClip _highlightClip;
    AudioClip _selectClip;

    // Use this for initialization
    protected virtual void Start () {
        //_selectedPos = transform.position;
        _moved = false;

        if (isFirstSelection) {
            _isHighlighted = true;
        } else {
            _isHighlighted = false;
        }

        _audioSource = GetComponent<AudioSource>();
        _highlightClip = Resources.Load<AudioClip>("Audio/SFX/Highlight");
        _selectClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");
    }

    // Update is called once per frame
    protected virtual void Update () {
        if(!isReady) {
            return;
        }

        if (_isHighlighted) {
            if (Input.GetButtonDown("Submit")) {
                Select();
            }
        }

        if (!_moved && _isHighlighted && !_justHighlighted) {
            // Right
            if (InputRight()) {
                if (adjOptions[0] != null && adjOptions[0].isReady) {
                    //_isHighlighted = false;
                    _moved = true;
                    // move selector to adjOptions[0]
                    adjOptions[0].Highlight();
                }
            }
            // Left
            if (InputLeft()) {
                if (adjOptions[2] != null && adjOptions[2].isReady) {
                    //_isHighlighted = false;
                    _moved = true;
                    // move selector to adjOptions[2]
                    adjOptions[2].Highlight();
                }
            }
            // Up
            if (Input.GetAxis("Vertical") < -0.3f || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                if (adjOptions[3] != null && adjOptions[3].isReady) {
                    //_isHighlighted = false;
                    _moved = true;
                    // move selector to adjOptions[3]
                    adjOptions[3].Highlight();
                }
            }
            // Down
            if (Input.GetAxis("Vertical") > 0.3f || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                if (adjOptions[1] != null && adjOptions[4].isReady) {
                    //_isHighlighted = false;
                    _moved = true;
                    // move selector to adjOptions[1]
                    adjOptions[1].Highlight();
                }
            }
        } else {
            if (Input.GetAxis("Horizontal") < 0.3f && Input.GetAxis("Horizontal") > -0.3f &&
                Input.GetAxis("Vertical") < 0.3f && Input.GetAxis("Vertical") > -0.3f) {
                _moved = false;
                _justHighlighted = false;
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
        _isHighlighted = true;
        _justHighlighted = true;

        // Make sure it's adjacent options are NOT highlighted
        DeHighlightAdjOptions();
    }

    protected virtual void Select() {
        PlaySelectSound();
    }

    protected void DeHighlightAdjOptions() {
        for (int i = 0; i < 4; ++i) {
            if (adjOptions[i] != null) {
                adjOptions[i]._isHighlighted = false;
            }
        }
    }

    protected bool InputRight() {
        if (Input.GetAxis("Horizontal") > 0.3f || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            return true;
        }

        return false;
    }

    protected bool InputLeft() {
        if (Input.GetAxis("Horizontal") < -0.3f || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            return true;
        }

        return false;
    }

    public void PlayHighlightSound() {
        if (_audioSource != null) {
            _audioSource.clip = _highlightClip;
            _audioSource.Play();
        }
    }

    public void PlaySelectSound() {
        if (_audioSource != null) {
            _audioSource.clip = _selectClip;
            _audioSource.Play();
        }
    }

    void OnMouseEnter() {
        Highlight();
    }

}
