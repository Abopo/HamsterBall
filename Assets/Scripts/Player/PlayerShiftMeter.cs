using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShiftMeter : MonoBehaviour {
    public SpriteRenderer outline;

    PlayerController _playerController;

    Transform _shiftMeterBack;
    Transform _shiftMeterFront;

    Transform _effect;

    float tempFloat;

    bool _display;

    private void Awake() {
        _playerController = transform.GetComponentInParent<PlayerController>();

        _shiftMeterBack = transform.Find("Backer");
        _shiftMeterFront = transform.Find("Fronter");
        _effect = transform.Find("Effect");
    }
    // Start is called before the first frame update
    void Start() {
        _display = GameManager.instance.isSinglePlayer ? false : true;

        if(!_display) {
            gameObject.SetActive(false);
        }

        // Outline should be hidden by default
        outline.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(!_display) {
            return;
        }

        if (_playerController.shifted || _playerController.ShiftCooldownTimer < _playerController.ShiftCooldownTime) {
            // Make sure meter is visible
            if (!_shiftMeterFront.gameObject.activeSelf) {
                _shiftMeterBack.gameObject.SetActive(true);
                _shiftMeterFront.gameObject.SetActive(true);

                outline.gameObject.SetActive(false);
                //outline.enabled = false;
                //_effect.gameObject.SetActive(false);
            }

            // Adjust width of meter based on shift timer
            tempFloat = (_playerController.ShiftCooldownTimer / _playerController.ShiftCooldownTime);
            _shiftMeterFront.localScale = new Vector2(tempFloat, _shiftMeterFront.localScale.y);

        // If we can shift
        } else if(_shiftMeterFront.gameObject.activeSelf) {
            // Hide the meter
            _shiftMeterBack.gameObject.SetActive(false);
            _shiftMeterFront.gameObject.SetActive(false);

            // Show the effect
            outline.gameObject.SetActive(true);
            //outline.enabled = true;
            //_effect.gameObject.SetActive(true);
        }
    }

    public void HideEffects() {
        _display = false;

        // Hide the meter
        _shiftMeterBack.gameObject.SetActive(false);
        _shiftMeterFront.gameObject.SetActive(false);

        // Show the effect
        outline.gameObject.SetActive(false);
    }

    public void ShowEffects() {
        _display = true;

        // Hide the meter
        _shiftMeterBack.gameObject.SetActive(true);
        _shiftMeterFront.gameObject.SetActive(true);

        // Show the effect
        outline.gameObject.SetActive(true);
    }
}
