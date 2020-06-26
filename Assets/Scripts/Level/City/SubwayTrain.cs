using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayTrain : MonoBehaviour {
    float _moveSpd = 50f;

    bool _isMoving = false;

    float _moveTime = 5f;
    float _moveTimer = 0f;

    // Start is called before the first frame update
    void Start() {
        transform.position = new Vector3(15f, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update() {
        if(_isMoving) {
            transform.Translate(-_moveSpd * Time.deltaTime, 0f, 0f);

            _moveTimer += Time.deltaTime;
            if(_moveTimer >= _moveTime) {
                StopMoving();
            }
        } else {
            // Maybe move?
            _moveTimer += Time.deltaTime;
            if(_moveTimer >= _moveTime * 4) {
                StartMoving();
            }
        }
    }

    void StartMoving() {
        _isMoving = true;
        _moveTimer = 0f;
    }

    void StopMoving() {
        _isMoving = false;
        _moveTimer = 0f;
        transform.position = new Vector3(15f, transform.position.y, transform.position.z);
    }
}
