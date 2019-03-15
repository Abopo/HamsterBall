using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterDoor : MonoBehaviour {

    protected bool _isOpen;
    float _openTime = 0.5f;
    protected float _openTimer = 0f;

    float _dir = 1f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(_isOpen) {
            _openTimer += Time.deltaTime;
            if(_openTimer >= _openTime) {
                Close();
            }
        }
	}

    public virtual void Open() {
        if (!_isOpen) {
            transform.Rotate(0f, 0f, 90f);

            _isOpen = true;
        }

        _openTimer = 0f;
    }

    protected virtual void Close() {
        if (_isOpen) {
            transform.Rotate(0f, 0f, -90f);

            _isOpen = false;
        }
    }
}
