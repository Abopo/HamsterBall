using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Script in charge of displaying the countdown before the game starts
public class GameCountdown : MonoBehaviour {
    public bool started;

    Text _displayText;

    float _minScale;
    float _maxScale;

    float _scaleTime = 1f;
    float _scaleTimer = 0f;

    int _stage;

    bool _done;

	// Use this for initialization
	void Start () {
		//FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountDown321);
        _displayText = transform.GetChild(0).GetComponent<Text>();
        _displayText.enabled = true;
        _displayText.text = "3";

        _minScale = 4f;
        _maxScale = _displayText.transform.localScale.x;

        _stage = 0;

        started = true;
        _done = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(_done || !started) {
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
					FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountDown321);
                    _displayText.text = "2";
                    break;
                case 2:
                    _displayText.text = "1";
					FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountDown321);
                    break;
                case 3:
					FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CountDownGo);
                    _displayText.text = "Scramble!";
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
        FindObjectOfType<LevelManager>().gameStarted = true;
		SoundManager.mainAudio.MusicMainEvent.setParameterValue("RowDanger", 1f);
    }
}
