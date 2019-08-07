using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : SeaCreature {

    float _moveSpeed = 0f;
    bool _move;

    protected new void Awake() {
        base.Awake();
    }
    // Use this for initialization
    protected new void Start() {
        base.Start();

        species = 1;
        _animator.SetInteger("Species", species);
    }

    // Update is called once per frame
    void Update () {
		if(_move) {
            transform.parent.Translate(_moveSpeed * Time.deltaTime, 0f, 0f);
        } else {
            if (left) {
                if (_moveSpeed < 0f) {
                    _moveSpeed += 3 * Time.deltaTime;
                    transform.parent.Translate(_moveSpeed * Time.deltaTime, 0f, 0f);
                }
            } else {
                if (_moveSpeed > 0f) {
                    _moveSpeed -= 3 * Time.deltaTime;
                    transform.parent.Translate(_moveSpeed * Time.deltaTime, 0f, 0f);
                }
            }
        }
	}

    public void StartMove() {
        _moveSpeed = moveSpeed;
        _move = true;
    }
    public void StopMove() {
        _move = false;
    }
}
