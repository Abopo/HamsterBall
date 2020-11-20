using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividerFlash : MonoBehaviour {
    public int team;

    Material _material;

    public bool isFlashing;
    float _flashTime = 0.2f;
    float _flashTimer = 0f;

    SpriteRenderer _dangerFlash;
    float _dangerTimer = 0f;
    float _dangerTime = 0.75f;
    bool _dangerUp = true;

    Color brown = new Color(188, 97, 0);

	// Use this for initialization
	void Start () {
        //_material = GetComponent<MeshRenderer>().material;

        _dangerFlash = GetComponentInChildren<SpriteRenderer>(true);
        if (_dangerFlash != null) {
            _dangerFlash.enabled = false;
        }

        isFlashing = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(isFlashing) {
            // Old stuff
            /*
            _flashTimer += Time.deltaTime;

            if (_flashTimer >= _flashTime) {
                Flash();
                _flashTimer = 0f;
            }
            */

            // New flash
            if (_dangerFlash != null) {
                if (_dangerUp) {
                    _dangerTimer += Time.deltaTime;
                    if (_dangerTimer >= _dangerTime) {
                        _dangerUp = false;
                    }
                } else {
                    _dangerTimer -= Time.deltaTime;
                    if (_dangerTimer <= 0) {
                        _dangerUp = true;
                    }
                }
                _dangerFlash.color = new Color(_dangerFlash.color.r, _dangerFlash.color.g, _dangerFlash.color.b, (75 + 125 * (_dangerTimer / _dangerTime)) / 255);
            }
        }

    }

    void Flash() {
        if (_material.color == brown) {
            _material.color = Color.red;
        } else {
            _material.color = brown;
        }
    }

    public void StartFlashing() {
        if (!isFlashing) {
            isFlashing = true;
            _flashTimer = 0f;

            if (_dangerFlash != null) {
                _dangerFlash.enabled = true;
            }
        }
        
        //SoundManager.mainAudio.MountainMusicEvent.setParameterValue("RowDanger", 2f);
    }

    public void StopFlashing() {
        isFlashing = false;
        //_material.color = brown;

        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.CrowdSmall1);

        if (_dangerFlash != null) {
            _dangerFlash.enabled = false;
        }
        Debug.Log("Stop Flash");

		//SoundManager.mainAudio.MountainMusicEvent.setParameterValue("RowDanger", 1f);
    }
}