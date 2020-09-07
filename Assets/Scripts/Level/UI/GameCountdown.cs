﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Script in charge of displaying the countdown before the game starts
public class GameCountdown : MonoBehaviour {
    public bool started;
    public bool autoStart;

    SuperTextMesh _displayText;

    float _minScale;
    float _maxScale;

    float _scaleTime = 1f;
    float _scaleTimer = 0f;

    int _stage;

    bool _done;

	// Use this for initialization
	void Start () {
        //FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownThree);
        _displayText = transform.GetChild(0).GetComponent<SuperTextMesh>();
        _displayText.enabled = true;
        _displayText.text = "3";
        _displayText.gameObject.SetActive(false);

        _minScale = 4f;
        _maxScale = _displayText.transform.localScale.x;

        _stage = 0;

        _done = false;

        // If this is a continued level
        if (FindObjectOfType<GameManager>().prevLevel != "") {
            // Skip the countdown and just start
            GameStart();
        } else if (!PhotonNetwork.connectedAndReady && autoStart) {
            StartCoroutine("StartCountdownLater");
        }
	}

    IEnumerator StartCountdownLater() {
        yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(1.0f);

        StartCountdown();
    }

    public void StartCountdown() {
        _displayText.gameObject.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownThree);
        started = true;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.C)) {
            FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownThree);
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownTwo);
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownOne);
			FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownToScramble);
        }

        if (_done || !started) {
            return;
        }

        _scaleTimer += Time.deltaTime;

        // Make display text get smaller over time
        _displayText.transform.localScale = new Vector3(_displayText.transform.localScale.x - (50f * Time.deltaTime),
                                                        _displayText.transform.localScale.y - (50f * Time.deltaTime),
                                                        _displayText.transform.localScale.z);

        if (_displayText.transform.localScale.x <= _minScale) {
            _displayText.transform.localScale = new Vector3(_minScale,
                                                            _minScale,
                                                            _displayText.transform.localScale.z);
        }

        if (_scaleTimer >= _scaleTime) { 
            // Move to the next stage of the countdown
            _stage++;
            switch(_stage) {
                case 1:
					FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownTwo);
                    _displayText.text = "2";
                    break;
                case 2:
                    _displayText.text = "1";
					FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountdownOne);
                    break;
                case 3:
					FMODUnity.RuntimeManager.PlayOneShot("event:/Game Sounds/Scramble_Crowd");
                    _displayText.text = "<c=rainbow><j=once>Scramble!";
                    _minScale = 3f;
                    GameStart();
                    break;
                case 4:
                    _displayText.enabled = false;
                    _done = true;
                    break;
            }

            // Reset display text to the max scale
            _displayText.transform.localScale = new Vector3(_maxScale,
                                                            _maxScale,
                                                            _displayText.transform.localScale.z);

            _scaleTimer = 0f;
        }
    }

    void GameStart() {
        FindObjectOfType<LevelManager>().GameStart();
    }
}
