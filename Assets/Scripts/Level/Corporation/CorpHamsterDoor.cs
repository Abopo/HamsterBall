using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpHamsterDoor : HamsterDoor {

    float _slideDir;
    float _slideSpeed = 10f;

    float _outXPos = -1.89f;
    float _inXPos = -0.984f;

    float xDelta;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(_isMoving) {
            // Slide over time
            xDelta = _slideSpeed * _slideDir * Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x + xDelta, transform.localPosition.y, transform.localPosition.z);

            if(_slideDir < 0 && transform.localPosition.x < _outXPos) {
                transform.localPosition = new Vector3(_outXPos, transform.localPosition.y, transform.localPosition.z);
                _isMoving = false;
            } else if(_slideDir > 0 && transform.localPosition.x > _inXPos) {
                transform.localPosition = new Vector3(_inXPos, transform.localPosition.y, transform.localPosition.z);
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
