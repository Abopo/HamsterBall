using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {
    public ParticleSystem leftwardWind;
    public ParticleSystem rightwardWind;

    public float windForce;

    ShakeableTransform mainCamera;

    bool _windBlowing;
    int _windBlowingDir = 0; // -1 - left, 1 - right

    float _windBlowTime = 10f;
    float _windBlowTimer = 0f;
    float _windCooldownTime = 15f;
    float _windCooldownTimer = 0f;


    PlayerController[] _allPlayers;

    public bool WindBlowing {
        get { return _windBlowing; }
    }

    private void Awake() {
        mainCamera = FindObjectOfType<Camera>().GetComponent<ShakeableTransform>();
    }
    // Start is called before the first frame update
    void Start() {
        _windBlowing = false;

        _allPlayers = FindObjectsOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
        if(_windBlowing) {
            // Find any bubbles that are in midair
            PushThrownBubbles();

            _windBlowTimer += Time.deltaTime;
            if(_windBlowTimer >= _windBlowTime) {
                StopBlowing();
            }
        } else {
            _windCooldownTimer += Time.deltaTime;
            if(_windCooldownTimer >= _windCooldownTime) {
                StartBlowing();
            }
        }
    }

    void StartBlowing() {
        _windBlowing = true;

        if(_windBlowingDir == -1) {
            _windBlowingDir = 1;
            rightwardWind.Play();
        } else {
            _windBlowingDir = -1;
            leftwardWind.Play();
        }

        _windBlowTimer = 0f;

        mainCamera.StartShake(_windBlowTime, 8f, new Vector2(0.05f, 0.05f));
    }

    void StopBlowing() {
        _windBlowing = false;

        rightwardWind.Stop();
        leftwardWind.Stop();

        _windCooldownTimer = 0f;
    }

    void PushThrownBubbles() {
        if(_allPlayers.Length == 0) {
            _allPlayers = FindObjectsOfType<PlayerController>();
        }

        // Run through all the players
        foreach (PlayerController player in _allPlayers) {
            // if any of them have thrown a bubble and it's still in midair
            if(player.heldBall != null && player.heldBall.wasThrown && !player.heldBall.locked) {
                // Push it based on wind direction
                player.heldBall.AddForce(new Vector2(windForce * _windBlowingDir * Time.deltaTime, 0f));
            }
        }
    }
}
