using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBrushReaction : MonoBehaviour {

    public bool _reacting;

    float _reactTime = 0.15f;
    float _reactTimer = 0f;

    Animator _animator;
    ParticleSystem _particleSystem;

	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();
        _particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if(_reacting) {
            _reactTimer += Time.deltaTime;
            if(_reactTimer >= _reactTime) {
                _reacting = false;
                _animator.SetBool("Reacting", _reacting);
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log("Tree Collision");

        if(collision.tag == "Player" && !_reacting) {
            React();
        }
    }

    void React() {
        //Debug.Log("Reaction");

        _reacting = true;
        _reactTimer = 0f;

        // Play a little squishing animation
        _animator.SetBool("Reacting", _reacting);

        // Toss out some leaf particles
        _particleSystem.Play();
    }
}
