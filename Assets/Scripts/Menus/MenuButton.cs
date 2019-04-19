using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent (typeof(Button))]
public class MenuButton : MenuOption {

    Button _button;

    Vector3 _baseScale;

    public bool IsInteractable {
        get { return _button.interactable; }
    }

    protected override void Awake() {
        base.Awake();

        _baseScale = transform.localScale;

        _button = GetComponent<Button>();
        if(isFirstSelection) {
            ButtonHighlight();
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        isReady = _button.interactable;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        // If nothing is selected and we are the default selection
        if (EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject == null && isFirstSelection) {
            // If we get any button input
            if (InputRight() || InputLeft() || InputUp() || InputDown()) {
                // Highlight the first selection
                Highlight();
            }
        }
    }

    protected override void Select() {
        if (isReady && _button.interactable) {
            base.Select();
        }
    }

    public override void Highlight() {
        base.Highlight();

        ButtonHighlight();
    }

    void ButtonHighlight() {
        if (_button == null) {
            _button = GetComponent<Button>();
        }

        _button.Select();
        transform.localScale = new Vector3(transform.localScale.x * 1.1f,
                                            transform.localScale.y * 1.1f,
                                            transform.localScale.z);
    }

    public override void Unhighlight() {
        base.Unhighlight();

        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        transform.localScale = _baseScale;
    }

    public void Enable() {
        if (_button == null) {
            _button = GetComponent<Button>();
        }

        _button.interactable = true;
        isReady = true;
    }
    public void Disable() {
        if(_button == null) {
            _button = GetComponent<Button>();
        }

        _button.interactable = false;
        isReady = false;
    }
}
