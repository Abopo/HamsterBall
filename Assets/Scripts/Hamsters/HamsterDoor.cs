using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterDoor : MonoBehaviour {

    bool _isOpen;
    float _openTime = 0.5f;
    float _openTimer = 0f;

    float _dir = 1f;

	// Use this for initialization
	void Start () {
        if(transform.GetComponentInParent<HamsterSpawner>().rightSidePipe) {
            //_dir = -1f;
        }
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

    public void Open() {
        if (!_isOpen) {
            // TODO: Rotate the proper way when on the right side
            transform.Rotate(0f, 0f, 90f * _dir);

            _isOpen = true;
        }

        _openTimer = 0f;
    }

    void Close() {
        if (_isOpen) {
            transform.Rotate(0f, 0f, -90f * _dir);

            _isOpen = false;
        }
    }
}
