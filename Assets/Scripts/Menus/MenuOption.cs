using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// A base class for all menu options, i.e. buttons, sliders, etc.
// Mostly allows for any menu option to move the selector to any other kind of menu option.
[RequireComponent (typeof(AudioSource))]
public class MenuOption : MonoBehaviour {
    public GameObject selector;
    public MenuOption[] adjOptions = new MenuOption[4]; // 0 - right, 1 - down, 2 - left, 3 - up
    public bool isFirstSelection;
    public bool isReady = true;

    //protected Vector2 _selectedPos;
    public bool _isHighlighted;
    protected bool _justHighlighted; // use this to stop inputs from flowing over into multiple options.
    protected bool _moved;

    protected AudioSource _audioSource;
    AudioClip _highlightClip;
    AudioClip _selectClip;

    MenuOption[] _allOtherOptions;

    private void Awake() {
    }

    // Use this for initialization
    protected virtual void Start () {
        if (isFirstSelection) {
            _isHighlighted = true;
        } else {
            _isHighlighted = false;
        }

        //_selectedPos = transform.position;
        _moved = false;

        _audioSource = GetComponent<AudioSource>();
        _highlightClip = Resources.Load<AudioClip>("Audio/SFX/Highlight");
        _selectClip = Resources.Load<AudioClip>("Audio/SFX/Blip_Select");

        _allOtherOptions = FindObjectsOfType<MenuOption>();
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
        _isHighlighted = true;
        _justHighlighted = true;

        // Make sure it's adjacent options are NOT highlighted
        //DeHighlightAdjOptions();
        DeHighlightOtherOptions();
    }

    protected virtual void Select() {
        //PlaySelectSound();
        GetComponent<Button>().onClick.Invoke();
    }

    protected void DeHighlightAdjOptions() {
        for (int i = 0; i < 4; ++i) {
            if (adjOptions[i] != null) {
                adjOptions[i]._isHighlighted = false;
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
                    mO._isHighlighted = false;
                }
            }
        }
    }

    protected bool InputRight() {
        if (Input.GetAxis("Horizontal") > 0.3f || Input.GetAxis("Horizontal DPad") > 0.3f || 
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            return true;
        }

        return false;
    }

    protected bool InputLeft() {
        if (Input.GetAxis("Horizontal") < -0.3f || Input.GetAxis("Horizontal DPad") < -0.3f || 
            Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            return true;
        }

        return false;
    }

    protected bool InputUp() {
        if(Input.GetAxis("Vertical") < -0.3f || Input.GetAxis("Vertical DPad") > 0.3f || 
            Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            return true;
        }

        return false;
    }

    protected bool InputDown() {
        if (Input.GetAxis("Vertical") > 0.3f || Input.GetAxis("Vertical DPad") < -0.3f ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            return true;
        }

        return false;
    }

    protected bool InputReset() {
        if(Input.GetAxis("Horizontal") < 0.3f && Input.GetAxis("Horizontal") > -0.3f &&
                Input.GetAxis("Vertical") < 0.3f && Input.GetAxis("Vertical") > -0.3f &&
                Input.GetAxis("Horizontal DPad") < 0.3f && Input.GetAxis("Horizontal DPad") > -0.3f &&
                Input.GetAxis("Vertical DPad") < 0.3f && Input.GetAxis("Vertical DPad") > -0.3f) {
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
        if (isReady) {
            Highlight();
        }
    }
}
