using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InteractIcon : MonoBehaviour {

    Animator _icon;

    private void Awake() {
        _icon = GetComponentInChildren<Animator>(true);
    }
    // Start is called before the first frame update
    void Start() {
        _icon.gameObject.SetActive(true);
        _icon.keepAnimatorControllerStateOnDisable = true;

        if (ReInput.controllers.joystickCount > 0) {
            _icon.SetBool("keyboard", false);
            _icon.Play("DPadPress");
        } else {
            _icon.SetBool("keyboard", true);
            _icon.Play("KeyPress");
        }

        _icon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Activate() {
        _icon.gameObject.SetActive(true);
    }
    public void Deactivate() {
        _icon.gameObject.SetActive(false);
    }
}
