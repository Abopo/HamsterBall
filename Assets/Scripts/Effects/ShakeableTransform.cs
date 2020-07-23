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

    float _fadeTime = 0.01f;
    float _fadeTimer = 0f;
    float _fadeRateX;
    float _fadeRateY;
    bool _fading;

    Vector3 _basePos;

    float _seed;

    private void Awake() {
        _seed = Random.value;

        _basePos = transform.position;
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (_shaking) {
            transform.localPosition = _basePos + new Vector3(
                _maxShake.x * Mathf.PerlinNoise(_seed, Time.time * _frequency) * 2,
                _maxShake.y * Mathf.PerlinNoise(_seed + 1, Time.time * _frequency) * 2,
                _basePos.z);

            _shakeTimer += Time.deltaTime;
            if(_shakeTimer >= _shakeTime && !_fading) {
                StartFadeOut();
            }
        }

        if (_fading) {
            _fadeTimer += Time.deltaTime;

            // Fade out
            if(_fadeTimer >= _fadeTime) {
                FadeOut();

                _fadeTimer = 0f;
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
        _fading = false;
        transform.position = _basePos;
    }

    public void StartFadeOut() {
        _fading = true;
        _fadeTimer = 0f;

        _fadeRateX = _maxShake.x / 100;
        _fadeRateY = _maxShake.y / 100;
    }
    
    void FadeOut() {
        // Reduce by fadeRates
        _maxShake.x = _maxShake.x - _fadeRateX;
        _maxShake.y = _maxShake.y - _fadeRateY;


        // Make sure we don't go past 0
        if (_maxShake.x < 0) {
            _maxShake.x = 0;
        }
        if (_maxShake.y < 0) {
            _maxShake.y = 0;
        }

        // Once all shaking has been reduced to 0
        if (_maxShake.x <= 0 && _maxShake.y <= 0) {
            StopShaking();
        }
    }
}
