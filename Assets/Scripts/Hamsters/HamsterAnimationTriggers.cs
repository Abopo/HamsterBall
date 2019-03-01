using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterAnimationTriggers : MonoBehaviour {

    Hamster _hamster;
    Animator _animator;

	// Use this for initialization
	void Start () {
        _hamster = GetComponentInParent<Hamster>();
        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LongIdleEnd() {
        if (_hamster != null) {
            _hamster.Animator.SetBool("LongIdle", false);
            _hamster.SetState(0);
        } else {
            _animator.SetBool("LongIdle", false);
        }
    }
}
