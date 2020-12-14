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

    bool playedBuzz = false;

    FMOD.Studio.EventInstance electricityMeterBuzzEvent;

    LevelManager _levelManager;

    private void Awake() {
        _levelManager = FindObjectOfType<LevelManager>();
    }
    // Start is called before the first frame update
    void Start() {
        _lightningRods = FindObjectsOfType<LightningRod>();

        // Start with all the sprites disabled
        DisableSprites();
        electricityMeterBuzzEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Stages/Electricity Meter");
        electricityMeterBuzzEvent.setParameterValue("ElectricityMeter", 3f);
    }
    
    // Update is called once per frame
    void Update() {
        if(_levelManager == null || !_levelManager.gameStarted) {
            return;
        }

        if (playedBuzz == false) {
            electricityMeterBuzzEvent.start();
            playedBuzz = true;
        }

        if (!_isActive) {
            _lightningTimer += Time.deltaTime;

             if (_lightningTimer >= _lightningTime) {
                Activate();
                // Turn on third meter
                Debug.Log("Electric 3");
                electricityMeterBuzzEvent.setParameterValue("ElectricityMeter", 2f);
                meterLights[2].enabled = true;
                _lightningTimer = 0f;
            } else if (_lightningTimer >= _lightningTime - 5f) {
                // Show some sparks
                StartPrep();
                // Turn on second meter
                Debug.Log("Electric 2");
                electricityMeterBuzzEvent.setParameterValue("ElectricityMeter", 1f);
                meterLights[1].enabled = true;
            } else if (_lightningTimer >= _lightningTime - 10f) {
                // Turn on first meter
                Debug.Log("Electric 1");
                electricityMeterBuzzEvent.setParameterValue("ElectricityMeter", 0f);
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
        Debug.Log("Electric Stop");

        electricityMeterBuzzEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        // Turn on the bolt icon
        boltIcon.enabled = true;

        _isActive = true;
    }

    void Deactivate() {
        foreach (LightningRod lRod in _lightningRods) {
            lRod.Deactivate();
        }

        DisableSprites();
        Debug.Log("Electric Start");
        electricityMeterBuzzEvent.start();

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
