using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningManager : MonoBehaviour {

    public SpriteRenderer[] meterLights;
    public SpriteRenderer boltIcon;

    LightningRod[] _lightningRods;

    float _lightningTimer = 0f;
    float _lightningTime = 15f;

    bool _prepping = false;

    bool _isActive = false;
    float _activeTimer = 0f;
    float _activeTime = 5f;

    LevelManager _levelManager;

    private void Awake() {
        _levelManager = FindObjectOfType<LevelManager>();
    }
    // Start is called before the first frame update
    void Start() {
        _lightningRods = FindObjectsOfType<LightningRod>();

        // Start with all the sprites disabled
        DisableSprites();
    }
    
    // Update is called once per frame
    void Update() {
        if(_levelManager == null || !_levelManager.gameStarted) {
            return;
        }

        if (!_isActive) {
            _lightningTimer += Time.deltaTime;

             if (_lightningTimer >= _lightningTime) {
                Activate();
                // Turn on third meter
                meterLights[2].enabled = true;
                _lightningTimer = 0f;
            } else if (_lightningTimer >= _lightningTime - 5f) {
                // Show some sparks
                StartPrep();

                // Turn on second meter
                meterLights[1].enabled = true;
            } else if (_lightningTimer >= _lightningTime - 10f) {
                // Turn on first meter
                meterLights[0].enabled = true;
            }
        } else {
            _activeTimer += Time.deltaTime;
            if (_activeTimer >= _activeTime) {
                Deactivate();
                _activeTimer = 0f;
            }
        }
    }

    void StartPrep() {
        // Only do once
        if(_prepping) {
            return;
        }

        foreach (LightningRod lRod in _lightningRods) {
            lRod.StartSparks();
        }
        _prepping = true;
    }

    void Activate() {
        foreach(LightningRod lRod in _lightningRods) {
            lRod.Activate();
        }

        // Turn on the bolt icon
        boltIcon.enabled = true;

        _isActive = true;
    }

    void Deactivate() {
        foreach (LightningRod lRod in _lightningRods) {
            lRod.Deactivate();
        }

        DisableSprites();

        _isActive = false;
        _prepping = false;
    }

    void DisableSprites() {
        // Turn off all sprites
        foreach (SpriteRenderer sprite in meterLights) {
            sprite.enabled = false;
        }
        boltIcon.enabled = false;
    }
}
