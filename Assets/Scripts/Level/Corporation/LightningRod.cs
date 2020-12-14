using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : MonoBehaviour {

    GameObject _collider;
    Animator _animator;

    bool _sparking;
    float _sparkTimer = 0f;
    float _sparkTime = 1f;
    int _sparkCount = 0;

    bool _isActive = false;
    float _activeTimer = 0f;
    float _activeTime = 5f;

    FMOD.Studio.EventInstance electricityMeterEvent;

    private void Awake() {
        _collider = transform.GetComponentInChildren<BoxCollider2D>(true).gameObject;
        _animator = transform.GetComponentInChildren<Animator>();
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(_activeTimer);
        if(_sparking) {
            _sparkTimer += Time.deltaTime;
            if(_sparkTimer >= _sparkTime) {
                // Play one spark
                _animator.Play("ElectricGatePrepStart");
                _sparkTimer = 0f;
                FMODUnity.RuntimeManager.PlayOneShot("event:/Stages/Electric Spark");

                // Spark 3 times, then move into a looping spark
                _sparkCount++;
                if(_sparkCount >= 3) {
                    _animator.SetInteger("State", 2);
                    _sparking = false;
                    electricityMeterEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Stages/Electricity Loop");
                    electricityMeterEvent.start();
                }
            }
        }
	}

    public void StartSparks() {
        _sparking = true;
        _sparkTimer = 0f;
        _sparkCount = 0;
        _animator.SetInteger("State", 0);
        _animator.Play("ElectricGatePrepStart");
        electricityMeterEvent.setParameterValue("ElectricityBuildUp", 0);
    }

    public void Activate() {
        electricityMeterEvent.setParameterValue("ElectricityBuildUp", 1);
        Debug.Log("Gate On");
        _isActive = true;
        _activeTimer = 0f;
        _collider.SetActive(true);
        _animator.SetInteger("State", 3);
    }

    public void Deactivate() {
        electricityMeterEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        electricityMeterEvent.release();
        Debug.Log("Deactivate Gates");
        _isActive = false;
        _collider.SetActive(false);

        _animator.SetInteger("State", 5);
    }
}
