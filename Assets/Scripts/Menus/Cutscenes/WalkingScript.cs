﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MAkes the cutscene character "walk" into frame, bobbing up and down
public class WalkingScript : MonoBehaviour {

    public bool isDone;
    public bool isWalking = false;

    float _walkSpeed = 5f;
    float _bobSpeed = 3f;

    float _bobTime = 1f;
    float _bobTimer;

    CutsceneCharacter _character;

	// Use this for initialization
	void Start () {
        _character = GetComponent<CutsceneCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
		if(isWalking) {
            _character.Translate(_walkSpeed * Time.deltaTime, _bobSpeed * Time.deltaTime);

            _bobTimer += Time.deltaTime;
            if(_bobTimer >= _bobTime) {
                _bobSpeed = -_bobSpeed;
            }

            if (_character.side < 0) {
                if (_character.RectTransform.anchoredPosition.x >= _character.screenPos) {
                    Finish();
                }
            } else if (_character.side > 0) {
                if (_character.RectTransform.anchoredPosition.x <= _character.screenPos) {
                    Finish();
                }
            }
        }
    }

    public void StartWalking() {
        isWalking = true;
        // The character is walking in so don't slide
        _character.StopSliding();
    }

    void Finish() {
        isDone = true;
        isWalking = false;

        _character.RectTransform.anchoredPosition = new Vector2(_character.screenPos, 0);
    }
}
