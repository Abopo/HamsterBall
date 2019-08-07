using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish : SeaCreature {

    // Moves in a wavy motion
    float _waveSpeed = 0f;
    bool _waveUp = true;

    protected new void Awake() {
        base.Awake();
    }
    // Use this for initialization
    protected new void Start() {
        base.Start();

        species = 0;
        _animator.SetInteger("Species", species);
    }

    // Update is called once per frame
    void Update () {
        if(_waveUp) {
            _waveSpeed += Time.deltaTime;
            if (_waveSpeed >= 0.75f) {
                _waveUp = false;
            }
        } else {
            _waveSpeed -= Time.deltaTime;
            if(_waveSpeed <= -0.75f) {
                _waveUp = true;
            }
        }

        transform.Translate(moveSpeed * Time.deltaTime, _waveSpeed * Time.deltaTime, 0f);
	}
}
