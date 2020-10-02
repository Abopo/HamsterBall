using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterDoor : MonoBehaviour {
    public bool leftSide;

    protected bool _isOpen;
    float _openTime = 0.5f;
    protected float _openTimer = 0f;

    protected bool _isMoving;
    float _rotDir;
    float _rotSpeed = 500f;

    float _openRot = 90f;
    float _closeRot = 0;

    float rotDelta;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(_isOpen) {
            _openTimer += Time.deltaTime;
            if(_openTimer >= _openTime) {
                Close();
            }
        }

        if(_isMoving) {
            // Rotate over time
            rotDelta = _rotSpeed * _rotDir * Time.deltaTime;
            transform.Rotate(0f, 0f, rotDelta);

            if (leftSide) {
                LeftSideRot();
            } else {
                RightSideRot();
            }
        }
    }

    // Rotation math sucks and is dumb so we have to have separate rotation functions
    void LeftSideRot() {
        if (_rotDir > 0 && transform.eulerAngles.z > _openRot) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _openRot);
            _isMoving = false;
        } else if (_rotDir < 0 && transform.eulerAngles.z > _openRot) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _closeRot);
            _isMoving = false;
        }
    }
    void RightSideRot() {
        if (_rotDir > 0 && transform.eulerAngles.z < 270f) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 270f);
            _isMoving = false;
        } else if (_rotDir < 0 && transform.eulerAngles.z < 270f) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            _isMoving = false;
        }
    }

    public virtual void Open() {
        if (!_isOpen) {
            _isMoving = true;
            _rotDir = 1;

            _isOpen = true;
        }

        _openTimer = 0f;
    }

    protected virtual void Close() {
        if (_isOpen) {
            _isMoving = true;
            _rotDir = -1;

            _isOpen = false;
        }
    }
}
