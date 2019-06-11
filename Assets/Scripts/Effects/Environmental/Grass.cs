using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour {

    public int grassType;

    bool _hit;

    Animator _animator;

	// Use this for initialization
	void Start () {
        _animator = GetComponentInChildren<Animator>();

        _animator.SetInteger("Type", grassType);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player" || collision.tag == "Hamster") {
            // Play the hit animation
            _hit = true;
            _animator.SetBool("Hit", _hit);
        }
    }

    public void EndHit() {
        _hit = false;
        _animator.SetBool("Hit", _hit);
    }
}
