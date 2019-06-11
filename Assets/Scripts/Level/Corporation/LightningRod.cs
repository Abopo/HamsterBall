using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : MonoBehaviour {

    GameObject lightningObj;

    float _lightningTimer = 0f;
    float _lightningTime = 15f;

    bool _isActive = false;
    float _activeTimer = 0f;
    float _activeTime = 5f;

	// Use this for initialization
	void Start () {
        lightningObj = transform.GetChild(0).gameObject;
        lightningObj.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!_isActive) {
            _lightningTimer += Time.deltaTime;

            if(_lightningTimer >= _lightningTime-5f) {
                // Show some sparks
            }
            if(_lightningTimer >= _lightningTime) {
                Activate();
                _lightningTimer = 0f;
            }
        } else {
            _activeTimer += Time.deltaTime;
            if(_activeTimer >= _activeTime) {
                Deactivate();
            }
        }
	}

    void Activate() {
        _isActive = true;
        _activeTimer = 0f;

        lightningObj.SetActive(true);
    }

    void Deactivate() {
        _isActive = false;

        lightningObj.SetActive(false);
    }
}
