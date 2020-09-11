using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class HowToPlayMenu : Menu {

    public GameObject page1;
    public GameObject page2;

    public SuperTextMesh catText;
    public SuperTextMesh swapText;
    public SuperTextMesh attackText;

    bool _controller;

    protected override void Awake() {
        base.Awake();
    }
    
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        if (ReInput.controllers.joystickCount > 0) {
            _controller = true;
        }

        SetupText();
    }

    void SetupText() {
        if(_controller) {
            catText.text = "Press <c=blue>X<c=black> to catch hamsters\nHolding a hamster press <c=blue>X<c=black> to aim\nLine up a good shot and press<c=blue>X<c=black> one more time to throw!";
            swapText.text = "When your meter is full press <c=yellow>Y<c=black> to SWAP into your opponent's area.\nHarass them by throwing at their board or stealing their hamsters!";
            attackText.text = "If someone has swapped into your area, attack them with the <c=red>B<c=black> button!";
        } else {
            catText.text = "Press <c=blue>J<c=black> to catch hamsters\nHolding a hamster press<c=blue>J<c=black> to aim\nLine up a good shot and press <c=blue>J<c=black> one more time to throw!";
            swapText.text = "When your meter is full press <c=yellow>L<c=black> to SWAP into your opponent's area.\nHarass them by throwing at their board or stealing their hamsters!";
            attackText.text = "If someone has swapped into your area, attack them with the <c=red>K<c=black> button!";
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void CheckInput() {
        base.CheckInput();

        if(InputState.GetButtonOnAnyControllerPressed("Submit")) {
            // Move forward one page
            if(page1.activeSelf) {
                page1.SetActive(false);
                page2.SetActive(true);
            } else if(page2.activeSelf) {
                page2.SetActive(false);
                page1.SetActive(true);
                Deactivate();
            }
        } else if(InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            // Move back one page
            if (page1.activeSelf) {
                Deactivate();
            } else if (page2.activeSelf) {
                page2.SetActive(false);
                page1.SetActive(true);
            }
        }
    }
}
