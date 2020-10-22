using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabHamDoor : HamsterDoor {

    public Transform leftDoor;
    public Transform rightDoor;

    float _slideDir;
    float _slideSpeed = 10f;

    float _outXPos = 0.7f;
    float _inXPos = 0.22f;

    float xDelta;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (_isMoving) {
            // Slide over time

            // Right door slides right
            xDelta = _slideSpeed * _slideDir * Time.deltaTime;
            rightDoor.localPosition = new Vector3(rightDoor.localPosition.x + xDelta, rightDoor.localPosition.y, rightDoor.localPosition.z);
            if (_slideDir < 0 && rightDoor.localPosition.x < _inXPos) {
                rightDoor.localPosition = new Vector3(_inXPos, rightDoor.localPosition.y, rightDoor.localPosition.z);
                _isMoving = false;
            } else if (_slideDir > 0 && rightDoor.localPosition.x > _outXPos) {
                rightDoor.localPosition = new Vector3(_outXPos, rightDoor.localPosition.y, rightDoor.localPosition.z);
                _isMoving = false;
            }

            // Left door slides left
            xDelta = -_slideSpeed * _slideDir * Time.deltaTime;
            leftDoor.localPosition = new Vector3(leftDoor.localPosition.x + xDelta, leftDoor.localPosition.y, leftDoor.localPosition.z);
            if (_slideDir < 0 && leftDoor.localPosition.x > -_inXPos) {
                leftDoor.localPosition = new Vector3(-_inXPos, leftDoor.localPosition.y, leftDoor.localPosition.z);
                _isMoving = false;
            } else if (_slideDir > 0 && leftDoor.localPosition.x < -_outXPos) {
                leftDoor.localPosition = new Vector3(-_outXPos, leftDoor.localPosition.y, leftDoor.localPosition.z);
                _isMoving = false;
            }
        }
    }

    public override void Open() {
        // Corp doors slide inward instead of rotate
        _isOpen = true;
        _isMoving = true;
        _slideDir = 1f;

        _openTimer = 0f;
    }

    protected override void Close() {
        // Corp doors slide inward instead of rotate
        _isOpen = false;
        _isMoving = true;
        _slideDir = -1f;
    }
}
