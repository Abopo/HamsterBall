﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullDownWindow : MonoBehaviour {
    public GameObject changeText;

    bool _isShowing = false;
    bool _isHiding = false;

    float _moveSpeed = 10f;
    float _upYPos = 5.8f;
    float _downYPos = 1.86f;

	// Use this for initialization
	void Start () {
        // Make sure the window is up at the start
        transform.localPosition = new Vector3(transform.localPosition.x, _upYPos, transform.localPosition.z);
    }

    // Update is called once per frame
    void Update () {
		if(_isShowing) {
            // Move downward
            transform.Translate(0f, -_moveSpeed * Time.deltaTime, 0f);

            if (transform.localPosition.y <= _downYPos) {
                transform.localPosition = new Vector3(transform.localPosition.x, _downYPos, transform.localPosition.z);
                _isShowing = false;
            }
        } else if(_isHiding) {
            // Move upward
            transform.Translate(0f, _moveSpeed * Time.deltaTime, 0f);

            if (transform.localPosition.y >= _upYPos) {
                transform.localPosition = new Vector3(transform.localPosition.x, _upYPos, transform.localPosition.z);
                _isHiding = false;
            }
        }
	}

    public void Show() {
        _isShowing = true;
    }

    public void Hide() {
        _isHiding = true;
        // make sure player sprite is behind ui
        GetComponentInChildren<CSPlayerController>().SpriteRenderer.sortingOrder = -6;
    }

    public void PlayerLeft() {
        changeText.SetActive(true);
    }
    public void PlayerReturned() {
        changeText.SetActive(false);
    }

}
