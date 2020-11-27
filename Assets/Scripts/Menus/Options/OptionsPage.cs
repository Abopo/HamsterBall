using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPage : Menu {

    OptionsTab _tab;

    public OptionsPage leftPage;
    public OptionsPage rightPage;

    public RectTransform viewport;

    RectTransform[] _childRects;

    bool _isOpening;
    bool _isClosing;
    int _turnDir; // Direction the page is opening/closing
    float _turnSpeed = 500f;

    protected override void Awake() {
        base.Awake();

        _tab = GetComponentInChildren<OptionsTab>();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        // Get all our child menu transforms
        _childRects = new RectTransform[viewport.childCount];

        for(int i = 0; i < viewport.childCount; ++i) {
            _childRects[i] = viewport.GetChild(i).GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isOpening) {
            if(_turnDir > 0) {
                // Move the right side of the viewport
                viewport.SetRight(viewport.offsetMax.x + _turnSpeed * Time.deltaTime);
                if (viewport.offsetMax.x >= 295f) {
                    // Stop turning
                    EndOpen();
                }
            } else if(_turnDir < 0) {
                // Move the left side of the viewport
                viewport.SetLeft(viewport.offsetMin.x - _turnSpeed * Time.deltaTime);
                if(viewport.offsetMin.x <= -295f) {
                    // Stop turning
                    EndOpen();
                }
            }
        } else if(_isClosing) {
            if (_turnDir > 0) {
                // Move the left side of the viewport
                viewport.SetLeft(viewport.offsetMin.x + _turnSpeed * Time.deltaTime);
                if (viewport.offsetMin.x >= 380f) {
                    // Stop turning
                    EndClose();
                }
            } else if (_turnDir < 0) {
                // Move the right side of the viewport
                viewport.SetRight(viewport.offsetMax.x - _turnSpeed * Time.deltaTime);
                if (viewport.offsetMax.x <= -380f) {
                    // Stop turning
                    EndClose();
                }
            }
        }
    }

    protected override void CheckInput() {
        base.CheckInput();

        if (!_isOpening && !_isClosing) {
            if (InputState.GetButtonOnAnyControllerPressed("PageRight")) {
                // Move page right
                MovePageRight();
            } else if (InputState.GetButtonOnAnyControllerPressed("PageLeft")) {
                // Move page left
                MovePageLeft();
            }
        }
    }

    void MovePageRight() {
        // If we are chaning to the next page on the right, we need to close to the left
        ClosePageLeft();
        // And open our right page to the left
        rightPage.OpenPageLeft();
    }

    void MovePageLeft() {
        // If we are chaning to the next page on the left, we need to close to the right
        ClosePageRight();
        // And open our left page to the right
        leftPage.OpenPageRight();
    }

    public void OpenPageLeft() {
        // We are opening to the left

        // Make sure our page is visible
        if (menuObj != null) {
            menuObj.SetActive(true);
        }

        // So to start we need to take all our menu objects and set their anchor to the right
        AnchorChildren(1f);

        // Then we need to set our viewport to 'closed' on the right side
        // which is done by moving the left side over to the right
        viewport.SetLeft(380f);

        // Being turning the page to the left
        _turnDir = -1;
        _isOpening = true;
    }

    public void OpenPageRight() {
        // We are opening to the right

        // Make sure our page is visible
        if (menuObj != null) {
            menuObj.SetActive(true);
        }

        // So to start we need to take all our menu objects and set their anchor to the left
        AnchorChildren(0f);

        // Then we need to set our viewport to 'closed' on the right side
        // which is done by moving the left side over to the right
        viewport.SetRight(-380f);

        // Being turning the page to the left
        _turnDir = 1;
        _isOpening = true;
    }

    public void ClosePageLeft() {
        // We are closing to the left

        // So to start we need to take all our menu objects and set their anchor to the left
        AnchorChildren(0f);

        // Disable our options so user can't use them
        DisableOptions();

        // Being turning the page to the left
        _turnDir = -1;
        _isClosing = true;
    }

    public void ClosePageRight() {
        // We are closing to the right

        // So to start we need to take all our menu objects and set their anchor to the left
        AnchorChildren(1f);

        // Disable our options so user can't use them
        DisableOptions();

        // Being turning the page to the left
        _turnDir = 1;
        _isClosing = true;
    }

    void AnchorChildren(float anchorX) {
        foreach (RectTransform rect in _childRects) {
            // skip over the viewport
            if (rect == viewport) {
                continue;
            }

            // Setting anchor
            rect.anchorMin = new Vector2(anchorX, 0.5f);
            rect.anchorMax = new Vector2(anchorX, 0.5f);

            // Make sure they are centered
            rect.position = new Vector3(0, rect.position.y);
        }
    }

    void EndOpen() {
        // Stop opening
        _isOpening = false;

        // Activate page
        Activate();
    }

    void EndClose() {
        // Stop closing
        _isClosing = false;

        // Deactivate self
        Deactivate();

        // Double check we're not visible
        if (menuObj != null) {
            menuObj.SetActive(false);
        }

        // Return viewport to normal size
        viewport.SetLeft(-295f);
        viewport.SetRight(295f);
    }

    public override void Activate() {
        base.Activate();

        // Change tabs
        _tab.Select();
    }

    public override void Deactivate() {
        //base.Deactivate();

        // Change tab
        _tab.Deselect();
    }
}
