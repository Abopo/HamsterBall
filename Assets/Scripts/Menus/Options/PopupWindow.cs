using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : Menu {
    public Menu parentMenu;

    float _slideSpd = 1500f;
    int _slideDir;
    bool _sliding;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(_sliding) {
            transform.Translate(_slideSpd * _slideDir * Time.deltaTime, 0f, 0f);
            CheckSlidePos();
        } else {
            if(InputState.GetButtonOnAnyControllerPressed("Cancel")) {
                ClosePopup();
            }
        }
    }

    void CheckSlidePos() {
        // If sliding in
        if (_slideDir < 0 && transform.localPosition.x <= 0) {
            // End the slide
            _sliding = false;
            transform.localPosition = new Vector3(0f, 0f, 0f);
            TakeFocus();

        // If sliding out
        } else if(_slideDir > 0 && transform.localPosition.x > 725) {
            // End the slide
            _sliding = false;
            transform.localPosition = new Vector3(725f, 0f, 0f);
            parentMenu.TakeFocus();
        }
    }

    public void OpenPopup() {
        // Disable parent menu
        parentMenu.LoseFocus();

        // Slide in popup
        SlideIn();
    }

    public void ClosePopup() {
        // Disable self
        LoseFocus();

        // Slide out
        SlideOut();
    }

    void SlideIn() {
        _sliding = true;
        _slideDir = -1;
    }

    void SlideOut() {
        _sliding = true;
        _slideDir = 1;
    }
}
