using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlider : MenuOption {

    protected Slider _slider;

    protected override void Awake() {
        base.Awake();

        _slider = GetComponent<Slider>();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public override void CheckInput() {
        base.CheckInput();

        // Right
        if (_player.GetButtonRepeating("Right")) {
            // Mode slider right
            _slider.value += 5;
            if (_slider.value > 100) {
                _slider.value = 100;
            }
        }
        // Left
        if (_player.GetButtonRepeating("Left")) {
            // Move slider left
            _slider.value -= 5;
            if (_slider.value < 0) {
                _slider.value = 0;
            }
        }
    }

    protected override void Select() {
        if (IsReady && _slider.interactable) {
            base.Select();
        }
    }

    public override void Highlight() {
        if (!IsReady) {
            return;
        }

        base.Highlight();

        if(_slider == null) {
            _slider = GetComponent<Slider>();
        }

        _slider.Select();
    }
}
