﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// TODO:
//  Need a menu stack for focus, so when one loses focus, we go back to the previous one
//  Just a variable for the previous menu will probably work

public class Menu : MonoBehaviour {
    public bool hasFocus;
    public bool holdsSelection; // whether or not this menu should keep track of the previously selected option
    public bool pauses;

    public GameObject menuObj;
    public MenuOption selectedOption;

    [SerializeField]
    Menu _prevMenu;

    MenuOption[] _menuOptions;

    EventSystem _eventSystem;
    protected GameManager _gameManager;

    protected virtual void Awake() {
        GetChildOptions();

        _eventSystem = EventSystem.current.GetComponent<EventSystem>();
        _gameManager = GameManager.instance;
    }

    protected void GetChildOptions() {
        _menuOptions = transform.GetComponentsInChildren<MenuOption>(true);
        foreach (MenuOption mO in _menuOptions) {
            mO.SetParentMenu(this);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        if(_menuOptions.Length == 0) {
            GetChildOptions();
        }
    }

    // Update is called once per frame
    protected virtual void Update() {
        if (hasFocus) {
            CheckInput();

            if (pauses && !_gameManager.isPaused) {
                _gameManager.FullPause();
            }

            if (selectedOption != null) {
                selectedOption.CheckInput();
            }

            // If we don't have anything selected
            //if (_eventSystem.currentSelectedGameObject == null) {
            // If we get a controller input
            if (InputState.GetButtonOnAnyControllerPressed("Up") ||
                InputState.GetButtonOnAnyControllerPressed("Down")) {
                // And there are no options highlighted
                if(!IsAnyOptionHighlighted()) {
                    // Select an option                
                    if (selectedOption != null) {
                        selectedOption.Highlight();
                    } else if (_menuOptions != null) {
                        // Find the firstSelection
                        foreach (MenuOption mO in _menuOptions) {
                            if (mO.isFirstSelection) {
                                mO.Highlight();
                            }
                        }
                    }
                }
            }
            //}
        }
    }

    protected virtual void CheckInput() {

    }

    public virtual void TakeFocus() {
        Menu[] allMenus = FindObjectsOfType<Menu>();
        foreach(Menu menu in allMenus) {
            if (menu.hasFocus && menu != this) {
                _prevMenu = menu;
                menu.LoseFocus();
            }
        }

        StartCoroutine(GetFocusLater());

        // If we're already active
        if (gameObject.activeSelf) {
            // Enable the buttons
            StartCoroutine(EnableButtonsLater());
        }
        // Otherwise wait for OnEnable to trigger
    }

    public virtual void LoseFocus() {
        hasFocus = false;

        DisableOptions();
    }

    protected void DisableOptions() {
        if (_menuOptions != null) {
            Selectable tempSelectable;
            foreach (MenuOption mO in _menuOptions) {
                mO.Unhighlight();
                mO.isReady = false;
                tempSelectable = mO.GetComponent<Selectable>();

                if (tempSelectable != null) {
                    tempSelectable.interactable = false;
                }
            }
        }
    }

    // Open the menu
    public virtual void Activate() {
        if(menuObj != null) {
            menuObj.SetActive(true);
        }

        TakeFocus();

        if(_gameManager == null) {
            _gameManager = GameManager.instance;
        }
        if (pauses && !_gameManager.isPaused) {
            _gameManager.FullPause();
        }
    }

    // Fully close the menu
    public virtual void Deactivate() {
        // Make sure to only go through this if we have focus
        if (hasFocus) {
            if (menuObj != null) {
                menuObj.SetActive(false);
            }

            _gameManager.Unpause();

            LoseFocus();

            // If there was a menu below, give it focus
            if (_prevMenu != null) {
                StartCoroutine(EnablePrevMenuLater());
            }
        }
    }

    private void OnEnable() {
        // idk why this would ever be false but just in case
        if (hasFocus) {
            StartCoroutine(EnableButtonsLater());
        }
    }

    IEnumerator GetFocusLater() {
        yield return null;

        hasFocus = true;
    }

    IEnumerator EnablePrevMenuLater() {
        yield return null;

        _prevMenu.TakeFocus();
    }

    IEnumerator EnableButtonsLater() {
        yield return null;

        if(_menuOptions != null && _menuOptions.Length == 0) {
            GetChildOptions();
        }

        if (_menuOptions != null) {
            foreach (MenuOption mO in _menuOptions) {
                mO.isReady = true;
                if (mO.GetComponent<Selectable>() != null) {
                    mO.GetComponent<Selectable>().interactable = true;
                }
            }
        }

        // Reselect option
        if (selectedOption != null && holdsSelection) {
            selectedOption.Highlight();
        } else {
            if (_menuOptions != null) {
                foreach (MenuOption mO in _menuOptions) {
                    if (mO.isFirstSelection) {
                        mO.Highlight();
                    }
                }
            }
        }
    }

    bool IsAnyOptionHighlighted() {
        bool foundHighlighted = false;

        MenuOption[] allMenuOptions = FindObjectsOfType<MenuOption>();
        foreach(MenuOption mOption in allMenuOptions) {
            if(mOption.isHighlighted) {
                foundHighlighted = true;
                break;
            }
        }

        return foundHighlighted;
    }
}
