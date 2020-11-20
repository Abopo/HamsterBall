using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will tick a number text up or down to a given value
public class NumberTick : MonoBehaviour {
    SuperTextMesh _valueText;

    float _value;
    int _endValue;

    bool _ticking;
    int _tickDir;

    float _tickTimer;
    float _tickTime = 0.01f;
    float _tickRate; // based on the initial difference between the start/end values

    private void Awake() {
        _valueText = GetComponent<SuperTextMesh>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(_ticking) {
            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickTime) {
                _tickTimer = 0;

                _value = _value + (_tickRate * _tickDir);

                if (_tickDir > 0 && _value >= _endValue) {
                    EndTick();
                } else if (_tickDir < 0 && _value <= _endValue) {
                    EndTick();
                }

                _valueText.text = ((int)_value).ToString();
                _valueText.Rebuild();
            }
        }
    }

    public void StartTick(int startValue, int endValue) {
        if(endValue > startValue) {
            _tickDir = 1;
        } else if(endValue < startValue) {
            _tickDir = -1;
        } else {
            // they're the same value
            return;
        }

        _value = startValue;
        _endValue = endValue;

        _tickRate = Mathf.Abs(endValue - startValue) / 100f;

        if (_valueText != null) {
            _valueText.text = startValue.ToString();
        }

        _ticking = true;
    }

    void EndTick() {
        _value = _endValue;
        _ticking = false;
    }
}
