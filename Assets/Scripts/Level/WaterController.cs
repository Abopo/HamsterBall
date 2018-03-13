using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves the water up and down in the beach level
public class WaterController : MonoBehaviour {

    public float WaterHeight {
        get {
            return transform.position.y + 5.0f;
        }
    }

    float _moveTime = 25f;
    float _moveTimer = 0f;

    float _moveDir = 0f;
    float _moveSpeed = 1f;

    EdgeCollider2D[] _floatingObjects;

    // Use this for initialization
    void Start () {
        _floatingObjects = GetComponentsInChildren<EdgeCollider2D>();

        TurnOffFloatingObjects();
	}
	
	// Update is called once per frame
	void Update () {
        _moveTimer += Time.deltaTime;
        if(_moveTimer >= _moveTime && _moveDir == 0) {
            // Change position
            if(transform.position.y > -8f) {
                MoveWaterDown();
            } else if(transform.position.y < -11f) {
                MoveWaterUp();
            }
        }

        transform.Translate(0f, _moveSpeed * _moveDir * Time.deltaTime, 0f, Space.World);

        if(_moveDir == -1f && transform.position.y < -12f) {
            StopMoving();
        }
        if(_moveDir == 1f && transform.position.y > -7.2f) {
            StopMoving();
        }
	}

    void MoveWaterUp() {
        _moveDir = 1f;
        TurnOffFloatingObjects();
    }

    void MoveWaterDown() {
        _moveDir = -1f;
        TurnOffFloatingObjects();
    }

    void StopMoving() {
        if(_moveDir > 0) {
            TurnOnFloatingObjects();
        } else if (_moveDir < 0) {
            TurnOffFloatingObjects();
        }

        _moveDir = 0f;
        _moveTimer = 0f;
    }

    void TurnOnFloatingObjects() {
        foreach (EdgeCollider2D obj in _floatingObjects) {
            obj.enabled = true;
        }
    }

    void TurnOffFloatingObjects() {
        foreach (EdgeCollider2D obj in _floatingObjects) {
            obj.enabled = false;
        }
    }
}
