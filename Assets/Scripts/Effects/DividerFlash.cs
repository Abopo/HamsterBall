using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividerFlash : MonoBehaviour {
    public int team;

    Material _material;

    bool _isFlashing;
    float _flashTime = 0.2f;
    float _flashTimer = 0f;

    Color brown = new Color(188, 97, 0);

	// Use this for initialization
	void Start () {
        _material = GetComponent<MeshRenderer>().material;

        _isFlashing = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(_isFlashing) {
            _flashTimer += Time.deltaTime;
            if(_flashTimer >= _flashTime) {
                Flash();
                _flashTimer = 0f;
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
        if (!_isFlashing) {
            _isFlashing = true;
            _flashTimer = 0f;
        }
		//SoundManager.mainAudio.MusicMainEvent.setParameterValue("RowDanger", 2f);
    }

    public void StopFlashing() {
        _isFlashing = false;
        _material.color = brown;
		SoundManager.mainAudio.MusicMainEvent.setParameterValue("RowDanger", 1f);
    }
}
