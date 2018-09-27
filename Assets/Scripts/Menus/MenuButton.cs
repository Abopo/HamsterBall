using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Button))]
public class MenuButton : MenuOption {

    Button _button;

    private void Awake() {
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
        if (isReady) {
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
