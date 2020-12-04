using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION { LEFT = -1, RIGHT = 1 };
// Arrow buttons bounce in the direction they are pointing when selected
public class ArrowButton : MonoBehaviour {

    public float distance;
    public DIRECTION direction;

    float _startPoint;
    float _endPoint;
    public float _speed = 50f;

    bool _movOut;
    bool _movIn;
    float _moveDir;

    // Start is called before the first frame update
    void Start() {
        _startPoint = transform.position.x;
        _endPoint = transform.position.x + (distance * (int)direction);
    }

    // Update is called once per frame
    void Update() {
        if(_movOut) {
            transform.Translate(_speed * _moveDir * Time.deltaTime, 0f, 0f);
            
            if(_moveDir > 0 && transform.position.x >= _endPoint ||
                _moveDir < 0 && transform.position.x <= _endPoint) {
                _movOut = false;
                _movIn = true;
                _moveDir *= -1;
            }
        } else if(_movIn) {
            transform.Translate(_speed * _moveDir * Time.deltaTime, 0f, 0f);

            if (_moveDir > 0 && transform.position.x >= _startPoint ||
                _moveDir < 0 && transform.position.x <= _startPoint) {
                _movIn = false;
            }
        }
    }

    public void Move() {
        if (direction == DIRECTION.RIGHT) {
            _moveDir = 1;
        } else {
            _moveDir = -1;
        }

        _movOut = true;
    }
}
