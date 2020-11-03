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

    private void Awake() {
        _collider = transform.GetComponentInChildren<BoxCollider2D>(true).gameObject;
        _animator = transform.GetComponentInChildren<Animator>();
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(_sparking) {
            _sparkTimer += Time.deltaTime;
            if(_sparkTimer >= _sparkTime) {
                // Play one spark
                _animator.Play("ElectricGatePrepStart");
                _sparkTimer = 0f;

                // Spark 3 times, then move into a looping spark
                _sparkCount++;
                if(_sparkCount >= 3) {
                    _animator.SetInteger("State", 2);
                    _sparking = false;
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
    }

    public void Activate() {
        _isActive = true;
        _activeTimer = 0f;
        _collider.SetActive(true);
        _animator.SetInteger("State", 3);
    }

    public void Deactivate() {
        _isActive = false;
        _collider.SetActive(false);

        _animator.SetInteger("State", 5);
    }
}
