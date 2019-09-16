using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGFish : MonoBehaviour {

    float _actionTime = 2.0f;
    float _actionTimer = 0f;

    int _rand;
    int _moveDir;
    float _moveSpd;

    bool _stopped;

    Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start() {
        _rand = Random.Range(0, 6);
        _animator.SetInteger("Fish", _rand);
    }

    // Update is called once per frame
    void Update() {
        _actionTimer += Time.deltaTime;
        if(_actionTimer >= _actionTime) {
            ChooseAction();
            _actionTimer = 0f;
        }

        // Move maybe
        if(_stopped) {
            // Slow to a stop
            if (_moveSpd > 0) {
                _moveSpd -= 1f * Time.deltaTime;
                if(_moveSpd < 0) {
                    _moveSpd = 0;
                }
            } else if(_moveSpd < 0) {
                _moveSpd += 1f * Time.deltaTime;
                if (_moveSpd > 0) {
                    _moveSpd = 0;
                }
            }
        }

        transform.Translate(_moveSpd * _moveDir * Time.deltaTime, 0f, 0f);
    }

    void ChooseAction() {
        // Either move or stop
        _rand = Random.Range(0, 2);
        
        if(_rand == 0) {
            // Stop
            _stopped = true;
            _animator.speed = 0.5f;
        } else {
            // Move
            _animator.speed = 1;

            // If we are currently stopped
            if (_stopped) {
                // Choose a random direction to move
                _rand = Random.Range(0, 2);
                _moveDir = _rand == 0 ? 1 : -1;

                _moveSpd = Random.Range(0.1f, 0.6f);
            } else {
                // Keep moving the same direction

                // But maybe at a different speed??
            }

            if(_moveDir == 1) {
                // Face right
                Vector3 theScale = transform.localScale;
                theScale.x = -Mathf.Abs(theScale.x);
                transform.localScale = theScale;
            } else {
                // Face left
                Vector3 theScale = transform.localScale;
                theScale.x = Mathf.Abs(theScale.x);
                transform.localScale = theScale;
            }

            _stopped = false;
        }
    }
}
