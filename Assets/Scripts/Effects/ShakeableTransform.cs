using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeableTransform : MonoBehaviour {

    [SerializeField]
    float _frequency;
    [SerializeField]
    Vector2 _maxShake;

    float _shakeTime;
    float _shakeTimer = 0f;
    bool _shaking;

    Vector3 _basePos;

    float _seed;

    private void Awake() {
        _seed = Random.value;

        _basePos = transform.position;
    }
    // Start is called before the first frame update
    void Start() {
        //StartShake(5f);
    }

    // Update is called once per frame
    void Update() {
        if (_shaking) {
            transform.localPosition = _basePos + new Vector3(
                _maxShake.x * Mathf.PerlinNoise(_seed, Time.time * _frequency) * 2,
                _maxShake.y * Mathf.PerlinNoise(_seed + 1, Time.time * _frequency) * 2,
                _basePos.z);

            _shakeTimer += Time.deltaTime;
            if(_shakeTimer >= _shakeTime) {
                StopShaking();
            }
        }
    }

    public void StartShake(float shakeTime) {
        _shakeTime = shakeTime;
        _shakeTimer = 0f;
        _shaking = true;
    }
    public void StartShake(float shakeTime, float frequency) {
        _shakeTime = shakeTime;
        _shakeTimer = 0f;
        _shaking = true;

        _frequency = frequency;
    }
    public void StartShake(float shakeTime, float frequency, Vector2 maxShake) {
        _shakeTime = shakeTime;
        _shakeTimer = 0f;
        _shaking = true;

        _frequency = frequency;
        _maxShake = maxShake;
    }

    public void StopShaking() {
        _shaking = false;

        transform.position = _basePos;
    }
}
