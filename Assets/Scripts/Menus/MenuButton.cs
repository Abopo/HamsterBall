using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Button))]
public class MenuButton : MenuOption {

    Button _button;

    public bool IsInteractable {
        get { return _button.interactable; }
    }

    protected override void Awake() {
        base.Awake();

        _button = GetComponent<Button>();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        isReady = _button.interactable;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    protected override void Select() {
        if (isReady && _button.interactable) {
            base.Select();

            GetComponent<Button>().onClick.Invoke();
        }
    }

    public void Enable() {
        _button.interactable = true;
        isReady = true;
    }
    public void Disable() {
        _button.interactable = false;
        isReady = false;
    }
}
